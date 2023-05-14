namespace Lekkerbek.Web.Models
{
    public class RestaurantHoliday
    {
        public int RestaurantHolidayId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public enum Type { publicHoliday }
        public string Description { get; set; }
         
           
        


    }
}
