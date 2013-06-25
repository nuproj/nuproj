using System;

using Microsoft.Collections.Immutable;
using Microsoft.VisualStudio.ProjectSystem.Utilities;

namespace NuProj.ProjectSystem
{
    internal static class NuProjCapabilities
    {
        public const string NuProj = "NuProj";

        public static readonly ImmutableHashSet<string> ProjectSystem = new[]
        {
            NuProj,
            ProjectCapabilities.ReferencesFolder,
            ProjectCapabilities.Cps
        }.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
