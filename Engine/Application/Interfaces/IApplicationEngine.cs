using Engine.Model;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Engine.Application
{
    public interface IApplicationEngine<TEngine>
    {
        string ErrorOrOutput { get; }
        ImmutableArray<string> Errors { get; }
        bool HasErrors { get; }
        string Output { get; }

        Dictionary<string, string> GetDynamicOutput();
        ImmutableArray<string> GetOutput(int count);
        string GetOutputFromVariable(string name);
        TEngine ImportMethods(ScriptObject methodsClass);
        TEngine ImportMethods(string name, Func<IEnumerable<Type>> typeFetcher);
        ImmutableArray<ModelPath> ModelPaths();
        TEngine Render();
        string RenderToErrorOrOutput();
        bool TryGetVariableAsJsonString(string name, out string res);
        TEngine WithDefinitions(IEnumerable<string> definitionAssignments);
        TEngine WithEnvironmentVariables();
        TEngine WithHelpers();
        TEngine WithIncludePaths(IEnumerable<string> paths);
        TEngine WithModel(string name, object obj);
        TEngine WithModel(string name, string modelText, ModelFormat format);
        TEngine WithTemplate(string templateText);
    }
}
