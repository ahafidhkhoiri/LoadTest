using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MsTeamStressTest.Models
{
    public partial class SendMeetingLinksModel
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
}
