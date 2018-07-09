using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpojDebug.Business.Logic
{
    public class SpojClient : HttpClient
    {

        private readonly string _spojLoginUri = @"/login/";

        public SpojClient()
        {
            AddHeaders();
        }

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

        public void AddHeaders()
        {
            //DefaultRequestHeaders.Add("Referrer","https://www.spoj.com");
            // DefaultRequestHeaders.Add("Host","https://www.spoj.com");
            BaseAddress = new System.Uri("https://www.spoj.com");
            DefaultRequestHeaders.Add("Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/apng,*/*;q=0.8");
            DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
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
