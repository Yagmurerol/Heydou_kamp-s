using Microsoft.EntityFrameworkCore;
using HeyDOU.KampusApp.Models; // YemekhaneMenu modelini kullanmak için

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Veritabanında YemekhaneMenuleri adında bir tablo oluşturulacak.
    public DbSet<YemekhaneMenu> YemekhaneMenuleri { get; set; }
}