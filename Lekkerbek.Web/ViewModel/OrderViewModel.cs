using Lekkerbek.Web.Models;

namespace Lekkerbek.Web.ViewModel
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public bool Finished { get; set; }//idk if this is needed, in perfect case we know an order is finished when the time is later than the orderfinished time


        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int? Discount { get; set; }

        public DateTime? TimeSlot { get; set; }

        public int? ChefId { get; set; }

    }
}
