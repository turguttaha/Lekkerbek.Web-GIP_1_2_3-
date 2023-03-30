using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Lekkerbek.Web.Models
{

    public class Customer
    {
        //[ScaffoldColumn(false)]
        public int CustomerId { get; set; }
        [StringLength(20, ErrorMessage = "Your First Name can contain only 20 characters")]
        [Display(Name = "First Name")]
        public string FName { get; set; } = string.Empty;
        [StringLength(20, MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LName { get; set; } = string.Empty;

        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "GSM")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        
        [Display(Name = "Date of Birthday")]
        [DataType(DataType.Date)]
            
        public DateTime Birthday { get; set; }
        
        


        // als loyaltyScore true is(meer dan 2 betelling is al gedaan), dan betekent dat klant 10% korting kan hebben 
        [ScaffoldColumn(false)]
        [HiddenInput(DisplayValue = false)]
        public bool LoyaltyScore { get; set; }

        //Foreign Key van Preferred Dish
        
        public int? PreferredDishId { get; set; }
        [Display(Name = "Preferred Dish")]
        public virtual PreferredDish PreferredDish { get; set; }

        //Relatie met order
        //aan de Order class - virtual Customer property - int CustomerId -  moeten toegevoegd worden dus elke klant kan een of meer bestelling hebben maar elke bestelling is van slechts een klant
        public virtual ICollection<Order> Orders { get; set; }
        [NotMapped]
        public string Name
        {
            get { return FName + " " + LName; }
        }

    }
}
