using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorage
{
    public interface INoSqlStorage<T>
    {
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task Delete(string rowKey,string partitionKey);
        Task<T> Get(string rowKey,string partitionKey);
        IQueryable<T> GetAll();
        IQueryable<T> Query(Expression<Func<T, bool>> query);

    }
}
