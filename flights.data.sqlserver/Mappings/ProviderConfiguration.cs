using flights.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace flights.data.sqlserver.Mappings
{
    public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
    {
        public void Configure(EntityTypeBuilder<Provider> builder)
        {
            builder.HasKey(c => c.ProviderId);
            builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Initials).HasMaxLength(3).IsRequired();
            builder.Property(c => c.UrlProduction).HasMaxLength(200);
            builder.Property(c => c.UrlHomolog).HasMaxLength(200);
        }
    }
}
