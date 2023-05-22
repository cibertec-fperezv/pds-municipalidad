using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProyectoGrupal.Service;
using ProyectoGrupal.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Dependecy Injection
builder.Services.AddSingleton<DatabaseUtils>();
builder.Services.AddScoped<IViaTypeService, ViaTypeService>();
builder.Services.AddScoped<IServiceTypeService, ServiceTypeService>();
builder.Services.AddScoped<IRelationshipService, RelationshipService>();
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Acceso/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new ResponseCacheAttribute
    {
        NoStore = true,
        Location = ResponseCacheLocation.None,
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();