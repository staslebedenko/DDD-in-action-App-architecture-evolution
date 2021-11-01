using System.ComponentModel.DataAnnotations;

namespace TPaper.DeliveryRequest
{
    public class BaseEntity
    {
        [Required]
        public int Id { get; set; }
    }
}