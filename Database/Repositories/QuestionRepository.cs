using System.Linq.Expressions;
using Database.DbContext;
using Database.WitCheckEntities;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class QuestionRepository: ARepository<Question>
{
    public QuestionRepository(WitCheckContext context) : base(context)
    {
    }

   
}