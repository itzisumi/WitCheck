using Database.DbContext;
using Database.WitCheckEntities;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class UserRepository:ARepository<User>
{
    public UserRepository(WitCheckContext context) : base(context)
    {
    }

    public override async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet.Where(i=>i.UserId==id).Include(p=>p.Password).FirstAsync();
    }
}