using System;

using Microsoft.Collections.Immutable;
using Microsoft.VisualStudio.ProjectSystem.Utilities;

namespace NuProj.ProjectSystem
{
    internal static class Capabilities
    {
        public const string NuProj = "NuProj";

        public static readonly ImmutableHashSet<string> All = new[]
        {
            NuProj,
            ProjectCapabilities.ReferencesFolder,
            ProjectCapabilities.Cps
        }.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
