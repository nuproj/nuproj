namespace NuProj.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Execution;
    using Microsoft.Build.Framework;
    using Xunit;

    public class BasicTests
    {
        [Fact(Skip = "Test fails because GenerateNuSpec target is skipped when no inputs are declared in the project.")]
        public async Task EmptyProjectCanBuild()
        {
            var pre = Assets.FromTemplate();
            var projectDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(projectDirectory);
            Directory.CreateDirectory(Path.Combine(projectDirectory, "obj", "Debug"));
            Directory.CreateDirectory(Path.Combine(projectDirectory, "bin", "Debug"));
            pre.FullPath = Path.Combine(projectDirectory, "test.nuproj");

            var errors = new List<object>();
            var project = new Project(pre, MSBuild.Properties.Empty.Add("NuProjPath", Environment.CurrentDirectory), null);
            
            // This test focuses on a completely empty project.
            project.RemoveItems(project.GetItems("Content"));

            var result = await MSBuild.ExecuteAsync(project.CreateProjectInstance());
            Assert.Equal(0, result.ErrorEvents.Count());
            Assert.Equal(BuildResultCode.Success, result.Result.OverallResult);
        }
    }
}
