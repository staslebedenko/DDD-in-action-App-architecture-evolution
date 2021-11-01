using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace TPaper.Orders
{
    public class OrderController
    {
        private readonly PaperDbContext context;

        private readonly DeliveryDbContext deliveryContext;

        private readonly ILogger<OrderController> logger;

        public OrderController(PaperDbContext context, DeliveryDbContext deliveryContext, ILogger<OrderController> logger)
        {
            this.context = context;
            this.deliveryContext = deliveryContext;
            this.logger = logger;
        }

        [FunctionName("ProcessEdiOrder")]
        public async Task<IActionResult> ProcessEdiOrder(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/create/{quantity}")] HttpRequest req,
            decimal quantity,
            CancellationToken cts)
        {
            this.logger.LogInformation("C# HTTP trigger function processed a request.");

            var deliveryRequest = new DeliveryRequest(1, 1, quantity);

            DeliveryRequest savedOrder = (await this.context.DeliveryRequest.AddAsync(deliveryRequest, cts)).Entity;
            await this.context.SaveChangesAsync(cts);

            var deliveryController = new DeliveryController(deliveryContext);
            string responseMessage = await deliveryController.ProcessEdiOrder(savedOrder, cts);

            return new OkObjectResult(responseMessage);
        }
    }
}
