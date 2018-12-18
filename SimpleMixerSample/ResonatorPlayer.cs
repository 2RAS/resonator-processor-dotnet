using System;
using CSCore;
using CSCore.SoundOut;

namespace ResonatorBeta
{
    internal class ResonatorPlayer : IDisposable
    {
        private readonly ISoundOut outputApi;

        public ResonatorPlayer(int latency = 200) // better use a quite high latency
            => outputApi = new WasapiOut() { Latency = latency };

        internal void Play()
            => outputApi.Play();
        internal void Stop()
            => outputApi.Stop();

        internal void Pause()
            => outputApi.Pause();

        internal void Resume()
            => outputApi.Resume();

        internal void Use(ResonatorMixer mixer)
            => outputApi.Initialize(mixer.ToWaveSource());

        void IDisposable.Dispose()
            => outputApi.Dispose();
    }
}