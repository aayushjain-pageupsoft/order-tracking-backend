//Helpers/ServiceBusHelper.cs
using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;

public class ServiceBusHelper
{
    private readonly string _connectionString;
    private readonly string _queueName;
    private readonly QueueClient _queueClient;

    public ServiceBusHelper(string connectionString, string queueName)
    {
        _connectionString = connectionString;
        _queueName = queueName;
        _queueClient = new QueueClient(_connectionString, _queueName);
    }

    // Method to send messages to the Service Bus
    public async Task SendMessageAsync(string message)
    {
        var messageToSend = new Message(Encoding.UTF8.GetBytes(message));
        await _queueClient.SendAsync(messageToSend);
    }

    // Method to receive messages from the Service Bus
    public async Task<Message> ReceiveMessageAsync()
    {
        var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = 1,
            AutoComplete = false
        };

        Message? receivedMessage = null;
        _queueClient.RegisterMessageHandler(async (message, token) =>
        {
            receivedMessage = message;
            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }, messageHandlerOptions);

        while (receivedMessage == null)
        {
            await Task.Delay(100);
        }

        return receivedMessage;
    }

    // Method to acknowledge a received message
    public async Task CompleteMessageAsync(Message message)
    {
        await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        // Handle the exception
        return Task.CompletedTask;
    }
}

