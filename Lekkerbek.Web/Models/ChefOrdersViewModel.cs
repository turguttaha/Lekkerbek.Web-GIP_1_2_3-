using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lekkerbek.Web.Models
{
    public class ChefOrdersViewModel
    {
        [NotMapped]
        public int OrderId { get; set; }
        public bool Finished { get; set; }//idk if this is needed, in perfect case we know an order is finished when the time is later than the orderfinished time

        //Foreign Key van Customer
        public int? CustomerId { get; set; }
        public int? Discount { get; set; }
        public virtual Customer Customer { get; set; }

        //Realatie met OrderLine
        public virtual ICollection<OrderLine> OrderLines { get; set; }

        //Foreign Key van Time Slot
        public int? TimeSlotID { get; set; }
        [Display(Name = "Time Slot")]
        [DataType(DataType.DateTime)]
        public DateTime StartTimeSlot { get; set; }
        public int? ChefId { get; set; }
        
        [StringLength(20, ErrorMessage = "Your First Name can contain only 20 characters")]
        [Display(Name = "First Name")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "GSM")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
        //public string Address { get; set; } = string.Empty;

        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime? Birthday { get; set; }

        [StringLength(30, ErrorMessage = "Your Firm Name can contain only 30 characters")]
        [Display(Name = "Firm Name")]
        public string? FirmName { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "Your contact person can contain only 30 characters")]
        [Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; } = string.Empty;

        [StringLength(450, ErrorMessage = "Your street name can contain only 450 characters")]
        [Display(Name = "Street name")]
        public string? StreetName { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Your city can contain only 20 characters")]
        [Display(Name = "City")]
        public string? City { get; set; } = string.Empty;

        [Display(Name = "Postal code")]
        public string? PostalCode { get; set; }

        [StringLength(20, ErrorMessage = "Your BTW can contain only 20 characters")]
        [Display(Name = "BTW")]
        public string? Btw { get; set; } = string.Empty;

        [Display(Name = "BTW number")]
        public string? BtwNumber { get; set; }

        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        // als loyaltyScore true is(meer dan 2 betelling is al gedaan), dan betekent dat klant 10% korting kan hebben 
        [ScaffoldColumn(false)]
        [HiddenInput(DisplayValue = false)]
        public bool? LoyaltyScore { get; set; }

        //Foreign Key van Preferred Dish

        public int? PreferredDishId { get; set; }

        public virtual PreferredDish PreferredDish { get; set; }

    }
}
