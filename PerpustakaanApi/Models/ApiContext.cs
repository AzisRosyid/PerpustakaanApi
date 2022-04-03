using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PerpustakaanApi.Models
{
    public partial class ApiContext : DbContext
    {
        public ApiContext()
        {
        }

        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<BookGenre> BookGenres { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Favorite> Favorites { get; set; } = null!;
        public virtual DbSet<Genre> Genres { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\sqlexpress;Database=Perpustakaan;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Book");

                entity.Property(e => e.Id)
                    .HasMaxLength(24)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Author)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Download)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Image)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Publisher)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.Category)
                    .HasConstraintName("FK_Book_Category");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Book_User");
            });

            modelBuilder.Entity<BookGenre>(entity =>
            {
                entity.ToTable("BookGenre");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BookId)
                    .HasMaxLength(24)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookGenres)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookGenre_Book");

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.BookGenres)
                    .HasForeignKey(d => d.GenreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookGenre_Genre");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.ToTable("Favorite");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BookId)
                    .HasMaxLength(24)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Favorite_Book");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Favorite_User");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("Genre");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Image)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
