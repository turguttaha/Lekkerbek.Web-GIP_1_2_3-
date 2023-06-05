using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.ViewModel
{
    public class CustomerViewModel
    {
        internal IdentityUser? IdentityUser;

        public int CustomerId { get; set; }
        [Display(Name = "Naam")]
        public string Name { get; set; }
        [Display(Name = "GSM nummer")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Geboorte datum")]
        public DateTime? Birthday { get; set; }
        [Display(Name = "Bedrijfsnaam")]
        public string? FirmName { get; set; }
        [Display(Name = "Contactpersoon")]
        public string? ContactPerson { get; set; }
        [Display(Name = "Adres")]
        public string Address { get; set; }
        [Display(Name = "BTW")]
        public string? Btw { get; set; }
        [Display(Name = "BTW nummer")]
        public string? BtwNumber { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool? LoyaltyScore { get; set; }
        [Display(Name = "Voorkeur gerecht")]
        public string? PreferredDishName { get; set; }

       
    }
}
