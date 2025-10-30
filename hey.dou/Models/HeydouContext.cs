using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace hey.dou.Models;

public partial class HeydouContext : DbContext
{
    public HeydouContext()
    {
    }

    public HeydouContext(DbContextOptions<HeydouContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AkademikTakvim> AkademikTakvims { get; set; }

    public virtual DbSet<HaftalikMenu> HaftalikMenus { get; set; }

    public virtual DbSet<SelamBacılar> SelamBacılars { get; set; }

    public virtual DbSet<TestTablosu> TestTablosus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=proje-heydou2.database.windows.net;Initial Catalog=Heydou;Persist Security Info=False;User ID=dilan;Password=AzureProje_123!;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AkademikTakvim>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Akademik__7944C870C6B6AF77");

            entity.ToTable("AkademikTakvim");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.Aciklama).HasMaxLength(500);
            entity.Property(e => e.Kategori).HasMaxLength(50);
        });

        modelBuilder.Entity<HaftalikMenu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Haftalik__C99ED250A7367CF7");

            entity.ToTable("HaftalikMenu");

            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.AnaYemek)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Corba)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Gun)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TatliVeyaMeyve)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.YardimciYemek)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SelamBacılar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__selamBAc__3213E83FAD7153AF");

            entity.ToTable("selamBAcılar");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Veri)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("veri");
        });

        modelBuilder.Entity<TestTablosu>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TestTablosu");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Mesaj).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
