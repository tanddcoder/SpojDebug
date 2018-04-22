using System;

namespace SpojDebug.Ultil.Spoj
{
    public class SpojDataTokenizer
    {
        int _index = 0;
        string[] _lines = new string[] { };

        public SpojDataTokenizer(string text)
        {
            _lines = text.Split('\n');
        }

        public string GetNext()
        {
            return _lines[_index++].Replace("\r", "");
        }

        public int GetInt()
        {
            return int.Parse(GetNext());
        }

        public long GetLong()
        {
            return long.Parse(GetNext());
        }

        public float GetFloat()
        {
            return float.Parse(GetNext());
        }

        public DateTime GetUnixTime()
        {
            var time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return time.AddSeconds(GetInt());
        }

        public void Skip(int n)
        {
            _index += n;
        }
    }
}
