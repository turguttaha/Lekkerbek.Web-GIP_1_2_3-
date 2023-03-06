namespace Lekkerbek.Web.Models
{
    public class OrderLine
    {
        public int OrderLineID { get; set; }
        public string? ExtraDetails { get; set; }
        public int DishAmount { get; set; }

        ////Foreign Key van Order
        public int? OrderID { get; set; }
        public virtual Order Order { get; set; }

        //Foreign Key van Dish
        public int? DishID { get; set; }
        public virtual Dish Dish { get; set; }

        //Foreign Key van Time Slot
        public int? TimeSlotID { get; set; }
        public virtual TimeSlot TimeSlot { get; set; }//this is something we need to think about what we call it


    }
}
