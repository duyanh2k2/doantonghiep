using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DoAn.Models
{
    public partial class DoAnTotNghiepContext : DbContext
    {
        public DoAnTotNghiepContext()
        {
        }

        public DoAnTotNghiepContext(DbContextOptions<DoAnTotNghiepContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblComment> TblComments { get; set; } = null!;
        public virtual DbSet<TblFavoritePost> TblFavoritePosts { get; set; } = null!;
        public virtual DbSet<TblFeedBack> TblFeedBacks { get; set; } = null!;
        public virtual DbSet<TblImage> TblImages { get; set; } = null!;
        public virtual DbSet<TblLessee> TblLessees { get; set; } = null!;
        public virtual DbSet<TblRole> TblRoles { get; set; } = null!;
        public virtual DbSet<TblRoomPost> TblRoomPosts { get; set; } = null!;
        public virtual DbSet<TblSupport> TblSupports { get; set; } = null!;
        public virtual DbSet<TblUser> TblUsers { get; set; } = null!;

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Data Source=DESKTOP-6HQB58S\\SQLEXPRESS;Initial Catalog=DoAnTotNghiep;Integrated Security=True");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblComment>(entity =>
            {
                entity.HasKey(e => e.IdComment)
                    .HasName("PK__tblComme__0370297E04FB5C65");

                entity.ToTable("tblComment");

                entity.Property(e => e.IdComment).HasColumnName("idComment");

                entity.Property(e => e.IdRoomPost).HasColumnName("idRoomPost");

                entity.Property(e => e.IdUser).HasColumnName("idUser");

                entity.Property(e => e.NoiDung).HasMaxLength(255);

                entity.HasOne(d => d.IdRoomPostNavigation)
                    .WithMany(p => p.TblComments)
                    .HasForeignKey(d => d.IdRoomPost)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_RoomPost");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.TblComments)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_User");
            });

            modelBuilder.Entity<TblFavoritePost>(entity =>
            {
                entity.HasKey(e => e.IdFavoritePost)
                    .HasName("PK__tblFavor__68FF61E3E3DD2D92");

                entity.ToTable("tblFavoritePost");

                entity.HasIndex(e => new { e.IdUser, e.IdRoomPost }, "UQ__tblFavor__5F3EEFA2AC746A2A")
                    .IsUnique();

                entity.Property(e => e.IdFavoritePost).HasColumnName("idFavoritePost");

                entity.Property(e => e.IdRoomPost).HasColumnName("idRoomPost");

                entity.Property(e => e.IdUser).HasColumnName("idUser");

                entity.HasOne(d => d.IdRoomPostNavigation)
                    .WithMany(p => p.TblFavoritePosts)
                    .HasForeignKey(d => d.IdRoomPost)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavoritePost_RoomPost");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.TblFavoritePosts)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavoritePost_User");
            });

            modelBuilder.Entity<TblFeedBack>(entity =>
            {
                entity.HasKey(e => e.IdFeedBack)
                    .HasName("PK__tblFeedB__535470E9EA5348D2");

                entity.ToTable("tblFeedBack");

                entity.Property(e => e.IdFeedBack).HasColumnName("idFeedBack");

                entity.Property(e => e.IdComment).HasColumnName("idComment");

                entity.Property(e => e.IdUser).HasColumnName("idUser");

                entity.Property(e => e.NoiDung).HasMaxLength(255);

                entity.HasOne(d => d.IdCommentNavigation)
                    .WithMany(p => p.TblFeedBacks)
                    .HasForeignKey(d => d.IdComment)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeedBack_Comment");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.TblFeedBacks)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeedBack_User");
            });

            modelBuilder.Entity<TblImage>(entity =>
            {
                entity.HasKey(e => e.IdImage)
                    .HasName("PK__tblImage__84D649AF98044EBF");

                entity.ToTable("tblImage");

                entity.Property(e => e.IdImage).HasColumnName("idImage");

                entity.Property(e => e.HinhAnh).HasMaxLength(255);

                entity.Property(e => e.IdRoomPost).HasColumnName("idRoomPost");

                entity.HasOne(d => d.IdRoomPostNavigation)
                    .WithMany(p => p.TblImages)
                    .HasForeignKey(d => d.IdRoomPost)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Image_RoomPost");
            });

            modelBuilder.Entity<TblLessee>(entity =>
            {
                entity.HasKey(e => e.IdLessee)
                    .HasName("PK__tblLesse__A6CB76E95984F0DD");

                entity.ToTable("tblLessee");

                entity.HasIndex(e => e.IdUser, "UQ__tblLesse__3717C98345C683FF")
                    .IsUnique();

                entity.Property(e => e.IdLessee).HasColumnName("idLessee");

                entity.Property(e => e.IdRoomPost).HasColumnName("idRoomPost");

                entity.Property(e => e.IdUser).HasColumnName("idUser");

                entity.HasOne(d => d.IdRoomPostNavigation)
                    .WithMany(p => p.TblLessees)
                    .HasForeignKey(d => d.IdRoomPost)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lessee_RoomPost");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithOne(p => p.TblLessee)
                    .HasForeignKey<TblLessee>(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lessee_User");
            });

            modelBuilder.Entity<TblRole>(entity =>
            {
                entity.HasKey(e => e.IdRole)
                    .HasName("PK__tblRole__E5045C54293B87B2");

                entity.ToTable("tblRole");

                entity.Property(e => e.IdRole).HasColumnName("idRole");

                entity.Property(e => e.SRole)
                    .HasMaxLength(50)
                    .HasColumnName("sRole");
            });

            modelBuilder.Entity<TblRoomPost>(entity =>
            {
                entity.HasKey(e => e.IdRoomPost)
                    .HasName("PK__tblRoomP__829262173F80EB9F");

                entity.ToTable("tblRoomPost");

                entity.Property(e => e.IdRoomPost).HasColumnName("idRoomPost");

                entity.Property(e => e.DiaChi).HasMaxLength(255);

                entity.Property(e => e.IdUser).HasColumnName("idUser");

                entity.Property(e => e.MoTa).HasMaxLength(1000);

                entity.Property(e => e.NgayDang).HasColumnType("date");

                entity.Property(e => e.TieuDe).HasMaxLength(255);

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.TblRoomPosts)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoomPost_User");
            });

            modelBuilder.Entity<TblSupport>(entity =>
            {
                entity.HasKey(e => e.IdSupport)
                    .HasName("PK__tblSuppo__2320C0369817920E");

                entity.ToTable("tblSupport");

                entity.HasIndex(e => e.TuKhoa, "UQ__tblSuppo__2E7DF675A2E3C7AF")
                    .IsUnique();

                entity.Property(e => e.IdSupport).HasColumnName("idSupport");

                entity.Property(e => e.IdUser).HasColumnName("idUser");

                entity.Property(e => e.TraLoi).HasMaxLength(1000);

                entity.Property(e => e.TuKhoa).HasMaxLength(255);

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.TblSupports)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Support_User");
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.HasKey(e => e.IdUser)
                    .HasName("PK__tblUser__3717C982EFB4EBF4");

                entity.ToTable("tblUser");

                entity.Property(e => e.IdUser).HasColumnName("idUser");

                entity.Property(e => e.CanCuoc)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Gt).HasColumnName("GT");

                entity.Property(e => e.HoTen).HasMaxLength(255);

                entity.Property(e => e.IdRole).HasColumnName("idRole");

                entity.Property(e => e.MatKhau)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Sdt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SDT");

                entity.Property(e => e.TaiKhoan)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdRoleNavigation)
                    .WithMany(p => p.TblUsers)
                    .HasForeignKey(d => d.IdRole)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
