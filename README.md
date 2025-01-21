# Real-Time Order Tracking Backend

## Overview
This project implements a real-time order tracking system using .NET Core, Azure Table Storage, Azure Service Bus, and SignalR. It enables clients to create and track orders, update order statuses, and receive real-time updates.

---

## Features

1. **Order Management**
   - Create new orders.
   - Retrieve a list of all orders.
   - Update the status of orders.

2. **Real-Time Updates**
   - Real-time order status updates via SignalR.

3. **Azure Service Integration**
   - **Table Storage**: Store and manage orders.
   - **Service Bus**: Handle order status updates asynchronously.

4. **Scalability and Flexibility**
   - Built-in CORS support for frontend integration.
   - Background service for listening to Service Bus messages.

---

## Folder Structure

### `Controllers`
- **OrdersController**: Handles API endpoints for order management.

### `Helpers`
- **TableStorageHelper**: Interacts with Azure Table Storage to store, update, and retrieve orders.
- **ServiceBusHelper**: Sends and receives messages to/from Azure Service Bus.

### `Models`
- **OrderDto**: Data transfer object for orders.
- **OrderEntity**: Azure Table Storage entity model.

### `Hubs`
- **OrderHub**: SignalR hub for broadcasting real-time order updates.

### `Services`
- **ServiceBusListenerService**: Background service for processing Service Bus messages and broadcasting updates via SignalR.

---

## API Endpoints

### **GET /api/orders**
Retrieve all orders.
- **Response**:
  - `200 OK`: List of orders.
  - `404 Not Found`: No orders found.

### **POST /api/orders/create**
Create a new order.
- **Request Body**:
  ```json
  {
    "PartitionKey": "<string>",
    "CustomerName": "<string>",
    "ProductName": "<string>",
    "Status": "<string>"
  }
  ```
- **Response**:
  - `200 OK`: Order creation confirmation with Order ID.

### **PUT /api/orders/update-status/{orderId}**
Update the status of an order.
- **Request Body**:
  ```json
  "<string>" // Status
  ```
- **Response**:
  - `200 OK`: Status update confirmation.

---

## SignalR Integration

### Endpoint
- **/orderHub**: SignalR hub for real-time updates.

### Methods
- **BroadcastOrderUpdate**: Broadcasts order updates to all connected clients.

---

## Azure Integration

### Table Storage
- Used for storing order data.
- Connection string and table name configured in `appsettings.json`.

### Service Bus
- Used for handling asynchronous communication.
- Processes order status updates and broadcasts them via SignalR.

---

## Configuration

### `appsettings.json`
Configure the following:
```json
{
  "ServiceBus": {
    "ConnectionString": "<Your Service Bus Connection String>",
    "QueueName": "OrderTracker"
  },
  "TableStorage": {
    "ConnectionString": "<Your Table Storage Connection String>",
    "TableName": "OrderTracking"
  }
}
```

### CORS
- Configured to allow requests from `http://localhost:3000`.
- Update the origin in `Program.cs` as per your frontend URL.

---

## Running the Application

### Prerequisites
- .NET 6.0 or later
- Azure account with:
  - Table Storage.
  - Service Bus.

### Steps
1. Clone the repository.
2. Update the configuration in `appsettings.json`.
3. Run the application:
   ```bash
   dotnet run
   ```
4. Access Swagger documentation at: `https://localhost:<port>/swagger`.

---

## Frontend Integration

- Use SignalR to connect to the `/orderHub` endpoint for real-time updates.
- Ensure the frontend is hosted at the origin configured in the CORS policy.

---

## Acknowledgments

- **Azure**: For providing scalable storage and messaging solutions.
- **SignalR**: For enabling real-time communication.
- **.NET Core**: For building a robust backend.

---

## License
This project is licensed under the MIT License.

