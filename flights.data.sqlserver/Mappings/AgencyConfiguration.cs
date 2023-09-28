using flights.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace flights.data.sqlserver.Mappings
{
    public class AgencyConfiguration : IEntityTypeConfiguration<Agency>
    {
        public void Configure(EntityTypeBuilder<Agency> builder)
        {
            builder.HasKey(c => c.AgencyId);
            builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
            builder.Property(c => c.CompanyName).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Document).HasMaxLength(25).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(50).IsRequired();
            builder.Property(c => c.Phone).HasMaxLength(50).IsRequired();
            builder.Property(c => c.Address).HasMaxLength(200).IsRequired();
            builder.Property(c => c.City).HasMaxLength(200).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();
        }
    }
}
