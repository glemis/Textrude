using Engine.Application;
using Scriban.Runtime;
using System;

namespace TextrudeInteractive
{
    public class TemplateManagerFactory
    {

        private readonly IFileSystemOperations _ops;
        private readonly ITemplateLoader _loader;

        public TemplateManagerFactory(IFileSystemOperations _ops, ITemplateLoader _loader)
        {
            this._ops = _ops;
            this._loader = _loader;
        }
        public ITemplateManager Create<T> () where T : ITemplateManager
        {
            if (typeof(T) == typeof(TemplateManager))
            {
                return (ITemplateManager)new TemplateManager(_ops, _loader);
            }
            else
            {
                throw new NotImplementedException(String.Format("Creation of {0} interface is not supported yet.", typeof(T)));
            }
        }
    }
}
