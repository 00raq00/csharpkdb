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

public class MarketEntry
{
  public string Code { get; set; }

  [JsonConverter(typeof(NullableConverter))]
  public long? Timestamp { get; set; }

  public int GmtOffset { get; set; }

  [JsonConverter(typeof(NullableConverter))]
  public double? Open { get; set; }

  [JsonConverter(typeof(NullableConverter))]
  public double? High { get; set; }

  [JsonConverter(typeof(NullableConverter))]
  public double? Low { get; set; }

  [JsonConverter(typeof(NullableConverter))]
  public double? Close { get; set; }

  [JsonConverter(typeof(NullableConverter))]
  public long? Volume { get; set; }

  [JsonConverter(typeof(NullableConverter))]

  public double? Change { get; set; }

  [JsonConverter(typeof(NullableConverter))]
  public double? ChangeP { get; set; }
}






