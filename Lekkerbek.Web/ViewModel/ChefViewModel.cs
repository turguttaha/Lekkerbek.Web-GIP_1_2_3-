using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.ViewModel
{
    public class ChefViewModel
    {
        [ScaffoldColumn(false)]
        public int ChefId { get; set; }
        [Display(Name = "Chef Name")]
        public string ChefName { get; set; }
        public string Email { get; set; }
        public string IdentityId { get; set; }  
    }
}
