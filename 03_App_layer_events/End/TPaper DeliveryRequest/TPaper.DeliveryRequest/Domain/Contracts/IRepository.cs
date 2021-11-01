using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace TPaper.DeliveryRequest
{
    public interface IRepository<T> where T : class, IAggregateRootMarker
    {
        Task<IEnumerable<T>> GetAll(CancellationToken cts);

        Task<T> AddAndReturn(T entity, CancellationToken cts);

        Task Update(T entity, CancellationToken cts);
    }
}