using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuProj.Tests
{
    public static class NuGetHelper
    {
        private static readonly object restoreTasksLock = new object();

        private static readonly Dictionary<string, Task> restorePackagesTasks = new Dictionary<string, Task>(StringComparer.OrdinalIgnoreCase);

        public static Task RestorePackagesAsync(string path)
        {
            Task restorePackagesTask;
            lock (restoreTasksLock)
            {
                if (!restorePackagesTasks.TryGetValue(path, out restorePackagesTask))
                {
                    restorePackagesTask = NuGetExeRestoreAsync(path);
                    restorePackagesTasks[path] = restorePackagesTask;
                }
            }

            return restorePackagesTask;
        }

        private static Task<int> NuGetExeRestoreAsync(string path)
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
            process.EnableRaisingEvents = true;
            process.Start();

            var tcs = new TaskCompletionSource<int>();
            process.Exited += (sender, args) => tcs.TrySetResult(process.ExitCode);
            if (process.HasExited)
            {
                tcs.TrySetResult(process.ExitCode);
            }

            return tcs.Task;
        }

        public static string GetScenarioDirectory(string scenarioName)
        {
            return Path.Combine(Assets.BuildOutputDirectory, scenarioName);
        }
    }
}
