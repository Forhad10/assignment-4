using System.ComponentModel.DataAnnotations;

namespace DataServiceLibrary.DTOs;

//For  Category operations
public class CategoryDto   
{
    public int CategoryId { get; set; }

    [StringLength(100)]
    public string? CategoryName { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}
