using AuthenticationDemo.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace AuthenticationDemo.Data.Configurations
{
	public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
	{
		public void Configure(EntityTypeBuilder<UserRole> builder)
		{
			builder.ToTable("UserRoles");

			builder.HasKey(x => new
			{
				x.UserId,
				x.RoleId
			});

			builder.
			  HasOne(x => x.User)
			   .WithMany(r => r.UserRoles)
			   .HasForeignKey(x => x.UserId)
			   .OnDelete(DeleteBehavior.Cascade);

			builder.
			   HasOne(x => x.Role)
				 .WithMany(u => u.UserRoles)
				 .HasForeignKey(x => x.RoleId)
				 .OnDelete(DeleteBehavior.Cascade); 
		}
	}
}
