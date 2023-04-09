namespace Lekkerbek.Web.Models
{
    public class WorkerSchedule
    {
        public int WorkerScheduleId { get; set; }
        public int ChefId { get; set; }
        public string DayOfTheWeek { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public virtual ICollection<Chef> Chefs { get; set; }

    }
}
