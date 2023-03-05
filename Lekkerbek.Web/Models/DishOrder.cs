namespace Lekkerbek.Web.Models
{
    public class DishOrder
    {
        public int DishOrderID { get; set; }
        
        public int OrderID { get; set; }
        public virtual Order Order { get; set; }
        
        public int DishID { get; set; }
        public virtual Dish Dish { get; set; }
        
        public int TimeSlotID { get; set; }
        public virtual TimeSlot TimeSlot { get; set; }//this is something we need to think about what we call it
        
        public string? ExtraDetails { get; set; }
        public int DishAmount { get; set; }
    }
}
