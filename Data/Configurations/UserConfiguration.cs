using AuthenticationDemo.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationDemo.Data.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
	  public void Configure(EntityTypeBuilder<User>builder) {

			builder.ToTable("Users");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Username).IsRequired().HasMaxLength(50);

			builder.Property(x => x.Email)
			.IsRequired()
			.HasMaxLength(255);

			builder.Property(x => x.HashedPassword)
			.IsRequired();

			builder.Property(x => x.CreatedAt)
	       .IsRequired();

			builder.HasIndex(x => x.Email)
			.IsUnique();

			builder.HasIndex(x => x.Username)
			.IsUnique(); 


	   }

	}
}
