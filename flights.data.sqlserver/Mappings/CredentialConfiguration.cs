using flights.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace flights.data.sqlserver.Mappings
{
    public class CredentialConfiguration : IEntityTypeConfiguration<Credential>
    {
        public void Configure(EntityTypeBuilder<Credential> builder)
        {
            builder.HasKey(c => c.CredentialId);
            builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Username).HasMaxLength(60).IsRequired();
            builder.Property(c => c.Password).HasMaxLength(200).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.HasMany(a => a.CredentialParameters).WithOne(b => b.Credential);
        }
    }
}
