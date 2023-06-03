using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class Chef
    {
        [ScaffoldColumn(false)]
        public int ChefId { get; set; }
        [Display(Name = "Chef Naam")]
        public string ChefName { get; set; }

        public virtual ICollection<TimeSlot> TimeSlot { get; set; }
        public virtual IdentityUser? IdentityUser { get; set; }

    }
}
