using Newtonsoft.Json;
using System;

namespace MsTeamStressTest.Models
{
    public class EnvironmentModel
    {
        [JsonProperty("MY_URL")]
        public string MyUrl { get; set; }

        [JsonProperty("APP_ID")]
        public string AppId { get; set; }

        [JsonProperty("APP_PASSWORD")]
        public string AppPassword { get; set; }

        [JsonProperty("RESULT_MEETINGLINK")]
        public string ResultMeetinglink { get; set; }

        [JsonProperty("RESULT_GO_ACCOUNT")]
        public string ResultGoAccount { get; set; }

        [JsonProperty("CSV_DATA")]
        public string CsvData { get; set; }
    }

    public class AppSettings
    {
        [JsonProperty("ENV")]
        public string Env { get; set; }

        [JsonProperty("THREAD")]
        public int Thread { get; set; }

        [JsonProperty("TOTAL_ITERATIONS")]
        public int TotalIterations { get; set; }
    }
}
