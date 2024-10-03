using Library.CustomDataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static StackExchange.Redis.Role;

namespace Library.Models;

public partial class Book
{
    public string? Id { get; set; } = null!;

    public string? Title { get; set; }

    public string? Author { get; set; }

    public string? Genre { get; set; }

    public int? Year { get; set; }

    public string? Editorial { get; set; }

    public bool Available { get; set; }
    
    public byte[] Cover { get; set; } = Array.Empty<Byte>();

    [JsonIgnore]
    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}
