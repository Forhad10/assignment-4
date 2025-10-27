using DataServiceLibrary.DTOs;
using DataServiceLibrary;

using Microsoft.AspNetCore.Mvc;

namespace assigment4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly DataService _dataService;

    public CategoryController(DataService dataService)
    {
        _dataService = dataService;
    }

    // GET: api/Category/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _dataService.GetCategoryByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    // GET: api/Category
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _dataService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    // POST: api/Category
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> PostCategory(CategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdCategory = await _dataService.AddCategoryAsync(categoryDto);
        return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.CategoryId }, createdCategory);
    }

    // PUT: api/Category/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategory(int id, CategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _dataService.UpdateCategoryAsync(id, categoryDto);

        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // DELETE: api/Category/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _dataService.DeleteCategoryAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return Ok(); // 200
    }
}
