namespace Lekkerbek.Web.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }

        private bool isAvailable;
        public bool IsAvailable { 
            get { return isAvailable; } 
            set { if (Chef1Id != null && Chef2Id != null)
                { isAvailable = false; }
                  isAvailable = true;  }}

        public DateTime StartTimeSlot { get; set; }
        public DateTime EndTimeSlot { get; set;}
        public int? Chef1Id  { get; set; }

        public virtual Chef Chef1 { get; set; }
        public int? Chef2Id { get; set; }

        public virtual Chef Chef2 { get; set; }


    }
}
