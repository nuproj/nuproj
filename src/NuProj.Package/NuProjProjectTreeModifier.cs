using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Designers;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using Microsoft.VisualStudio.ProjectSystem.Utilities.Designers;

namespace NuProj.ProjectSystem
{
    [Export(typeof(IProjectTreeModifier))]
#if Dev12
    [PartMetadata(ProjectCapabilities.Requires, NuProjCapabilities.NuProj)]
#else
    [AppliesTo(NuProjCapabilities.NuProj)]
#endif
    internal sealed class NuProjProjectTreeModifier : IProjectTreeModifier
    {
        [Import]
        public UnconfiguredProject UnconfiguredProject { get; set; }

        public IProjectTree ApplyModifications(IProjectTree tree, IProjectTreeProvider projectTreeProvider)
        {
#if Dev12
            if (tree.Capabilities.Contains(ProjectTreeCapabilities.ProjectRoot))
                tree = tree.SetIcon(Resources.NuProj);
#else
            // TODO: code here
#endif

            return tree;
        }
    }
}
