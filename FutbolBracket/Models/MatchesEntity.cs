namespace FutbolBracket.Models
{
    using FutbolBracket.Services;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;
    using System;

    public class MatchesEntity : ICosmosDbEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id => $"{CompetitionId}-{MatchDay}-{HomeTeam}-{AwayTeam}";

        [JsonIgnore]
        public PartitionKey PartitionKey => new PartitionKey(this.Id);

        [JsonProperty(PropertyName = "competitionId")]
        public string CompetitionId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "utcDate")]
        public DateTime UtcDate { get; set; }

        [JsonProperty(PropertyName = "matchDay")]
        public int MatchDay { get; set; }

        [JsonProperty(PropertyName = "winner")]
        public string Winner { get; set; }

        [JsonProperty(PropertyName = "venue")]
        public string Venue { get; set; }

        [JsonProperty(PropertyName = "homeTeam")]
        public string HomeTeam { get; set; }

        [JsonProperty(PropertyName = "awayTeam")]
        public string AwayTeam { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public string Duration { get; set; }

        [JsonProperty(PropertyName = "fullTimeHomeTeamScore")]
        public int FullTimeHomeTeamScore { get; set; }

        [JsonProperty(PropertyName = "fullTimeAwayTeamScore")]
        public int FullTimeAwayTeamScore { get; set; }

        [JsonProperty(PropertyName = "halfTimeHomeTeamScore")]
        public int HalfTimeHomeTeamScore { get; set; }

        [JsonProperty(PropertyName = "halfTimeAwayTeamScore")]
        public int HalftTimeAwayTeamScore { get; set; }

        [JsonProperty(PropertyName = "extraTimeHomeTeamScore")]
        public int ExtraTimeHomeTeamScore { get; set; }

        [JsonProperty(PropertyName = "extraTimeAwayTeamScore")]
        public int ExtraTimeAwayTeamScore { get; set; }

        [JsonProperty(PropertyName = "penaltiesHomeTeamScore")]
        public int PenaltiesHomeTeamScore { get; set; }

        [JsonProperty(PropertyName = "penaltiesAwayTeamScore")]
        public int PenaltiesAwayTeamScore { get; set; }

        [JsonIgnore]
        public int _ts { get; set; }
    }
}
