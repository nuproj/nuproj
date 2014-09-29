using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Designers;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuProj.ProjectSystem
{
    [Export]
    [PartMetadata(ProjectCapabilities.Requires, NuProjCapabilities.NuProj)]
    [ProjectTypeRegistration(NuProjPackage.ProjectTypeGuid,
                             "NuGet",
                             "#2",
                             NuProjPackage.ProjectExtension,
                             NuProjPackage.ProjectLanguage,
                             NuProjPackage.PackageGuid,
                             PossibleProjectExtensions = NuProjPackage.ProjectExtension,
                             ProjectTemplatesDir = @"..\..\Templates\Projects\NuProj")]
    internal sealed class NuProjUnconfiguredProject
    {
        [Import]
        public UnconfiguredProject UnconfiguredProject { get; private set; }

        [Import]
        public ActiveConfiguredProject<NuProjConfiguredProject> NuProjActiveConfiguredProject { get; private set; }

        [Import]
        public ConfiguredProject ActiveConfiguredProject
        {
            get { return this.NuProjActiveConfiguredProject.Value.ConfiguredProject; }
        }
    }
}
