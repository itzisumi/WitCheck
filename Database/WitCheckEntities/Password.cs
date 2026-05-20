using System;
using System.Collections.Generic;

namespace Database.WitCheckEntities;

public partial class Password
{
    public int UserId { get; set; }

    public string Password1 { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
