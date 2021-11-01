using System;

namespace TPaper.DeliveryRequest
{
    public class DeliveryRequest : BaseEntity, IAggregateRootMarker
    {
        private DeliveryRequest()
        {

        }

        public DeliveryRequest(int clientId, int? productCode, decimal quantity)
        {
            if (productCode == null)
            {
                throw new ArgumentException("Product code should be in place", nameof(productCode));
            }

            this.Id = 0;
            this.ClientId = clientId;
            this.ProductCode = (int)productCode;
            this.Quantity = quantity;
        }

        public int ClientId { get; private set; }

        public decimal Quantity { get; private set; }

        public int ProductCode { get; private set; }

        public string Notes { get; private set; }

        public int? DeliveryId { get; private set; }

        public Response Response { get; private set; }

        public bool IsValid()
        {
            return Quantity > 0;
        }

        public void SetDeliveryId(int? id)
        {
            this.DeliveryId = id ?? throw new InvalidOperationException();
        }
        
        public void AddNotes(string notes)
        {
            this.Notes = notes;
        }

        public void AddResponse(string status)
        {
            Response = new Response(status);
        }
    }
}
