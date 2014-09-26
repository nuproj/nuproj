using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuProj.Tests
{
    public class TestsHelper
    {
        private static readonly object restoreTasksLock = new object();

        private static readonly Dictionary<string, Task> restorePackagesTasks = new Dictionary<string, Task>();
        
        public static void RestorePackages(string path)
        {
            Task restorePackagesTask;
            lock (restoreTasksLock)
            {
                path = path.ToLowerInvariant();
                if (!restorePackagesTasks.ContainsKey(path))
                {
                    restorePackagesTasks[path] = Task.Run(()=>
                    {
                        NuGetExeRestore(path);
                    });
                }

                restorePackagesTask = restorePackagesTasks[path];
            }

            restorePackagesTask.Wait();
        }

        private static void NuGetExeRestore(string path)
        {
            var testOutDir = Directory.GetCurrentDirectory();
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = Path.Combine(testOutDir, "nuget.exe"),
                Arguments = "restore",
                WorkingDirectory = path,
                UseShellExecute = false,
            };

            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

        }

        public static string GetScenarioDirectory(string scenarioName)
        {
            var solutionDir = @"..\..\" + scenarioName;
            return Path.GetFullPath(solutionDir);

        }
    }
}
