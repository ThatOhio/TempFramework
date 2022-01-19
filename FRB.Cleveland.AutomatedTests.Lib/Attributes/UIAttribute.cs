using System;
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FRB.Cleveland.AutomatedTests.Lib.Attributes
{
    /// <summary>
    /// Adds an XUnit Trait of "Type=UI" to a given test.
    /// </summary>
    [TraitDiscoverer("Nexus.Core.Attributes." + nameof(UITraitDiscoverer), "Nexus.Core")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class UIAttribute : Attribute, ITraitAttribute
    {
    }

    public class UITraitDiscoverer : ITraitDiscoverer
    {
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            yield return new KeyValuePair<string, string>("Type", "UI");
        }
    }
}