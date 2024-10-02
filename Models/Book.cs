using Library.CustomDataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static StackExchange.Redis.Role;

namespace Library.Models;

public partial class Book
{
    //[Required(ErrorMessage = "ID field is required.")]
    //[MaxLength(60, ErrorMessage = "ID cannot exceed 60 characters.")]
    public string? Id { get; set; } = null!;

    //[Required(ErrorMessage = "Title field is required.")]
    //[MaxLength(35, ErrorMessage = "Title length cannot exceed 35 characters.")]
    public string? Title { get; set; }

    //[Required(ErrorMessage = "Author field is required.")]
    //[MaxLength(30, ErrorMessage = "Author cannot exceed 30 characters.")]
    public string? Author { get; set; }

    //[Required(ErrorMessage = "Genre field is required.")]
    //[MaxLength(20, ErrorMessage = "Genre cannot exceed 20 characters.")]
    public string? Genre { get; set; }

    //[Required(ErrorMessage = "Year field is required.")]
    //[Range(1950, 2024, ErrorMessage = "Year must be between 1950 and 2024.")]
    public int? Year { get; set; }

    //[Required(ErrorMessage = "Editorial field is required.")]
    //[MaxLength(20, ErrorMessage = "Editorial cannot exceed 20 characters.")]
    public string? Editorial { get; set; }

    //[Required]
    public bool Available { get; set; }
    
    public byte[] Cover { get; set; } = Array.Empty<Byte>();
    //public IFormFile Cover { get; set; }

    [JsonIgnore]
    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}
