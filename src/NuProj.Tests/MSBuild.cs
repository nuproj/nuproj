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
    using Microsoft.Build.Logging;

    public static class MSBuild
    {
        public static Task<BuildResultAndLogs> RebuildAsync(string projectPath, IDictionary<string, string> properties = null)
        {
            return MSBuild.ExecuteAsync(projectPath, new[] { "Rebuild" }, properties);
        }

        public static Task<BuildResultAndLogs> ExecuteAsync(string projectPath, string targetToBuild, IDictionary<string, string> properties = null)
        {
            return MSBuild.ExecuteAsync(projectPath, new[] { targetToBuild }, properties);
        }

        /// <summary>
        /// Builds a project.
        /// </summary>
        /// <param name="projectPath">The absolute path to the project.</param>
        /// <param name="targetsToBuild">The targets to build. If not specified, the project's default target will be invoked.</param>
        /// <param name="properties">The optional global properties to pass to the project. May come from the <see cref="MSBuild.Properties"/> static class.</param>
        /// <returns>A task whose result is the result of the build.</returns>
        public static async Task<BuildResultAndLogs> ExecuteAsync(string projectPath, string[] targetsToBuild = null, IDictionary<string, string> properties = null)
        {
            targetsToBuild = targetsToBuild ?? new string[0];

            var logger = new EventLogger();
            var logLines = new List<string>();
            var parameters = new BuildParameters
            {
                Loggers = new List<ILogger>
                {
                    new ConsoleLogger(LoggerVerbosity.Detailed, logLines.Add, null, null),
                    logger,
                },
            };

            BuildResult result;
            using (var buildManager = new BuildManager())
            {
                buildManager.BeginBuild(parameters);
                try
                {
                    var requestData = new BuildRequestData(projectPath, properties ?? Properties.Empty, null, targetsToBuild, null);
                    var submission = buildManager.PendBuildRequest(requestData);
                    result = await submission.ExecuteAsync();
                }
                finally
                {
                    buildManager.EndBuild();
                }
            }

            return new BuildResultAndLogs(result, logger.LogEvents, logLines);
        }

        /// <summary>
        /// Builds a project.
        /// </summary>
        /// <param name="projectInstance">The project to build.</param>
        /// <param name="targetsToBuild">The targets to build. If not specified, the project's default target will be invoked.</param>
        /// <param name="properties">The optional global properties to pass to the project. May come from the <see cref="MSBuild.Properties"/> static class.</param>
        /// <returns>A task whose result is the result of the build.</returns>
        public static async Task<BuildResultAndLogs> ExecuteAsync(ProjectInstance projectInstance, string[] targetsToBuild = null)
        {
            targetsToBuild = targetsToBuild ?? projectInstance.DefaultTargets.ToArray();

            var logger = new EventLogger();
            var logLines = new List<string>();
            var parameters = new BuildParameters
            {
                Loggers = new List<ILogger>
                {
                    new ConsoleLogger(LoggerVerbosity.Detailed, logLines.Add, null, null),
                    logger,
                },
            };

            BuildResult result;
            using (var buildManager = new BuildManager())
            {
                buildManager.BeginBuild(parameters);
                try
                {
                    var requestData = new BuildRequestData(projectInstance, targetsToBuild);
                    var submission = buildManager.PendBuildRequest(requestData);
                    result = await submission.ExecuteAsync();
                }
                finally
                {
                    buildManager.EndBuild();
                }
            }

            return new BuildResultAndLogs(result, logger.LogEvents, logLines);
        }

        private static Task<BuildResult> ExecuteAsync(this BuildSubmission submission)
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

        public class BuildResultAndLogs
        {
            internal BuildResultAndLogs(BuildResult result, List<BuildEventArgs> events, IReadOnlyList<string> logLines)
            {
                this.Result = result;
                this.LogEvents = events;
                this.LogLines = logLines;
            }

            public BuildResult Result { get; private set; }

            public List<BuildEventArgs> LogEvents { get; private set; }

            public IEnumerable<BuildErrorEventArgs> ErrorEvents
            {
                get { return this.LogEvents.OfType<BuildErrorEventArgs>(); }
            }

            public IReadOnlyList<string> LogLines { get; private set; }

            public string EntireLog
            {
                get { return string.Join(string.Empty, this.LogLines); }
            }
        }

        private class EventLogger : ILogger
        {
            private IEventSource eventSource;

            internal EventLogger()
            {
                this.Verbosity = LoggerVerbosity.Normal;
                this.LogEvents = new List<BuildEventArgs>();
            }

            public LoggerVerbosity Verbosity { get; set; }

            public string Parameters { get; set; }

            public List<BuildEventArgs> LogEvents { get; set; }

            public void Initialize(IEventSource eventSource)
            {
                this.eventSource = eventSource;
                this.eventSource.AnyEventRaised += this.eventSource_AnyEventRaised;
            }

            private void eventSource_AnyEventRaised(object sender, BuildEventArgs e)
            {
                this.LogEvents.Add(e);
            }

            public void Shutdown()
            {
                this.eventSource.AnyEventRaised -= this.eventSource_AnyEventRaised;
            }
        }
    }
}
