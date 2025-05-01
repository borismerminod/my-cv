using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCV.Server.Models;
using MyCV.Server.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
// Add services to the container.
//<Original>
builder.Services.AddControllers();
//builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCVContext") ?? throw new InvalidOperationException("Connection string 'MyCVContext' not found.")));

//</Original>
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();

    //<original>
    app.UseSwagger();
    app.UseSwaggerUI();
    //</original>
}

//<original>
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();
//app.MapControllers();
app.MapFallbackToFile("/index.html");

//</original>
app.UseRouting();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();