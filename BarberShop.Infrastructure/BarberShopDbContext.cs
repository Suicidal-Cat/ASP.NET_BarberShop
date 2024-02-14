using BarberShop.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarberShop.Infrastructure
{
	public class BarberShopDbContext:IdentityDbContext<IdentityUser>
	{
        public DbSet<Barber> Barbers { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BarberShopMVC;Integrated Security=True;");
			optionsBuilder.LogTo(m => Console.WriteLine(m))
				.EnableSensitiveDataLogging();
		}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ServiceCategory>().HasData(
                new ServiceCategory { Id = 1, Name = "Haircuts" },
                new ServiceCategory { Id = 2, Name = "Beard cuts" },
                new ServiceCategory { Id = 3, Name = "Other" }
            );
        }

    }
}