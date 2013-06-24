using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Microsoft.Collections.Immutable;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Designers;
using Microsoft.VisualStudio.ProjectSystem.Utilities;

namespace NuProj.ProjectSystem
{
    [Export(typeof(IValidProjectReferenceChecker))]
    [OrderPrecedence(1000)]
    [PartMetadata(ProjectCapabilities.Requires, Capabilities.NuProj)]
    internal sealed class ValidProjectReferenceChecker : IValidProjectReferenceChecker
    {
        // This import must be present so that this part applies to a specific project.
        [Import]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public UnconfiguredProject UnconfiguredProject { get; set; }

        public Task<SupportedCheckResult> CanAddProjectReferenceAsync(object referencedProject)
        {
            return Task.FromResult(SupportedCheckResult.Unknown);
        }

        public Task<CanAddProjectReferencesResult> CanAddProjectReferencesAsync(IImmutableSet<object> referencedProjects)
        {
            if (referencedProjects == null)
                throw new ArgumentNullException("referencedProjects");

            IImmutableMap<object, SupportedCheckResult> results = ImmutableDictionary<object, SupportedCheckResult>.Empty;

            foreach (object referencedProject in referencedProjects)
            {
                results = results.Add(referencedProject, SupportedCheckResult.Unknown);
            }

            return Task.FromResult(new CanAddProjectReferencesResult(results, null));
        }

        public Task<SupportedCheckResult> CanBeReferencedAsync(object referencingProject)
        {
            return Task.FromResult(SupportedCheckResult.NotSupported);
        }
    }
}
