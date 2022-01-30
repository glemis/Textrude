using Engine.Application;
using Scriban.Runtime;
using System;
using System.Threading;

namespace TextrudeInteractive
{
    public class ApplicationEngineFactory
    {

        private readonly IFileSystemOperations _ops;
        private readonly ITemplateLoader _loader;
        private readonly IRunTimeEnvironment _environment;

        public ApplicationEngineFactory()
        {
            this._ops = new FileSystem();
            this._environment = new RunTimeEnvironment(this._ops);
            this._loader = new ScriptLoader(this._ops);
        }
        public ApplicationEngine Create<TTempalteManager> (CancellationToken cancel) where TTempalteManager : ITemplateManager
        {
            TemplateManager templateManager = new(_ops, _loader);
            templateManager._context.LoopLimit = 500;
            templateManager._context.ObjectRecursionLimit = 50;
            templateManager._context.StrictVariables = false;
            return new ApplicationEngine(templateManager, _environment, cancel);
        }
    }
}
