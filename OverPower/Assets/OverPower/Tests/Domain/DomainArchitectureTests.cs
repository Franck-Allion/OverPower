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
    }
}
