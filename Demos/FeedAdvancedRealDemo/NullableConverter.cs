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



public class NullableConverter : Newtonsoft.Json.JsonConverter
{
  public override bool CanWrite => false;

  public override bool CanConvert(Type objectType)
  {
    throw new NotImplementedException();
  }

  public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
  {

    try
    {
      if (reader.TokenType == JsonToken.Null)
        return null;

      if (reader.TokenType == JsonToken.String)
      {
        string str = reader.Value?.ToString();
        if (string.IsNullOrWhiteSpace(str) || str == "NA")
          return null;

        if (long.TryParse(str, out var result))
          return result;

        return null;
      }

      if (reader.TokenType == JsonToken.Integer)


      {
        return Convert.ToInt64(reader.Value);
      }
      if (reader.TokenType == JsonToken.Float)


      {
        return Convert.ToDouble(reader.Value);
      }
    }
    catch
    {
    }

    return null;
  }


  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  {
    throw new NotImplementedException();
  }
}



