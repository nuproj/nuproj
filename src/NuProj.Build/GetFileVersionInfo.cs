using System;
using System.Diagnostics;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NuProj.Build
{
    public sealed class GetFileVersionInfo : Task
    {
        [Required]
        public ITaskItem File { get; set; }

        [Output]
        public string CompanyName { get; set; }

        [Output]
        public int FileBuildPart { get; set; }

        [Output]
        public string FileDescription { get; set; }

        [Output]
        public int FileMajorPart { get; set; }

        [Output]
        public int FileMinorPart { get; set; }

        [Output]
        public int FilePrivatePart { get; set; }

        [Output]
        public string FileVersion { get; set; }

        [Output]
        public string InternalName { get; set; }

        [Output]
        public bool IsDebug { get; set; }

        [Output]
        public bool IsPatched { get; set; }

        [Output]
        public bool IsPreRelease { get; set; }

        [Output]
        public bool IsPrivateBuild { get; set; }

        [Output]
        public bool IsSpecialBuild { get; set; }

        [Output]
        public string Language { get; set; }

        [Output]
        public string LegalCopyright { get; set; }

        [Output]
        public string LegalTrademarks { get; set; }

        [Output]
        public string OriginalFilename { get; set; }

        [Output]
        public string PrivateBuild { get; set; }

        [Output]
        public int ProductBuildPart { get; set; }

        [Output]
        public int ProductMajorPart { get; set; }

        [Output]
        public int ProductMinorPart { get; set; }

        [Output]
        public string ProductName { get; set; }

        [Output]
        public int ProductPrivatePart { get; set; }

        [Output]
        public string ProductVersion { get; set; }

        [Output]
        public string SpecialBuild { get; set; }

        [Output]
        public string Comments { get; set; }

        public override bool Execute()
        {
            try
            {
                var info = FileVersionInfo.GetVersionInfo(File.ItemSpec);
                Comments = info.Comments;
                CompanyName = info.CompanyName;
                FileBuildPart = info.FileBuildPart;
                FileDescription = info.FileDescription;
                FileMajorPart = info.FileMajorPart;
                FileMinorPart = info.FileMinorPart;
                FilePrivatePart = info.FilePrivatePart;
                FileVersion = info.FileVersion;
                InternalName = info.InternalName;
                IsDebug = info.IsDebug;
                IsPatched = info.IsPatched;
                IsPreRelease = info.IsPreRelease;
                IsPrivateBuild = info.IsPrivateBuild;
                IsSpecialBuild = info.IsSpecialBuild;
                Language = info.Language;
                LegalCopyright = info.LegalCopyright;
                LegalTrademarks = info.LegalTrademarks;
                OriginalFilename = info.OriginalFilename;
                PrivateBuild = info.PrivateBuild;
                ProductBuildPart = info.ProductBuildPart;
                ProductMajorPart = info.ProductMajorPart;
                ProductMinorPart = info.ProductMinorPart;
                ProductName = info.ProductName;
                ProductPrivatePart = info.ProductPrivatePart;
                ProductVersion = info.ProductVersion;
                SpecialBuild = info.SpecialBuild;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
            }

            return !Log.HasLoggedErrors;
        }
    }
}