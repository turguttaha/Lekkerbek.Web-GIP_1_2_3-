using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class RestaurantHoliday
    {
        public int RestaurantHolidayId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
        public string Description { get; set; }     


    }
}
