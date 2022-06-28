namespace FutbolBracket.Models
{
    using FutbolBracket.Services;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class CompetitionEntity : ICosmosDbEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonIgnore]
        public PartitionKey PartitionKey => new PartitionKey(this.Id);

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "shortName")]
        public string ShortName { get; set; }

        [JsonProperty(PropertyName = "teams")]
        public List<TeamEntity> Teams { get; set; }

        [JsonProperty(PropertyName = "logoUrl")]
        public string LogoUrl { get; set; }

        [JsonProperty(PropertyName = "startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty(PropertyName = "currentMatchDay")]
        public int CurrentMatchDay { get; set; }

        [JsonProperty(PropertyName = "winner")]
        public string Winner { get; set; }

        [JsonIgnore]
        public int _ts { get; set; }
    }
}
