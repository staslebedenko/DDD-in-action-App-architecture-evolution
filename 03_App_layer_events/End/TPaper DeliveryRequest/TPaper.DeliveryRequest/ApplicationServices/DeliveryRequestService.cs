using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace TPaper.DeliveryRequest
{
    public class DeliveryRequestService
    {

        private readonly ILogger<DeliveryRequestService> logger;


        private readonly IRepository<DeliveryRequest> deliveryRequestRepository;

        private static readonly CloudStorageAccount StorageAccount = StorageAccountSetup.CreateStorageAccountFromConnectionString();

        private static readonly CloudQueue ReceivedOrderQueue = StorageAccountSetup.CreateCloudQueue("received-orders");

        public DeliveryRequestService(
            IRepository<DeliveryRequest> deliveryRequestRepository,
            ILogger<DeliveryRequestService> logger)
        {
            this.deliveryRequestRepository = deliveryRequestRepository;
            this.logger = logger;
        }

        [FunctionName("ProcessEdiOrder")]
        public async Task<IActionResult> ProcessEdiOrder(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/create/{quantity}")] HttpRequest req,
            decimal quantity,
            CancellationToken cts,
            [Queue("failed-orders", Connection = "ordersAccConString")] IAsyncCollector<string> messages)
        {
            this.logger.LogInformation("C# HTTP trigger function processed a request.");

            var deliveryRequest = new DeliveryRequest(1, 1, quantity);
            deliveryRequest = await this.deliveryRequestRepository.AddAndReturn(deliveryRequest, cts);
            string response = await CreateDeliveryForDeliveryRequest(cts, deliveryRequest);

            return new OkObjectResult($"Order processed");
        }

        private async Task<string> CreateDeliveryForDeliveryRequest(CancellationToken cts, DeliveryRequest deliveryRequest)
        {
            var processedOrderMessage = new CloudQueueMessage(JsonConvert.SerializeObject(new DeliveryRequestDto(deliveryRequest)));
            await ReceivedOrderQueue.AddMessageAsync(processedOrderMessage, TimeSpan.FromSeconds(-1), null, null, null, cts);

            return "success";
        }     
    }
}
