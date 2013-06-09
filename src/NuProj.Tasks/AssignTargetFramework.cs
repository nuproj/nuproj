using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using NuGet;

namespace NuProj.Tasks
{
    public class AssignTargetFramework : Task
    {
        [Required]
        public ITaskItem[] OutputsWithTargetFrameworkInformation { get; set; }

        [Output]
        public ITaskItem[] Libraries { get; set; }

        public override bool Execute()
        {
            var seenPackagePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Libraries = (from item in OutputsWithTargetFrameworkInformation.Select(ConvertToLibrary)
                         let packagePath = item.GetMetadata("TargetPath")
                         where seenPackagePaths.Add(packagePath)
                         select item).ToArray();

            return true;
        }

        private static ITaskItem ConvertToLibrary(ITaskItem output)
        {
            var fileName = output.ItemSpec;
            var frameworkName = new FrameworkName(output.GetMetadata("TargetFrameworkMoniker"));
            var frameworkString = VersionUtility.GetShortFrameworkName(frameworkName);
            var metadata = output.CloneCustomMetadata();
            metadata["TargetFramework"] = frameworkString;
            metadata["TargetPath"] = Path.Combine(frameworkString, Path.GetFileName(fileName));
            return new TaskItem(fileName, metadata);
        }
    }
}