using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class Chef
    {
        [ScaffoldColumn(false)]
        public int ChefId { get; set; }
        [Display(Name = "Chefnaam")]
        [Required(ErrorMessage = "{0}")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Chefnaam moet minimum 2 en maximum 20 karakters bevatten")]
        public string ChefName { get; set; }

        public virtual ICollection<TimeSlot> TimeSlot { get; set; }
        public virtual IdentityUser? IdentityUser { get; set; }

    }
}
