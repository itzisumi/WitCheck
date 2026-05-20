using System.Linq.Expressions;
using System.Text.Json;
using Database.DbContext;
using Database.WitCheckEntities;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class QuizRepository:ARepository<Quiz>
{
    public QuizRepository(WitCheckContext context) : base(context)
    {
    }

    public override async Task<Quiz?> GetByIdAsync(int id)
    {
        return await _dbSet.Where((i=>i.QuizId==id)).Include(x=>x.Questions).ThenInclude(q=>q.Answers).FirstAsync();
    }
   
}