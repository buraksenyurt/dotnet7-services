using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RockShop.Shared;

[Table("Invoice")]
public partial class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    public int CustomerId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime InvoiceDate { get; set; }

    [StringLength(70)]
    public string? BillingAddress { get; set; }

    [StringLength(40)]
    public string? BillingCity { get; set; }

    [StringLength(40)]
    public string? BillingState { get; set; }

    [StringLength(40)]
    public string? BillingCountry { get; set; }

    [StringLength(10)]
    public string? BillingPostalCode { get; set; }

    [Precision(10, 2)]
    public decimal Total { get; set; }
}
