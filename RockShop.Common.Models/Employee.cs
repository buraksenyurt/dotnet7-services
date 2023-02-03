using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RockShop.Shared;

[Table("Employee")]
[Index("ReportsTo", Name = "IFK_EmployeeReportsTo")]
public partial class Employee
{
    [Key]
    public int EmployeeId { get; set; }

    [StringLength(20)]
    public string LastName { get; set; } = null!;

    [StringLength(20)]
    public string FirstName { get; set; } = null!;

    [StringLength(30)]
    public string? Title { get; set; }

    public int? ReportsTo { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? BirthDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? HireDate { get; set; }

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
    public string? Email { get; set; }

    [InverseProperty("SupportRep")]
    public virtual ICollection<Customer> Customers { get; } = new List<Customer>();

    [InverseProperty("ReportsToNavigation")]
    public virtual ICollection<Employee> InverseReportsToNavigation { get; } = new List<Employee>();

    [ForeignKey("ReportsTo")]
    [InverseProperty("InverseReportsToNavigation")]
    public virtual Employee? ReportsToNavigation { get; set; }
}
