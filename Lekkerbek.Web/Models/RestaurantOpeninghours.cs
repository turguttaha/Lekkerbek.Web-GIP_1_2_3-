namespace Lekkerbek.Web.Models
{
    public class RestaurantOpeninghours
    {
        public int RestaurantOpeninghoursId { get; set; }
        public enum DayOfWeek
        {
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday
        }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
