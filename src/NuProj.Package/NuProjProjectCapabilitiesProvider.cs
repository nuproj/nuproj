using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Utilities;

namespace NuProj.ProjectSystem
{
    [Export(typeof(IProjectCapabilitiesProvider))]
    [SupportsFileExtension("." + NuProjPackage.ProjectExtension )]
    internal class CustomProjectCapabilitiesProvider : IProjectCapabilitiesProvider
    {
        /// <summary>
        /// The project capability itself to include for matching project types.
        /// </summary>
        internal const string CapabilityName = "NuProj";

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomProjectCapabilitiesProvider"/> class.
        /// </summary>
        public CustomProjectCapabilitiesProvider()
        {
        }

        /// <summary>
        /// Gets the capabilities that fit the project in context that this provider contributes.
        /// </summary>
        /// <value>A sequence, possibly empty but never null.</value>
        public Task<IEnumerable<string>> GetCapabilitiesAsync()
        {
            return Task.FromResult<IEnumerable<string>>(new[] { CapabilityName, "CPS" });
        }
    }
}
