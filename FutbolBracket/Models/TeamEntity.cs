namespace FutbolBracket.Models
{
    using FutbolBracket.Services;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;

    public class TeamEntity : ICosmosDbEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonIgnore]
        public PartitionKey PartitionKey => new PartitionKey(this.Id);

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "logoUrl")]
        public string LogoUrl { get; set; }

        [JsonProperty(PropertyName = "venue")]
        public string Venue { get; set; }

        [JsonIgnore]
        public int _ts { get; set; }
    }
}
