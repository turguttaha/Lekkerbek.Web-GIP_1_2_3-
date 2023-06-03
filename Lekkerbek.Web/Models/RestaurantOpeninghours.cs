using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public enum DayOfWeekEnum {
        [Display(Name ="Maandag")]
        Monday,
        [Display(Name = "Dinsdag")]

        Tuesday,
        [Display(Name = "Woensdag")]

        Wednesday,
        [Display(Name = "Donderdag")]

        Thursday,
        [Display(Name = "Vrijdag")]

        Friday,
        [Display(Name = "Zaterdag")]

        Saturday,
        [Display(Name = "Zondag")]
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
