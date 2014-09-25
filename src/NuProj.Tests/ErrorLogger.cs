using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuProj.Tests
{
    internal class ErrorLogger : ILogger
    {
        private IEventSource eventSource;
        private List<BuildErrorEventArgs> errors;

        public void Initialize(IEventSource eventSource)
        {
            this.eventSource = eventSource;
            this.errors = new List<BuildErrorEventArgs>();
            this.eventSource.ErrorRaised += eventSource_ErrorRaised;
        }

        private void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            this.errors.Add(e);
        }

        public string Parameters { get; set; }

        public void Shutdown()
        {
        }

        public LoggerVerbosity Verbosity { get; set; }

        public IReadOnlyCollection<BuildErrorEventArgs> Errors
        {
            get
            {
                return this.errors;
            }
        }
    }
}
