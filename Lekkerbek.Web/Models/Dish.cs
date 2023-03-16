using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class Dish
    {
        public int DishId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Range(1, 1000)]
        public double Price { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }

    }
}
