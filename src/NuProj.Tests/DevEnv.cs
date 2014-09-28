using EnvDTE;
using EnvDTE80;
using System;
using System.Linq;

namespace NuProj.Tests
{
    public class DevEnv : IDisposable
    {
        private bool disposed;

        private DTE2 dte;

        public DTE2 Dte
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("DevEnv");
                }

                return this.dte;
            }
        }

        public DevEnv()
        {
            Type visualStudioType = Type.GetTypeFromProgID("VisualStudio.DTE.12.0");
            this.dte = Activator.CreateInstance(visualStudioType) as DTE2;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.dte.Quit();
            this.dte = null;
            this.disposed = true;
        }


        public void OpenSolution(string fileName)
        {
            this.Dte.Solution.Open(fileName);
        }

        public void ActivateConfiguration(
           string configuration = "Debug",
           string platform = "Any CPU")
        {
            SolutionConfigurations solutionConfigurations = this.Dte.Solution.SolutionBuild.SolutionConfigurations;
            
            var solutionConfiguration = solutionConfigurations
                .OfType<SolutionConfiguration2>()
                .Where(x => x.Name == configuration && x.PlatformName == platform)
                .First();

            if (solutionConfiguration == null)
            {
                throw new ArgumentException("Invalid configuration or platform name.");
            }

            solutionConfiguration.Activate();

        }

        public void Build()
        {
            this.Dte.Solution.SolutionBuild.Build(true);
        }

        public void Clean()
        {
            this.Dte.Solution.SolutionBuild.Clean(true);
        }
    }
}