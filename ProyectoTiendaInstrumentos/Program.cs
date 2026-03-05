using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Helpers;
using ProyectoTiendaInstrumentos.Repositories;
using ProyectoTiendaInstrumentos.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddControllersWithViews();
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
app.UseRouting();


app.UseAuthorization();

app.MapStaticAssets();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();