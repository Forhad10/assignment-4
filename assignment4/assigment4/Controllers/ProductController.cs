using DataServiceLibrary;
using DataServiceLibrary.DTOs;
using DataServiceLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace assigment4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly DataService _dataService;

    public ProductController(DataService dataService)
    {
        _dataService = dataService;
    }


    // Requirement: Get a single product by ID: Return Id, Name, UnitPrice, QuantityPerUnit and UnitsInStock from product plus the name of the category
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductWithCategoryDto>> GetProduct(int id)
    {
        var product = await _dataService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }


    // Requirement 7: Get products by substring
    [HttpGet("search/{substring}")]
    public async Task<ActionResult<IEnumerable<ProductSearchDto>>> GetProductsBySubstring(string substring)
    {
        var products = await _dataService.GetProductsBySubstringAsync(substring);
        return Ok(products);
    }


    // Requirement 8: Get products by category ID
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductWithCategoryDto>>> GetProductsByCategory(int categoryId)
    {
        var products = await _dataService.GetProductsByCategoryIdAsync(categoryId);

        if (products == null  || !products.Any())
        {
            return NotFound();
        }
        return Ok(products);
    }
}
