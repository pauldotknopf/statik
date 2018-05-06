#load "nuget:simple-targets-csx, 6.0.0"
#load "process.csx"
#load "path.csx"
#load "runner.csx"
#load "gitversion.csx"
#load "log.csx"
#load "travis.csx"

using static SimpleTargets;
using static Runner;

var options = ParseOptions(Args);
var gitversion = GitVersion.Get("./");

Log.Info($"Version: {gitversion.FullVersion}");

var commandBuildArgs = $"--configuration {options.Configuration} --version-suffix \"{gitversion.PreReleaseTag}\"";

var targets = new TargetDictionary();

targets.Add("clean", () =>
{
    Path.CleanDirectory(Path.Expand("./output"));
});

targets.Add("build", () =>
{
    Process.Run($"dotnet build Statik.sln {commandBuildArgs}");
});

targets.Add("test", () =>
{
    Process.Run($"dotnet test test/Statik.Tests/");
    Process.Run($"dotnet test test/Statik.Files.Tests/");
    Process.Run($"dotnet test test/Statik.Mvc.Tests/");
});

targets.Add("deploy", SimpleTargets.DependsOn("clean"), () =>
{
    // Deploy our nuget packages.
    Process.Run($"dotnet pack --output {Path.Expand("./output")} {commandBuildArgs}");
});

targets.Add("update-version", () =>
{
    if(Path.FileExists("./build/version.props")) Path.DeleteFile("./build/version.props");
    Path.WriteFile("./build/version.props",
$@"<Project>
    <PropertyGroup>
        <VersionPrefix>{gitversion.Version}</VersionPrefix>
    </PropertyGroup>
</Project>");
});

targets.Add("publish", () =>
{
    if(Travis.IsTravis)
    {
        // If we are on travis, we only want to deploy if this is a release tag.
        if(Travis.EventType != Travis.EventTypeEnum.Push)
        {
            // Only pushes (no cron jobs/api/pull rqeuests) can deploy.
            Log.Warning("Not a push build, skipping publish...");
            return;
        }

        if(Travis.Branch != "master")
        {
            Log.Warning("Not on master, skipping publish...");
            return;
        }
    }

    // TODO: deploy nuget packages
    Log.Warning("TODO: deploy to NuGet...");
});

targets.Add("ci", DependsOn("update-version", "test", "deploy", "publish"), () =>
{
    
});

targets.Add("default", SimpleTargets.DependsOn("build"));

Runner.Run(options, targets);