using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NuProj.Tasks
{
    public class AssignSourceTargetPaths : Task
    {
        [Required]
        public string[] SourceFiles { get; set; }

        [Output]
        public ITaskItem[] SourceFilesWithTargetPath { get; set; }

        public override bool Execute()
        {
            var targetPaths = GetTargetPaths(SourceFiles);
            SourceFilesWithTargetPath = new ITaskItem[targetPaths.Count];

            for (var i = 0; i < SourceFiles.Length; i++)
            {
                var sourcePath = SourceFiles[i];
                var targetPath = targetPaths[i];
                var metadata = new Dictionary<string, string>()
                {
                    {"TargetPath", targetPath}
                };
                var item = new TaskItem(sourcePath, metadata);
                SourceFilesWithTargetPath[i] = item;
            }

            return true;
        }

        private static IReadOnlyList<string> GetTargetPaths(IEnumerable<string> paths)
        {
            var normalizedPaths = GetNormalizedPaths(paths);
            var commonRoot = GetCommonRoot(normalizedPaths);
            return normalizedPaths.Select(p => GetTargetPath(commonRoot, p)).ToArray();
        }

        private static string GetTargetPath(string commonRoot, string path)
        {
            const string srcFolderName = "src";

            if (!string.IsNullOrEmpty(commonRoot))
                return Path.Combine(srcFolderName, path.Substring(commonRoot.Length));

            var root = Path.GetPathRoot(path);
            var fixedRoot = root.Replace(":", string.Empty);
            var remainder = path.Substring(root.Length);
            return Path.Combine(srcFolderName, fixedRoot, remainder);
        }

        private static string[] GetNormalizedPaths(IEnumerable<string> paths)
        {
            var normalizedPaths = paths.Select(Path.GetFullPath).ToArray();
            return normalizedPaths;
        }

        private static string GetCommonRoot(IReadOnlyCollection<string> normalizedPaths)
        {
            if (normalizedPaths.Count == 0)
                return string.Empty;

            var maxPathLength = normalizedPaths.Max(s => s.Length);
            var longestPath = normalizedPaths.First(p => p.Length == maxPathLength)
                .Split(Path.DirectorySeparatorChar)
                .Select(p => p + Path.DirectorySeparatorChar);

            var result = string.Empty;

            foreach (var path in longestPath)
            {
                // If this is the first path segment we need check whether it's a valid
                // prefix of all paths.
                //
                // If it's not then it means there is no common root.

                if (result.Length == 0 && normalizedPaths.All(p => p.StartsWith(path)))
                {
                    result = path;
                    continue;
                }

                // We already have a path.
                //
                // In that case the combined path must be a valid prefix.

                var fullPath = Path.Combine(result, path);
                if (normalizedPaths.All(p => p.StartsWith(fullPath)))
                {
                    result = fullPath;
                    continue;
                }

                break;
            }

            return result;
        }
    }
}