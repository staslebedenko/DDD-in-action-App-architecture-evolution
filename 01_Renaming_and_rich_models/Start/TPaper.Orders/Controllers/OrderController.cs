using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TPaper.Orders
{
    public class OrderController
    {
        private readonly PaperDbContext context;

        public OrderController(PaperDbContext context)
        {
            this.context = context;
        }

        [FunctionName("ProcessEdiOrder")]
        public async Task<IActionResult> ProcessEdiOrder(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/create/{quantity}")] HttpRequest req,
            decimal quantity,
            ILogger log,
            CancellationToken cts)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var order = new EdiOrder
            {
                Id = 0,
                ClientId = 1,
                Delivery = null,
                DeliveryId = null,
                Notes = "Test order",
                ProductCode = 1,
                Quantity = quantity
            };

            EdiOrder savedOrder = (await this.context.EdiOrder.AddAsync(order, cts)).Entity;
            await this.context.SaveChangesAsync(cts);

            // get product
            Product product = await this.context.Product.FirstOrDefaultAsync(x => x.ExternalCode == savedOrder.ProductCode, cts);

            // check if inventory have needed amount.
            //var availableNumber = (await this.context.Inventory.FirstOrDefaultAsync(x => x.ProductId == product.Id, cts)).Number;

            // create devliery from order. 
            var newDelivery = new Delivery
            {
                Id = 0,
                ClientId = savedOrder.ClientId,
                EdiOrder = savedOrder,
                EdiOrderId = savedOrder.Id,
                Number = savedOrder.Quantity,
                ProductId = product.Id,
                ProductCode = product.ExternalCode,
                Notes = "Prepared for shipment"
            };

            Delivery savedDelivery = (await this.context.Delivery.AddAsync(newDelivery, cts)).Entity;
            await this.context.SaveChangesAsync(cts);

            string responseMessage = $"Accepted EDI message {order.Id} and created delivery {savedDelivery.Id}";

            return new OkObjectResult(responseMessage);
        }
    }
}
