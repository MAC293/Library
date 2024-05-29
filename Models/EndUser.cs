using System;
using System.Collections.Generic;

namespace Library.Models;

public partial class EndUser
{
    public string Id { get; set; } = null!;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public virtual Librarian? Librarian { get; set; }

    public virtual Reader? Reader { get; set; }
}
