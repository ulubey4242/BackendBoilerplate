using Core.DataAccess.Abstract.Mapping;
using Core.DataAccess.Concrete.Mapping;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Mapping
{
    public partial class SystemLogMap : AppEntityTypeConfiguration<SystemLog>, IMapping
    {
        public override void Configure(EntityTypeBuilder<SystemLog> builder)
        {
            builder.ToTable(nameof(SystemLog));
            builder.HasKey(t => t.Id);

            builder.Ignore(t => t.RowGuid);

            builder.Property(t => t.Detail);
            builder.Property(t => t.Date);
            builder.Property(t => t.Audit).HasMaxLength(50);

            base.Configure(builder);
        }
    }
}
