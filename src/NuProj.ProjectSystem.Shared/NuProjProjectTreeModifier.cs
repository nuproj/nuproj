using System;
using System.ComponentModel.Composition;

#if !Dev12
using Microsoft.VisualStudio.Imaging;
#endif
using Microsoft.VisualStudio.ProjectSystem;
#if Dev12 || Dev14
using Microsoft.VisualStudio.ProjectSystem.Designers;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using Microsoft.VisualStudio.ProjectSystem.Utilities.Designers;
#endif

namespace NuProj.ProjectSystem
{
#if Dev12 || Dev14
    [Export(typeof(IProjectTreeModifier))]
#else
    [Export(typeof(IProjectTreePropertiesProvider))]
#endif
#if Dev12
    [PartMetadata(ProjectCapabilities.Requires, NuProjCapabilities.NuProj)]
#else
    [AppliesTo(NuProjCapabilities.NuProj)]
#endif
    internal sealed class NuProjProjectTreeModifier
#if Dev12 || Dev14
        : IProjectTreeModifier
#else
        : IProjectTreePropertiesProvider
#endif
    {
        [Import]
        public UnconfiguredProject UnconfiguredProject { get; set; }

#if Dev12 || Dev14
        public IProjectTree ApplyModifications(IProjectTree tree, IProjectTreeProvider projectTreeProvider)
        {
#if Dev12
            if (tree.Capabilities.Contains(ProjectTreeCapabilities.ProjectRoot))
                tree = tree.SetIcon(Resources.NuProj);
#else
            if (tree.Capabilities.Contains(ProjectTreeCapabilities.ProjectRoot))
                tree = tree.SetIcon(KnownMonikers.NuGet.ToProjectSystemType());
#endif

            return tree;
        }
#else
        public void CalculatePropertyValues(IProjectTreeCustomizablePropertyContext propertyContext, IProjectTreeCustomizablePropertyValues propertyValues)
        {
            if (propertyValues != null)
            {
                if (propertyValues.Flags.Contains(ProjectTreeFlags.Common.ProjectRoot))
                {
                    propertyValues.Icon = KnownMonikers.NuGet.ToProjectSystemType();
                }
            }
        }
#endif
    }
}
