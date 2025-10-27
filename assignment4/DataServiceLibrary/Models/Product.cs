using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataServiceLibrary.Models;

public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    [StringLength(200)]
    public string? ProductName { get; set; }

    public int? SupplierId { get; set; }

    public int? CategoryId { get; set; }

    [StringLength(100)]
    public string? QuantityPerUnit { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? UnitPrice { get; set; }

    public int? UnitsInStock { get; set; }
}
