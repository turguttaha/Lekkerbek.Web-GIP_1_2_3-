using System.Collections;

namespace Lekkerbek.Web.Models
{
    public class Chef
    {
        #region Properties
        public string ChefId { get; set; }
        public string ChefName { get; set; }
        //public DateTime WorkStartTime { get; set; }
        //public DateTime WorkEndTime { get; set; }
        ////public string timeSlot { get; set; }
        //public List<String> TimeSlots { get; set; }
        #endregion

        #region Methods


  //      public void setWorkTime(DateTime startHour, DateTime endHour)
  //      {
		//	WorkStartTime = startHour;
  //          WorkEndTime = endHour;

		//}
  //      public Chef() { timeSlotMaker(); }
  //      public void timeSlotMaker()
  //      {
  //          DateTime startSlot = WorkStartTime;
  //          while (startSlot.Hour < WorkEndTime.Hour)
  //          {
  //              TimeSlots.Add(startSlot.ToString());
  //              TimeSlots.Add("Available");
  //              startSlot = startSlot.AddMinutes(15);
  //          }
  //      }

  //      public void chooseTimeSlot(DateTime hour) //Chosen timeslot will change from available to unavailable
  //      {
  //          int position = Java.util.TimeSlots.indexOf(hour);
  //          TimeSlots[position + 1] = "Unavailable";
  //      }

  //      public string ToString()
  //      {
  //          int counter = 0;
  //          string toReturn = "";
  //          foreach (slot in TimeSlots)
  //          {
  //              counter += 1;
  //              if (counter % 2 == 0)
  //              {
  //                  toReturn += TimeSlots[counter - 1] + TimeSlots[counter] + "\n";

  //              }
  //          }
            
  //      }
        #endregion
    }
}
