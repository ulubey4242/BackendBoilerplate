using Core.DataAccess.Abstract.Mapping;
using Core.DataAccess.Concrete.Mapping;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Mapping
{
    public partial class UserOperationClaimMap : AppEntityTypeConfiguration<UserOperationClaim>, IMapping
    {
        public override void Configure(EntityTypeBuilder<UserOperationClaim> builder)
        {
            builder.ToTable(nameof(UserOperationClaim));
            builder.HasKey(t => t.Id);

            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => t.OperationClaimId);
            builder.HasIndex(t => t.RowGuid);

            builder.HasOne(t => t.OperationClaim).WithMany(t => t.UserOperationClaims).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(t => t.User).WithMany(t => t.UserOperationClaims).OnDelete(DeleteBehavior.NoAction);

            base.Configure(builder);
        }
    }
}
