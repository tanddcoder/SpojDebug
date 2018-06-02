using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpojDebug.Business.Logic
{
    public class SpojClient : HttpClient
    {

        private readonly string _spojLoginUri = @"http://www.spoj.com/login/";

        public async Task<HttpResponseMessage> LoginAsync(string username, string password)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("autologin","1"),
                new KeyValuePair<string, string>("login_user",$"{username}"),
                new KeyValuePair<string, string>("password",$"{password}"),
                new KeyValuePair<string, string>("next_raw","/"),
            });

            var result = await PostAsync(_spojLoginUri, content);
            return result;
        }

        public string GetText(string url)
        {
            var response = GetByteArrayAsync(url);
            response.Wait();
            var bytes = response.Result;
            var textResult = Encoding.UTF8.GetString(bytes);
            return textResult;
        }

        public async Task<MatchCollection> MatchesHtmlAndPatternAsync(string url, string pattern)
        {
            var htmlText = await DownLoadHtmlAsync(url);
            var matches = Regex.Matches(htmlText.Trim(), pattern);
            return matches;
        }

        public async Task<bool> IsMatchHtmlAndPatternAsync(string url, string pattern)
        {
            var htmlText = await DownLoadHtmlAsync(url);
            return Regex.IsMatch(htmlText.Trim(), pattern);
        }

        public async Task<string> DownLoadHtmlAsync(string url)
        {
            return await GetStringAsync(url);
        }

        public async Task<bool> IsLoginSuccess()
        {
            var matches = await MatchesHtmlAndPatternAsync("http://www.spoj.com/", "<a href=\"#\" class=\"username_dropdown dropdown-toggle\" data-toggle=\"dropdown\">");
            return matches.Count > 0;
        }
    }
}
