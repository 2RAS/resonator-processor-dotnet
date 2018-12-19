using ResonatorBeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ResonatorBeta.Tests
{
    [TestClass]
    public class ResonatorProcessorTests
    {
        [TestMethod]
        [DeploymentItem(@"Resources\test.amb")]
        public void ImportTest()
        {
            using (var processor = new ResonatorProcessor())
            {
                processor.Import("test.amb");

                Assert.IsTrue(processor.States.Any());
            }
        }

        [TestMethod]
        [DeploymentItem(@"Resources\test.amb")]
        public void StatesTest()
        {
            using (var processor = new ResonatorProcessor())
            {
                processor.Import("test.amb");

                var actualStates = processor.States;
                var expectedStates = new List<string> { "DefaultState", "DangerState", "AttentionState", "NoisyState" };

                Assert.AreEqual(actualStates.Intersect(expectedStates).Count(), actualStates.Count());
            }
        }

        [TestMethod]
        [DeploymentItem(@"Resources\test.amb")]
        public void CurrentStateTest()
        {
            using (var processor = new ResonatorProcessor())
            {
                processor.Import("test.amb");

                Assert.AreEqual(processor.CurrentState, processor.DefaultState);

                var newState = "AttentionState";
                processor.CurrentState = newState;

                Assert.AreEqual(processor.CurrentState, newState);
            }
        }
    }
}