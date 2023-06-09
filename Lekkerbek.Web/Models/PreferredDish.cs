using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models
{
    public class PreferredDish
    {

        public int PreferredDishId { get; set; }
        [Display(Name = "Favoriete Gerechten")]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Customer> Customers { get; set; }

    }
}
