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
                             ProjectTemplatesDir=@"..\..\Templates\Projects\NuProj")]
    internal sealed class NuProjUnconfiguredProject
    {
        [Import]
        public UnconfiguredProject UnconfiguredProject { get; private set; }

        [Import]
        public Lazy<IProjectConfigurationsService> ProjectConfigurationsService { get; set; }

        [Import]
        public Lazy<IActiveConfiguredProjectBrokerService> ActiveConfiguredProjectBrokerService { get; set; }

        private object _syncObject = new object();
        private NuProjConfiguredProjectImporter _configuredProjectImporter;
        private Lazy<IVsSolutionBuildManager> _solutionBuildManager;

        public NuProjUnconfiguredProject()
        {
            _solutionBuildManager = new Lazy<IVsSolutionBuildManager>(() => Package.GetGlobalService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager, true);
        }

        public NuProjConfiguredProject NuProjActiveConfiguredProject
        {
            get
            {
                Initialize();
                return _configuredProjectImporter.NuProjConfiguredProject;
            }
        }

        public ConfiguredProject ActiveConfiguredProject
        {
            get
            {
                Initialize();
                return _configuredProjectImporter.ConfiguredProject;
            }
        }

        private void Initialize()
        {
            lock (_syncObject)
            {
                if (_configuredProjectImporter == null)
                {
                    _configuredProjectImporter = new NuProjConfiguredProjectImporter();
                    ActiveConfiguredProjectBrokerService.Value.Register(_configuredProjectImporter);
                }
            }
        }

        private sealed class NuProjConfiguredProjectImporter
        {
            [Import]
            internal NuProjConfiguredProject NuProjConfiguredProject { get; private set; }

            [Import]
            internal ConfiguredProject ConfiguredProject { get; private set; }
        }
    }
}
