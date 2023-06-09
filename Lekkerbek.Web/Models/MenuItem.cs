using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Lekkerbek.Web.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }

        [Required]
        [Display(Name = "Naam")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Beschrijving")]

        public string Description { get; set; } = string.Empty;
        
        [Range(0.00, 1000.00)]
        [Display(Name = "Prijs")]
        
        public double Price { get; set; }
        [Display(Name = "BTW %")]
        public double BtwNumber { get; set; }

        [Display(Name = "Soort")]
        public string Sort { get; set; } = string.Empty;
        public virtual ICollection<OrderLine>? OrderLines { get; set; }

    }
}
