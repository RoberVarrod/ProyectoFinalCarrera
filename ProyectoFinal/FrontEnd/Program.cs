using Microsoft.EntityFrameworkCore; // Importar Entity Framework Core
using FrontEnd.Models;
using FrontEnd.Services;
using FrontEnd.Controllers; // Importar tu contexto

var builder = WebApplication.CreateBuilder(args);


// Configurar el DbContext con la cadena de conexión desde appsettings.json
builder.Services.AddDbContext<ProyectoPaqueteriaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Habilitar el uso de sesiones
builder.Services.AddDistributedMemoryCache(); // Necesario para almacenar sesiones en memoria


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
   // options.Cookie.HttpOnly = true; // Asegura que la cookie de sesión no sea accesible por scripts
   // options.Cookie.IsEssential = true; // Necesario para que funcione en GDPR-compliance
});

// Agregar servicios para controladores y vistas
builder.Services.AddControllersWithViews();


// para que trabaje con la interfaz
builder.Services.AddTransient<ICorreoService, CorreoService>();

builder.Services.AddTransient<CorreoController, CorreoController>();

// testing
//builder.Services.AddMvc().AddControllersAsServices();









var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Middleware para usar sesiones

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
