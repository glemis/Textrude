using Scriban.Runtime;
using System.Collections.Immutable;
using System.Threading;

namespace Engine.Application
{
    /// <summary>
    /// Added to allow for custom implementations to be injected.
    /// Most often would just be a copy of main varsion but but additional changes to the _context for options.
    /// Considered making the _context public so settings can be pushed.
    /// </summary>
    public interface ITemplateManager
    {
        ImmutableArray<string> ErrorList { get; }
        void AddIncludePath(string path);
        void AddVariable(string name, object val);
        ImmutableArray<ModelPath> GetBuiltIns();
        ImmutableArray<ModelPath> GetObjectTree();
        string GetStringOrEmpty(string variableName);
        void ImportScriptObjectToTop(ScriptObject import);
        ImmutableArray<ModelPath> ModelPaths();
        string Render();
        string Render(CancellationToken cancel);
        void SetTemplate(string templateText);
        bool TryGetVariableObject<T>(string variableName, out T val);
    }
}
