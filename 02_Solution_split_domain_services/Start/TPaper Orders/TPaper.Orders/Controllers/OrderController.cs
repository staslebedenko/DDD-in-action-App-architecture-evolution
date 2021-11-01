using System.Net.Http;
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

        private readonly ILogger<OrderController> logger;

        private readonly HttpClient httpClient;

        public OrderController(PaperDbContext context, HttpClient httpClient, ILogger<OrderController> logger)
        {
            this.context = context;
            this.logger = logger;
            this.httpClient = httpClient;
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


            Delivery deliveryModel = await CreateDeliveryForOrder(cts, savedOrder);

            return new OkObjectResult($"Order processed and completed with delivery {deliveryModel.Id}");
        }

        private async Task<Delivery> CreateDeliveryForOrder(CancellationToken cts, DeliveryRequest deliveryRequest)
        {
            Delivery deliveryModel;
            string url =
                $"http://localhost:7072/api/delivery/create/{deliveryRequest.ClientId}/{deliveryRequest.Id}/{deliveryRequest.ProductCode}/{deliveryRequest.Quantity}";
            HttpResponseMessage response = await this.httpClient.GetAsync(url, cts);
            deliveryModel = await response.Content.ReadAsAsync<Delivery>(cts);

            return deliveryModel;
        }
    }
}
