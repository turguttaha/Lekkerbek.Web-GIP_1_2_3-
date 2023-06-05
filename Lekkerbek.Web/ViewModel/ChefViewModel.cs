using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.ViewModel
{
    public class ChefViewModel
    {
        [ScaffoldColumn(false)]
        public int ChefId { get; set; }
        [Display(Name = "Chefnaam")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Chefnaam moet minimum 2 en maximum 20 karakters bevatten")]
        public string ChefName { get; set; }
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Ongeldige email")]
        public string Email { get; set; }
        public string IdentityId { get; set; }  
    }
}
