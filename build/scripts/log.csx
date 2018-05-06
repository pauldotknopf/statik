#load "nuget:simple-targets-csx, 6.0.0"

public static class Log
{
    private static readonly Dictionary<MessageType, Func<bool, string>> colors = new Dictionary<MessageType, Func<bool, string>>
    {
        [MessageType.Success] = color => Green(color),
        [MessageType.Info] = color => Blue(color),
        [MessageType.Warning] = color => BrightYellow(color),
        [MessageType.Failure] = color => BrightRed(color),
    };

    private enum MessageType
    {
        Success,
        Info,
        Warning,
        Failure,
    }

    private static void WriteMessage(string message, MessageType type)
    {
        Console.WriteLine(Message(type, message));
    }

    public static void Success(string message)
    {
        WriteMessage(message, MessageType.Success);
    }

    public static void Info(string message)
    {
        WriteMessage(message, MessageType.Info);
    }

    public static void Warning(string message)
    {
        WriteMessage(message, MessageType.Warning);
    }

    public static void Failure(string message)
    {
        WriteMessage(message, MessageType.Failure);
    }

    private static string Default          (bool color) => color ? "\x1b[0m"   : "";
    private static string Green            (bool color) => color ? "\x1b[32m"  : "";
    private static string Blue          (bool color) => color ? "\x1b[34m"  : "";
    private static string Magenta          (bool color) => color ? "\x1b[35m"  : "";
    private static string Cyan             (bool color) => color ? "\x1b[36m"  : "";
    private static string White            (bool color) => color ? "\x1b[37m"  : "";
    private static string BrightRed        (bool color) => color ? "\x1b[91m"  : "";
    private static string BrightYellow     (bool color) => color ? "\x1b[93m"  : "";
    private static string BrightMagenta    (bool color) => color ? "\x1b[95m"  : "";

    private static string Message(MessageType messageType, string text, bool color = true) =>
        $"{colors[messageType](color)}{text}{Default(color)}";
}