using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Designers;
using Microsoft.VisualStudio.ProjectSystem.Utilities;

namespace NuProj.ProjectSystem
{
    [Export]
    [PartMetadata(ProjectCapabilities.Requires, Capabilities.NuProj)]
    [ProjectTypeRegistration(NuProjPackage.ProjectTypeGuid,
                             "NuGet",
                             "#2",
                             NuProjPackage.ProjectExtension,
                             NuProjPackage.ProjectLanguage,
                             NuProjPackage.PackageGuid,
                             PossibleProjectExtensions = NuProjPackage.ProjectExtension,
                             ProjectTemplatesDir=@"..\..\Templates\Projects\NuProj")]
    internal class MyUnconfiguredProject
    {
        private object syncObject = new object();

        private MyConfiguredProjectImporter configuredProjectImporter;

        /// <summary>
        /// Undocumented.
        /// </summary>
        private Lazy<IVsSolutionBuildManager> solutionBuildManager;

        public MyUnconfiguredProject()
        {
            this.solutionBuildManager = new Lazy<IVsSolutionBuildManager>(() => Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager, true);
        }

        internal MyConfiguredProject MyActiveConfiguredProject
        {
            get
            {
                this.Initialize();
                return this.configuredProjectImporter.MyConfiguredProject;
            }
        }

        internal ConfiguredProject ActiveConfiguredProject
        {
            get
            {
                this.Initialize();
                return this.configuredProjectImporter.ConfiguredProject;
            }
        }

        [Import]
        internal UnconfiguredProject UnconfiguredProject { get; private set; }

        /// <summary>
        /// Undocumented.
        /// </summary>
        [Import]
        internal Lazy<IProjectConfigurationsService> ProjectConfigurationsService { get; set; }

        [Import]
        private Lazy<IActiveConfiguredProjectBrokerService> ActiveConfiguredProjectBrokerService { get; set; }

        private void Initialize()
        {
            lock (this.syncObject)
            {
                if (this.configuredProjectImporter == null)
                {
                    this.configuredProjectImporter = new MyConfiguredProjectImporter();
                    this.ActiveConfiguredProjectBrokerService.Value.Register(this.configuredProjectImporter);
                }
            }
        }

        private class MyConfiguredProjectImporter
        {
            [Import]
            internal MyConfiguredProject MyConfiguredProject { get; private set; }

            [Import]
            internal ConfiguredProject ConfiguredProject { get; private set; }
        }
    }
}
