using flights.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace flights.data.sqlserver.Mappings
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.CustomerId);
            builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
            builder.Property(c => c.CompanyName).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Document).HasMaxLength(25).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(50).IsRequired();
            builder.Property(c => c.Phone).HasMaxLength(50).IsRequired();
            builder.Property(c => c.Responsible).HasMaxLength(200).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();
        }
    }
}
