using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EzTrack.Droid.Model;
using Newtonsoft.Json;

namespace EzTrack.Droid
{
    public static class Http
    {
        public static void PostOrder(OrderAsset order)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApplicationData.ApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");              
                var response = client.PostAsync("api/entity/postOrder", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    order = JsonConvert.DeserializeObject<OrderAsset>(data);
                }
            }
        }

        public static async Task<string> GetAssetName(string barcodeString)
        {
            using (var client = new HttpClient())
            {
                int.TryParse(barcodeString, out var barCodeNumber);
                client.BaseAddress = new Uri(ApplicationData.ApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();

                var serviceUrl = $"api/entity/LookupAsset?barcodeValue={barCodeNumber}";
                var response = await client.GetAsync(serviceUrl);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }

            return "";
        }
    }
}