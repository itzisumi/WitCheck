using System.Linq.Expressions;
using Database.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class ARepository<T> where T : class
{
    protected readonly WitCheckContext _context;
    protected readonly DbSet<T> _dbSet;

    public ARepository(WitCheckContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        var ent=await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
        return ent.Entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entity)
    {
        await _dbSet.AddRangeAsync(entity);
        await SaveChangesAsync();
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        var ent=_dbSet.Update(entity);
        await SaveChangesAsync();
        return ent.Entity;
    }

    public virtual async Task<T> RemoveAsync(T entity)
    {
        var ent=_dbSet.Remove(entity);
        await SaveChangesAsync();
        return ent.Entity;
    }

    private void SaveChanges()
    {
        _context.SaveChanges();
    }

    private async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}