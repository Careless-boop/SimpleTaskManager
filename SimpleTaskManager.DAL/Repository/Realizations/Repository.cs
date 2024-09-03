using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManager.DAL.Repository.Interfaces;
using System.Linq.Expressions;

namespace SimpleTaskManager.DAL.Repository.Realizations
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly SimpleTaskManagerDbContext _dbContext = null!;

        protected Repository(SimpleTaskManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> FindAll(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            return GetQuery(predicate, include);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            return await GetQuery(predicate, include).ToListAsync();
        }

        public async Task<bool> AnyAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            return predicate == null ? await GetQuery(null, include).AnyAsync() : await GetQuery(null, include).AnyAsync(predicate);
        }

        public async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null ? await _dbContext.Set<T>().FirstOrDefaultAsync() : await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<T> CreateAsync(T item)
        {
            var temp = await _dbContext.Set<T>().AddAsync(item);
            return temp.Entity;
        }

        public T Update(T item)
        {
            return _dbContext.Set<T>().Update(item).Entity;
        }

        public T Delete(T item)
        {
            return _dbContext.Set<T>().Remove(item).Entity;
        }

        private IQueryable<T> GetQuery(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            var query = _dbContext.Set<T>().AsNoTracking();

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }
            if (include is not null)
            {
                query = include(query);
            }

            return query.AsNoTracking();
        }
    }
}
