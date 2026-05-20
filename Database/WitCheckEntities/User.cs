using System;
using System.Collections.Generic;

namespace Database.WitCheckEntities;

public partial class User
{
    public int UserId { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual Password? Password { get; set; }

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
