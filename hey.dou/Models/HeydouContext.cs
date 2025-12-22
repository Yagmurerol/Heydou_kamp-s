using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace hey.dou.Models;

public partial class HeydouContext : DbContext
{
    // Boş yapıcı metot: Bağımlılık enjeksiyonu kullanılmadığı durumlar için.
    public HeydouContext() { }

    // Yapılandırıcı metot: Veritabanı ayarlarını (bağlantı dizesi vb.) dışarıdan alır.
    public HeydouContext(DbContextOptions<HeydouContext> options) : base(options) { }

    #region Veritabanı Tablo Tanımları (DbSets)
    public virtual DbSet<AkademikTakvim> AkademikTakvims { get; set; }
    public virtual DbSet<Anket> Ankets { get; set; }
    public virtual DbSet<AnketSecenegi> AnketSecenegis { get; set; }
    public virtual DbSet<EtkinlikKatılımcısı> EtkinlikKatılımcısıs { get; set; }
    public virtual DbSet<Event> Events { get; set; }
    public virtual DbSet<HaftalikMenu> HaftalikMenus { get; set; }
    public virtual DbSet<Katilim> Katilimlars { get; set; }
    public virtual DbSet<Katlar> Katlars { get; set; }
    public virtual DbSet<Kullanicilar> Kullanicilars { get; set; }
    public virtual DbSet<Mekanlar> Mekanlars { get; set; }
    public virtual DbSet<Oy> Oys { get; set; }
    
    public virtual DbSet<StajIlanlari> StajIlanlaris { get; set; }
    public virtual DbSet<TestTablosu> TestTablosus { get; set; }
    #endregion

    // Veritabanı bağlantı yapılandırması (Azure SQL Server bağlantı dizesi)
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=proje-heydou2.database.windows.net;Initial Catalog=Heydou;Persist Security Info=False;User ID=dilan;Password=AzureProje_123!;Encrypt=False;Trust Server Certificate=True");

    // Fluent API: Veritabanı tabloları ve sütunları arasındaki kuralların tanımlandığı yer
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Akademik Takvim tablosu ayarları
        modelBuilder.Entity<AkademikTakvim>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Akademik__7944C870C6B6AF77");
            entity.ToTable("AkademikTakvim");
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.Aciklama).HasMaxLength(500);
            entity.Property(e => e.Kategori).HasMaxLength(50);
        });

        // Anket ve Seçenekler arasındaki ilişki
        modelBuilder.Entity<AnketSecenegi>(entity =>
        {
            entity.HasKey(e => e.SecenekId);
            entity.Property(e => e.SecenekId).HasColumnName("SecenekID");
            entity.HasOne(d => d.Anket).WithMany(p => p.AnketSecenegis).HasForeignKey(d => d.AnketId);
        });

        // Etkinlik Katılımcıları ve İlişkiler
        modelBuilder.Entity<EtkinlikKatılımcısı>(entity =>
        {
            entity.HasKey(e => e.KatilimciId);
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.HasOne(d => d.Event).WithMany(p => p.EtkinlikKatılımcısıs).HasForeignKey(d => d.EventId);
        });

        // Haftalık Yemek Menüsü 
        modelBuilder.Entity<HaftalikMenu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Haftalik__C99ED250A7367CF7");
            entity.ToTable("HaftalikMenu");
            entity.Property(e => e.AnaYemek).HasMaxLength(150).IsUnicode(false);
            entity.Property(e => e.Corba).HasMaxLength(100).IsUnicode(false);
        });

        // Kullanıcılar tablosu ve Benzersiz Email kuralı
        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.HasKey(e => e.KullaniciId).HasName("PK__Kullanic__E011F77BDCEC82A8");
            entity.ToTable("Kullanicilar");
            entity.HasIndex(e => e.Email, "UQ__Kullanic__A9D1053418D8E0B7").IsUnique();
            entity.Property(e => e.AdSoyad).HasMaxLength(100);
            entity.Property(e => e.Rol).HasMaxLength(20);
        });

        // Mekanlar ve Katlar arasındaki ilişki
        modelBuilder.Entity<Mekanlar>(entity =>
        {
            entity.HasKey(e => e.MekanId).HasName("PK__Mekanlar__7312BA59B4AE78EB");
            entity.HasOne(d => d.Kat).WithMany(p => p.Mekanlars)
                .HasForeignKey(d => d.KatId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Oy tablosu
        modelBuilder.Entity<Oy>(entity =>
        {
            entity.HasIndex(e => new { e.AnketId, e.KullaniciId }, "UQ_Oys_AnketID_UserID").IsUnique();
            entity.HasOne(d => d.Anket).WithMany(p => p.Oys).HasForeignKey(d => d.AnketId).OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Staj İlanları tablosu 
        modelBuilder.Entity<StajIlanlari>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__staj_ila__3213E83F27C772E9");
            entity.Property(e => e.Aktif).HasDefaultValue(true);
            entity.Property(e => e.YayinlanmaTarihi).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}