using flights.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace flights.data.sqlserver.Mappings
{
    public class CredentialParametersConfiguration : IEntityTypeConfiguration<CredentialParameters>
    {
        public void Configure(EntityTypeBuilder<CredentialParameters> builder)
        {
            builder.HasKey(c => c.CredentialParametersId);
            builder.Property(c => c.Parameter).HasMaxLength(200);
            builder.Property(c => c.Value).HasMaxLength(400);
        }
    }
}
