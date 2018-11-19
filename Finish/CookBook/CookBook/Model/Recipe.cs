using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace CookBook.Model
{
    public partial class Recipe
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }

        [JsonProperty("Details")]
        public string Details { get; set; }

        [JsonProperty("Image")]
        public Uri Image { get; set; }

        [JsonProperty("Population")]
        public long Population { get; set; }

        [JsonProperty("Latitude")]
        public double Latitude { get; set; }

        [JsonProperty("Longitude")]
        public double Longitude { get; set; }
    }

    public partial class Recipe
    {
        public static Recipe[] FromJson(string json) => JsonConvert.DeserializeObject<Recipe[]>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Recipe[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
