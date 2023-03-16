using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class Chef
    {
        public int ChefId { get; set; }
        [Display(Name = "Chef Name")]
        public string ChefName { get; set; }

        public virtual ICollection<TimeSlot> TimeSlot { get; set; }

    }
}
