using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Bullseye.Targets;
using static Build.Buildary.Directory;
using static Build.Buildary.Path;
using static Build.Buildary.Shell;
using static Build.Buildary.Runner;
using static Build.Buildary.Runtime;
using static Build.Buildary.Log;
using static Build.Buildary.File;
using static Build.Buildary.GitVersion;

namespace Build
{
    static class Program
    {
        static void Main(string[] args)
        {
            var options = ParseOptions<RunnerOptions>(args);
            
            var gitversion = GetGitVersion(ExpandPath("./"));
            var commandBuildArgs = $"--configuration {options.Config} /p:Platform=\"Any CPU\"";
            var commandBuildArgsWithVersion = commandBuildArgs;
            if (!string.IsNullOrEmpty(gitversion.PreReleaseTag))
            {
                commandBuildArgsWithVersion += $" --version-suffix \"{gitversion.PreReleaseTag}\"";
            }
            
            Info($"Version: {gitversion.FullVersion}");
            
            Target("clean", () =>
            {
                CleanDirectory(ExpandPath("./output"));
            });
            
            Target("test", () =>
            {
                RunShell($"dotnet test test/Statik.Tests/");
                RunShell($"dotnet test test/Statik.Files.Tests/");
                RunShell($"dotnet test test/Statik.Mvc.Tests/");
            });

            Target("build", () =>
            {
                RunShell($"dotnet build Statik.sln {commandBuildArgs}");
            });

            Target("deploy", DependsOn("clean"), () =>
            {
                // Deploy our nuget packages.
                RunShell($"dotnet pack --output {ExpandPath("./output")} {commandBuildArgsWithVersion}");
            });
            
            Target("update-version", () =>
            {
                if (FileExists("./build/version.props"))
                {
                    DeleteFile("./build/version.props");
                }
                
                WriteFile("./build/version.props",
                    $@"<Project>
    <PropertyGroup>
        <VersionPrefix>{gitversion.Version}</VersionPrefix>
    </PropertyGroup>
</Project>");
            });
            
            Target("default", DependsOn("clean", "build"));

            Target("ci", DependsOn("update-version", "build", "test", "deploy"));

            Execute(options);
        }
    }
}