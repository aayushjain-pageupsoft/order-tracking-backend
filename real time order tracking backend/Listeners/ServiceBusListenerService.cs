using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;

public class ServiceBusListenerService : BackgroundService
{
    private readonly IHubContext<OrderHub> _hubContext;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<ServiceBusListenerService> _logger;

    public ServiceBusListenerService(
        IConfiguration configuration,
        IHubContext<OrderHub> hubContext,
        ILogger<ServiceBusListenerService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;

        var connectionString = configuration["ServiceBus:ConnectionString"];
        var queueName = "ordertracker";

        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(queueName))
        {
            throw new ArgumentNullException("ServiceBus configuration values are missing.");
        }

        // Initialize Service Bus client and processor
        _serviceBusClient = new ServiceBusClient(connectionString);
        _processor = _serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        await _processor.StartProcessingAsync();

        stoppingToken.Register(async () =>
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
            await _serviceBusClient.DisposeAsync();
        });
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var messageBody = args.Message.Body.ToString();
            var orderId = GetOrderIdFromMessage(messageBody);
            var status = GetStatusFromMessage(messageBody);

            // Broadcast order update to SignalR clients
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", orderId, status);

            // Complete the message
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing message: {ex.Message}");
            throw; // Allow retry if enabled
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError($"Service Bus error: {args.Exception.Message}");
        return Task.CompletedTask;
    }

    private string GetOrderIdFromMessage(string message)
    {
        var parts = message.Split(",");
        return parts[0].Split(":")[1]; // Example: "Order:1234"
    }

    private string GetStatusFromMessage(string message)
    {
        var parts = message.Split(",");
        return parts[1].Split(":")[1]; // Example: "Status:Shipped"
    }
}
