using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace SpojDebug.Ultil.Logger
{
    public static class LogHepler
    {
        private static readonly object DataLock = new object();
        private static readonly object SystemLock = new object();

        public static void WriteSystemErrorLog(System.Exception e, string path)
        {
            lock (SystemLock)
            {

                Directory.CreateDirectory(path);
                File.AppendAllText(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), path.Trim('/').Trim('\\')), "Error.log"),
                    $"==============================================================\r\n---------------------------{DateTime.Now.ToLongDateString()}-------------------------\r\n{e.Message}\r\n{e.StackTrace}\r\n\r\n");
            }
        }

        public static void WriteDataErrorLog<TEntity>(System.Exception e, TEntity entity, string path) where TEntity : class 
        {
            lock (DataLock)
            {
                Directory.CreateDirectory(path);

                var info = new StringBuilder();
                info.Append("==============================================================\r\n");
                info.Append(
                    $"---------------------------{DateTime.Now.ToLongDateString()}-------------------------\r\n");
                info.Append($"{e.Message}\r\n{e.StackTrace}\r\nData:\r\n");

                var myEntityType = entity.GetType();
                var properties = myEntityType.GetProperties();

                foreach (var property in properties)
                {
                    info.Append($"      {property.Name}: {property.GetValue(entity, null)}");
                }

                info.Append("+++++++++++++++++++++++++++++++++++++++++++++++\r\n\r\n");
                File.AppendAllText(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), path.Trim('/').Trim('\\')), "Error.log"),info.ToString());
            }
        }

        public static void WriteCustomErrorLog(System.Exception e, string path)
        {
            Directory.CreateDirectory(path);
            File.AppendAllText(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), path.Trim('/').Trim('\\')), "Error.log"),
               $"==============================================================\r\n---------------------------{DateTime.Now.ToLongDateString()}-------------------------\r\n{e.Message}\r\n{e.StackTrace}\r\n\r\n");

        }
    }
}
