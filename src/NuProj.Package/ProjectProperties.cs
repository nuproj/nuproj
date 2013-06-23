using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft.VisualStudio.ProjectSystem.Utilities;

namespace NuProj.ProjectSystem
{
    /// <summary>
    /// Provides rule-based property access.
    /// </summary>
    [Export]
    [PartMetadata(ProjectCapabilities.Requires, CustomProjectCapabilitiesProvider.CapabilityName)]
    internal partial class ProjectProperties
    {
        /// <summary>
        /// The configured project.
        /// </summary>
        [Import]
        private ConfiguredProject ConfiguredProject { get; set; }

        /// <summary>
        /// The file context for the properties.
        /// </summary>
        private string File { get; set; }

        /// <summary>
        /// The item type context for the properties.
        /// </summary>
        private string ItemType { get; set; }

        /// <summary>
        /// The item name context for the properties.
        /// </summary>
        private string ItemName { get; set; }
    }
}
