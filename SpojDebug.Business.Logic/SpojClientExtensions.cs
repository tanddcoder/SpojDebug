using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpojDebug.Business.Logic
{
    public static class SpojClientExtensions
    {
        private const string SpojProblemInfoUrl = "http://www.spoj.com/problems/{0}/edit/";

        public static int GetTotalTestCase(this SpojClient spojClient, string spojProblemCode)
        {
            var text = spojClient.GetText(string.Format(SpojProblemInfoUrl, spojProblemCode));
            var matches = Regex.Matches(text.Trim(), "<option value=\"(\\d+)\">Modify testcase #\\d+</option>");

            var listNumber = new List<int>();
            foreach (Match match in matches)
            {
                try
                {
                    var value = int.Parse(match.Groups[1].Value);
                    listNumber.Add(value);
                }
                catch (Exception e)
                {
                    //ignore
                }
            }

            return listNumber.Max();
        }
    }
}
