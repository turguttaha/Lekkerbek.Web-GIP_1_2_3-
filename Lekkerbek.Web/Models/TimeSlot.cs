namespace Lekkerbek.Web.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime StartTimeSlot { get; set; }
        public DateTime EndTimeSlot { get; set;}
        public int? ChefId  { get; set; }
        public virtual Chef Chef { get; set; }



    }
}
