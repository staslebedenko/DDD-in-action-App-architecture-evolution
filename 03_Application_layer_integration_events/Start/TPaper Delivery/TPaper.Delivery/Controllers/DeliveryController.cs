using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        [ProducesResponseType(typeof(Delivery), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
        [FunctionName("CreateDelivery")]
        public async Task<IActionResult> CreateDelivery(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "delivery/create/{clientId}/{ediOrderId}/{productCode}/{number}")] HttpRequest req,
            int clientId,
            int ediOrderId,
            int productCode,
            int number,
            CancellationToken cts)
        {
            this.logger.LogInformation("C# HTTP trigger function processed a request.");

            Product product = await this.context.Product.FirstOrDefaultAsync(x => x.ExternalCode == productCode, cts);

            var newDelivery = new Delivery
            {
                Id = 0,
                ClientId = clientId,
                EdiOrderId = ediOrderId,
                Number = number,
                ProductId = product.Id,
                ProductCode = product.ExternalCode,
                Notes = "Prepared for shipment"
            };

            Delivery savedDelivery = (await this.context.Delivery.AddAsync(newDelivery, cts)).Entity;
            await this.context.SaveChangesAsync(cts);

            return new OkObjectResult(savedDelivery);
        }
    }
}
