using Scriban.Runtime;
using System.Collections.Immutable;
using System.Threading;

namespace Engine.Application
{
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
