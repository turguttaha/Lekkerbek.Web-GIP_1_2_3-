namespace Lekkerbek.Web.Models
{
    public enum DishesList
    {
        vlees,
        vis,
        aziatisch,
        vegetarisch
    }
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime Birthday { get; set; }

        private bool loyaltyScore;

        // als loyaltyScore true is(meer dan 2 betelling is al gedaan), dan betekent dat klant 10% korting kan hebben 
        public bool LoyaltyScore { 
            get { return loyaltyScore; } 
            set 
            { 
                if (Orders.Count >= 3)
                    loyaltyScore = true;
                else
                    loyaltyScore = false;
            } }

        //binnen de oerder class Customer property moet toegevoegd worden dus elke klant kan een of meer bestelling hebben maar elke bestelling is van slechts een klant
        public virtual ICollection <Order> Orders { get; set; }
        public virtual DishesList PreferedDishes { get; set; }
    }
}
