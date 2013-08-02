using System;
using System.Net.Http;
using System.Threading.Tasks;
using EventMapper.Authentication;
using Google.Apis.Calendar.v3;
using Google.Apis.Http;
using Google.Apis.Services;

namespace EventMapper
{
    internal class Bootstrapper
    {
        public async void Start()
        {
            var authenticator = new Authenticator();
            var accessToken = await authenticator.AuthenticateAsync(true);

            var calendars = await GetCalendars(accessToken);
        }

        private async Task<string> GetCalendars(string accessToken)
        {
            var initializer = new BaseClientService.Initializer();
            var interceptor = new WinRTMessageAuthenticationInterceptor(accessToken);
            initializer.HttpClientInitializer = interceptor;

            //var handler = new HttpClientHandler();
            //var httpClient = new ConfigurableHttpClient(new ConfigurableMessageHandler(handler));
            //interceptor.Initialize(httpClient);
            var calendarService = new CalendarService(initializer);
            var calendarsResource = calendarService.CalendarList.List().Execute();

            string address = "https://www.googleapis.com/oauth2/v1/use%rinfo?access_token=" + accessToken;
            //var response = await httpClient.GetAsync(new Uri(address));
            //var content = await response.Content.ReadAsStringAsync();
            //return content;
            return null;
        }

        private class WinRTMessageAuthenticationInterceptor : IHttpExecuteInterceptor, IConfigurableHttpClientInitializer
        {
            private readonly string _accessToken;

            public WinRTMessageAuthenticationInterceptor(string accessToken)
            {
                _accessToken = accessToken;
            }

            public void Intercept(HttpRequestMessage request)
            {
                request.Headers.Add("Authorization", new[] { "Bearer " + _accessToken });
            }

            public void Initialize(ConfigurableHttpClient httpClient)
            {
                httpClient.MessageHandler.ExecuteInterceptors.Add(this);
            }
        }
    }
}