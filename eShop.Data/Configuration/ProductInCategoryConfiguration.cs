using eShop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Data.Configuration
{
    public class ProductInCategoryConfiguration : IEntityTypeConfiguration<ProductInCategory>
    {
        public void Configure(EntityTypeBuilder<ProductInCategory> builder)
        {
            builder.HasKey(t => new { t.CategoryId, t.ProductId });

            builder.ToTable("ProductInCategories");

            builder.HasOne(p => p.Product).WithMany(pc => pc.ProductInCategories).HasForeignKey(pc => pc.ProductId);

            builder.HasOne(c => c.Category).WithMany(pc => pc.ProductInCategories).HasForeignKey(pc => pc.CategoryId);



















            //builder.HasOne(t => t.Product).WithMany(pc => pc.ProductInCategories)
            //    .HasForeignKey(pc => pc.ProductId);

            //builder.HasOne(t => t.Category).WithMany(pc => pc.ProductInCategories)
            //  .HasForeignKey(pc => pc.CategoryId);
        }
    }
}
