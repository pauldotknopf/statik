public static class Travis
{
    public enum EventTypeEnum
    {
        Push,
        PullRequest,
        Api,
        Cron,
        Unknown
    }

    public static bool IsTravis
    {
        get
        {
            return System.Environment.GetEnvironmentVariable("TRAVIS") == "true";
        }
    }

    public static EventTypeEnum EventType
    {
        get
        {
            switch(System.Environment.GetEnvironmentVariable("TRAVIS_EVENT_TYPE"))
            {
                case "push":
                    return EventTypeEnum.Push;
                case "pull_request":
                    return EventTypeEnum.PullRequest;
                case "api":
                    return EventTypeEnum.Api;
                case "cron":
                    return EventTypeEnum.Cron;
                default:
                    return EventTypeEnum.Unknown;
            }
        }
    }

    public static bool IsTagBuild
    {
        get
        {
            return !string.IsNullOrEmpty(Tag);
        }
    }

    public static string Tag
    {
        get
        {
            return System.Environment.GetEnvironmentVariable("TRAVIS_TAG");
        }
    }

    public static string Branch
    {
        get
        {
            return System.Environment.GetEnvironmentVariable("TRAVIS_BRANCH");
        }
    }
}