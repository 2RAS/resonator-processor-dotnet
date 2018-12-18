using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Json;
using System.Text;
using System.Windows.Forms;

namespace ResonatorBeta
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var player = new ResonatorPlayer())
            using (var mixer = new ResonatorMixer())
            {
                player.Use(mixer);
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select adaptive bundle config file",
                    Filter = "Resonator Bundle Config | *.amb"
                };

                while (openFileDialog.ShowDialog() != DialogResult.OK) ;

                // Load Adaptive Bundle
                var bundleArchivePath = openFileDialog.FileName;
                using (ZipArchive archive = ZipFile.Open(bundleArchivePath, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.Name == "")
                            Directory.CreateDirectory(entry.FullName);
                        else
                            entry.ExtractToFile(entry.FullName, true);
                    }
                }

                string bundleConfigText = null;
                using (var streamReader = new StreamReader("config.ambd", Encoding.UTF8))
                {
                    bundleConfigText = streamReader.ReadToEnd();
                }

                var bundleConfig = JsonValue.Parse(bundleConfigText) as JsonObject;
                var pipelineConfig = bundleConfig["pipeline"];

                var connections = new Dictionary<string, string>();
                foreach (var connection in pipelineConfig["connections"]["stream"] as JsonArray)
                {
                    connections[connection["to"]["id"]] = connection["from"]["id"];
                }

                var sources = new Dictionary<string, string>();
                foreach (var element in pipelineConfig["elements"] as JsonObject)
                {
                    if (element.Value["type"] == "source")
                    {
                        sources[element.Key] = element.Value["config"]["url"];
                    }
                }

                var states = new Dictionary<string, Dictionary<string, float>>();
                foreach (var state in bundleConfig["states"] as JsonObject)
                {
                    foreach (var element in state.Value["elements"] as JsonObject)
                    {
                        if (!states.ContainsKey(state.Key))
                        {
                            states.Add(state.Key, new Dictionary<string, float>());
                        }

                        states[state.Key][element.Key] = element.Value["gain"];
                    }
                }

                foreach (var source in sources)
                {
                    var sourceFilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, source.Value.Replace("\"", string.Empty)));
                    mixer.AddSource(source.Key, sourceFilePath);
                }

                player.Play();

                while (true)
                {
                    Console.WriteLine("Chose semantic state:");
                    var stateName = Console.ReadLine();
                    if(states.ContainsKey(stateName))
                    {
                        foreach(var gain in states[stateName])
                        {
                            var source = connections[gain.Key];
                            mixer.VolumeSources[source].Volume = gain.Value;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unknown semantic state.");
                    }
                }
            }
        }
    }
}
