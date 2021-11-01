namespace TPaper.DeliveryRequest
{
    public class DeliveryRequestDto
    {
        public DeliveryRequestDto(DeliveryRequest deliveryRequest)
        {
            this.Id = deliveryRequest.Id;
            this.ClientId = deliveryRequest.ClientId;
            this.Quantity = deliveryRequest.Quantity;
            this.ProductCode = deliveryRequest.ProductCode;
            this.Notes = deliveryRequest.Notes;
            this.DeliveryId = deliveryRequest.DeliveryId;
            this.ResponseStatus = deliveryRequest?.Response?.Status;
        }

        public int Id { get; set; }

        public int ClientId { get; set; }

        public decimal Quantity { get; set; }

        public int ProductCode { get; set; }

        public string Notes { get; set; }

        public int? DeliveryId { get; set; }

        public string ResponseStatus { get; set; }
    }
}
