using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace TPaper.DeliveryRequest
{
    public class StorageAccountSetup
    {
        public static CloudStorageAccount StorageAccount;

        public static CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            CloudStorageAccount storageAccount;
            try
            {
                var storageConnectionString = Environment.GetEnvironmentVariable("ordersAccConString");
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }

            StorageAccount = storageAccount;

            return storageAccount;
        }

        public static CloudTable CreateCloudTable(string name)
        {
            return StorageAccount.CreateCloudTableClient().GetTableReference(name);
        }

        public static CloudQueue CreateCloudQueue(string name)
        {
            return StorageAccount.CreateCloudQueueClient().GetQueueReference(name);
        }
    }
}