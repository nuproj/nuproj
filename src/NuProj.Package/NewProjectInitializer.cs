using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Designers;
using Microsoft.VisualStudio.ProjectSystem.Utilities;

namespace NuProj.ProjectSystem
{
    [Export(typeof(INewProjectInitializationProvider))]
    [PartMetadata(ProjectCapabilities.Requires, Capabilities.NuProj)]
    internal class NewProjectInitializer : INewProjectInitializationProvider
    {
        [Import]
        private UnconfiguredProject UnconfiguredProject { get; set; }
        
        [Import]
        private MyUnconfiguredProject MyUnconfiguredProject { get; set; }

        public void InitializeNewProject()
        {
            //string projectName = Path.GetFileNameWithoutExtension(this.UnconfiguredProject.FullPath);

            //var properties = this.MyUnconfiguredProject.MyActiveConfiguredProject.Properties.Value;
            //if (String.IsNullOrEmpty(properties.ConfigurationGeneral.AssemblyName.EvaluatedValueAtEnd))
            //{
            //    properties.ConfigurationGeneral.AssemblyName.UnevaluatedValue = projectName;
            //}
            //if (String.IsNullOrEmpty(properties.ConfigurationGeneral.Name.EvaluatedValueAtEnd))
            //{
            //    properties.ConfigurationGeneral.Name.UnevaluatedValue = projectName;
            //}
            //if (String.IsNullOrEmpty(properties.ConfigurationGeneral.RootNamespace.EvaluatedValueAtEnd))
            //{
            //    properties.ConfigurationGeneral.RootNamespace.UnevaluatedValue = projectName;
            //}
        }
    }
}
