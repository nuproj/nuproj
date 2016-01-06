namespace NuProj.Tasks
{
    public static class Metadata
    {
        public const string FileExclude = "Exclude";
        
        public const string FileSource = "FullPath";
        
        public const string FileTarget = "TargetPath";

        public const string PackageDirectory = "PackageDirectory";

        public const string TargetSubdirectory = "TargetSubdirectory";

        public const string TargetFramework = "TargetFramework";

        public const string TargetFrameworkMoniker = "TargetFrameworkMoniker";

        public const string Version = "Version";

        public static class ContentFile
        {
            public const string BuildAction = "PackageBuildAction";

            public const string CopyToOutput = "PackageCopyToOutput";

            public const string Flatten = "PackageFlatten";
        }
    }
}