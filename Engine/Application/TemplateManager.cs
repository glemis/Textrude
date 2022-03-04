using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;

namespace Engine.Application
{
    /// <summary>
    ///     Wraps the Scriban template engine to make it easier to use
    /// </summary>
    public class TemplateManager
    {
        /// <summary>
        ///     underlying template context
        /// </summary>
        internal TemplateContext _context = new();


        /// <summary>
        ///     loader for included scripts
        /// </summary>
        internal readonly ITemplateLoader _templateLoader;

        /// <summary>
        ///     The top-most scriptObject in the context
        /// </summary>
        private readonly ScriptObject _top = new();

        /// <summary>
        ///     The template after compilation
        /// </summary>
        private Template _compiledTemplate;

        /// <summary>
        ///     Output after render pass
        /// </summary>
        private string _output = string.Empty;

        public TemplateManager(IFileSystemOperations ops)
        {
            _context.StrictVariables = true;
            _context.LoopLimit = int.MaxValue;
            _context.ObjectRecursionLimit = 100;
            _templateLoader = new ScriptLoader(ops);
            _context.TemplateLoader = _templateLoader;
            _top.Add("templateManager", "TemplateManager:FileSystem:ScriptLoader");
            _context.PushGlobal(_top);
        }

        /// <summary>
        /// Create Template manager with defined template loader passed in.
        /// </summary>
        /// <param name="loader"></param>
        public TemplateManager(IFileSystemOperations ops, ITemplateLoader loader)
        {
            _context.StrictVariables = false;
            _context.LoopLimit = int.MaxValue;
            _context.ObjectRecursionLimit = 100;
            _templateLoader = loader;

            _context.TemplateLoader = _templateLoader;
            _top.Add("templateManager", $"TemplateManager:Custom:{loader.GetType().ToString()}");

            _context.PushGlobal(_top);
        }

        internal void WithConfiguration(Action<TemplateContext> config)
        {
            _context?.Invoke(config);
        }

        /// <summary>
        ///     Cumulative list of errors from template operations
        /// </summary>
        public ImmutableArray<string> ErrorList { get; private set; } = ImmutableArray<string>.Empty;

        /// <summary>
        ///     Run the render pass and capture any errors
        /// </summary>
        public string Render() => Render(CancellationToken.None);

        public string Render(CancellationToken cancel)
        {
            if (ErrorList.Any())
                return string.Empty;

            _context.CancellationToken = cancel;
            _output = _compiledTemplate.Render(_context);
            if (_compiledTemplate.HasErrors)
                ErrorList = ErrorList
                    .Concat(_compiledTemplate.Messages.Select(e => $"ERROR: {e}"))
                    .ToImmutableArray();

            return _output;
        }

        /// <summary>
        ///     Compile a template
        /// </summary>
        /// <remarks>
        ///     This should not be able to throw an exception but we want to ensure that we catch any Scriban bugs
        /// </remarks>
        public void SetTemplate(string templateText)
        {
            try
            {
                _compiledTemplate = Template.Parse(templateText);

                ErrorList = _compiledTemplate
                    .Messages
                    .Select(e => $"ERROR: {e}")
                    .ToImmutableArray();
            }
            catch (Exception e)
            {
                ErrorList = ErrorList.Add($"SCRIBAN INTERNAL EXCEPTION: {e}");
            }
        }

        /// <summary>
        ///     Adds a script object to top.
        ///     Intention is to import delegate function classes that inherit from ScriptObject to global
        /// </summary>
        /// <param name="import"></param>
        public virtual void ImportScriptObjectToTop(ScriptObject import)
        {
            _top.Import(import);
        }


        /// <summary>
        ///    Sets a readonly variable to the context
        /// </summary>
        /// <remarks>
        ///    Primary use is adding a function implementing IScriptCustomFunction much like include which is callable but cannot be modified
        /// </remarks>
        public virtual void AddReadonlyVariable(string name, object val)
        {
            _top.SetValue(name, val, true);
        }

        /// <summary>
        ///     Adds a variable to the context
        /// </summary>
        public virtual void AddVariable(string name, object val)
        {
            _top.Add(name, val);
        }

        /// <summary>
        ///     Adds an include path to the ScriptLoader used by the template engine
        ///     If template loader is not script loader throw error
        /// </summary>
        public virtual void AddIncludePath(string path)
        {
            if (_templateLoader.GetType() != typeof(ScriptLoader))
            {
                throw new InvalidOperationException("Calling AddIncludePath on Template manager not supported unless managers template loader was set to type 'ScriptLoader'");
            }
            ((ScriptLoader)_templateLoader).AddIncludePath(path);
        }

        /// <summary>
        ///     Attempts to read the value of a global variable from the engine
        /// </summary>
        /// <remarks>
        ///     If the variable doesn't exist, we return the empty string
        /// </remarks>
        public string GetStringOrEmpty(string variableName)
        {
            var scribanVariable = new ScriptVariableGlobal(variableName);
            try
            {
                return _context.GetValue(scribanVariable)?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool TryGetVariableObject<T>(string variableName, out T val)
        {
            val = default;
            var scribanVariable = new ScriptVariableGlobal(variableName);
            try
            {
                val = (T)_context.GetValue(scribanVariable);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual ImmutableArray<ModelPath> PathsForObjectTree(IDictionary<string, object> container,
            ModelPath prefix)
        {
            var ret = new List<ModelPath>();


            foreach (var keyValuePair in container)
            {
                //hide library objects
                if (keyValuePair.Key.StartsWith("__"))
                    continue;
                //we need to be careful here - there is nothing to stop the user
                //deliberately or accidentally injecting null values into the model
                var type = keyValuePair.Value?.GetType()?.FullName ?? string.Empty;
                var mType = ModelPath.PathType.Property;
                if (type.Contains("Scriban") && type.Contains("Function"))
                    mType = ModelPath.PathType.Method;


                var p = prefix.WithChild(keyValuePair.Key).WithType(mType);
                if (keyValuePair.Value is IDictionary<string, object> child)
                    ret.AddRange(PathsForObjectTree(child, p));
                else ret.Add(p);
            }

            return ret.ToImmutableArray();
        }

        public virtual ImmutableArray<ModelPath> GetBuiltIns() => PathsForObjectTree(_context.BuiltinObject, ModelPath.Empty);
        public virtual ImmutableArray<ModelPath> GetObjectTree() => PathsForObjectTree(_top, ModelPath.Empty);

        public virtual ImmutableArray<ModelPath> GetKeywords()
        {
            var keywords =
                @"func end if else for break continue
                  in  while capture readonly import
                  with wrap include ret case  when this
                empty tablerow";
            return keywords.Split(" \r\n".ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => ModelPath.FromString(s).WithType(ModelPath.PathType.Keyword))
                .ToImmutableArray();
        }

        public virtual ImmutableArray<ModelPath> ModelPaths() =>
            GetBuiltIns()
                .Concat(GetObjectTree())
                .Concat(GetKeywords())
                .ToImmutableArray();
    }
}
