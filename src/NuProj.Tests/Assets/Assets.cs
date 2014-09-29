namespace NuProj.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.Build.Construction;

    public static class Assets
    {
        private const string Namespace = "NuProj.Tests.Assets";

        public static ProjectRootElement FromTemplate()
        {
            return GetResourceProjectRootElement("FromTemplate.nuproj");
        }

        private static ProjectRootElement GetResourceProjectRootElement(string name)
        {
            return ProjectRootElement.Create(GetResourceXmlReader(name));
        }

        private static XmlReader GetResourceXmlReader(string name)
        {
            return XmlReader.Create(GetResourceStream(name));
        }

        private static Stream GetResourceStream(string name)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(Namespace + "." + name);
        }
    }
}
