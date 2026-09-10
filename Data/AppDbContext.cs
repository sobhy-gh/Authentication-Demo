using AuthenticationDemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationDemo.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<User> Users => Set<User>();
		public DbSet<Role> Roles => Set<Role>(); 
		public DbSet<UserRole> UserRoles => Set<UserRole>();
		public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfigurationsFromAssembly(
			typeof(AppDbContext).Assembly);

			modelBuilder.Entity<Role>().HasData(

				 new Role
				 {
					 Id = 1,
					 Name = "User"
				 } ,

				 new Role {

				    Id = 2 ,
					Name = "Admin"

				 }
			);
		}
	}
}
