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

        /// <summary>
        /// The current directory when this type was first accessed.
        /// </summary>
        /// <remarks>
        /// This value is captured because as tests run, the current directory may change,
        /// and we want to be sure we know the original current directory because xunit
        /// sets it to be the test project's output directory, where many of our test assets are.
        /// </remarks>
        private static readonly string InitialCurrentDirectory = Environment.CurrentDirectory;

        public static ProjectRootElement FromTemplate()
        {
            return GetResourceProjectRootElement("FromTemplate.nuproj");
        }

        public static string NuProjImportsDirectory
        {
            get { return InitialCurrentDirectory; }
        }

        public static string BuildOutputDirectory
        {
            get { return InitialCurrentDirectory; }
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
