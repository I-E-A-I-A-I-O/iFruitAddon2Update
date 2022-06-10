using System;
using System.IO;

namespace iFruitAddon2;

/// <summary>
/// Static logger class that allows direct logging of anything to a text file
/// </summary>
static class Logger
{
    private static string _logFileName = "iFruitAddon2.log";

    public static void Log(object message)
    {
        File.AppendAllText(_logFileName, DateTime.Now + " : " + message + Environment.NewLine);
    }

    public static void ResetLogFile()
    {
        var fs = File.Create(_logFileName);
        fs.Close();
    }
}