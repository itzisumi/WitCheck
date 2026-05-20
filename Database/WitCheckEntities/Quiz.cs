using System;
using System.Collections.Generic;

namespace Database.WitCheckEntities;

public partial class Quiz
{
    public int QuizId { get; set; }

    public int UserId { get; set; }

    public string Quizname { get; set; } = null!;

    public string? ColorCode { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual User User { get; set; } = null!;
}
