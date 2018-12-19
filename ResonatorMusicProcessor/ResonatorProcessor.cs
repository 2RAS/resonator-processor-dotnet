using CSCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Json;
using System.Text;

namespace ResonatorBeta
{
    public class ResonatorProcessor : IDisposable
    {
        private readonly ResonatorMixer mixer = new ResonatorMixer();
        private Dictionary<string, string> connections;
        private Dictionary<string, string> sources;
        private Dictionary<string, Dictionary<string, float>> states;
        private string currentState;

        public string DefaultState { get; } = "DefaultState";

        public IEnumerable<string> States { get => states.Keys; }

        public string CurrentState
        {
            get => currentState;
            set
            {
                currentState = value;
                SetCurrentState(value);
            }
        }

        public void Import(string bundleFileName)
        {
            using (ZipArchive archive = ZipFile.Open(bundleFileName, ZipArchiveMode.Read))
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

            connections = new Dictionary<string, string>();
            foreach (var connection in pipelineConfig["connections"]["stream"] as JsonArray)
            {
                connections[connection["to"]["id"]] = connection["from"]["id"];
            }

            sources = new Dictionary<string, string>();
            foreach (var element in pipelineConfig["elements"] as JsonObject)
            {
                if (element.Value["type"] == "source")
                {
                    sources[element.Key] = element.Value["config"]["url"];
                }
            }

            states = new Dictionary<string, Dictionary<string, float>>();
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

            CurrentState = DefaultState;
        }

        private void SetCurrentState(string state)
        {
            if (states.ContainsKey(state))
            {
                foreach (var gain in states[state])
                {
                    var source = connections[gain.Key];
                    mixer.VolumeSources[source].Volume = gain.Value;
                }
            }
            else
            {
                throw new ArgumentException("Unknown semantic state.");
            }
        }

        public IWaveSource ToWaveSource()
            => mixer.ToWaveSource();

        public void Dispose()
            => mixer.Dispose();
    }
}
