using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace SimpleTaskManager.DAL.Repository.Interfaces
{
    public interface IRepository<T>
    {
        public IQueryable<T> FindAll(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        public Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        public Task<bool> AnyAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        public Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>>? predicate = null);


        public Task<T> CreateAsync(T item);

        public T Update(T item);

        public T Delete(T item);
    }
}
