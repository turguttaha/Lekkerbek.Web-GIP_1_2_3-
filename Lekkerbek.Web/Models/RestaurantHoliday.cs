using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class RestaurantHoliday
    {
        [Display(Name = "Restaurant Vakantiedagen ID")]
        public int RestaurantHolidayId { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Begindatum")]
        public DateTime StartDate { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Einddatum")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Beschrijving")]
        public string Description { get; set; }     


    }
}
