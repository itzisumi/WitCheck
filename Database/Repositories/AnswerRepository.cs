using Database.DbContext;
using Database.WitCheckEntities;

namespace Database;

public class AnswerRepository : ARepository<Answer>
{
    public AnswerRepository(WitCheckContext context) : base(context)
    {
    }
}