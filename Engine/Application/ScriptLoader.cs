﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Engine.Application
{
    /// <summary>
    ///     Locates and loads 'included' scripts for  the template
    /// </summary>
    public class ScriptLoader : ITemplateLoader
    {
        /// <summary>
        ///     The underlying file system
        /// </summary>
        private readonly IFileSystemOperations _filesystem;

        /// <summary>
        ///     List of folders in priority order
        /// </summary>
        private readonly List<string> _includePaths = new();

        internal readonly Dictionary<string,string> _includeMap = new();

        public ScriptLoader(IFileSystemOperations filesystem) => _filesystem = filesystem;

        /// <summary>
        ///     Resolve the path for the specified filename
        /// </summary>
        /// <remarks>
        ///     Searches through all available folders to try and locate the
        ///     desired file
        /// </remarks>
        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            foreach (var i in _includePaths)
            {
                var path = Path.Combine(i, templateName);
                if (_filesystem.Exists(path))
                {
                    _includeMap[templateName] = path;
                    return path;
                }
            }
            _includeMap[templateName] = templateName;
            return templateName;
        }


        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            
            return TemplateProcessor.ApplyAllTransforms(_filesystem.ReadAllText(templatePath));
        }


        public ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath) =>
            ValueTask.FromResult(Load(context, callerSpan, templatePath));

        public void AddIncludePath(string path)
        {
            _includePaths.Add(path);
        }
    }
}
