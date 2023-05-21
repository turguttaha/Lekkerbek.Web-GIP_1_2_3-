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
        public int RestaurantOpeninghoursId { get; set; }
        public DayOfWeekEnum DayOfWeek { get; set; }
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [DataType(DataType.Time)]

        public DateTime EndTime { get; set; }

    }
}
