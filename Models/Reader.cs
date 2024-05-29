using System;
using System.Collections.Generic;

namespace Library.Models;

public partial class Reader
{
    public string Id { get; set; } = null!;

    public string Member { get; set; } = null!;

    public string EndUser { get; set; } = null!;

    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();

    public virtual EndUser EndUserNavigation { get; set; } = null!;

    public virtual Member MemberNavigation { get; set; } = null!;
}
