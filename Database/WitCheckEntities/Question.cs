using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Database.WitCheckEntities;

public partial class Question
{
    public int QuestionId { get; set; }

    public int QuizId { get; set; }

    public sbyte OrderNumber { get; set; }

    public int Points { get; set; }

    public string QuestionText { get; set; } = null!;

    public short? Time { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    [JsonIgnore]
    public virtual Quiz Quiz { get; set; } = null!;
}
