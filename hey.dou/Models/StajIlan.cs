using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeyDou.Models
{
    [Table("staj_ilanlari")]
    public class StajIlan
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // 'required' anahtar kelimesi, nullability hatalarını önler (C# 11+)
        [Required]
        [Column("baslik")]
        [StringLength(255)]
        public required string Baslik { get; set; }

        [Required]
        [Column("sirket")]
        [StringLength(100)]
        public required string Sirket { get; set; }

        [Required]
        [Column("lokasyon")]
        [StringLength(100)]
        public required string Lokasyon { get; set; }

        [Required]
        [Column("staj_turu")]
        [StringLength(50)]
        public required string StajTuru { get; set; }

        [Required]
        [Column("aciklama")]
        public required string Aciklama { get; set; }

        [Column("yayinlanma_tarihi")]
        public DateTime YayinlanmaTarihi { get; set; } = DateTime.Now;

        [Column("son_basvuru")]
        public DateTime? SonBasvuru { get; set; }

        [Column("aktif")]
        public bool Aktif { get; set; } = true;

        [Required]
        [Column("basvuru_linki")]
        [StringLength(500)]
        public required string BasvuruLinki { get; set; }
    }
}