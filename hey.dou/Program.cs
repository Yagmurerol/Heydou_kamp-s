using Microsoft.EntityFrameworkCore;
using hey.dou.Models; // Model klasörünüzün yolu


var builder = WebApplication.CreateBuilder(args);

// 1. Connection string'i appsettings.json'dan al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. DbContext'i servislere ekle
// (HeydouContext adýnýn doðru olduðunu varsayýyoruz)
builder.Services.AddDbContext<HeydouContext>(options =>
	options.UseSqlServer(connectionString));

// 3. Controller'larý ve Swagger'ý ekle
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Session servisini ekle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSession(); // Session'ý aktif et

app.UseAuthorization();

// ----- ÇAKIÞMA BURADAYDI - DÜZELTÝLDÝ -----
// API Controller'larýný eþleþtirmek için bu gereklidir
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// 'app.Run()' komutu en sonda olmalý
app.Run();