using DataServiceLibrary.Data;
using DataServiceLibrary.DTOs;
using DataServiceLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataServiceLibrary;

public class DataService
{
    private readonly NorthwindContext _context;

    public DataService(NorthwindContext context)
    {
        _context = context;
    }




    // Order operations
    //1
    public async Task<OrderWithDetailsDto?> GetOrderWithDetailsByIdAsync(int orderId)
    {
        var order = await _context.Orders
    .Where(o => o.OrderId == orderId)
    .Select(o => new OrderWithDetailsDto
    {
        OrderId = o.OrderId,
        OrderDate = o.OrderDate,
        ShipName = o.ShipName,
        ShipCity = o.ShipCity,
        OrderDetails = _context.OrderDetails
            .Where(od => od.OrderId == o.OrderId)
            .Select(od => new OrderDetailWithProductDto
            {
                OrderDetailId = od.OrderDetailId,
                UnitPrice = od.UnitPrice,
                Quantity = od.Quantity,
                Discount = od.Discount,
                Product = _context.Products
                    .Where(p => p.ProductId == od.ProductId)
                    .Select(p => new ProductWithCategoryDto
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        UnitPrice = p.UnitPrice,
                        QuantityPerUnit = p.QuantityPerUnit,
                        UnitsInStock = p.UnitsInStock,
                        CategoryName = _context.Categories
                            .Where(c => c.CategoryId == p.CategoryId)
                            .Select(c => c.CategoryName)
                            .FirstOrDefault()
                    })
                    .FirstOrDefault()
            })
            .ToList()
    })
    .FirstOrDefaultAsync();


        return order;
    }

    //2
    public async Task<IEnumerable<OrderSummaryDto>> GetOrdersByShippingNameAsync(string shipName)
    {
        return await _context.Orders
            .Where(o => o.ShipName != null && o.ShipName.Contains(shipName))
            .Select(o => new OrderSummaryDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                ShipName = o.ShipName,
                ShipCity = o.ShipCity
            })
            .ToListAsync();
    }
    //3
    public async Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Select(o => new OrderSummaryDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                ShipName = o.ShipName,
                ShipCity = o.ShipCity
            })
            .ToListAsync();
    }

    //4
    public async Task<IEnumerable<OrderDetailWithProductDto>> GetOrderDetailsByOrderIdAsync(int orderId)
    {
        return await _context.OrderDetails
     .Where(od => od.OrderId == orderId)
     .Select(od => new OrderDetailWithProductDto
     {
         OrderDetailId = od.OrderDetailId,
         UnitPrice = od.UnitPrice,
         Quantity = od.Quantity,
         Discount = od.Discount,
         Product = _context.Products.Where(p => p.ProductId == od.ProductId)
             .Select(p => new ProductWithCategoryDto
             {
                 ProductId = p.ProductId,
                 ProductName = p.ProductName,
                 UnitPrice = p.UnitPrice,
                 QuantityPerUnit = p.QuantityPerUnit,
                 UnitsInStock = p.UnitsInStock,
                 CategoryName = _context.Categories
                     .Where(c => c.CategoryId == p.CategoryId)
                     .Select(c => c.CategoryName)
                     .FirstOrDefault()
             })
             .FirstOrDefault()
     })
     .ToListAsync();

    }

    //5
    public async Task<IEnumerable<OrderDetailWithOrderDto>> GetOrderDetailsByProductIdAsync(int productId)
    {
        return await _context.OrderDetails
       .Where(od => od.ProductId == productId)
       .Select(od => new OrderDetailWithOrderDto
       {
           OrderDetailId = od.OrderDetailId,
           OrderId = od.OrderId,
           OrderDate = _context.Orders
               .Where(o => o.OrderId == od.OrderId)
               .Select(o => o.OrderDate)
               .FirstOrDefault(),
           ShipName = _context.Orders
               .Where(o => o.OrderId == od.OrderId)
               .Select(o => o.ShipName)
               .FirstOrDefault(),
           ShipCity = _context.Orders
               .Where(o => o.OrderId == od.OrderId)
               .Select(o => o.ShipCity)
               .FirstOrDefault(),
           UnitPrice = od.UnitPrice,
           Quantity = od.Quantity,
           Discount = od.Discount,
           Product = _context.Products
               .Where(p => p.ProductId == od.ProductId)
               .Select(p => new ProductWithCategoryDto
               {
                   ProductId = p.ProductId,
                   ProductName = p.ProductName,
                   UnitPrice = p.UnitPrice,
                   QuantityPerUnit = p.QuantityPerUnit,
                   UnitsInStock = p.UnitsInStock,
                   CategoryName = _context.Categories
                       .Where(c => c.CategoryId == p.CategoryId)
                       .Select(c => c.CategoryName)
                       .FirstOrDefault()
               })
               .FirstOrDefault()
       })
       .OrderBy(dto => dto.OrderId)
       .ToListAsync();

    }


    // Product operations

    //6
    public async Task<ProductWithCategoryDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Where(p => p.ProductId == id)
            .Join(_context.Categories,
                  p => p.CategoryId,
                  c => c.CategoryId,
                  (p, c) => new ProductWithCategoryDto
                  {
                      ProductId = p.ProductId,
                      ProductName = p.ProductName,
                      UnitPrice = p.UnitPrice,
                      QuantityPerUnit = p.QuantityPerUnit,
                      UnitsInStock = p.UnitsInStock,
                      CategoryName = c.CategoryName
                  })
            .FirstOrDefaultAsync();

        return product;
    }

    //7
    public async Task<IEnumerable<ProductSearchDto>> GetProductsBySubstringAsync(string substring)
    {
        return await _context.Products
            .Where(p => p.ProductName != null && p.ProductName.Contains(substring))
            .Join(_context.Categories,
                  p => p.CategoryId,
                  c => c.CategoryId,
                  (p, c) => new ProductSearchDto
                  {
                      ProductName = p.ProductName,
                      CategoryName = c.CategoryName
                  })
            .ToListAsync();
    }
    //8
    public async Task<IEnumerable<ProductWithCategoryDto>> GetProductsByCategoryIdAsync(int categoryId)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .Join(_context.Categories,
                  p => p.CategoryId,
                  c => c.CategoryId,
                  (p, c) => new ProductWithCategoryDto
                  {
                      ProductId = p.ProductId,
                      ProductName = p.ProductName,
                      UnitPrice = p.UnitPrice,
                      QuantityPerUnit = p.QuantityPerUnit,
                      UnitsInStock = p.UnitsInStock,
                      CategoryName = c.CategoryName
                  })
            .ToListAsync();
    }

    // Category operations

    //9
    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null) return null;

        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description
        };
    }
    //10
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description
            })
            .ToListAsync();
    }
    //11
    public async Task<CategoryDto> AddCategoryAsync(CategoryDto categoryDto)
    {
        var category = new Category
        {
            CategoryName = categoryDto.CategoryName,
            Description = categoryDto.Description
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        categoryDto.CategoryId = category.CategoryId;
        return categoryDto;
    }
    //12
    public async Task<bool> UpdateCategoryAsync(int id, CategoryDto categoryDto)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null) return false;

        category.CategoryName = categoryDto.CategoryName;
        category.Description = categoryDto.Description;

        await _context.SaveChangesAsync();
        return true;
    }
    //13
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }



}
