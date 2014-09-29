namespace NuProj.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Build.Construction;
    using Microsoft.Build.Evaluation;

    public static class ProjectBuilder
    {
        private static string NuProjTargetsDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static ProjectRootElement AssignNuProjDirectory(this ProjectRootElement nuProj)
        {
            var projectDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(projectDirectory);
            nuProj.FullPath = Path.Combine(projectDirectory, "test.nuproj");
            return nuProj;
        }

        public static Project ToProject(this ProjectRootElement nuProj)
        {
            var project = new Project(
                nuProj,
                MSBuild.Properties.Default,
                toolsVersion: null);
            return project;
        }

        public static Project CreateMockContentFiles(this Project nuProj)
        {
            var random = new Random();
            foreach (var item in nuProj.GetItems("Content"))
            {
                byte[] randomData = new byte[10];
                random.NextBytes(randomData);
                File.WriteAllText(item.GetMetadataValue("FullPath"), Convert.ToBase64String(randomData));
            }

            return nuProj;
        }

        public static void Cleanup(Project nuProj)
        {
            Directory.Delete(Path.GetDirectoryName(nuProj.FullPath), recursive: true);
        }

        public static string GetNuPkgPath(this Project nuProj)
        {
            return nuProj.GetPropertyValue("NuGetOutputPath");
        }
    }
}
