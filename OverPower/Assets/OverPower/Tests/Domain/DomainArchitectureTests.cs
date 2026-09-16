using NUnit.Framework;
using System.Reflection;
using System.Linq;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class DomainArchitectureTests
    {
        [Test]
        public void Domain_MustNotReferenceUnityEngine()
        {
            // Get the Domain assembly using one of its types
            Assembly domainAssembly = typeof(OverPower.Domain.DomainAssemblyMarker).Assembly;

            // Get all referenced assemblies
            AssemblyName[] referencedAssemblies = domainAssembly.GetReferencedAssemblies();

            // Check if any referenced assembly contains UnityEngine or UnityEditor
            bool referencesUnity = referencedAssemblies.Any(a => 
                a.Name.StartsWith("UnityEngine") || a.Name.StartsWith("UnityEditor"));

            Assert.IsFalse(referencesUnity, 
                "Architectural Constraint Violation: OverPower.Domain must not reference UnityEngine or UnityEditor.");
        }

        [Test]
        public void UnitySceneCatalog_EverySceneIdHasExactlyOneMapping()
        {
            // Load OverPower.Unity and OverPower.Application assemblies
            Assembly unityAssembly = Assembly.Load("OverPower.Unity");
            Assembly appAssembly = Assembly.Load("OverPower.Application");

            // Get GameSceneId enum type
            System.Type sceneIdEnum = appAssembly.GetType("OverPower.Application.GameSceneId");
            System.Type catalogType = unityAssembly.GetType("OverPower.Unity.SceneFlow.GameSceneCatalog");

            Assert.IsNotNull(sceneIdEnum, "GameSceneId type must exist.");
            Assert.IsNotNull(catalogType, "GameSceneCatalog type must exist.");

            // Get all enum values
            System.Array enumValues = System.Enum.GetValues(sceneIdEnum);
            MethodInfo getScenePathMethod = catalogType.GetMethod("GetScenePath", BindingFlags.Public | BindingFlags.Static);

            Assert.IsNotNull(getScenePathMethod, "GetScenePath method must exist in GameSceneCatalog.");

            foreach (var val in enumValues)
            {
                try
                {
                    // Verify we can call GetScenePath for each GameSceneId value without throwing
                    string path = (string)getScenePathMethod.Invoke(null, new object[] { val });
                    Assert.IsFalse(string.IsNullOrEmpty(path), $"Scene path for {val} must not be empty.");
                }
                catch (TargetInvocationException ex)
                {
                    Assert.Fail($"Mapping validation failed for {val}: {ex.InnerException?.Message}");
                }
            }
        }
    }
}
