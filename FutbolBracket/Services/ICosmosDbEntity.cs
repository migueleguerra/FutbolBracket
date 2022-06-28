namespace FutbolBracket.Services
{
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;

    public interface ICosmosDbEntity
    {
        [JsonIgnore]
        PartitionKey PartitionKey { get; }

        [JsonProperty(PropertyName = "id")]
        string Id { get; }
    }
}
