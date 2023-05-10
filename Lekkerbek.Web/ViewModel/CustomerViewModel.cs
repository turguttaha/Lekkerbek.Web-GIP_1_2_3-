using Microsoft.AspNetCore.Identity;

namespace Lekkerbek.Web.ViewModel
{
    public class CustomerViewModel
    {
        internal IdentityUser? IdentityUser;

        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public string FirmName { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public string Btw { get; set; }
        public string BtwNumber { get; set; }
        public string Email { get; set; }
        public bool? LoyaltyScore { get; set; }
        public string? PreferredDishName { get; set; }

       
    }
}
