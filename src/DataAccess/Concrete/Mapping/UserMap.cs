using Core.DataAccess.Abstract.Mapping;
using Core.DataAccess.Concrete.Mapping;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Mapping
{
    public partial class UserMap : AppEntityTypeConfiguration<User>, IMapping
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(t => t.Id);

            builder.HasIndex(t => t.RowGuid);

            builder.Property(t => t.FirstName).HasMaxLength(250);
            builder.Property(t => t.LastName).HasMaxLength(250);
            builder.Property(t => t.Email).HasMaxLength(250);
            builder.Property(t => t.Password);
            builder.Property(t => t.Token);
            builder.Property(t => t.TokenExpiredAt);
            builder.Property(t => t.Deleted);

            base.Configure(builder);
        }
    }
}
