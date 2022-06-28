namespace FutbolBracket.Services
{
    using Microsoft.Azure.Cosmos;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    public class CosmosDbService<TEntity> : ICosmosDbService<TEntity> where TEntity : ICosmosDbEntity
    {
        private readonly CosmosClient cosmosClient;
        private readonly Container container;

        private CosmosDbService(CosmosClient cosmosClient, Container container)
        {
            this.cosmosClient = cosmosClient;
            this.container = container;
        }

        public static CosmosDbService<TEntity> Create(
            string connectionString,
            string databaseName,
            string containerName)
        {
            var cosmosClient = new CosmosClient(connectionString);
            var container = cosmosClient.GetContainer(databaseName, containerName);

            return new CosmosDbService<TEntity>(cosmosClient, container);
        }

        public async Task<TEntity> CreateEntityAsync(TEntity entity)
        {
            try
            {
                ItemResponse<TEntity> response = await this.container.CreateItemAsync(entity, entity.PartitionKey);
                TEntity createdEntity = response;
                return createdEntity;
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to {nameof(CreateEntityAsync)} {typeof(TEntity).Name}", exception);
            }
        }

        public async Task<ItemResponse<TEntity>> DeleteEntityAsync(string id, PartitionKey partitionKey)
        {
            try
            {
                ItemResponse<TEntity> response = await this.container.DeleteItemAsync<TEntity>(id, partitionKey);
                return response;
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to {nameof(DeleteEntityAsync)} {typeof(TEntity).Name}", exception);
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllEntitiesAsync()
        {
            try
            {
                List<TEntity> result = new List<TEntity>();
                using (FeedIterator<TEntity> resultSet = this.container.GetItemQueryIterator<TEntity>())
                {
                    while (resultSet.HasMoreResults)
                    {
                        FeedResponse<TEntity> response = await resultSet.ReadNextAsync();
                        result.AddRange(response);
                    }
                }

                return result;
            }
            catch (Exception exception) when (IsDueToEntityNotFound(exception))
            {
                return new List<TEntity>();
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to {nameof(GetAllEntitiesAsync)} {typeof(TEntity).Name}", exception);
            }
        }

        public async Task<TEntity> GetEntityAsync(string id, PartitionKey partitionKey)
        {
            try
            {
                var entity = await this.container.ReadItemAsync<TEntity>(id, partitionKey);
                return entity;
            }
            catch (Exception exception) when (IsDueToEntityNotFound(exception))
            {
                return default;
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to {nameof(GetEntityAsync)} {typeof(TEntity).Name}", exception);
            }
        }

        public async Task<ItemResponse<TEntity>> UpdateEntityAsync(TEntity entity)
        {
            try
            {
                ItemResponse<TEntity> response = await this.container.UpsertItemAsync(
                    partitionKey: entity.PartitionKey,
                    item: entity);
                return response;
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to {nameof(UpdateEntityAsync)} {typeof(TEntity).Name}", exception);
            }
        }

        private static bool IsDueToEntityNotFound(Exception exception)
        {
            while (true)
            {
                if (exception == null)
                {
                    return false;
                }
                else if (exception is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.NotFound)
                {
                    return true;
                }
                else
                {
                    exception = exception.InnerException;
                }
            }
        }
    }
}
