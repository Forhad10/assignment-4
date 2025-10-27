using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DataServiceLibrary;
using DataServiceLibrary.Data;
using DataServiceLibrary.DTOs;
using DataServiceLibrary.Models;

namespace Assignment4.Tests
{
    public class DataServiceTests : IDisposable
    {
        private readonly NorthwindContext _context;
        private readonly DataService _dataService;

        public DataServiceTests()
        {
            var options = new DbContextOptionsBuilder<NorthwindContext>()
                .UseInMemoryDatabase(databaseName: "NorthwindTestDb")
                .Options;
            _context = new NorthwindContext(options);
            _dataService = new DataService(_context);
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Beverages", Description = "Soft drinks" });
                _context.Categories.Add(new Category { CategoryId = 2, CategoryName = "Dairy Products", Description = "Dairy items" });
                _context.Products.Add(new Product { ProductId = 1, ProductName = "Chai", UnitPrice = 18, QuantityPerUnit = "10 boxes", UnitsInStock = 39, CategoryId = 1 });
                _context.Products.Add(new Product { ProductId = 11, ProductName = "Queso Cabrales", UnitPrice = 21, QuantityPerUnit = "1 kg pkg.", UnitsInStock = 22, CategoryId = 2 });
                _context.Orders.Add(new Order { OrderId = 10248, OrderDate = new DateTime(2020, 7, 4), ShipName = "Vins et alcools Chevalier", ShipCity = "Reims" });
                _context.OrderDetails.Add(new OrderDetail { OrderDetailId = 1, OrderId = 10248, ProductId = 11, UnitPrice = 14, Quantity = 12, Discount = 0 });
                _context.SaveChanges();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        /* Categories */

        [Fact]
        public async Task Category_Object_HasIdNameAndDescription()
        {
            var category = new Category();
            Assert.Equal(0, category.CategoryId);
            Assert.Null(category.CategoryName);
            Assert.Null(category.Description);
        }

        [Fact]
        public async Task GetAllCategories_NoArgument_ReturnsAllCategories()
        {
            var categories = await _dataService.GetAllCategoriesAsync();
            Assert.Equal(2, categories.Count()); 
            Assert.Equal("Beverages", categories.First().CategoryName);
        }

        [Fact]
        public async Task GetCategory_ValidId_ReturnsCategoryObject()
        {
            var category = await _dataService.GetCategoryByIdAsync(1);
            Assert.NotNull(category);
            Assert.Equal("Beverages", category.CategoryName);
        }

        [Fact]
        public async Task CreateCategory_ValidData_CreateCategoryAndReturnsNewObject()
        {
            var newCategory = new CategoryDto { CategoryName = "Test", Description = "CreateCategory_ValidData_CreateCategoryAndReturnsNewObject" };
            var category = await _dataService.AddCategoryAsync(newCategory);
            Assert.True(category.CategoryId > 0);
            Assert.Equal("Test", category.CategoryName);
            Assert.Equal("CreateCategory_ValidData_CreateCategoryAndReturnsNewObject", category.Description);

            // Cleanup
            await _dataService.DeleteCategoryAsync(category.CategoryId);
        }

        [Fact]
        public async Task DeleteCategory_ValidId_RemoveTheCategory()
        {
            var newCategory = new CategoryDto { CategoryName = "Test", Description = "DeleteCategory_ValidId_RemoveTheCategory" };
            var category = await _dataService.AddCategoryAsync(newCategory);
            var result = await _dataService.DeleteCategoryAsync(category.CategoryId);
            Assert.True(result);
            category = await _dataService.GetCategoryByIdAsync(category.CategoryId);
            Assert.Null(category);
        }

        [Fact]
        public async Task DeleteCategory_InvalidId_ReturnsFalse()
        {
            var result = await _dataService.DeleteCategoryAsync(-1);
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateCategory_NewNameAndDescription_UpdateWithNewValues()
        {
            var newCategory = new CategoryDto { CategoryName = "TestingUpdate", Description = "UpdateCategory_NewNameAndDescription_UpdateWithNewValues" };
            var category = await _dataService.AddCategoryAsync(newCategory);

            var updatedCategory = new CategoryDto { CategoryName = "UpdatedName", Description = "UpdatedDescription" };
            var result = await _dataService.UpdateCategoryAsync(category.CategoryId, updatedCategory);
            Assert.True(result);

            category = await _dataService.GetCategoryByIdAsync(category.CategoryId);

            Assert.Equal("UpdatedName", category.CategoryName);
            Assert.Equal("UpdatedDescription", category.Description);

            // Cleanup
            await _dataService.DeleteCategoryAsync(category.CategoryId);
        }

        [Fact]
        public async Task UpdateCategory_InvalidID_ReturnsFalse()
        {
            var updatedCategory = new CategoryDto { CategoryName = "UpdatedName", Description = "UpdatedDescription" };
            var result = await _dataService.UpdateCategoryAsync(-1, updatedCategory);
            Assert.False(result);
        }

        /* Products */

        [Fact]
        public async Task Product_Object_HasIdNameUnitPriceQuantityPerUnitAndUnitsInStock()
        {
            var product = new Product();
            Assert.Equal(0, product.ProductId);
            Assert.Null(product.ProductName);
            Assert.Null(product.UnitPrice);  
            Assert.Null(product.QuantityPerUnit);
            Assert.Equal(0, product.UnitsInStock);
        }

        [Fact]
        public async Task GetProduct_ValidId_ReturnsProductWithCategory()
        {
            var product = await _dataService.GetProductByIdAsync(1);
            Assert.NotNull(product);
            Assert.Equal("Chai", product.ProductName);
            Assert.Equal("Beverages", product.CategoryName);
        }

        [Fact]
        public async Task GetProductsByCategory_ValidId_ReturnsProductsWithCategory()
        {
            var products = await _dataService.GetProductsByCategoryIdAsync(1);
            Assert.Single(products); 
            Assert.Equal("Chai", products.First().ProductName);
            Assert.Equal("Beverages", products.First().CategoryName);
        }

        [Fact]
        public async Task GetProduct_NameSubstring_ReturnsProductsThatMatchTheSubstring()
        {
            var products = await _dataService.GetProductsBySubstringAsync("em");
            Assert.Empty(products); 
        }

        /* Orders */

        [Fact]
        public async Task Order_Object_HasIdDatesAndOrderDetails()
        {
            var order = new Order();
            Assert.Equal(0, order.OrderId);
            Assert.Null(order.OrderDate);  
            //Assert.Equal(default(DateTime), order.RequiredDate);
            //Assert.Null(order.OrderDetails);
            Assert.Null(order.ShipName);
            Assert.Null(order.ShipCity);
        }

        [Fact]
        public async Task GetOrder_ValidId_ReturnsCompleteOrder()
        {
            var order = await _dataService.GetOrderWithDetailsByIdAsync(10248);
            Assert.NotNull(order);
            Assert.Single(order.OrderDetails);
            Assert.Equal("Queso Cabrales", order.OrderDetails.First().Product.ProductName);
            Assert.Equal("Dairy Products", order.OrderDetails.First().Product.CategoryName);
        }

        [Fact]
        public async Task GetOrders()
        {
            var orders = await _dataService.GetAllOrdersAsync();
            Assert.Single(orders); 
        }

        /* OrderDetails */



        [Fact]
        public async Task GetOrderDetailByOrderId_ValidId_ReturnsProductNameUnitPriceAndQuantity()
        {
            var orderDetails = await _dataService.GetOrderDetailsByOrderIdAsync(10248);
            Assert.Single(orderDetails);
            Assert.Equal("Queso Cabrales", orderDetails.First().Product.ProductName);
            Assert.Equal(14, orderDetails.First().UnitPrice);
            Assert.Equal(12, orderDetails.First().Quantity);
        }

        [Fact]
        public async Task GetOrderDetailByProductId_ValidId_ReturnsOrderDateUnitPriceAndQuantity()
        {
            var orderDetails = await _dataService.GetOrderDetailsByProductIdAsync(11);
            Assert.Single(orderDetails);
            Assert.Equal(new DateTime(2020, 7, 4).ToString("yyyy-MM-dd"), orderDetails.First().OrderDate.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(14, orderDetails.First().UnitPrice);
            Assert.Equal(12, orderDetails.First().Quantity);
        }
    }
}