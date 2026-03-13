using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Helpers;
using ProyectoTiendaInstrumentos.Repositories;
using ProyectoTiendaInstrumentos.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization(opt =>
{
   // opt.AddPolicy("SOLOJEFES", policy => policy.RequireRole("PRESIDENTE", "DIRECTOR", "ANALISTA"));
    opt.AddPolicy("AdminOnly",
        policy => policy.RequireClaim("Admin"));
    //opt.AddPolicy("SoloRicos", policy => policy.Requirements.Add(new OverSalarioRequirement()));
    //opt.AddPolicy("TieneSubordinados", policy => policy.Requirements.Add(new TieneSubordinadosRequirement()));
});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(
    CookieAuthenticationDefaults.AuthenticationScheme,
    config =>
    {
        config.AccessDeniedPath = "/Cuenta/ErrorAcceso";
    }
);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
string connectionString = builder.Configuration.GetConnectionString("SqlTiendaCasa");
builder.Services.AddDbContext<ProyectoTiendaInstrumentosContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddTransient<RepositoryProductos>();
builder.Services.AddTransient<RepositoryTipos>();
builder.Services.AddTransient<RepositorySubtipos>();
builder.Services.AddTransient<RepositoryUser>();
builder.Services.AddTransient<RepositoryPedidos>();
builder.Services.AddTransient<RepositoryValoraciones>();
builder.Services.AddSingleton<HelperPathProvider>();
builder.Services.AddControllersWithViews(opt => opt.EnableEndpointRouting = false)
    .AddSessionStateTempDataProvider();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IRepositoryCarrito, RepositoryCarritoSession>();


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
//app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.MapStaticAssets();
app.UseSession();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();
app.UseMvc(routes =>
{
    routes.MapRoute(name: "pagina",
        template: "{controller=Home}/{action=Index}/{pagina?}");
    routes.MapRoute(name: "familia",
        template: "{controller=Home}/{action=Index}/{idFamilia?}");
    routes.MapRoute(name: "producto",
        template: "{controller=Home}/{action=Index}/{idProducto?}");
    routes.MapRoute(name: "pedido",
        template: "{controller=Home}/{action=Index}/{idPedido?}");
    routes.MapRoute(name: "tipo",
        template: "{controller=Home}/{action=Index}/{idTipo?}");
    routes.MapRoute(name: "subtipo",
        template: "{controller=Home}/{action=Index}/{idSubtipo?}");
    routes.MapRoute(name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});


app.Run();