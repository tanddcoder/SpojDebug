using System.IO;
using SpojDebug.Ultil.Exception;

namespace SpojDebug.Ultil.FileHelper
{
    public static class FileUltils
    {
        public static void SaveFile(string path, string fileName, string content)
        {
            if (path == null)
                throw new SpojDebugException("Path cannot be null");

            if (!path.Trim().EndsWith(Path.DirectorySeparatorChar.ToString()))
                path = path + Path.DirectorySeparatorChar;

            File.WriteAllText(path + fileName, content);
        }
    }
}
