using Microsoft.EntityFrameworkCore;
using hey.dou.Models;
// using hey.dou.Services; // Eğer böyle bir servisiniz yoksa yorumda kalabilir

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HeydouContext>(options =>
	options.UseSqlServer(connectionString));

// 2. Temel Servisler (MVC, Session vb.)
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// 3. Swagger Servisleri
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Özel Servisler (AI Danışman vb. - Eğer yoksa yorumda kalabilir)
// builder.Services.AddScoped<IAiDanismanService, AiDanismanService>();

var app = builder.Build();

// --- Middleware (Ara Yazılımlar) Sırası Önemlidir ---

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔴 BU SATIR EKLENDİ: Resim dosyalarını (wwwroot) dışarı açar
app.UseStaticFiles();

app.UseSession();
app.UseRouting(); // Routing eklenmesi iyi olur

app.UseAuthorization(); // Authentication varsa önce o gelmeli

// Route Ayarları (Login Sayfası Açılışı)
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Account}/{action=Login}/{id?}");

// API Controller'larını da ekle
app.MapControllers();

app.Run();