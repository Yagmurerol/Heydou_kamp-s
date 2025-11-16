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

    // === MEVCUT MODELLERİNİZ ===
    public virtual DbSet<AkademikTakvim> AkademikTakvims { get; set; }
    public virtual DbSet<HaftalikMenu> HaftalikMenus { get; set; }
    public virtual DbSet<SelamBacılar> SelamBacılars { get; set; }
    public virtual DbSet<TestTablosu> TestTablosus { get; set; }
    // (YemekhaneMenu DbSet'iniz eksikse onu da ekleyebilirsiniz)
    // public virtual DbSet<YemekhaneMenu> YemekhaneMenus { get; set; }


    // === GÜNCELLEME 1: 5. FONKSİYON İÇİN 4 YENİ MODEL ===
    public virtual DbSet<Anket> Ankets { get; set; }
    public virtual DbSet<AnketSecenegi> AnketSecenegis { get; set; }
    public virtual DbSet<Oy> Oys { get; set; }
    public virtual DbSet<Event> Events { get; set; }
    // ====================================================


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#warning To protect potentially sensitive information in your connection string...

        // === GÜNCELLEME 2: ŞİFREDEKİ '!' HATASI DÜZELTMESİ ===
        // Şifre (AzureProje_123!) tek tırnak (' ') içine alındı.
        optionsBuilder.UseSqlServer("Server=proje-heydou2.database.windows.net;Initial Catalog=Heydou;Persist Security Info=False;User ID=dilan;Password='AzureProje_123!';Encrypt=False;Trust Server Certificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- MEVCUT KURALLARINIZ ---
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
            entity.Property(e => e.AnaYemek).HasMaxLength(150).IsUnicode(false);
            entity.Property(e => e.Corba).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Gun).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.TatliVeyaMeyve).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.YardimciYemek).HasMaxLength(100).IsUnicode(false);
        });

        modelBuilder.Entity<SelamBacılar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__selamBAc__3213E83FAD7153AF");
            entity.ToTable("selamBAcılar");
            entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
            entity.Property(e => e.Veri).HasMaxLength(255).IsUnicode(false).HasColumnName("veri");
        });

        modelBuilder.Entity<TestTablosu>(entity =>
        {
            entity.HasNoKey().ToTable("TestTablosu");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Mesaj).HasMaxLength(100);
        });

        // === GÜNCELLEME 3: 5. FONKSİYON İÇİN YENİ KURAL ===
        // "1 kullanıcı 1 ankete sadece 1 oy verebilir" kuralı
        modelBuilder.Entity<Oy>(entity =>
        {
            // 'oy.cs' modelinizde 'AnketID' ve 'UserID' alanları olmalı
            entity.HasIndex(e => new { e.AnketID, e.UserID }).IsUnique();
        });
        // ===============================================

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}