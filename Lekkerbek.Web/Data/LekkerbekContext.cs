using Lekkerbek.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Data
{
    public class LekkerbekContext : DbContext
    {
        public LekkerbekContext()
        {
        }



        public LekkerbekContext(DbContextOptions<LekkerbekContext> options) 
            :base(options)

        {
        }
        //Models
        public DbSet<Customer> Customers { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Chef> Chefs { get; set; }
        public DbSet<PreferredDish> PreferredDishes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Dish> Dishes { get; set; }








        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=ucllgipsqlteam2.database.windows.net;Database=gip2db;user=gipteam2;password=Gipteam_2;trustservercertificate=true;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
