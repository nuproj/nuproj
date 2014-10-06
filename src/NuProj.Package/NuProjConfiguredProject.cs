using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft.VisualStudio.ProjectSystem.Utilities;

namespace NuProj.ProjectSystem
{
    [Export]
#if Dev12
    [PartMetadata(ProjectCapabilities.Requires, NuProjCapabilities.NuProj)]
#else
    [AppliesTo(NuProjCapabilities.NuProj)]
#endif
    internal sealed class NuProjConfiguredProject
    {
        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        public ConfiguredProject ConfiguredProject { get; private set; }

        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        public Lazy<NuProjProjectProperties> Properties { get; private set; }
    }
}
