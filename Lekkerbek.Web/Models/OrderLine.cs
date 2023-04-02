using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class OrderLine
    {
        public int OrderLineID { get; set; }
        [Display(Name = "Extra Details")]
        public string? ExtraDetails { get; set; }
        [Display(Name = "Dish Amount")]
        [Range(1,100)]
        public int DishAmount { get; set; }

        ////Foreign Key van Order
        public int? OrderID { get; set; }
        public virtual Order Order { get; set; }

        //Foreign Key van Dish
        public int? DishID { get; set; }
        public virtual MenuItem Dish { get; set; }


    }
}
