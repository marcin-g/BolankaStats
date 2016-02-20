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
            string entrancesResults=await this.getResponse("");
            _entrances = new ObservableCollection<Entrance>();
            JsonArray jsonArray = JsonArray.Parse(entrancesResults);
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
            //await this.GetSampleDataAsync();
            await this.GetEntrancesData();
            return this.Entrances;
        }

        public async Task<string> PostEntrance(Entrance entrance) { 
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                   { "swimmers", entrance.PeopleNumber.ToString() },
                   { "date", String.Format("{0:u}", entrance.Date) },
                   { "device_id", entrance.DeviceId }
                });
            var response = await _client.PostAsync(ENDPOINT, content);
            return null;
        }

    }
}
