using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ResonatorBeta
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var player = new ResonatorPlayer())
            {
                var processor = new ResonatorProcessor();
                player.Use(processor);

                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select adaptive bundle config file",
                    Filter = "Resonator Bundle Config | *.amb"
                };

                bool paused = false;
                while (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    processor.Import(openFileDialog.FileName);

                    player.Play();

                    Console.WriteLine("Choose states: ");

                    var states = new Dictionary<int, string>();
                    int i = 1;
                    foreach (var state in processor.States)
                    {
                        Console.WriteLine("\t[" + i + "] " + state);
                        states[i++] = state;
                    }

                    var currentState = processor.CurrentState;

                    Console.WriteLine("\n[0] Pause/Resume");
                    Console.WriteLine("\nCurrent state: " + processor.CurrentState);

                    int lastStateKey = states.FirstOrDefault(x => x.Value == processor.DefaultState).Key;
                    while (true)
                    {
                        int key = 0;
                        do
                        {
                            var input = Console.ReadKey(true);
                            key = (input.KeyChar - '0');
                        }
                        while (key > states.Count);

                        if (key == 0) {
                            if (paused)
                            {
                                player.Resume();
                                currentState = processor.CurrentState;
                            }
                            else
                            {
                                player.Pause();
                                paused = true;
                                currentState = "paused";
                            }
                            key = lastStateKey;
                        }
                        else
                        {
                            lastStateKey = key;
                            processor.CurrentState = states[key];
                            currentState = processor.CurrentState;
                        }

                        Console.WriteLine("Current state: " + currentState);
                    }
                }
            }
        }
    }
}
