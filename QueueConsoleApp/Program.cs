using AzureStorage;
using AzureStorage.Services;
using System;
using System.Text;

namespace QueueConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionString.AzureStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=udemyrealstrorageaccount;AccountKey=6TWAWj5oL6/UQCWAHMW1lV0/asO8rJ+W2feGnGIpypz1ndj/cRdkTxATTwxcoMn8tGnLjroKmGc0+AStdxlLLw==;BlobEndpoint=https://udemyrealstrorageaccount.blob.core.windows.net/;QueueEndpoint=https://udemyrealstrorageaccount.queue.core.windows.net/;TableEndpoint=https://udemyrealstrorageaccount.table.core.windows.net/;FileEndpoint=https://udemyrealstrorageaccount.file.core.windows.net/;";
            AzureQueue queue = new AzureQueue("testqueue");
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test"));
            queue.SendMessage(base64).Wait();
            var queueMessage = queue.RetrieveNextMessage().Result;
            Console.WriteLine(Encoding.UTF8.GetString(Convert.FromBase64String(queueMessage.MessageText)));
            queue.DeleteMessage(queueMessage.MessageId,queueMessage.PopReceipt).Wait();
        }
    }
}
