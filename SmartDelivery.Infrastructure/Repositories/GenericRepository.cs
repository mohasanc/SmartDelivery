using Microsoft.EntityFrameworkCore;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Domain.Common;
using SmartDelivery.Infrastructure.Data;
using System.Linq.Expressions;

namespace SmartDelivery.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbset;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbset = context.Set<T>();
        }
        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
         => await _dbset.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbset.ToListAsync(cancellationToken);

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbset.Where(predicate).ToListAsync(cancellationToken);

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbset.FirstOrDefaultAsync(predicate, cancellationToken);

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbset.AnyAsync(predicate, cancellationToken);

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => predicate is null
                ? await _dbset.CountAsync(cancellationToken)
                : await _dbset.CountAsync(predicate, cancellationToken);

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbset.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            => await _dbset.AddRangeAsync(entities, cancellationToken);

        public virtual void Update(T entity)
        {
            _dbset.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
            =>  _dbset.UpdateRange(entities);
        public virtual void Delete(T entity)
            => _dbset.Remove(entity);

        public virtual void DeleteRange(IEnumerable<T> entities)
            => _dbset.RemoveRange(entities);

        public virtual void SoftDelete(T entity)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            Update(entity);
        }
        public virtual IQueryable<T> GetQueryable()
            => _dbset.AsQueryable();
    }
}
