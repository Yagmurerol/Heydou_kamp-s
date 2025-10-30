// Gerekli using ifadelerini en üste ekleyin
using Microsoft.EntityFrameworkCore;
// HATA 1: Proje adýnýz 'hey.dou' olduðu için burasý 'hey.dou.Models' olmalý.
using hey.dou.Models;

var builder = WebApplication.CreateBuilder(args);

// ----- BURAYI EKLÝYORSUNUZ -----
// 1. Connection string'i appsettings.json'dan al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. DbContext'i servislere ekle (Dependency Injection)
// 'HeydouContext' adýnýn Models klasörünüzdeki ...Context.cs dosya adýyla
// ayný olduðundan emin olun.
builder.Services.AddDbContext<HeydouContext>(options =>
	options.UseSqlServer(connectionString));

// 3. Controller'larý ekle
builder.Services.AddControllers();
// ---------------------------------

// (Swagger/OpenAPI için olan kodlar zaten burada vardýr, onlara dokunmayýn)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// (Kalan kodlara dokunmayýn...)
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// HATA 2: Buraya JSON KODU YAPIÞTIRILMAZ.
// O kod 'appsettings.json' dosyasýna aittir.
// O yüzden buraya yapýþtýrdýðýnýz { "Logging": ... } bloðunu sildim.