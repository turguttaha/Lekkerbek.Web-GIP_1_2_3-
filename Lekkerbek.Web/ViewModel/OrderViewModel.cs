using Lekkerbek.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.ViewModel
{
    public class OrderViewModel
    {
        //Properties from Order
        [Display(Name = "Bestelling ID")]
        public int OrderID { get; set; }
        [Display(Name = "Afgerond")]
        public bool Finished { get; set; }//idk if this is needed, in perfect case we know an order is finished when the time is later than the orderfinished time


        public int? CustomerId { get; set; }
        public int? Discount { get; set; }
        [Display(Name = "Klantnaam")]
        public string CustomerName { get; set; }

        //Properties from OrderLine
        public string? ExtraDetails { get; set; }
        public int MenuItemId { get; set; }
        public DateTime? StartTimeSlot { get; set; }
        public int? ChefId { get; set; }
        [Display(Name = "BTW nummer")]
        public string? BtwNumber { get; set; }
        public string? Btw { get; set; }
        [Display(Name = "Stad")]
        public string City { get; set; }
        [Display(Name = "Contactpersoon")]
        public string ContactPerson { get; set; }
        [Display(Name = "Geboortedatum")]
        public DateTime? Birthday { get; set; }
        public bool? LoyaltyScore { get; set; }
        public string Email { get; set; }
        [Display(Name = "GSM nummer")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Favoriete gerecht ID")]
        public int? PreferredDishId { get; set; }
        [Display(Name = "Tijdslot ID")]
        public int? TimeSlotID { get; set; }
        [Display(Name = "Tijdslot")]

        public DateTime? TimeSlot { get; set; }
    }
}
