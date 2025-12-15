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

    public virtual DbSet<SelamBacılar> SelamBacılars { get; set; }

    public virtual DbSet<StajIlanlari> StajIlanlaris { get; set; }

    public virtual DbSet<TestTablosu> TestTablosus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                                                                                                                        
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

        modelBuilder.Entity<Anket>(entity =>
        {
            entity.Property(e => e.AnketId).HasColumnName("AnketID");
        });

        modelBuilder.Entity<AnketSecenegi>(entity =>
        {
            entity.HasKey(e => e.SecenekId);

            entity.Property(e => e.SecenekId).HasColumnName("SecenekID");
            entity.Property(e => e.AnketId).HasColumnName("AnketID");

            entity.HasOne(d => d.Anket).WithMany(p => p.AnketSecenegis).HasForeignKey(d => d.AnketId);
        });

        modelBuilder.Entity<EtkinlikKatılımcısı>(entity =>
        {
            entity.HasKey(e => e.KatilimciId);

            entity.Property(e => e.KatilimciId).HasColumnName("KatilimciID");
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("UserID");

            entity.HasOne(d => d.Event).WithMany(p => p.EtkinlikKatılımcısıs).HasForeignKey(d => d.EventId);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.AnketId).HasColumnName("AnketID");
            entity.Property(e => e.AnketSonucuSecildi).HasDefaultValue(false);
            entity.Property(e => e.KatilimOylamasiAcik).HasDefaultValue(false);

            entity.HasOne(d => d.Anket).WithMany(p => p.Events).HasForeignKey(d => d.AnketId);
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

        modelBuilder.Entity<Katilim>(entity =>
        {
            entity.HasKey(e => e.KatilimId).HasName("PK__Katiliml__77395AAB59196C70");

            entity.ToTable("Katilimlar");

            entity.Property(e => e.KatilimDurumu).HasDefaultValue(true);
        });

        modelBuilder.Entity<Katlar>(entity =>
        {
            entity.HasKey(e => e.KatId).HasName("PK__Katlar__04016A0A79006243");

            entity.ToTable("Katlar");

            entity.Property(e => e.KatAdi).HasMaxLength(50);
            entity.Property(e => e.KrokiResimAdi).HasMaxLength(100);
        });

        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.HasKey(e => e.KullaniciId).HasName("PK__Kullanic__E011F77BDCEC82A8");

            entity.ToTable("Kullanicilar");

            entity.HasIndex(e => e.Email, "UQ__Kullanic__A9D1053418D8E0B7").IsUnique();

            entity.Property(e => e.AdSoyad).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Rol).HasMaxLength(20);
            entity.Property(e => e.Sifre).HasMaxLength(50);
        });

        modelBuilder.Entity<Mekanlar>(entity =>
        {
            entity.HasKey(e => e.MekanId).HasName("PK__Mekanlar__7312BA59B4AE78EB");

            entity.ToTable("Mekanlar");

            entity.Property(e => e.Aciklama).HasMaxLength(200);
            entity.Property(e => e.MekanKodu).HasMaxLength(20);

            entity.HasOne(d => d.Kat).WithMany(p => p.Mekanlars)
                .HasForeignKey(d => d.KatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Mekanlar__KatId__2334397B");
        });

        modelBuilder.Entity<Oy>(entity =>
        {
            entity.HasIndex(e => new { e.AnketId, e.KullaniciId }, "UQ_Oys_AnketID_UserID").IsUnique();

            entity.Property(e => e.OyId).HasColumnName("OyID");
            entity.Property(e => e.AnketId).HasColumnName("AnketID");
            entity.Property(e => e.SecenekId).HasColumnName("SecenekID");

            entity.HasOne(d => d.Anket).WithMany(p => p.Oys)
                .HasForeignKey(d => d.AnketId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Secenek).WithMany(p => p.Oys)
                .HasForeignKey(d => d.SecenekId)
                .OnDelete(DeleteBehavior.ClientSetNull);
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

        modelBuilder.Entity<StajIlanlari>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__staj_ila__3213E83F27C772E9");

            entity.ToTable("staj_ilanlari");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aciklama).HasColumnName("aciklama");
            entity.Property(e => e.Aktif)
                .HasDefaultValue(true)
                .HasColumnName("aktif");
            entity.Property(e => e.Baslik)
                .HasMaxLength(255)
                .HasColumnName("baslik");
            entity.Property(e => e.BasvuruLinki)
                .HasMaxLength(500)
                .HasColumnName("basvuru_linki");
            entity.Property(e => e.Lokasyon)
                .HasMaxLength(100)
                .HasColumnName("lokasyon");
            entity.Property(e => e.Sirket)
                .HasMaxLength(100)
                .HasColumnName("sirket");
            entity.Property(e => e.SonBasvuru).HasColumnName("son_basvuru");
            entity.Property(e => e.StajTuru)
                .HasMaxLength(50)
                .HasColumnName("staj_turu");
            entity.Property(e => e.YayinlanmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("yayinlanma_tarihi");
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
