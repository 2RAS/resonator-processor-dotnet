using System;
using CSCore.SoundOut;

namespace ResonatorBeta
{
    public class ResonatorPlayer : IDisposable
    {
        private ResonatorProcessor processor = null;
        private readonly ISoundOut outputApi;

        public ResonatorPlayer(int latency = 200) // better use a quite high latency
            => outputApi = new WasapiOut() { Latency = latency };

        public void Play()
            => outputApi.Play();
        public void Stop()
            => outputApi.Stop();

        public void Pause()
            => outputApi.Pause();

        public void Resume()
            => outputApi.Resume();

        public void Use(ResonatorProcessor processor)
        {
            this.processor = processor;
            outputApi.Initialize(processor.ToWaveSource());
        }

        public void Dispose()
        {
            outputApi.Dispose();
            if (processor != null)
            {
                processor.Dispose();
            }
        }
    }
}