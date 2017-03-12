using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.ProjectSystem;
#if Dev12 || Dev14
using Microsoft.VisualStudio.ProjectSystem.Utilities;
#else
using Microsoft.VisualStudio.ProjectSystem.VS;
#endif

namespace NuProj.ProjectSystem
{
    [Export]
#if Dev12
    [PartMetadata(ProjectCapabilities.Requires, NuProjCapabilities.NuProj)]
#else
    [AppliesTo(NuProjCapabilities.NuProj)]
#endif
#if Dev12 || Dev14
    [ProjectTypeRegistration(NuProjPackage.ProjectTypeGuid,
                             "NuGet",
                             "#2",
                             NuProjPackage.ProjectExtension,
                             NuProjPackage.ProjectLanguage,
                             NuProjPackage.PackageGuid,
                             PossibleProjectExtensions = NuProjPackage.ProjectExtension,
                             ProjectTemplatesDir = @"..\..\Templates\Projects\NuProj")]
#else
    [ProjectTypeRegistration(NuProjPackage.ProjectTypeGuid,
                             "NuGet",
                             "#2",
                             NuProjPackage.ProjectExtension,
                             NuProjPackage.ProjectLanguage,
                             NuProjPackage.PackageGuid,
                             PossibleProjectExtensions = NuProjPackage.ProjectExtension,
                             ProjectTemplatesDir = @"ProjectTemplates")]
#endif
    internal sealed class NuProjUnconfiguredProject
    {
        [Import]
        public UnconfiguredProject UnconfiguredProject { get; private set; }

        [Import]
        public ActiveConfiguredProject<NuProjConfiguredProject> NuProjActiveConfiguredProject { get; private set; }

        [Import]
        public ConfiguredProject ActiveConfiguredProject
        {
            get { return NuProjActiveConfiguredProject.Value.ConfiguredProject; }
        }
    }
}
