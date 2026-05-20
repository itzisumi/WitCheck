using Database.DbContext;
using Database.WitCheckEntities;

namespace Database;

public class PasswordRepository:ARepository<Password>
{
    public PasswordRepository(WitCheckContext context) : base(context)
    {
    }

    public async Task<string> getHashedPassword(int passwordid)
    {
        return (await base.GetByIdAsync(passwordid)).Password1;
    }
}