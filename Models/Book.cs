using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Library.Models;

public partial class Book
{
    public string Id { get; set; } = null!;

    public string? Title { get; set; }

    public string? Author { get; set; }

    public string? Genre { get; set; }

    public int? Year { get; set; }

    public string? Editorial { get; set; }

    public byte[]? Cover { get; set; }

    public bool? Available { get; set; }

    [JsonIgnore]
    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}
