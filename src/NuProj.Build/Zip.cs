using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NuProj.Build
{
    public sealed class Zip : Task
    {
        [Required]
        public ITaskItem ZipFileName { get; set; }

        [Required]
        public ITaskItem[] Files { get; set; }

        public override bool Execute()
        {
            try
            {
                var zipFileName = ZipFileName.ItemSpec;
                var fileNames = Files.Select(f => f.ItemSpec);
                CreateZipArchive(zipFileName, fileNames);

            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
            }

            return !Log.HasLoggedErrors;
        }

        private static void CreateZipArchive(string zipFileName, IEnumerable<string> fileNames)
        {
            using (var fileStream = File.Create(zipFileName))
            using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                foreach (var fileName in fileNames)
                    zipArchive.CreateEntryFromFile(fileName, Path.GetFileName(fileName), CompressionLevel.Optimal);
            }
        }
    }
}