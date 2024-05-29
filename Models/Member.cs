using System;
using System.Collections.Generic;

namespace Library.Models;

public partial class Member
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public int? Phone { get; set; }

    public string? Email { get; set; }

    public int? Age { get; set; }

    public virtual Reader? Reader { get; set; }
}
