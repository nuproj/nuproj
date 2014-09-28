namespace NuProj.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Build.Execution;
    using Microsoft.Build.Framework;

    public static class MSBuild
    {
        public static Task<BuildResult> RebuildAsync(string projectPath, IDictionary<string, string> properties = null, Action<BuildErrorEventArgs> onError = null)
        {
            return MSBuild.ExecuteAsync(projectPath, new[] { "Rebuild" }, properties, onError);
        }

        public static Task<BuildResult> ExecuteAsync(string projectPath, string targetToBuild, IDictionary<string, string> properties = null, Action<BuildErrorEventArgs> onError = null)
        {
            return MSBuild.ExecuteAsync(projectPath, new[] { targetToBuild }, properties, onError);
        }

        /// <summary>
        /// Builds a project.
        /// </summary>
        /// <param name="projectPath">The absolute path to the project.</param>
        /// <param name="targetsToBuild">The targets to build. If not specified, the project's default target will be invoked.</param>
        /// <param name="properties">The optional global properties to pass to the project. May come from the <see cref="MSBuild.Properties"/> static class.</param>
        /// <param name="onError">An optional handler to receive errors logged during the build.</param>
        /// <returns>A task whose result is the result of the build.</returns>
        public static async Task<BuildResult> ExecuteAsync(string projectPath, string[] targetsToBuild = null, IDictionary<string, string> properties = null, Action<BuildErrorEventArgs> onError = null)
        {
            var loggers = new List<ILogger>();
            if (onError != null)
            {
                loggers.Add(new ErrorLogger(onError));
            }

            var parameters = new BuildParameters
            {
                Loggers = loggers,
            };

            BuildResult result;
            using (var buildManager = new BuildManager())
            {
                buildManager.BeginBuild(parameters);

                var requestData = new BuildRequestData(projectPath, properties ?? Properties.Empty, null, targetsToBuild, null);
                var submission = buildManager.PendBuildRequest(requestData);
                result = await submission.ExecuteAsync();
                buildManager.EndBuild();
            }

            return result;
        }

        public static Task<BuildResult> ExecuteAsync(this BuildSubmission submission)
        {
            var tcs = new TaskCompletionSource<BuildResult>();
            submission.ExecuteAsync(s => tcs.SetResult(s.BuildResult), null);
            return tcs.Task;
        }

        /// <summary>
        /// Common properties to pass to a build request.
        /// </summary>
        public static class Properties
        {
            /// <summary>
            /// No properties. The project will be built in its default configuration.
            /// </summary>
            public static readonly ImmutableDictionary<string, string> Empty = ImmutableDictionary.Create<string, string>(StringComparer.OrdinalIgnoreCase);

            /// <summary>
            /// The project will build in the same manner as if it were building inside Visual Studio.
            /// </summary>
            public static readonly ImmutableDictionary<string, string> BuildingInsideVisualStudio = Empty
                .Add("BuildingInsideVisualStudio", "true");
        }

        private class ErrorLogger : ILogger
        {
            private readonly Action<BuildErrorEventArgs> onError;
            private IEventSource eventSource;

            internal ErrorLogger(Action<BuildErrorEventArgs> onError)
            {
                this.onError = onError;
                this.Verbosity = LoggerVerbosity.Minimal;
            }

            public LoggerVerbosity Verbosity { get; set; }

            public string Parameters { get; set; }

            public void Initialize(IEventSource eventSource)
            {
                this.eventSource = eventSource;
                this.eventSource.ErrorRaised += this.eventSource_ErrorRaised;
            }

            public void Shutdown()
            {
                this.eventSource.ErrorRaised -= this.eventSource_ErrorRaised;
            }

            private void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
            {
                this.onError(e);
            }
        }
    }
}
