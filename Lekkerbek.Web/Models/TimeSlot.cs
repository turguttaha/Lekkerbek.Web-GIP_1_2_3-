using Lekkerbek.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class TimeSlot
    {
        //[ScaffoldColumn(false)]
        public int Id { get; set; }
        [Display(Name = "Time Slot")]
        [DataType(DataType.DateTime)]
        public DateTime StartTimeSlot { get; set; }
        public int? ChefId  { get; set; }
        public virtual Chef Chef { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
