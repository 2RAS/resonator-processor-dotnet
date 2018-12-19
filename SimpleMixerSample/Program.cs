using System;
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
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select adaptive bundle config file",
                    Filter = "Resonator Bundle Config | *.amb"
                };

                while (openFileDialog.ShowDialog() != DialogResult.OK) ;

                var processor = new ResonatorProcessor();
                processor.Import(openFileDialog.FileName);

                player.Use(processor);
                player.Play();

                Console.WriteLine(processor.CurrentState);

                foreach (var state in processor.States)
                {
                    Console.ReadLine();
                    processor.CurrentState = state;
                    Console.WriteLine(processor.CurrentState);
                }

                Console.ReadLine();
            }
        }
    }
}
