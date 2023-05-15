namespace Lekkerbek.Web.Models
{
    public class WorkerHoliday
    {
        public int WorkerHolidayId { get; set; }
        public int ChefId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Description { get; set; }

    }
}
