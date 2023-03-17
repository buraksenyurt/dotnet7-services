using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockShop.Shared;

[Table("Album")]
public partial class Album
{
    [Key]
    public int AlbumId { get; set; }

    [StringLength(160)]
    public string Title { get; set; } = null!;

    public int ArtistId { get; set; }
}
