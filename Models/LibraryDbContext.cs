using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Library.Models;

public partial class LibraryDbContext : DbContext
{
    public LibraryDbContext()
    {
    }

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Borrow> Borrows { get; set; }

    public virtual DbSet<EndUser> EndUsers { get; set; }

    public virtual DbSet<Librarian> Librarians { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=TUF293;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Book__3DE0C227CDF9E5BA");

            entity.ToTable("Book");

            entity.Property(e => e.Id)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.Author)
                .HasMaxLength(30)
                .IsFixedLength();
            entity.Property(e => e.Editorial)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.Genre)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.Title)
                .HasMaxLength(45)
                .IsFixedLength();
        });

        modelBuilder.Entity<Borrow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Borrow__4295F85FF88232C2");

            entity.ToTable("Borrow");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.Book)
                .HasMaxLength(60)
                .IsFixedLength();
            entity.Property(e => e.BorrowDate).HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Reader)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.ReturnDate).HasColumnType("datetime");

            entity.HasOne(d => d.BookNavigation).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.Book)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Borrow__BookID__47DBAE45");

            entity.HasOne(d => d.ReaderNavigation).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.Reader)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Borrow__ReaderID__46E78A0C");
        });

        modelBuilder.Entity<EndUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EndUser__E18F2163B9574016");

            entity.ToTable("EndUser");

            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.Password)
                .HasMaxLength(4000)
                .IsFixedLength();
            entity.Property(e => e.Username)
                .HasMaxLength(12)
                .IsFixedLength();
        });

        modelBuilder.Entity<Librarian>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Libraria__E4D86D9D9BE67E07");

            entity.ToTable("Librarian");

            entity.HasIndex(e => e.EndUser, "UQ__Libraria__E18F21624C2B349E").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.EndUser)
                .HasMaxLength(6)
                .IsFixedLength();

            entity.HasOne(d => d.EndUserNavigation).WithOne(p => p.Librarian)
                .HasForeignKey<Librarian>(d => d.EndUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Librarian__EndUs__440B1D61");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Member__0CF04B382F483CE5");

            entity.ToTable("Member");

            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(25)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(35)
                .IsFixedLength();
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reader__8E67A5815D41B4EC");

            entity.ToTable("Reader");

            entity.HasIndex(e => e.Member, "UQ__Reader__0CF04B397504B80A").IsUnique();

            entity.HasIndex(e => e.EndUser, "UQ__Reader__E18F2162F1FFB626").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.EndUser)
                .HasMaxLength(6)
                .IsFixedLength();
            entity.Property(e => e.Member)
                .HasMaxLength(12)
                .IsFixedLength();

            entity.HasOne(d => d.EndUserNavigation).WithOne(p => p.Reader)
                .HasForeignKey<Reader>(d => d.EndUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reader__EndUserI__403A8C7D");

            entity.HasOne(d => d.MemberNavigation).WithOne(p => p.Reader)
                .HasForeignKey<Reader>(d => d.Member)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reader__MemberID__3F466844");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
