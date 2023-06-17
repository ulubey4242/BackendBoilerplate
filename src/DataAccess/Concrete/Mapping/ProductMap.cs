using Core.DataAccess.Abstract.Mapping;
using Core.DataAccess.Concrete.Mapping;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Mapping
{
    public partial class ProductMap : AppEntityTypeConfiguration<Product>, IMapping
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable(nameof(Product));
            builder.HasKey(t => t.Id);

            builder.Ignore(t => t.RowGuid);
            builder.HasIndex(t => t.UserId);

            builder.Property(t => t.Name);
            builder.Property(t => t.Price);

            builder.HasOne(t => t.User).WithMany(t => t.Products).OnDelete(DeleteBehavior.NoAction);

            base.Configure(builder);
        }
    }
}
