namespace Lekkerbek.Web.Models
{
    public class DishOrder
    {
        public int DishOrderID { get; set; }
        public virtual Order OrderID { get; set; }
        public int DishID { get; set; }//public virtual Dish DishID { get; set; }
        public virtual TimeSlot TimeSlotID { get; set; }//this is something we need to think about what we call it
        public string? ExtraDetails { get; set; }
        public int DishAmount { get; set; }
    }
}
