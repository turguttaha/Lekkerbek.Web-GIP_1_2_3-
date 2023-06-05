using Microsoft.AspNetCore.Identity;
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
        [StringLength(20, ErrorMessage = "Uw voornaam mag maar 20 tekens lang zijn")]
        [Display(Name = "Voornaam")]
        public string? FName { get; set; } = string.Empty;
        [StringLength(20, MinimumLength = 2)]
        [Display(Name = "Familienaam")]
        public string? LName { get; set; } = string.Empty;

        [Display(Name = "GSM")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Geboortedatum")]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Birthday { get; set; }





        [StringLength(30, ErrorMessage = "Uw bedrijfsnaam mag maar 30 tekens lang zijn")]
        [Display(Name = "Bedrijfsnaam")]
        public string? FirmName { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "De naam van de contactpersoon mag maar 30 tekens lang zijn")]
        [Display(Name = "Contactpersoon")]
        public string? ContactPerson { get; set; } = string.Empty;

        [StringLength(450, ErrorMessage = "De straatnaam mag maar 450 tekens lang zijn")]
        [Display(Name = "Straatnaam")]
        public string? StreetName { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "De stad mag maar 20 tekens lang zijn")]
        [Display(Name = "Stad")]
        public string? City { get; set; } = string.Empty;

        [Display(Name = "Postcode")]
        public string? PostalCode { get; set; }

       
        [MaxLength(2)]
        [Display(Name = "BTW")]
        [RegularExpression("^[a-zA-Z]{2}", ErrorMessage = "Enkel geldige landcodes mogen ingevuld worden")]
        
        public string? Btw { get; set; } = string.Empty;

        [Display(Name = "BTW nummer")]
        [RegularExpression("^[0][0-9]{9}", ErrorMessage ="Het btw nummer moet 9 cijfers lang zijn")]
        public string? BtwNumber { get; set; }

        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
         
        // als loyaltyScore true is(meer dan 2 betelling is al gedaan), dan betekent dat klant 10% korting kan hebben 
        [ScaffoldColumn(false)]
        [HiddenInput(DisplayValue = false)]
        public bool? LoyaltyScore { get; set; }

        //Foreign Key van Preferred Dish
        [Display(Name = "Favoriete Gerechten")]
        public int? PreferredDishId { get; set; }

        public virtual PreferredDish? PreferredDish { get; set; }

        //Relatie met order
        //aan de Order class - virtual Customer property - int CustomerId -  moeten toegevoegd worden dus elke klant kan een of meer bestelling hebben maar elke bestelling is van slechts een klant
        public virtual ICollection<Order>? Orders { get; set; }

        public virtual IdentityUser? IdentityUser { get; set; }

    }
}
