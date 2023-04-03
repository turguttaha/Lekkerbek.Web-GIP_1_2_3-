using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Range(0, 1000)]
        public double Price { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }

    }
}
