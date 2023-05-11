using Lekkerbek.Web.Models;

namespace Lekkerbek.Web.ViewModel
{
    public class OrderViewModel
    {
        //Properties from Order
        public int OrderID { get; set; }
        public bool Finished { get; set; }//idk if this is needed, in perfect case we know an order is finished when the time is later than the orderfinished time


        public int? CustomerId { get; set; }
        public int? Discount { get; set; }
        public string CustomerName { get; set; }

        //Properties from OrderLine
        public string? ExtraDetails { get; set; }
        public int MenuItemId { get; set; }


        public DateTime? StartTimeSlot { get; set; }

        public int? ChefId { get; set; }
        public string? BtwNumber { get; set; }
        public string? Btw { get; set; }
        public string City { get; set; }
        public string ContactPerson { get; set; }
        public DateTime? Birthday { get; set; }
        public bool? LoyaltyScore { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? PreferredDishId { get; set; }
        public int? TimeSlotID { get; set; }
        public DateTime? TimeSlot { get; set; }






    }
}
