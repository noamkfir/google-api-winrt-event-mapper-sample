using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Newtonsoft.Json;

namespace EventMapper.Authentication
{
    internal class GoogleCodes
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiKey { get; set; }

        public async static Task<GoogleCodes> Load() {
            var uri = new Uri("ms-appx:///Assets/GoogleCodes.json");
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var json = await FileIO.ReadTextAsync(file);
            var googleCodes = JsonConvert.DeserializeObject<GoogleCodes>(json);
            return googleCodes;
        }
    }
}