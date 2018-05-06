using System;

public static class Path
{
    public static string Expand(string path)
    {
        return System.IO.Path.GetFullPath(path);
    }

    public static void CleanDirectory(string directory)
    {
        var di = new System.IO.DirectoryInfo(directory);

        if(!di.Exists)
        {
            return;
        }

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete(); 
        }

        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true); 
        }
    }

    public static bool FileExists(string path)
    {
        return System.IO.File.Exists(path);
    }

    public static void DeleteFile(string file)
    {
        System.IO.File.Delete(file);
    }

    public static void WriteFile(string file, string content)
    {
        System.IO.File.WriteAllText(file, content);
    }

    public static void ReplaceInFile(string file, string oldValue, string newValue)
    {
        string text = System.IO.File.ReadAllText(file);
        text = text.Replace(oldValue, newValue);
        System.IO.File.WriteAllText(file, text);
    }
}
