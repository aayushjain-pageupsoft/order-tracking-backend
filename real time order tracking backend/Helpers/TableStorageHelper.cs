//Helpers/TableStorageHelper.cs
using Azure;
using Azure.Data.Tables;
using real_time_order_tracking_backend.Models;

public class TableStorageHelper
{
    private readonly TableClient _tableClient;

    public TableStorageHelper(string connectionString, string tableName)
    {
        var serviceClient = new TableServiceClient(connectionString);
        _tableClient = serviceClient.GetTableClient(tableName);
        _tableClient.CreateIfNotExists();
    }

    public async Task AddOrderAsync(OrderDto order)
    {
        var result = await _tableClient.AddEntityAsync(order.ToTableEntity());
    }

    public async Task UpdateOrderStatusAsync(string orderId, string status)
    {
        try
        {
            var entity = await _tableClient.GetEntityAsync<OrderEntity>("Orders", orderId);
            entity.Value.Status = status;
            await _tableClient.UpdateEntityAsync(entity.Value, ETag.All, TableUpdateMode.Replace);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            Console.WriteLine($"Order with ID {orderId} not found.");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating order status: {ex.Message}");
            throw;
        }
    }

    public async Task<Order> GetOrderAsync(string orderId)
    {
        var entity = await _tableClient.GetEntityAsync<OrderEntity>("Orders", orderId);
        return entity.Value.ToOrder();
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        var orders = new List<Order>();

        await foreach (var entity in _tableClient.QueryAsync<OrderEntity>())
        {
            orders.Add(entity.ToOrder());
        }

        return orders;
    }
}

public static class OrderExtensions
{
    public static OrderEntity ToTableEntity(this OrderDto order)
    {
        return new OrderEntity
        {
            PartitionKey = order.PartitionKey,
            RowKey = order.RowKey,
            CustomerName = order.CustomerName,
            Status = order.Status,
            
        };
    }

    public static Order ToOrder(this OrderEntity entity)
    {
        return new Order
        {
            PartitionKey = entity.PartitionKey,
            RowKey = entity.RowKey,
            CustomerName = entity.CustomerName,
            Status = entity.Status
        };
    }
}

public class OrderEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string CustomerName { get; set; }
    public string Status { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

}
