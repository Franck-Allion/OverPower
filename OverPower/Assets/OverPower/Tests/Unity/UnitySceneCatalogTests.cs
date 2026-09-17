using NUnit.Framework;
using System;
using OverPower.Application;
using OverPower.Unity.SceneFlow;

namespace OverPower.Tests.Unity
{
    [TestFixture]
    public class UnitySceneCatalogTests
    {
        [Test]
        public void UnitySceneCatalog_EverySceneIdHasExactlyOneMapping()
        {
            Array enumValues = Enum.GetValues(typeof(GameSceneId));

            foreach (var val in enumValues)
            {
                GameSceneId sceneId = (GameSceneId)val;
                string path = GameSceneCatalog.GetScenePath(sceneId);
                Assert.IsFalse(string.IsNullOrEmpty(path), $"Scene path for {sceneId} must not be empty.");
            }
        }
    }
}
