using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Library.Models;

public partial class Borrow
{
    public string Id { get; set; } = null!;

    public DateTime BorrowDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string Reader { get; set; } = null!;

    public string Book { get; set; } = null!;

    [JsonIgnore]
    public virtual Book BookNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual Reader ReaderNavigation { get; set; } = null!;
}
