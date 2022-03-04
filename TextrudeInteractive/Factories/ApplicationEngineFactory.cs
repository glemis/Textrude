using Engine.Application;
using OLA.DataAccess;
using OLA.ScribanMethodsObject;
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
                .WithConfiguration(new TemplateContext() { EnableNullIndexer = true, LoopLimit = 2000, RecursiveLimit = 0, ObjectRecursionLimit = 100, StrictVariables = false })
                .WithEnvironmentVariables()
                .WithHelpers()
                    .ImportMethods(new DataverseData())
                    .ImportMethods(new ObjectArrayMethods())
                    .ImportMethods(new DateMethods())
                    .ImportMethods(new LanguageMethods())
                    .ImportMethods(new NumberToWordMethods())
                    .ImportMethods(new OrdanalMethods())
                    .ImportMethods(new TypeConversionMethods());

            return applicationEngine;
        }
    }
}
