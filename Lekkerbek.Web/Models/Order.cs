namespace Lekkerbek.Web.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public virtual Customer CustomerID { get; set; }//all int of ids will need to be linked
        public DateTime OrderFinishedTime { get; set; }
        public bool Finished { get; set; }//idk if this is needed, in perfect case we know an order is finished when the time is later than the orderfinished time, 
    }
}
