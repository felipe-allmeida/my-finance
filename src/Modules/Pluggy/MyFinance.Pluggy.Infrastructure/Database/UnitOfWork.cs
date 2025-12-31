using Microsoft.EntityFrameworkCore;
using MyFinance.Common.Domain;

namespace MyFinance.Pluggy.Infrastructure.Database;

internal class UnitOfWork(PluggyContext context) : IUnitOfWork
{
    public IQueryable<T> Set<T>() where T : Entity
    {
        return context.Set<T>().AsQueryable();
    }

    public IQueryable<T> ReadSet<T>() where T : Entity
    {
        return context.Set<T>().AsNoTracking().AsQueryable();
    }

    public async Task<T> AddAsync<T>(T entity, CancellationToken ct = default) where T : Entity
    {
        var a = await context.Set<T>().AddAsync(entity, ct);
        return a.Entity;
    }

    public Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken ct = default) where T : Entity
    {
        return context.Set<T>().AddRangeAsync(entities, ct);
    }

    public T Update<T>(T entity) where T : Entity
    {
        return context.Set<T>().Update(entity).Entity;
    }

    public T Delete<T>(T entity) where T : Entity
    {
        entity.Delete();
        return context.Set<T>().Update(entity).Entity;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }
}
