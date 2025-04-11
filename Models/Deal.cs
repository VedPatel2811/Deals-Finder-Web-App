using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;

namespace Lab5.Models
{
	public class Deal
	{
        [Required]
        public int Id { get; set; }
        public string FoodDeliveryServiceId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Required]
        [Url]
        [MaxLength(2048)]
        public string Url { get; set; }
        public ICollection<FoodDeliveryService> FoodDeliveryService { get; set; } = new List<FoodDeliveryService>();
    }
}