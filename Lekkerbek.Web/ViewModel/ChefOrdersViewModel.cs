using Lekkerbek.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lekkerbek.Web.ViewModel
{
    public class ChefOrdersViewModel
    {
       
        public int OrderId { get; set; }
        public bool Finished { get; set; }//idk if this is needed, in perfect case we know an order is finished when the time is later than the orderfinished time

        //Foreign Key van Customer
        public int? CustomerId { get; set; }
        public int? Discount { get; set; }
        

        //Realatie met OrderLine
        public virtual ICollection<OrderLine> OrderLines { get; set; }

        
        [Display(Name = "Time Slot")]
        [DataType(DataType.DateTime)]
        public DateTime StartTimeSlot { get; set; }
        public int? ChefId { get; set; }

        

    }
}
