using Microsoft.EntityFrameworkCore;
using hey.dou.Models;
using hey.dou.Services;   // 🔴 EKLE

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<HeydouContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔴 AI Danışman servisini DI container'a kaydet
builder.Services.AddScoped<IAiDanismanService, AiDanismanService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseAuthorization();

// 🔴 Açılışta Login gelsin istiyorsun:
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
