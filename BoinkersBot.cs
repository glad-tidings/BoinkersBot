using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Boinkers
{
    internal class BoinkersBot
    {
        public readonly BoinkersQuery PubQuery;
        private readonly ProxyType[] PubProxy;
        private readonly string AccessToken;
        private readonly string VersionHash;
        private readonly string LiveOp;
        public readonly bool HasError;
        public readonly string ErrorMessage;
        public readonly string IPAddress;

        public BoinkersBot(BoinkersQuery Query, ProxyType[] Proxy)
        {
            PubQuery = Query;
            PubProxy = Proxy;
            IPAddress = GetIP().Result;
            AccessToken = Log.GetCache("Boinkers", PubQuery.Name);
            if (AccessToken == "empty")
            {
                PubQuery.Auth = getSession().Result;
                var Login = BoinkersLogin().Result;
                if (Login is not null)
                {
                    AccessToken = Login.Token;
                    Log.SetCache("Boinkers", PubQuery.Name, AccessToken);
                    HasError = false;
                    ErrorMessage = "";
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "login failed";
                }
            }
            else
            {
                HasError = false;
                ErrorMessage = "";
            }
            var Config = BoinkersConfig().Result;
            if (Config is not null)
            {
                VersionHash = Config.VersionHash;
                try
                {
                    LiveOp = Config.LiveOps.Where(x => x.LiveOpName.ToLower().Contains("wheel")).ElementAtOrDefault(0)?.Id ?? string.Empty;
                }
                catch
                {
                    LiveOp = string.Empty;
                }
            }
            else
            {
                VersionHash = string.Empty;
                LiveOp = string.Empty;
                HasError = true;
                ErrorMessage = "login failed";
            }
        }

        private async Task<string> GetIP()
        {
            HttpClient client;
            var FProxy = PubProxy.Where(x => (long)x.Index == PubQuery.Index);
            if (FProxy.Count() != 0)
            {
                if (!string.IsNullOrEmpty(FProxy.ElementAtOrDefault(0)?.Proxy))
                {
                    var handler = new HttpClientHandler() { Proxy = new WebProxy() { Address = new Uri(FProxy.ElementAtOrDefault(0)?.Proxy ?? string.Empty) } };
                    client = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 30) };
                }
                else
                {
                    client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
                }
            }
            else
            {
                client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            }
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await client.GetAsync($"https://httpbin.org/ip");
            }
            catch { }
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<Httpbin>(responseStream);
                    return responseJson?.Origin ?? string.Empty;
                }
            }

            return "";
        }

        private async Task<string> getSession()
        {
            var vw = new TelegramMiniApp.WebView(PubQuery.API_ID, PubQuery.API_HASH, PubQuery.Name, PubQuery.Phone, "boinker_bot", "https://boink.boinkers.co/");
            string url = await vw.Get_URL();
            if (!string.IsNullOrEmpty(url))
                return url.Split(new string[] { "tgWebAppData=" }, StringSplitOptions.None)[1].Split(new string[] { "&tgWebAppVersion" }, StringSplitOptions.None)[0];
            else
                return "";
        }

        private async Task<BoinkersConfigResponse?> BoinkersConfig()
        {
            var BAPI = new BoinkersApi(0, PubQuery.Auth, PubQuery.Index, PubProxy, "");
            var httpResponse = await BAPI.BAPIGet("https://boink.astronomica.io/public/data/config");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<BoinkersConfigResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        private async Task<BoinkersLoginResponse?> BoinkersLogin()
        {
            var BAPI = new BoinkersApi(0, PubQuery.Auth, PubQuery.Index, PubProxy, "");
            var request = new BoinkersLoginRequest() { InitDataString = PubQuery.Auth };
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var httpResponse = await BAPI.BAPIPost("https://boink.astronomica.io/public/users/loginByTelegram", serializedRequestContent);
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<BoinkersLoginResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<BoinkersUserInfoResponse?> BoinkersUserInfo()
        {
            var BAPI = new BoinkersApi(1, AccessToken, PubQuery.Index, PubProxy, VersionHash);
            var httpResponse = await BAPI.BAPIGet("https://boink.astronomica.io/api/users/me");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<BoinkersUserInfoResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<bool> BoinkersUpgrade()
        {
            var BAPI = new BoinkersApi(1, AccessToken, PubQuery.Index, PubProxy, VersionHash);
            var httpResponse = await BAPI.BAPIPost("https://boink.astronomica.io/api/boinkers/upgradeBoinker", null);
            if (httpResponse is not null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<bool> BoinkersBooster()
        {
            var BAPI = new BoinkersApi(1, AccessToken, PubQuery.Index, PubProxy, VersionHash);
            var request = new BoinkersBoosterRequest() { Multiplier = 2, OptionNumber = 1 };
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var httpResponse = await BAPI.BAPIPost("https://boink.astronomica.io/api/boinkers/addShitBooster", serializedRequestContent);
            if (httpResponse is not null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<BoinkersWheelOfFortuneResponse?> BoinkersLiveOpId()
        {
            var client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true, NoStore = true, MaxAge = TimeSpan.FromSeconds(0d) };
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await client.GetAsync("https://raw.githubusercontent.com/glad-tidings/BoinkersBot/refs/heads/main/shit.json");
            }
            catch { }
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<BoinkersWheelOfFortuneResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<BoinkersSpinResponse?> BoinkersWheelOfFortune(int x)
        {
            var BAPI = new BoinkersApi(1, AccessToken, PubQuery.Index, PubProxy, VersionHash);
            var request = new BoinkersWheelOfFortuneRequest() { LiveOpId = LiveOp };
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var httpResponse = await BAPI.BAPIPost($"https://boink.astronomica.io/api/play/spinWheelOfFortune/{x}", string.IsNullOrEmpty(LiveOp) ? null : serializedRequestContent);
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<BoinkersSpinResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<BoinkersSpinResponse?> BoinkersSlotMachine(int x)
        {
            var BAPI = new BoinkersApi(1, AccessToken, PubQuery.Index, PubProxy, VersionHash);
            var httpResponse = await BAPI.BAPIPost($"https://boink.astronomica.io/api/play/spinSlotMachine/{x}", (HttpContent)null);
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<BoinkersSpinResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<BoinkersRaffleResponse?> BoinkersRaffle()
        {
            var BAPI = new BoinkersApi(1, AccessToken, PubQuery.Index, PubProxy, VersionHash);
            var httpResponse = await BAPI.BAPIGet("https://boink.astronomica.io/api/raffle/getRafflesData");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<BoinkersRaffleResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<bool> BoinkersRaffleClaim()
        {
            var BAPI = new BoinkersApi(1, AccessToken, PubQuery.Index, PubProxy, VersionHash);
            var httpResponse = await BAPI.BAPIPost("https://boink.astronomica.io/api/raffle/claimTicketForUser", (HttpContent)null);
            if (httpResponse is not null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }
    }
}
