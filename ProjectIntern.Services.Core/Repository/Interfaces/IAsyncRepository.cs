using System.Linq.Expressions;

namespace ProjectIntern.Services.Core.Repository.Interfaces;

public interface IAsyncRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id,
                          bool asNoTracking = true,
                          bool ignoreQueryFilters = false);
    Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = true,
                                     bool ignoreQueryFilters = false);
    Task<IEnumerable<T>> GetAllWithIncludeAsync(Func<IQueryable<T>, IQueryable<T>> include,
                                               bool asNoTracking = true,
                                               bool ignoreQueryFilters = false);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate,
                                  bool asNoTracking = true,
                                  bool ignoreQueryFilters = false);
    Task<T?> SingleOrDefaultWithIncludeAsync(Expression<Func<T, bool>> predicate,
                                             Func<IQueryable<T>, IQueryable<T>> include,
                                             bool asNoTracking = true,
                                             bool ignoreQueryFilters = false);
    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate,
                                       bool asNoTracking = true,
                                       bool ignoreQueryFilters = false);
    Task<IEnumerable<T>> GetWhereWithIncludeAsync(Expression<Func<T, bool>> predicate,
                                                  Func<IQueryable<T>, IQueryable<T>> include,
                                                  bool asNoTracking = true,
                                                  bool ignoreQueryFilters = false);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate,
                        bool asNoTracking = true,
                        bool ignoreQueryFilters = false);
    Task<int> SaveChangesAsync();
}
