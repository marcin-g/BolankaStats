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
        private static string ENDPOINT = "http://www.peka.poznan.pl/vm/method.vm";
        
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
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Entrances"].GetArray();

            foreach (JsonValue groupValue in jsonArray)
            {
               _entrances.Add(this.parseEntrance(groupValue.GetObject()));
            }
            
        }

        private Entrance parseEntrance(JsonObject entranceObject)
        {
            return new Entrance(entranceObject["UniqueId"].GetString(),
            DateTime.Parse(entranceObject["Date"].GetString()), entranceObject["DeviceId"].GetString(), (int)entranceObject["PeopleNumber"].GetNumber(), entranceObject["Entered"].GetBoolean());

        }

        public async Task<IEnumerable<Entrance>> GetEntrances()
        {
            await this.GetSampleDataAsync();

            return this.Entrances;
        }
    }
}
