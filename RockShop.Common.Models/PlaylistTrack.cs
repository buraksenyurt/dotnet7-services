using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RockShop.Shared;

[Keyless]
[Table("PlaylistTrack")]
public partial class PlaylistTrack
{
    public int PlaylistId { get; set; }

    public int TrackId { get; set; }
}
