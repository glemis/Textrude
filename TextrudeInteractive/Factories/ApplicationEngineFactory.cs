using Engine.Application;
using Scriban.Runtime;
using System;
using System.Threading;

namespace TextrudeInteractive
{
    public class ApplicationEngineFactory
    {

        private readonly IRunTimeEnvironment _environment;
        private readonly TemplateManagerFactory _templateManagerFactory;

        public ApplicationEngineFactory(IRunTimeEnvironment _environment, TemplateManagerFactory _templateManagerFactory)
        {
            this._environment = _environment;
            this._templateManagerFactory = _templateManagerFactory;
        }
        public ApplicationEngine Create<TTempalteManager> (CancellationToken cancel) where TTempalteManager : ITemplateManager
        {
            return new ApplicationEngine(_templateManagerFactory.Create<TTempalteManager>(), _environment, cancel);
        }
    }
}
