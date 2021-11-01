namespace TPaper.Delivery
{
    public class DeliveryRequestDto
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public decimal Quantity { get; set; }

        public int ProductCode { get; set; }

        public string Notes { get; set; }

        public int? DeliveryId { get; set; }

        public string ResponseStatus { get; set; }
    }
}
