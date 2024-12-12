using System.Net.Http.Headers;
using System.Net;

namespace Boinkers
{
    internal class BoinkersApi
    {
        private readonly HttpClient client;
        private readonly string Platform;
        private readonly string _VersionHash;

        public BoinkersApi(int Mode, string queryID, int queryIndex, ProxyType[] Proxy, string VersionHash)
        {
            var FProxy = Proxy.Where(x => x.Index == queryIndex);
            if (FProxy.Count() != 0)
            {
                if (!string.IsNullOrEmpty(FProxy.ElementAtOrDefault(0)?.Proxy))
                {
                    var handler = new HttpClientHandler() { Proxy = new WebProxy() { Address = new Uri(FProxy.ElementAtOrDefault(0)?.Proxy ?? string.Empty) } };
                    client = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 30) };
                }
                else
                    client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            }
            else
                client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true, NoStore = true, MaxAge = TimeSpan.FromSeconds(0d) };
            if (Mode == 1)
                client.DefaultRequestHeaders.Add("Authorization", $"{queryID}");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Origin", "https://boink.astronomica.io");
            client.DefaultRequestHeaders.Add("Referer", "https://boink.astronomica.io/");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");
            client.DefaultRequestHeaders.Add("User-Agent", Tools.getUserAgents(queryIndex));
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", $"\"{Tools.getUserAgents(queryIndex, true)}\"");

            Platform = (Tools.getUserAgents(queryIndex, true) == "Android" ? "android" : "ios");
            _VersionHash = VersionHash;
        }

        public async Task<HttpResponseMessage> BAPIGet(string requestUri)
        {
            try
            {
                return (HttpResponseMessage)await client.GetAsync(requestUri + $"?&p={Platform}" + (!string.IsNullOrEmpty(_VersionHash) ? $"&v={_VersionHash}" : ""));
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.ExpectationFailed, ReasonPhrase = ex.Message };
            }
        }

        public async Task<HttpResponseMessage> BAPIPost(string requestUri, HttpContent content)
        {
            try
            {
                return (HttpResponseMessage)await client.PostAsync(requestUri + $"?&p={Platform}" + (!string.IsNullOrEmpty(_VersionHash) ? $"&v={_VersionHash}" : ""), content);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.ExpectationFailed, ReasonPhrase = ex.Message };
            }
        }
    }
}
