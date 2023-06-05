using Lekkerbek.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lekkerbek.Web.ViewModel
{
    public class ChefOrdersViewModel
    {
       
        public int OrderId { get; set; }
        [Display(Name ="Afgerond")]
        public bool Finished { get; set; }

        //Foreign Key van Customer
        public int? CustomerId { get; set; }
        [Display(Name = "Korting")]
        public int? Discount { get; set; }
        

        //Realatie met OrderLine
        public virtual ICollection<OrderLine> OrderLines { get; set; }

        
        [Display(Name = "Tijdslot")]
        [DataType(DataType.DateTime)]
        public DateTime StartTimeSlot { get; set; }
        public int? ChefId { get; set; }

        

    }
}
