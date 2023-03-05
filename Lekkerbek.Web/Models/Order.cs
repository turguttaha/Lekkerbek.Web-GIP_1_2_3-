namespace Lekkerbek.Web.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        
        public int? CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        
        public DateTime OrderFinishedTime { get; set; }
        public virtual ICollection<DishOrder> DishOrder { get; set; } 
        public bool Finished { get; set; }//idk if this is needed, in perfect case we know an order is finished when the time is later than the orderfinished time, 
    }
}
