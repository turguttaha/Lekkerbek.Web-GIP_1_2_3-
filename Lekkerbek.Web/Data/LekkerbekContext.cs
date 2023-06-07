using Lekkerbek.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Lekkerbek.Web.Data
{
    public class LekkerbekContext : IdentityDbContext
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
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<WorkerSchedule> WorkerSchedules { get; set; }
        public DbSet<WorkerHoliday> WorkerHolidays { get; set; }
        public DbSet<RestaurantOpeninghours> RestaurantOpeningHours { get; set; }
        public DbSet<RestaurantHoliday> RestaurantHolidays { get; set; }









      
    }
}
