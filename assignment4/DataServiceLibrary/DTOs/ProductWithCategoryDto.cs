namespace DataServiceLibrary.DTOs;
/// for  products 
public class ProductWithCategoryDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? QuantityPerUnit { get; set; }
    public int? UnitsInStock { get; set; }
    public string? CategoryName { get; set; }
}
