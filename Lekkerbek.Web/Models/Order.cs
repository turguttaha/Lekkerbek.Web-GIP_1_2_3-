namespace Lekkerbek.Web.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public bool Finished { get; set; }//idk if this is needed, in perfect case we know an order is finished when the time is later than the orderfinished time

        //Foreign Key van Customer
        public int? CustomerId { get; set; }
        public int? Discount { get; set; }
        public virtual Customer Customer { get; set; }
        
        //Realatie met OrderLine
        public virtual ICollection<OrderLine> OrderLines { get; set; }

        //Foreign Key van Time Slot
        public int? TimeSlotID { get; set; }
        public virtual TimeSlot TimeSlot { get; set; }//this is something we need to think about what we call it

        internal static List<OrderLine> TemproraryCart { get; set; } = new List<OrderLine>();
    }
}
