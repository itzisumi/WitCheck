using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Database.WitCheckEntities;

public partial class Answer
{
    public int AnswerId { get; set; }

    public string? Answer1 { get; set; }

    public sbyte IsCorrect { get; set; }

    public int QuestionId { get; set; }

    public sbyte OrderNumber { get; set; }
    [JsonIgnore]
    public virtual Question Question { get; set; } = null!;
}
