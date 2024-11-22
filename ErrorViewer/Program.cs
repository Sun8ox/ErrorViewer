using System.Runtime.CompilerServices;
using ErrorViewer.Controllers;
using ErrorViewer.Data;
using ErrorViewer.Functions;
using ErrorViewer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ErrorViewerDB>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ErrorViewerDB") ?? throw new InvalidOperationException("Connection string 'ErrorViewerDB' not found.")));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Login/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ErrorViewerDB>();
    
    if (db.Database.EnsureCreated())
    {
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = AuthController.generatePasswordHash("admin"),
                isAdmin = true,
                isBanned = false,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            });
                    
            db.SaveChanges();
        }    
    }
    
    
    SourceManager.sources = db.Sources.ToList();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}");

app.Run();