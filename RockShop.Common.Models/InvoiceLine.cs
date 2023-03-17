using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RockShop.Shared;

[Table("InvoiceLine")]
public partial class InvoiceLine
{
    [Key]
    public int InvoiceLineId { get; set; }

    public int InvoiceId { get; set; }

    public int TrackId { get; set; }

    [Precision(10, 2)]
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
}
