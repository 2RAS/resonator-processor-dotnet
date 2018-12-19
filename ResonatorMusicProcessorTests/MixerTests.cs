using CSCore;
using CSCore.Streams;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ResonatorBeta.Tests
{
    [TestClass]
    public class MixerTests
    {
        ISampleSource sampleSource = new SineGenerator(1, 1, 1).ToStereo();

        [TestMethod]
        public void AddSourceTest()
        {
            Mixer mixer = new Mixer(2, 44100);
            mixer.AddSource(sampleSource);

            Assert.IsTrue(mixer.Contains(sampleSource));
        }

        [TestMethod]
        public void RemoveSourceTest()
        {
            Mixer mixer = new Mixer(2, 44100);
            mixer.AddSource(sampleSource);
            mixer.RemoveSource(sampleSource);

            Assert.IsFalse(mixer.Contains(sampleSource));
        }
    }
}