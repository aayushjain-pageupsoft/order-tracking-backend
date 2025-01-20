//Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using real_time_order_tracking_backend.Models;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly TableStorageHelper _tableStorage;
    private readonly ServiceBusHelper _serviceBus;
    public OrdersController(TableStorageHelper tableStorage, ServiceBusHelper serviceBus)
    {
        _tableStorage = tableStorage;
        _serviceBus = serviceBus;
    }

    [HttpGet]
    public async Task<ActionResult<List<Order>>> Get()
    {
        // Assuming your TableStorageHelper has a method to fetch all orders
        var orders = await _tableStorage.GetAllOrdersAsync();
        if (orders == null || orders.Count == 0)
        {
            return NotFound("No orders found.");
        }

        var orderId = new Guid().ToString();
        // Return the list of orders
        return Ok(orders);
        //return Ok();
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDto order)
    {
        order.RowKey = Guid.NewGuid().ToString(); // Generate Order ID
        await _tableStorage.AddOrderAsync(order);
        return Ok(new { Message = "Order created successfully.", OrderId = order.RowKey });
    }

    [HttpPut("update-status/{orderId}")]
    public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] string status)
    {
        await _tableStorage.UpdateOrderStatusAsync(orderId, status);
        // Send status update to Service Bus
        await _serviceBus.SendMessageAsync($"Order:{orderId},Status:{status}");
        return Ok(new { Message = "Order status updated successfully." });
    }
}
