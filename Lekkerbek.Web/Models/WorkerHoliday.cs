using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class WorkerHoliday
    {
        public int WorkerHolidayId { get; set; }
        public int ChefId { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string Description { get; set; }

    }
}
