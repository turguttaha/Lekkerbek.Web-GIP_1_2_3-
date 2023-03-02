namespace Lekkerbek.Web.Models
{
    public class Dish
    {
        public int DishId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Discount { get; set; }

        public int? OrderId { get; set; }

        public virtual Order Order { get; set; }

    }
}
