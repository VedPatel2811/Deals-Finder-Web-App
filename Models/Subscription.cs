using System.ComponentModel.DataAnnotations.Schema;

namespace Lab5.Models
{
    public class Subscription
    {
        public required int CustomerId { get; set; }
        public required string ServiceId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("ServiceId")]
        public FoodDeliveryService FoodDeliveryService { get; set; }

    }
}
