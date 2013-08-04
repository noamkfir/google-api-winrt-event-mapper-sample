using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Credentials;
using Windows.Storage;
using Newtonsoft.Json;

namespace EventMapper.Authentication
{
    public class Authenticator
    {
        private const string GoogleApprovalAddress = "https://accounts.google.com/o/oauth2/approval";
        private const string GoogleCalendarScope = "https://www.googleapis.com/auth/calendar"; //  https://www.googleapis.com/auth/userinfo.id https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/plus.me https://www.googleapis.com/auth/plus.login https://www.google.com/accounts/OAuthLogin
        private const string GoogleAuthorizationAddress = "https://accounts.google.com/o/oauth2/auth";
        private const string GoogleInstalledAppRedirectAddress = "urn:ietf:wg:oauth:2.0:oob";
        private const string UrlEncodedContentType = "application/x-www-form-urlencoded";
        private const string GoogleTokenAddress = "https://accounts.google.com/o/oauth2/token";
        private const string GoogleRefreshTokenResourceName = "GoogleRefreshToken";
        private const string GoogleAuthFileName = "GoogleAuth";
        private const string VaultUserName = "USERNAME";

        private GoogleCodes _googleCodes;

        public string AccessToken { get; private set; }

        //public async Task<string> AuthenticateAsync()
        //{
        //    // https://developers.google.com/accounts/docs/OAuth2InstalledApp#formingtheurl

        //    const string baseAddress = GoogleAuthorizationAddress;

        //    var values = new Dictionary<string, string>()
        //    {
        //        { "response_type", "code" },
        //        { "client_id", GoogleClientId },
        //        { "redirect_uri", GoogleInstalledAppRedirectAddress },
        //        //{ "redirect_uri", "http://localhost" },
        //        { "access_type", "online" },
        //        { "scope", GoogleCalendarScope },
        //    };

        //    var parameterString = BuildParameterString(values);
        //    var address = baseAddress + "?" + parameterString;

        //    //"https://www.googleapis.com/auth/{0}",
        //    var options = WebAuthenticationOptions.None;
        //    var requestUri = new Uri(address);
        //    var callbackUri = new Uri(GoogleApprovalAddress);
        //    //var callbackUri = new Uri("http://localhost");
        //    //var callbackUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
        //    var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(options, requestUri, callbackUri);

        //    //string token = null;
        //    //if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
        //    //{
        //    //    //token = GetAuthorizationTokenAsync(webAuthenticationResult.ResponseData);
        //    //}
        //    var responseData = webAuthenticationResult.ResponseData;
        //    var a = await Post(responseData);
        //    var result = await GetAuthorizationTokenAsync(responseData);
        //    //var token = await GetAuthorizationTokenAsync(responseData);
        //    return result;
        //}

        public async Task AuthenticateAsync(bool force)
        {
            _googleCodes = await GoogleCodes.Load();

            Token token;

            var refreshToken = force ? null : GetRefreshToken();
            if (refreshToken == null)
            {
                var code = await GetCodeAsync();
                token = await GetAuthorizationTokenAsync(code);

                StoreRefreshToken(token.refresh_token);
            }
            else
            {
                token = await GetAccessToken(refreshToken);
            }

            AccessToken = token == null ? null : token.access_token;
        }

        private async Task<Token> GetAccessToken(string refreshToken)
        {
            var values = new Dictionary<string, string>()
            {
                { "client_id", _googleCodes.ClientId },
                { "client_secret", _googleCodes.ClientSecret },
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" },
            };

            var parameterString = BuildParameterString(values);

            var content = new StringContent(parameterString, Encoding.UTF8, UrlEncodedContentType);

            var handler = new HttpClientHandler();
            var httpClient = new HttpClient(handler);
            var result = await httpClient.PostAsync(GoogleTokenAddress, content);
            Token token = null;
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();

                token = JsonConvert.DeserializeObject<Token>(json);
            }
            return token;
        }

        private string GetRefreshToken()
        {
            var vault = new PasswordVault();
            string refreshToken = null;
            try
            {
                var credentials = vault.FindAllByResource(GoogleRefreshTokenResourceName);
                if (credentials.Count > 0)
                {
                    var credential = vault.Retrieve(GoogleRefreshTokenResourceName, VaultUserName);
                    refreshToken = credential.Password;
                }
            }
            catch (Exception exc)
            {
                var a = exc;
            }
            return refreshToken;
        }

        private void StoreRefreshToken(string refreshToken)
        {
            var vault = new PasswordVault();
            PasswordCredential credential;
            try
            {
                credential = vault.Retrieve(GoogleRefreshTokenResourceName, VaultUserName);
                if (credential == null)
                {
                    credential = new PasswordCredential(GoogleRefreshTokenResourceName, VaultUserName, refreshToken);
                    vault.Add(credential);
                }
            }
            catch (Exception exc)
            {
                credential = new PasswordCredential(GoogleRefreshTokenResourceName, VaultUserName, refreshToken);
                vault.Add(credential);
            }
        }

        private async Task<string> GetCodeAsync()
        {
            var values = new Dictionary<string, string>()
            {
                { "response_type", "code" },
                { "client_id", _googleCodes.ClientId },
                { "redirect_uri", GoogleInstalledAppRedirectAddress },
                //{ "redirect_uri", "http://localhost" },
                //{ "access_type", "online" },
                { "scope", Uri.EscapeUriString(GoogleCalendarScope) },
            };

            var parameterString = BuildParameterString(values);
            var requestAddress = GoogleAuthorizationAddress + "?" + parameterString;

            var options = WebAuthenticationOptions.UseTitle;
            var requestUri = new Uri(requestAddress);
            var callbackUri = new Uri(GoogleApprovalAddress);
            var result = await WebAuthenticationBroker.AuthenticateAsync(options, requestUri, callbackUri);

            var responseData = result.ResponseData;
            var code = responseData.Substring(responseData.IndexOf('=') + 1);
            return code;
        }

        //private async Task<string> Post(string responseData)
        //{
        //    var httpClient = new HttpClient();
        //    var content = new StringContent(string.Empty, Encoding.UTF8, UrlEncodedContentType);
        //    var httpResponseMessage = await httpClient.PostAsync(responseData, content);
        //    var httpContent = httpResponseMessage.Content;
        //    var code = await httpContent.ReadAsStringAsync();
        //    return code;
        //}

        private async Task<Token> GetAuthorizationTokenAsync(string code)
        {
            //var postData = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type={4}",
            //                                     oauthCode, clientId, googleClientSecret, GoogleRedirectId,
            //                                     "authorization_code");

            //var request = (HttpWebRequest)WebRequest.Create(TokenUrl);
            //var token = await request.GetValueFromRequest(postData);

            var values = new Dictionary<string, string>()
            {
                { "client_id", _googleCodes.ClientId },
                { "client_secret", _googleCodes.ClientSecret },
                //{ "redirect_uri", "http://localhost" },
                { "code", code },
                { "redirect_uri", GoogleInstalledAppRedirectAddress },
                { "grant_type", "authorization_code" },
            };
            var parameterString = BuildParameterString(values);

            var content = new StringContent(parameterString, Encoding.UTF8, UrlEncodedContentType);

            var handler = new HttpClientHandler();
            var httpClient = new HttpClient(handler);
            //httpClient.MaxResponseContentBufferSize = 100000;
            var result = await httpClient.PostAsync(GoogleTokenAddress, content);
            Token token = null;
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();

                token = JsonConvert.DeserializeObject<Token>(json);
                //var deserializer = new DataContractJsonSerializer(typeof(Token));
                //var data = (Token)deserializer.ReadObject(result.Content.ContentReadStream);
                //GetProfile(data.access_token);
            }
            //var token = await result.Content.ReadAsStringAsync();
            return token;
        }

        private static string BuildParameterString(Dictionary<string, string> values)
        {
            var parameters = values.Select(item => item.Key + "=" + item.Value).ToArray();

            var @join = string.Join("&", parameters);
            return @join;
        }
    }
}