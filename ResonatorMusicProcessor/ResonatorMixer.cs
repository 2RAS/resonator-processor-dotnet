using CSCore;
using CSCore.Codecs;
using CSCore.Streams;
using System.Collections.Generic;

namespace ResonatorBeta
{
    public class ResonatorMixer : Mixer
    {
        public ResonatorMixer(int channelCount = 2, int sampleRate = 44100) // output: stereo, 44,1kHz 
            : base(channelCount, sampleRate)
        {
            FillWithZeros = true;
            DivideResult = true;
        }

        public void AddSource(string name, string filename)
        {
            IWaveSource fileWaveSource = CodecFactory.Instance.GetCodec(filename);

            AddSource(
                fileWaveSource.ToStereo()
                .AppendSource(x => new VolumeSource(x.ToSampleSource()), out VolumeSource volumeSource));

            VolumeSources.Add(name, volumeSource);
        }

        public Dictionary<string, VolumeSource> VolumeSources { get; private set; } = new Dictionary<string, VolumeSource>();
    }
}