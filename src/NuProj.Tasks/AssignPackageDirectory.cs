using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;

namespace NuProj.Tasks
{
    public class AssignPackageDirectory : Task
    {
        [Required]
        public ITaskItem[] Files { get; set; }

        [Output]
        public ITaskItem[] FilesWithPackageDirectory { get; private set; }

        public override bool Execute()
        {
            FilesWithPackageDirectory = Files?
                .Select(SetPackageDirectory)
                .ToArray();

            return !Log.HasLoggedErrors;
        }

        private ITaskItem SetPackageDirectory(ITaskItem taskItem)
        {
            PackageDirectory targetPackageDirectory;
            string targetSubdirectory;
            taskItem.GetTargetPackageDirectory(out targetPackageDirectory, out targetSubdirectory);

            var packageDirectory = taskItem.GetPackageDirectory(targetPackageDirectory);

            if (packageDirectory != targetPackageDirectory)
            {
                Log.LogError($"File '{taskItem.ItemSpec}' has unexpected PackageDirectory metadata. Expected '{targetPackageDirectory}', actual '{packageDirectory}'.");
            }

            taskItem.SetMetadata(Metadata.PackageDirectory, targetPackageDirectory.ToString());
            return taskItem;

        }
    }
}
