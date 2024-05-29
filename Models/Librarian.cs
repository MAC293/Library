using System;
using System.Collections.Generic;

namespace Library.Models;

public partial class Librarian
{
    public string Id { get; set; } = null!;

    public string EndUser { get; set; } = null!;

    public virtual EndUser EndUserNavigation { get; set; } = null!;
}
