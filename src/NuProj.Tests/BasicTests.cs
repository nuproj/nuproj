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
        [Fact(Skip = "Test not finished yet.")]
        public async Task ProjectTemplatCanBuild()
        {
            var nuproj = Assets.FromTemplate()
                .AssignNuProjDirectory()
                .ToProject();

            var result = await MSBuild.ExecuteAsync(nuproj.CreateProjectInstance());
            result.AssertSuccessfulBuild();
        }

        [Fact(Skip = "Test fails because GenerateNuSpec target is skipped when no inputs are declared in the project.")]
        public async Task EmptyProjectCanBuild()
        {
            var nuproj = Assets.FromTemplate()
                .AssignNuProjDirectory()
                .ToProject();

            // This test focuses on a completely empty project.
            nuproj.RemoveItems(nuproj.GetItems("Content"));

            var result = await MSBuild.ExecuteAsync(nuproj.CreateProjectInstance());
            result.AssertSuccessfulBuild();
        }
    }
}
