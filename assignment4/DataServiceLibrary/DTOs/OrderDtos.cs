namespace DataServiceLibrary.DTOs;

public class OrderWithDetailsDto
{
    public int OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? ShipName { get; set; }
    public string? ShipCity { get; set; }
    public List<OrderDetailWithProductDto> OrderDetails { get; set; } = new();
}

public class OrderDetailWithProductDto
{
    public int OrderDetailId { get; set; }
    public decimal? UnitPrice { get; set; }
    public int? Quantity { get; set; }
    public double? Discount { get; set; }
    public ProductWithCategoryDto Product { get; set; } = new();
}

public class OrderSummaryDto
{
    public int OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? ShipName { get; set; }
    public string? ShipCity { get; set; }
}

public class OrderDetailWithOrderDto
{
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? ShipName { get; set; }
    public string? ShipCity { get; set; }
    public decimal? UnitPrice { get; set; }
    public int? Quantity { get; set; }
    public double? Discount { get; set; }
    public ProductWithCategoryDto Product { get; set; } = new();
}
