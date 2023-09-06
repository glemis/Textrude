using Engine.Application;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Threading;

namespace TextrudeInteractive
{
    public class ApplicationEngineFactory
    {
        private readonly ITemplateLoader _loader;
        private readonly RunTimeEnvironment _environment;

        public ApplicationEngineFactory(ITemplateLoader _loader)
        {
            this._loader = _loader;
            this._environment = new RunTimeEnvironment(new FileSystem());
        }
        public ApplicationEngine Create(CancellationToken cancel)
        {
            var applicationEngine = new ApplicationEngine(_loader, _environment, cancel)
                .WithTemplateManagerConfiguration(context =>
                {
                    context.EnableNullIndexer = true;
                    context.LoopLimit = 2000;
                    context.RecursiveLimit = 0;
                    context.ObjectRecursionLimit = 100;
                    context.StrictVariables = false;
                })
                .WithEnvironmentVariables()
                .WithHelpers();

            return applicationEngine;
        }
    }
}
