using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Microsoft.VisualStudio.ProjectSystem.Designers;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using Microsoft.VisualStudio.ProjectSystem.Utilities.Designers;

namespace NuProj.ProjectSystem
{
    [Export(typeof(IProjectTreeModifier))]
    [PartMetadata(ProjectCapabilities.Requires, CustomProjectCapabilitiesProvider.CapabilityName)]
    internal class TreeModifier : IProjectTreeModifier
    {
        //private Lazy<Image> projectIcon = new Lazy<Image>(LoadProjectIcon, true);

        [Import]
        private Lazy<IProjectTreeFactory> TreeFactory { get; set; }

        public IProjectTree ApplyModifications(IProjectTree tree, IProjectTreeProvider projectTreeProvider)
        {
            //if (tree.Capabilities.Contains(ProjectTreeCapabilities.ProjectRoot))
            //{
            //    tree = tree.SetIcon(this.projectIcon.Value);
            //}
            //else if (tree.Capabilities.Contains(ProjectTreeCapabilities.Reference))
            //{
            //    if (tree.Children.Count == 0)
            //    {
            //        IProjectTree node = this.TreeFactory.Value.NewTree("CustomProjectTest", icon: null, expandedIcon: null, visible: true, capabilities: new string[] { ProjectTreeCapabilities.AlwaysCopyable });
            //        tree = tree.Add(node).Parent;
            //    }
            //}

            return tree.Root;
        }

        //private static Image LoadProjectIcon()
        //{
        //    using (Stream iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(CustomProjectPackage.DefaultNamespace + ".ProjectIcon.bmp"))
        //    {
        //        return Image.FromStream(iconStream);
        //    }
        //}
    }
}
