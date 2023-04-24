using Microsoft.AspNetCore.Identity;
using System.Data.Common;
using Telerik.SvgIcons;

namespace Lekkerbek.Web.Models
{
    public class ApplicationUser : IdentityUser 
    {
        public virtual Customer? Customer { get; set; }
    }
}
