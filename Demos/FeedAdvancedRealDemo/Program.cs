using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Net.Http.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using JsonConverter = Newtonsoft.Json.JsonConverter;
using JsonConverterAttribute = Newtonsoft.Json.JsonConverterAttribute;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using FeedAdvancedRealDemo;
using System.Net.Http.Headers;







class Program
{
  private static readonly HttpClient client = new HttpClient();
  private const string products = "VTI,EUR.FOREX";

  private const string apiUrl = "https://eodhd.com/api/real-time/AAPL.US?s="+ products + "&api_token=demo&fmt=json";

  static async Task Main(string[] args)
  {
    while (true)
    {
      try
      {
        var json = await client.GetStringAsync(apiUrl);
        var dataList = JsonConvert.DeserializeObject<List<MarketEntry>>(json);
        Feeder feeder = new Feeder();

        Console.Clear();
        foreach (var data in dataList)
        {
          feeder.Feed(data);
          Console.WriteLine($"[{DateTime.Now}] → Code: {data.Code}, Open: {data.Open},→ Close: {data.Close}, Volume: {data.Volume}");
        }
      }


      catch (Exception ex)
      {

        Console.WriteLine($"Error while fetching data : {ex.Message}");
      }

      // await Task.Delay(10000); // 10 sekund
      await Task.Delay(0); // 10 sekund
    }
  }
}




