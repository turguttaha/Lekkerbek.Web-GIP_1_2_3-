using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Range(0.00, 1000.00)]
        public double Price { get; set; }
        [Display(Name = "BTW %")]
        public double BtwNumber { get; set; }
        public string Sort { get; set; } = string.Empty;
        public virtual ICollection<OrderLine> OrderLines { get; set; }

    }
}
