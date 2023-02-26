namespace Lekkerbek.Web.Models
{
    public Enum DishesList
    {
        vlees,
            }
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime Birthday { get; set; }

        private bool loyaltyScore;
        public bool LoyaltyScore { 
            get { return loyaltyScore; } 
            set 
            { 
                if (Orders.Count >= 3)
                    loyaltyScore = true;
                else
                    loyaltyScore = false;
            } }

        public virtual ICollection <Order> Orders { get; set; }

        

        //public virtual ICollection<Dishes> PreferredDishes { get; set; }
    }
}
