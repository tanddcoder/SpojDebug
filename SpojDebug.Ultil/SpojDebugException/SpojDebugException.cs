using Newtonsoft.Json;
using SpojDebug.Core.Constant;
using SpojDebug.Ultil.Reflection;
using System;

namespace SpojDebug.Ultil.SpojDebugException
{
    public class SpojDebugException : Exception
    {
        private const string _defaultSpojDebugException = "Super error occured";

        public SpojDebugException() : base(_defaultSpojDebugException) { }

        public SpojDebugException(string message) : base(message) { }

        public SpojDebugException(Error error) : base(GetJsonFromCode(error)) { }

        private static string GetJsonFromCode(Error errorCode, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                message = errorCode.GetDisplayName();
            var result = new { errorCode, message };

            return JsonConvert.SerializeObject(result);
        }
    }
}
