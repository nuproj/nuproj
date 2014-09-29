namespace NuProj.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Build.Evaluation;
    using Xunit;

    public static class NuPkg
    {
        public const string NuSpecXmlNamespace = @"http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd";

        public static XElement ExtractNuSpecFromPackage(Project nuProj)
        {
            using (var archive = GetArchive(nuProj))
            {
                ZipArchiveEntry nuspec = archive.GetEntry(nuProj.GetPropertyValue("Id") + ".nuspec");
                Assert.NotNull(nuspec);
                using (var nuspecStream = nuspec.Open())
                {
                    var xmlReader = XmlReader.Create(nuspecStream);
                    xmlReader.MoveToContent();
                    return (XElement)XNode.ReadFrom(xmlReader);
                }
            }
        }

        public static ZipArchive GetArchive(string nupkgPath)
        {
            var nupkgStream = File.OpenRead(nupkgPath);
            return new ZipArchive(nupkgStream, ZipArchiveMode.Read, leaveOpen: false);
        }

        public static ZipArchive GetArchive(Project nuProj)
        {
            return GetArchive(nuProj.GetNuPkgPath());
        }
    }
}
