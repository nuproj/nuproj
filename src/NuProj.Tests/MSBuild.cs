namespace NuProj.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Build.Execution;
    using Microsoft.Build.Framework;

    public static class MSBuild
    {
        public static Task<BuildResult> RebuildAsync(string projectPath, Action<BuildErrorEventArgs> onError = null)
        {
            return MSBuild.ExecuteAsync(projectPath, new[] { "Rebuild" }, onError);
        }

        public static Task<BuildResult> ExecuteAsync(string projectPath, string targetToBuild, Action<BuildErrorEventArgs> onError = null)
        {
            return MSBuild.ExecuteAsync(projectPath, new[] { targetToBuild }, onError);
        }

        public static async Task<BuildResult> ExecuteAsync(string projectPath, string[] targetsToBuild, Action<BuildErrorEventArgs> onError = null)
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

            var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            BuildResult result;
            using (var buildManager = new BuildManager())
            {
                buildManager.BeginBuild(parameters);

                var requestData = new BuildRequestData(projectPath, properties, null, targetsToBuild, null);
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
