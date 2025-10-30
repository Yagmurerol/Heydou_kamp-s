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
builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();