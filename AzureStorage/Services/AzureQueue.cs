using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorage.Services
{
    public class AzureQueue
    {
        public readonly QueueClient queueClient;

        public AzureQueue(string queueName)
        {
            queueClient = new QueueClient(ConnectionString.AzureStorageConnectionString,queueName);
            queueClient.CreateIfNotExists();
        }

        public async Task SendMessage(string message)
        {
            await queueClient.SendMessageAsync(message);
        }
        public async Task<QueueMessage> RetrieveNextMessage()
        {
            QueueProperties properties = await queueClient.GetPropertiesAsync();

            if (properties.ApproximateMessagesCount > 0)
            {
                QueueMessage[] queueMessages = await queueClient.ReceiveMessagesAsync(1, TimeSpan.FromMinutes(1));

                if (queueMessages.Any())
                {
                    return queueMessages[0];
                }
            }

            return null;
        }

        public async Task DeleteMessage(string messageId,string popReceipt)
        {
            await queueClient.DeleteMessageAsync(messageId, popReceipt);
        }
    }
}
