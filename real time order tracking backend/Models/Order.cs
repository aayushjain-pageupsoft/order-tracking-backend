//Models/OrdersController.cs
using Azure;

namespace real_time_order_tracking_backend.Models
{
    public class Order
    {
        public string PartitionKey { get; set; } = "Orders";
        public string RowKey { get; set; } // Order ID
        public string CustomerName { get; set; }
        public string Status { get; set; } = "Pending"; // Default status
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
    public class OrderDto
    {
        public string PartitionKey { get; set; } = "Orders";
        public string RowKey { get; set; } // Order ID
        public string CustomerName { get; set; }
        public string Status { get; set; } = "Pending";
    }


}
