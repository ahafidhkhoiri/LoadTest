using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MsTeamStressTest
{
    public partial class MeetingLink
    {
        [JsonProperty("task")]
        public Task Task { get; set; }
    }

    public partial class Task
    {
        [JsonProperty("value")]
        public Value Value { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Value
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class MeetingLink
    {
        public static MeetingLink FromJson(string json) => JsonConvert.DeserializeObject<MeetingLink>(json);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
