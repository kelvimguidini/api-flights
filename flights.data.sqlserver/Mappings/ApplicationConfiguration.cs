using flights.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace flights.data.sqlserver.Mappings
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.HasKey(c => c.ApplicationId);
            builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Description).HasMaxLength(300).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();
        }
    }
}
