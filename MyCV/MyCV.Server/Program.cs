using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCV.Server.Models;
using MyCV.Server.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using MyCV.Server.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Add services to the container.
//<Original>
builder.Services.AddControllers();
//builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCVContext") ?? throw new InvalidOperationException("Connection string 'MyCVContext' not found.")));

//builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<UserDbContext>();

builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<UserDbContext>();

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
//app.MapControllers();

//A dcéommenter pour utiliser Angular
//app.MapFallbackToFile("/index.html");

//</original>
app.UseRouting();

//Les deux lignes ci-dessous doivent être après app.UseRouting(); et avant app.UseEndpoints();
app.UseAuthentication(); // Authentification
app.UseAuthorization();

// Rediriger la route racine (/) vers la page de login
app.MapGet("/", context =>
{
    context.Response.Redirect("/Identity/Account/Login");
    return Task.CompletedTask;
});

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }