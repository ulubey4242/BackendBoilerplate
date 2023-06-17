using Core.DataAccess.Abstract.Mapping;
using Core.DataAccess.Concrete.Mapping;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Mapping
{
    public partial class OperationClaimMap : AppEntityTypeConfiguration<OperationClaim>, IMapping
    {
        public override void Configure(EntityTypeBuilder<OperationClaim> builder)
        {
            builder.ToTable(nameof(OperationClaim));
            builder.HasKey(t => t.Id);

            builder.HasIndex(t => t.RowGuid);

            builder.Property(t => t.Name).HasMaxLength(100);

            base.Configure(builder);
        }
    }
}
