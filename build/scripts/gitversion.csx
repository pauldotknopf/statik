#load "process.csx"
#load "path.csx"
#r "nuget:Newtonsoft.Json, 11.0.2"

public static class GitVersion
{
    public static GitVersion.GitVersionResult Get(string directory)
    {
        // TODO: Switch to using GitVersion directly (.NET Standard) when it is supported: https://github.com/GitTools/GitVersion/pull/1269
        var output = Process.Capture($"docker run --rm -v {Path.Expand(directory)}:/repo gittools/gitversion /overrideconfig tag-prefix=v");
        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(output);
        var result = new GitVersion.GitVersionResult
        {
            Version = json.MajorMinorPatch,
            PreReleaseLabel = json.PreReleaseLabel,
            PreReleaseTag = json.PreReleaseTag
        };
        if(!string.IsNullOrEmpty(result.PreReleaseTag))
        {
            result.FullVersion = $"{result.Version}-{result.PreReleaseTag}";
        }
        else
        {
            result.FullVersion = result.Version;
        }
        return result;
    }

    public class GitVersionResult
    {
        public string Version { get; set; }

        public string PreReleaseLabel { get; set; }

        public string PreReleaseTag { get; set; }

        public string FullVersion { get; set; }
    }
}