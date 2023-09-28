using flights.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace flights.data.sqlserver.Mappings
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(c => c.UserId);
            builder.Property(c => c.Username).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Password).HasMaxLength(200).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();
        }
    }
}
