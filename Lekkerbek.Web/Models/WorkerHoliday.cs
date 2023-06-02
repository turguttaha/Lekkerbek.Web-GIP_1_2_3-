using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class WorkerHoliday
    {
        public int WorkerHolidayId { get; set; }
        public int ChefId { get; set; }
        [DataType(DataType.Date)]
        [Display(Name ="Begindatum")]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Einddatum")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Beschrijving")]
        public string Description { get; set; }

    }
}
