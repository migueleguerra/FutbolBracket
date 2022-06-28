namespace FutbolBracket.Services
{
    using Microsoft.Azure.Cosmos;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICosmosDbService<TEntity> where TEntity : ICosmosDbEntity
    {
        Task<IEnumerable<TEntity>> GetAllEntitiesAsync();

        Task<TEntity> GetEntityAsync(string id, PartitionKey partitionKey);

        Task<TEntity> CreateEntityAsync(TEntity entity);

        Task<ItemResponse<TEntity>> UpdateEntityAsync(TEntity entity);

        Task<ItemResponse<TEntity>> DeleteEntityAsync(string id, PartitionKey partitionKey);
    }
}