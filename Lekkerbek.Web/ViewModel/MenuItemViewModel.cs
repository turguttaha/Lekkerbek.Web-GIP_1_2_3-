using Lekkerbek.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.ViewModel
{
    public class MenuItemViewModel
    {
        public int MenuItemId { get; set; }


        [Display(Name = "Naam")]
        public string Name { get; set; } = string.Empty;


        [Display(Name = "Beschrijving")]
        public string Description { get; set; } = string.Empty;



        [Display(Name = "Prijs")]
        [Range(0.00, 1000.00)]
        public double Price { get; set; }


        [Display(Name = "BTW %")]
        public double BtwNumber { get; set; }


        [Display(Name = "Soort")]
        public string Sort { get; set; } = string.Empty;
    }
}
