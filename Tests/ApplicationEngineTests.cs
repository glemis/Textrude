using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Engine.Application;
using Engine.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;
using System.Text.Json;
using System.Threading;

namespace Tests;

[TestClass]
public class ApplicationEngineTests
{
    private readonly MockFileSystem _files = new();
    private readonly RunTimeEnvironment _rte;


    public ApplicationEngineTests() => _rte = new RunTimeEnvironment(_files);
    [TestMethod]
    public void CodeCompletionProcessesProject()
    {
        var projectFile = File.ReadAllText("TestFiles/CodeCompletionProcessesProject/project.texproj");
        var proj = JsonSerializer.Deserialize<TextrudeProject>(projectFile);
        List<string> _includePaths = new();
        Dictionary<string, string> _includeMap = new();


        foreach (var include in proj.LastRecordedIncludes)
        {
            foreach (var p in proj.EngineInput.IncludePaths)
            {
                var path = Path.Combine(p, include);
                if (File.Exists(path))
                {
                    _files.WriteAllText(path, File.ReadAllText(path));
                }
            }
        }
        string templateText;
        if (String.IsNullOrEmpty(proj.EngineInput.TemplatePath))
        {
            //If relative can push into fake folder for assembly
            if (proj.EngineInput.TemplatePath.StartsWith("."))
            {
                templateText = File.ReadAllText("TestFiles" + proj.EngineInput.TemplatePath);
            }
            //If absolute push path
            else
            {
                templateText = File.ReadAllText(proj.EngineInput.TemplatePath);
            }

            //Must load into mock filesystem
            _files.WriteAllText(proj.EngineInput.TemplatePath, templateText);
        }
        else
        {
            templateText = proj.EngineInput.Template;
        }


        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token = source.Token;

        var applicationEngine = new ApplicationEngine(new ScriptLoader(_files), _rte, token)
            .WithEnvironmentVariables()
            .WithHelpers()
            .WithTemplate(templateText)
            .WithIncludePaths(new[] { "TestFiles" });



        applicationEngine.RenderProject(proj);

        var engine = new ApplicationEngine(_rte)
            ;
    }

    [TestMethod]
    public void CodeCompletionShowsModel()
    {
        var model =
            @"str: a";

        new ApplicationEngine(_rte)
            .WithModel("model", model, ModelFormat.Yaml)
            .ModelPaths()
            .Select(p => p.Render())
            .Should()
            .Contain("model.str");
    }

    [TestMethod]
    public void CodeCompletionShowsDefinitions()
    {
        new ApplicationEngine(_rte)
            .WithDefinitions(new[] { "abc=def" })
            .ModelPaths()
            .Select(p => p.Render())
            .Should()
            .Contain("def.abc");
    }

    [TestMethod]
    public void CodeCompletionShowsEnvironment()
    {
        var envKeys = Environment.GetEnvironmentVariables()
            .Keys
            .Cast<string>()
            .Select(e => $"env.{e}");

        new ApplicationEngine(_rte)
            .WithEnvironmentVariables()
            .ModelPaths()
            .Select(p => p.Render())
            .Should().Contain(envKeys);
    }

    [TestMethod]
    public void CodeCompletionRejectsLibraryMethods()
    {
        var offeredPaths = new ApplicationEngine(_rte)
            .WithTemplate(@"
{{
func __library ; ret 1;end;
}}")
            .Render()
            .ModelPaths()
            .Select(p => p.Render())
            .ToArray();

        offeredPaths
            .Should()
            .NotContain("__library");
    }

    [TestMethod]
    public void CodeCompletionIncludesFunctions()
    {
        var offeredPaths = new ApplicationEngine(_rte)
            .WithTemplate(@"
{{
func myfunc ; ret 1;end;
}}")
            .Render()
            .ModelPaths()
            .Select(p => p.Render())
            .ToArray();

        offeredPaths
            .Should()
            .Contain("myfunc");

     
    }


    [TestMethod]
    public void InfiniteRecursionAvoided()
    {
        var script = @"{{

m = {
   x: 123,
   def: 456 
 }
m.abc =m
m
}}
";

        var res = new ApplicationEngine(_rte)
            .WithTemplate(script)
            .Render();
        res.HasErrors.Should().BeTrue();
    }
}
