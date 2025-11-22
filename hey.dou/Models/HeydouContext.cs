using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using HeyDou.Models; // StajIlan modelinin bulunduğu namespace'i ekledik

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
    // public virtual DbSet<YemekhaneMenu> YemekhaneMenus { get; set; }

    // === 5. FONKSİYON MODELLERİ ===
    public virtual DbSet<Anket> Ankets { get; set; }
    public virtual DbSet<AnketSecenegi> AnketSecenegis { get; set; }
    public virtual DbSet<Oy> Oys { get; set; }
    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Kullanici> Kullanicilar { get; set; }

    // === GÜNCELLEME: 6. FONKSİYON İÇİN YENİ MODEL ===
    public virtual DbSet<Katilim> Katilimlar { get; set; }
    // ===============================================

    // ✨ GÜNCELLEME: STAJ İLANLARI İÇİN YENİ MODEL (7. FONKSİYON) ✨
    public virtual DbSet<StajIlan> StajIlanlari { get; set; }
    // ===============================================


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // UYARI GİDERİLDİ
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

        // === 5. FONKSİYON KURALI: 1 kullanıcı 1 ankete sadece 1 oy verebilir ===
        modelBuilder.Entity<Oy>(entity =>
        {
            entity.HasIndex(e => new { e.AnketId, e.KullaniciId }).IsUnique();
        });

        // === 6. FONKSİYON KURALI: Katılım ===
        modelBuilder.Entity<Katilim>(entity =>
        {
            entity.HasIndex(e => new { e.EtkinlikId, e.KullaniciId }).IsUnique();

            entity.HasOne(d => d.Etkinlik)
                    .WithMany(p => p.Katilimlar)
                    .HasForeignKey(d => d.EtkinlikId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Katilim_Event");
        });
        // ===============================================

        // ✨ GÜNCELLEME: StajIlan için kurallar burada tanımlanabilir.
        // Eğer StajIlan modelinde [Table] ve [Column] niteliklerini kullandıysanız, 
        // buraya ek bir kural eklemenize gerek yoktur.

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}