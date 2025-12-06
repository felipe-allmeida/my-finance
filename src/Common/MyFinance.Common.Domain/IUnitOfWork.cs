using System;
using System.Collections.Generic;
using System.Text;

namespace MyFinance.Common.Domain;

public interface IUnitOfWork
{
    public IQueryable<T> Set<T>() where T : Entity;
    public IQueryable<T> ReadSet<T>() where T : Entity;
    public Task AddAsync<T>(T entity, CancellationToken ct = default) where T : Entity;
    public Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken ct = default) where T : Entity;
    public void Update<T>(T entity) where T : Entity;
    public void Delete<T>(T entity)where T : Entity;
    public Task SaveChangesAsync(CancellationToken ct = default);
}