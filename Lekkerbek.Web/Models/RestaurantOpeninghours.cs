using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public enum DayOfWeekEnum {
        Monday,

        Tuesday,

        Wednesday,

        Thursday,

        Friday,

        Saturday,
        Sunday

    }
    public class RestaurantOpeninghours
    {
        [Display(Name = "Restaurant openingstijd ID")]
        public int RestaurantOpeninghoursId { get; set; }
        [Display(Name = "Dag van de week")]
        public DayOfWeekEnum DayOfWeek { get; set; }
        [DataType(DataType.Time)]
        [Display(Name = "Begin Tijd")]
        public DateTime StartTime { get; set; }
        [DataType(DataType.Time)]
        [Display(Name = "Eind Tijd")]
        public DateTime EndTime { get; set; }

    }
}
