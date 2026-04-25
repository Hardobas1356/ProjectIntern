using System.Linq.Expressions;

namespace ProjectIntern.Services.Core.Repository.Interfaces;

public interface IGenericRepository<T> : IAsyncRepository<T>, IIgnoreQueryFilters<T>,
    ISynchronousRepository<T>, IQueryableRepository<T>
    where T : class
{
}