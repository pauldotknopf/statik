#r "nuget:Bullseye, 1.1.0-rc.2"
#load "process.csx"
#load "path.csx"
#load "runner.csx"
#load "gitversion.csx"
#load "log.csx"
#load "travis.csx"

using static Bullseye.Targets;
using static Runner;

var options = ParseOptions(Args);
var gitversion = GitVersion.Get("./");
var nugetSource = "https://api.nuget.org/v3/index.json";
var nugetApiKey = System.Environment.GetEnvironmentVariable("NUGET_API_KEY");

Log.Info($"Version: {gitversion.FullVersion}");

var commandBuildArgs = $"--configuration {options.Configuration} --version-suffix \"{gitversion.PreReleaseTag}\"";

Target("clean", () =>
{
    Path.CleanDirectory(Path.Expand("./output"));
});

Target("build", () =>
{
    Process.Run($"dotnet build Statik.sln {commandBuildArgs}");
});

Target("test", () =>
{
    Process.Run($"dotnet test test/Statik.Tests/");
    Process.Run($"dotnet test test/Statik.Files.Tests/");
    Process.Run($"dotnet test test/Statik.Mvc.Tests/");
});

Target("deploy", DependsOn("clean"), () =>
{
    // Deploy our nuget packages.
    Process.Run($"dotnet pack --output {Path.Expand("./output")} {commandBuildArgs}");
});

Target("update-version", () =>
{
    if(Path.FileExists("./build/version.props")) Path.DeleteFile("./build/version.props");
    Path.WriteFile("./build/version.props",
$@"<Project>
    <PropertyGroup>
        <VersionPrefix>{gitversion.Version}</VersionPrefix>
    </PropertyGroup>
</Project>");
});

Target("publish", () =>
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
            // We aren't on master.
            // The only time we also auto-publish
            // is if this is a beta build.
            if(gitversion.PreReleaseLabel != "beta")
            {
                Log.Warning("Not on master or beta build, skipping publish...");
                return;
            }
        }
    }

    if(string.IsNullOrEmpty(nugetApiKey))
    {
        throw new Exception("No NUGET_API_KEY provided.");
    }

    foreach(var file in Path.GetFiles("./output", "*.nupkg"))
    {
        Log.Info($"Deploying {file}");
        Process.Run($"dotnet nuget push {file} --source \"{nugetSource}\" --api-key \"{nugetApiKey}\"");
    }
});

Target("ci", DependsOn("update-version", "test", "deploy", "publish"), () =>
{
    
});

Target("default", DependsOn("build"));

RunTargets(new[] { options.Target });