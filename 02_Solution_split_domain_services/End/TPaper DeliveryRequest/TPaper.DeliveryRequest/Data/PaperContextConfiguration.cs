using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TPaper.DeliveryRequest
{
    public static class PaperContextConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            EntityTypeBuilder<DeliveryRequest> order = modelBuilder.Entity<DeliveryRequest>();
            order.Property(p => p.Id).ValueGeneratedOnAdd();
            order.Property(p => p.ClientId);
            order.Property(p => p.Quantity);
            order.Property(p => p.ProductCode);
            order.Property(p => p.Notes);
            order.Property(p => p.DeliveryId);
        }
    }
}
