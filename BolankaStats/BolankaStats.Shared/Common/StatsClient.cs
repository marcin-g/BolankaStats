using BolankaStats.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace BolankaStats.Common
{
    class StatsClient
    {
        private HttpClient _client = new HttpClient();
        private static string ENDPOINT = "http://pool-stats.app-get.co";
        
        private ObservableCollection<Entrance> _entrances = new ObservableCollection<Entrance>();

        public ObservableCollection<Entrance> Entrances
        {
            get { return this._entrances; }
        }

        private async Task<string> getResponse(string method)
        {
            var response = await _client.GetAsync(ENDPOINT+'/'+method);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            return null;
        }
        private async Task GetEntrancesData(string pattern = "")
        {
            string entrancesResults=await this.getResponse("getEntrances");
            _entrances = new ObservableCollection<Entrance>();
            JsonObject jsonObject = JsonObject.Parse(entrancesResults);
            JsonArray jsonArray = jsonObject["success"].GetArray();
            foreach (JsonValue entranceValue in jsonArray)
            {
                _entrances.Add(this.parseEntrance(entranceValue.GetObject()));
            }
        }

        private async Task GetSampleDataAsync()
        {
           
            Uri dataUri = new Uri("ms-appx:///DataModel/EntranceData.json");

            _entrances = new ObservableCollection<Entrance>();
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonArray jsonObject = JsonArray.Parse(jsonText);
            //JsonArray jsonArray = jsonObject[""].GetArray();

           foreach (JsonValue groupValue in jsonObject)
            {
               _entrances.Add(this.parseEntrance(groupValue.GetObject()));
            }
            
        }

        private Entrance parseEntrance(JsonObject entranceObject)
        {
            return new Entrance(entranceObject["id"].GetNumber().ToString(),
            DateTime.Parse(entranceObject["date"].GetString()), 
            //entranceObject["DeviceId"].GetString(), 
            "1",
            (int)entranceObject["swimmers"].GetNumber(), 
            //(int)entranceObject["swimmers"].GetNumber(),
            true
            );

        }

        public async Task<IEnumerable<Entrance>> GetEntrances()
        {
            await this.GetSampleDataAsync();

            return this.Entrances;
        }
    }
}
