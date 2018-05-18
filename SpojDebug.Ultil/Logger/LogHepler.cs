using System;
using System.IO;

namespace SpojDebug.Ultil.Logger
{
    public static class LogHepler
    {
        public static void WriteErrorLog(string errorInfo, string path)
        {
            Directory.CreateDirectory(path);
            File.AppendAllText(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), path.Trim('/').Trim('\\')), "Error.log"),
                $"==============================================================\r\n---------------------------{DateTime.Now.ToLongDateString()}-------------------------\r\n{errorInfo}\r\n");
        }
    }
}
