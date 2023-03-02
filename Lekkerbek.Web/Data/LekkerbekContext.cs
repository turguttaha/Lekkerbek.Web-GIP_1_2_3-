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

        public DbSet<Customer> Customers { get; set; }

        public DbSet<TimeSlot> TimeSlot { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=ucllgipsqlteam2.database.windows.net;Database=gip2db;user=gipteam2;password=Gipteam_2;trustservercertificate=true;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
