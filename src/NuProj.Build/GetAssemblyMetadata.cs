using System;
using System.Linq;
using System.Reflection;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NuProj.Build
{
    public sealed class GetAssemblyMetadata : Task
    {
        [Required]
        public ITaskItem File { get; set; }

        [Required]
        public string AssemblyMetadataKey { get; set; }

        [Output]
        public string AssemblyMetadataValue { get; set; }

        public override bool Execute()
        {
            try
            {
                var assembly = Assembly.LoadFrom(File.ItemSpec);
                var type = typeof (AssemblyMetadataAttribute);
                AssemblyMetadataValue = (from ca in assembly.GetCustomAttributesData()
                                         where ca.Constructor.DeclaringType == type && ca.ConstructorArguments.Count == 2
                                         let key = ca.ConstructorArguments[0].Value as string
                                         let value = ca.ConstructorArguments[1].Value as string
                                         where
                                             key != null &&
                                             value != null &&
                                             string.Equals(key, AssemblyMetadataKey, StringComparison.OrdinalIgnoreCase)
                                         select value).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
            }

            return !Log.HasLoggedErrors;
        }
    }
}