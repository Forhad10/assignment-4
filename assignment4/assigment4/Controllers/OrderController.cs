using DataServiceLibrary.DTOs;
using DataServiceLibrary;

using Microsoft.AspNetCore.Mvc;

namespace assigment4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly DataService _dataService;

    public OrderController(DataService dataService)
    {
        _dataService = dataService;
    }

    // Requirement 3: List all orders (ID, date, shipping name, city)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetAllOrders()
    {
        var orders = await _dataService.GetAllOrdersAsync();
        return Ok(orders);
    }

    // Requirement 1: Get a single order by ID with complete details and order details
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderWithDetailsDto>> GetOrderWithDetails(int id)
    {
        var order = await _dataService.GetOrderWithDetailsByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    // Requirement 2: Get orders by shipping name
    [HttpGet("shipping/{shipName}")]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetOrdersByShippingName(string shipName)
    {
        var orders = await _dataService.GetOrdersByShippingNameAsync(shipName);
        return Ok(orders);
    }

    // Requirement 4: Get details for a specific order ID (order details including product)
    [HttpGet("{orderId}/details")]
    public async Task<ActionResult<IEnumerable<OrderDetailWithProductDto>>> GetOrderDetails(int orderId)
    {
        var orderDetails = await _dataService.GetOrderDetailsByOrderIdAsync(orderId);
        return Ok(orderDetails);
    }

    // Requirement 5: Get details for a specific product ID (order details including product and order)
    [HttpGet("product/{productId}/details")]
    public async Task<ActionResult<IEnumerable<OrderDetailWithOrderDto>>> GetOrderDetailsByProduct(int productId)
    {
        var orderDetails = await _dataService.GetOrderDetailsByProductIdAsync(productId);
        return Ok(orderDetails);
    }
}
