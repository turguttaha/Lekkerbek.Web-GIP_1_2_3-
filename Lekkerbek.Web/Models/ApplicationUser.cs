using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Telerik.SvgIcons;

namespace Lekkerbek.Web.Models
{
    public class ApplicationUser : IdentityUser 
    {
        [Display(Name = "Klant")]
        public virtual Customer? Customer { get; set; }
        public virtual Chef? Chef { get; set; }
    }
}
