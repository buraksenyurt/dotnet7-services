using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RockShop.Shared;

[Table("Genre")]
public partial class Genre
{
    [Key]
    public int GenreId { get; set; }

    [StringLength(120)]
    public string? Name { get; set; }

    [InverseProperty("Genre")]
    public virtual ICollection<Track> Tracks { get; } = new List<Track>();
}
