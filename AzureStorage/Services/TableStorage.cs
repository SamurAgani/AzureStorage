using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorage.Services
{
    public class TableStorage<T> : INoSqlStorage<T> where T : TableEntity, new()
    {
        private readonly CloudTableClient _tableClient;
        private readonly CloudTable _table;

        public TableStorage()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConnectionString.AzureStorageConnectionString);
            _tableClient = cloudStorageAccount.CreateCloudTableClient();

            _table = _tableClient.GetTableReference(typeof(T).Name);

            _table.CreateIfNotExists();
        }
        public async Task<T> Add(T entity)
        {
            var operation = TableOperation.InsertOrMerge(entity);

            var execute = await _table.ExecuteAsync(operation);

            return execute.Result as T;
        }

        public async Task Delete(string rowKey, string partitionKey)
        {
            var entity = await Get(rowKey, partitionKey);

            var operation = TableOperation.Delete(entity);

            await _table.ExecuteAsync(operation);
        }

        public async Task<T> Get(string rowKey, string partitionKey)
        {
            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var execute = await _table.ExecuteAsync(operation);

            return execute.Result as T;
        }

        public IQueryable<T> GetAll()
        {
            return _table.CreateQuery<T>().AsQueryable();
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> query)
        {
            return _table.CreateQuery<T>().Where(query);
        }

        public async Task<T> Update(T entity)
        {
            var operation = TableOperation.Replace(entity);
            var execute = await _table.ExecuteAsync(operation);
            return execute.Result as T;
        }
    }
}
