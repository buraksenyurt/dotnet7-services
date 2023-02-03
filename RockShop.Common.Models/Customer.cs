using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RockShop.Shared;

[Table("Customer")]
[Index("SupportRepId", Name = "IFK_CustomerSupportRepId")]
public partial class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [StringLength(40)]
    public string FirstName { get; set; } = null!;

    [StringLength(20)]
    public string LastName { get; set; } = null!;

    [StringLength(80)]
    public string? Company { get; set; }

    [StringLength(70)]
    public string? Address { get; set; }

    [StringLength(40)]
    public string? City { get; set; }

    [StringLength(40)]
    public string? State { get; set; }

    [StringLength(40)]
    public string? Country { get; set; }

    [StringLength(10)]
    public string? PostalCode { get; set; }

    [StringLength(24)]
    public string? Phone { get; set; }

    [StringLength(24)]
    public string? Fax { get; set; }

    [StringLength(60)]
    public string Email { get; set; } = null!;

    public int? SupportRepId { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Invoice> Invoices { get; } = new List<Invoice>();

    [ForeignKey("SupportRepId")]
    [InverseProperty("Customers")]
    public virtual Employee? SupportRep { get; set; }
}
