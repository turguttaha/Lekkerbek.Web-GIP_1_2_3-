using Lekkerbek.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.ViewModel
{
    public class MenuItemViewModel
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Range(0.00, 1000.00)]
        public double Price { get; set; }
        [Display(Name = "BTW %")]
        public double BtwNumber { get; set; }
        public string Sort { get; set; } = string.Empty;
    }
}
