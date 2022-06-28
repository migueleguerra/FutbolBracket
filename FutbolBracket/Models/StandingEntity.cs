namespace FutbolBracket.Models
{
    using FutbolBracket.Services;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;

    public class StandingEntity : ICosmosDbEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonIgnore]
        public PartitionKey PartitionKey => new PartitionKey(this.Id);

        [JsonProperty(PropertyName = "competitionId")]
        public string CompetitionId { get; set; }

        [JsonProperty(PropertyName = "teamId")]
        public string TeamId { get; set; }

        [JsonProperty(PropertyName = "position")]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "played")]
        public int Played { get; set; }

        [JsonProperty(PropertyName = "won")]
        public int Won { get; set; }

        [JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }

        [JsonProperty(PropertyName = "lost")]
        public int Lost { get; set; }

        [JsonProperty(PropertyName = "points")]
        public int Points { get; set; }

        [JsonProperty(PropertyName = "goalsFor")]
        public int GoalsFor { get; set; }

        [JsonProperty(PropertyName = "goalsAgainst")]
        public int GoalsAgainst { get; set; }

        [JsonProperty(PropertyName = "goalsDifference")]
        public int GoalsDifference { get; set; }

        [JsonIgnore]
        public int _ts { get; set; }
    }
}
