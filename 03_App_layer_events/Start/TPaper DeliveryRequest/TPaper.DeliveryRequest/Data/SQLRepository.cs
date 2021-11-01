using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TPaper.DeliveryRequest
{
    public class SqlRepository<T> : IRepository<T> where T : class, IAggregateRootMarker
    {
        private readonly PaperDbContext context;

        private readonly DbSet<T> dbSet;

        public SqlRepository(PaperDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll(CancellationToken cts)
        {
            return await this.dbSet.AsNoTracking().ToListAsync(cts);
        }

        public async Task<T> AddAndReturn(T entity, CancellationToken cts)
        {
            EntityEntry<T> createdEntity = await this.context.AddAsync(entity, cts);
            await this.context.SaveChangesAsync(cts);
            return createdEntity.Entity;
        }

        public async Task Update(T entity, CancellationToken cts)
        {
            this.context.Update(entity);
            await this.context.SaveChangesAsync(cts);
        }
    }
}