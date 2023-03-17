using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockShop.Shared;

[Table("Playlist")]
public partial class Playlist
{
    [Key]
    public int PlaylistId { get; set; }

    [StringLength(120)]
    public string? Name { get; set; }
}
