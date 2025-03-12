using System.ComponentModel.DataAnnotations;

namespace Lab5.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Name")]
        public string FullName => LastName + ", " + FirstName; //=> to replace get

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
