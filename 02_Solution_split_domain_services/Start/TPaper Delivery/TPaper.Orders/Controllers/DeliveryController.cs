using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace TPaper.Orders
{
    public class DeliveryController
    {
        private readonly DeliveryDbContext context;

        public DeliveryController(DeliveryDbContext context)
        {
            this.context = context;
        }

        public async Task<string> ProcessEdiOrder(DeliveryRequest order, CancellationToken cts)
        {
            Product product = await this.context.Product.FirstOrDefaultAsync(x => x.ExternalCode == order.ProductCode, cts);

            var newDelivery = new Delivery
            {
                Id = 0,
                ClientId = order.ClientId,
                EdiOrderId = order.Id,
                Number = order.Quantity,
                ProductId = product.Id,
                ProductCode = product.ExternalCode, 
                Notes = "Prepared for shipment"
            };

            Delivery savedDelivery = (await this.context.Delivery.AddAsync(newDelivery, cts)).Entity;
            await this.context.SaveChangesAsync(cts);

            return $"Accepted EDI message {order.Id} and created delivery {savedDelivery.Id}";
        }
    }
}
