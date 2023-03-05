namespace Lekkerbek.Web.Models
{
    public class PreferredDish
    {
        public int PreferredDishId { get; set; }

        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Customer> Customers { get; set; }

    }
}
