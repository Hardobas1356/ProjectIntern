using ProjectIntern.Data;
using Microsoft.EntityFrameworkCore;
using ProjectIntern.Services.Core.Repository.Interfaces;
using System.Linq.Expressions;

namespace ProjectIntern.Services.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext dbContext;
        private readonly DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.dbSet = dbContext.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            await dbSet.AddRangeAsync(entities);
        }

        public void Delete(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            dbSet.RemoveRange(entities);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate,
                                         bool asNoTracking = true,
                                         bool ignoreQueryFilters = false)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            IQueryable<T> query = BuildQueryable(asNoTracking, ignoreQueryFilters);

            return await query
                 .AnyAsync(predicate);
        }
        public async Task<IEnumerable<T>> GetAllWithIncludeAsync(Func<IQueryable<T>, IQueryable<T>> include,
                                                          bool asNoTracking = true,
                                                          bool ignoreQueryFilters = false)
        {
            ArgumentNullException.ThrowIfNull(include);
            IQueryable<T> query = BuildQueryable(asNoTracking, ignoreQueryFilters);

            query = include(query);

            return await query
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = true,
                                                      bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = BuildQueryable(asNoTracking, ignoreQueryFilters);

            return await query
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id,
                                           bool asNoTracking = true,
                                           bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = BuildQueryable(asNoTracking, ignoreQueryFilters);

            return await query
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate,
                                                        bool asNoTracking = true,
                                                        bool ignoreQueryFilters = false)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            IQueryable<T> query = BuildQueryable(asNoTracking, ignoreQueryFilters);

            query = query.Where(predicate);

            return await query
                .ToListAsync();
        }
        public async Task<IEnumerable<T>> GetWhereWithIncludeAsync(Expression<Func<T, bool>> predicate,
                                                                   Func<IQueryable<T>, IQueryable<T>> include,
                                                                   bool asNoTracking,
                                                                   bool ignoreQueryFilters)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(include);
            IQueryable<T> query = BuildQueryable(asNoTracking, ignoreQueryFilters)
                .Where(predicate);
            query = include(query);
            return await query.ToListAsync();
        }

        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate,
                                                    bool asNoTracking,
                                                    bool ignoreQueryFilters)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            IQueryable<T> query = BuildQueryable(asNoTracking, ignoreQueryFilters);

            return await query
                .SingleOrDefaultAsync(predicate);
        }

        public async Task<T?> SingleOrDefaultWithIncludeAsync(Expression<Func<T, bool>> predicate,
                                                        Func<IQueryable<T>, IQueryable<T>> include,
                                                        bool asNoTracking,
                                                        bool ignoreQueryFilters)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(include);
            IQueryable<T> query = BuildQueryable(asNoTracking, ignoreQueryFilters)
                .Where(predicate);

            query = include(query);

            return await query
               .SingleOrDefaultAsync();
        }
        public IQueryable<T> GetQueryable(bool asNoTracking = true,
                                          bool ignoreQueryFilters = false)
        {
            return BuildQueryable(asNoTracking, ignoreQueryFilters);
        }
        public void Update(T entity)
        {
            dbSet.Update(entity);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
        public IQueryable<T> IgnoreQueryFilters()
        {
            return dbSet.IgnoreQueryFilters();
        }

        private IQueryable<T> BuildQueryable(bool asNoTracking,
                                             bool ignoreQueryFilters)
        {
            IQueryable<T> query = dbSet;

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (asNoTracking)
                query = query.AsNoTracking();

            return query;
        }
    }
}
