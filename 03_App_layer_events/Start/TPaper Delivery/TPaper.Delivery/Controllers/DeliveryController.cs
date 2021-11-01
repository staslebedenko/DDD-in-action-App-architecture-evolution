using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TPaper.Delivery
{
    public class DeliveryController
    {
        private readonly DeliveryDbContext context;

        private readonly ILogger<DeliveryController> logger;

        public DeliveryController(DeliveryDbContext context, ILogger<DeliveryController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        [FunctionName("CreateDelivery")]
        public async Task CreateDelivery(
            [QueueTrigger("received-orders", Connection = "ordersAccConString")] string message,
            CancellationToken cts)
        {
            this.logger.LogInformation("C# HTTP trigger function processed a request.");

            var deliveryRequest = JsonConvert.DeserializeObject<DeliveryRequestDto>(message);

            Product product = await this.context.Product.FirstOrDefaultAsync(x => x.ExternalCode == deliveryRequest.ProductCode, cts);

            var newDelivery = new Delivery
            {
                Id = 0,
                ClientId = deliveryRequest.ClientId,
                EdiOrderId = deliveryRequest.Id,
                Number = deliveryRequest.Quantity,
                ProductId = product.Id,
                ProductCode = product.ExternalCode,
                Notes = "Prepared for shipment"
            };

            Delivery savedDelivery = (await this.context.Delivery.AddAsync(newDelivery, cts)).Entity;
            await this.context.SaveChangesAsync(cts);
        }
    }
}
