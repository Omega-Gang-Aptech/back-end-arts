using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace back_end_arts.Models
{
    public partial class ArtsDbWithKeyContext : DbContext
    {
        public ArtsDbWithKeyContext()
        {
        }

        public ArtsDbWithKeyContext(DbContextOptions<ArtsDbWithKeyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-3LRCUJV; Initial Catalog= ArtsDbWithKey;Integrated Security = true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.CategoryCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("categoryCode")
                    .IsFixedLength(true);

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("categoryName");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.Property(e => e.FeedbackId).HasColumnName("feedbackId");

                entity.Property(e => e.Comment)
                    .HasColumnType("text")
                    .HasColumnName("comment");

                entity.Property(e => e.FeedbackProductId)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("feedbackProductId")
                    .IsFixedLength(true);

                entity.Property(e => e.FeedbackUserId).HasColumnName("feedbackUserId");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.FeedbackProduct)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.FeedbackProductId)
                    .HasConstraintName("FK__Feedback__feedba__32E0915F");

                entity.HasOne(d => d.FeedbackUser)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.FeedbackUserId)
                    .HasConstraintName("FK__Feedback__feedba__31EC6D26");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("orderId")
                    .IsFixedLength(true);

                entity.Property(e => e.OrderAddress)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("orderAddress");

                entity.Property(e => e.OrderCreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("orderCreateDate");

                entity.Property(e => e.OrderDeliveryType).HasColumnName("orderDeliveryType");

                entity.Property(e => e.OrderDescription)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("orderDescription");

                entity.Property(e => e.OrderPaymentMethods).HasColumnName("orderPaymentMethods");

                entity.Property(e => e.OrderStatus).HasColumnName("orderStatus");

                entity.Property(e => e.OrderTotal).HasColumnName("orderTotal");

                entity.Property(e => e.OrderUserId).HasColumnName("orderUserId");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.OrderUser)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderUserId)
                    .HasConstraintName("FK__Order__orderUser__33D4B598");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId)
                    .HasName("PK__OrderDet__83077859B1EE9EC9");

                entity.ToTable("OrderDetail");

                entity.Property(e => e.DetailId).HasColumnName("detailId");

                entity.Property(e => e.DetailOrderId)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("detailOrderId")
                    .IsFixedLength(true);

                entity.Property(e => e.DetailPrice).HasColumnName("detailPrice");

                entity.Property(e => e.DetailProductId)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("detailProductId")
                    .IsFixedLength(true);

                entity.Property(e => e.DetailProductImage)
                    .IsUnicode(false)
                    .HasColumnName("detailProductImage");

                entity.Property(e => e.DetailProductName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("detailProductName");

                entity.Property(e => e.DetailQuantity).HasColumnName("detailQuantity");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.DetailOrder)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.DetailOrderId)
                    .HasConstraintName("FK__OrderDeta__detai__34C8D9D1");

                entity.HasOne(d => d.DetailProduct)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.DetailProductId)
                    .HasConstraintName("FK__OrderDeta__detai__35BCFE0A");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("productId")
                    .IsFixedLength(true);

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.ProductImage)
                    .IsUnicode(false)
                    .HasColumnName("productImage");

                entity.Property(e => e.ProductLongDescription)
                    .HasColumnType("text")
                    .HasColumnName("productLongDescription");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("productName");

                entity.Property(e => e.ProductPrice).HasColumnName("productPrice");

                entity.Property(e => e.ProductQuantity).HasColumnName("productQuantity");

                entity.Property(e => e.ProductShortDescription)
                    .IsUnicode(false)
                    .HasColumnName("productShortDescription");

                entity.Property(e => e.ProductStatus).HasColumnName("productStatus");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Product__categor__36B12243");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshToken");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__RefreshTo__userI__37A5467C");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DisplayName)
                    .IsUnicode(false)
                    .HasColumnName("displayName");

                entity.Property(e => e.RoleName)
                    .IsUnicode(false)
                    .HasColumnName("roleName");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UserAddress)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("userAddress");

                entity.Property(e => e.UserAvatar)
                    .IsUnicode(false)
                    .HasColumnName("userAvatar");

                entity.Property(e => e.UserEmail)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("userEmail");

                entity.Property(e => e.UserFullName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("userFullName");

                entity.Property(e => e.UserGender).HasColumnName("userGender");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userName");

                entity.Property(e => e.UserPhone)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("userPhone");

                entity.Property(e => e.UserRole).HasColumnName("userRole");

                entity.HasOne(d => d.UserRoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserRole)
                    .HasConstraintName("FK__User__userRole__38996AB5");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
