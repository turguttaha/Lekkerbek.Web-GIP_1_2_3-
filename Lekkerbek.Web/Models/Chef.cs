using System.Collections;

namespace Lekkerbek.Web.Models
{
    public class Chef
    {
        public int ChefId { get; set; }
        public string ChefName { get; set; }

        public virtual ICollection<TimeSlot> TimeSlot { get; set; }

    }
}
