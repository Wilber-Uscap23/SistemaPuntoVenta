
using Microsoft.EntityFrameworkCore;
using PuntoVentaAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONFIGURACIÓN DE SERVICIOS
// ==========================================

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// ==========================================
// CONEXIÓN CON MYSQL - ENTITY FRAMEWORK CORE
// ==========================================

// Obtener la cadena de conexión desde la configuración
// En desarrollo se utilizará la guardada en User Secrets
var cadenaConexion = builder.Configuration
    .GetConnectionString("ConexionMySQL");

if (string.IsNullOrWhiteSpace(cadenaConexion))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión de MySQL.");
}

// Registrar el DbContext
builder.Services.AddDbContext<PuntoVentaDbContext>(options =>
    options.UseMySql(
        cadenaConexion,
        ServerVersion.AutoDetect(cadenaConexion)
    )
);

// ==========================================
// CREAR LA APLICACIÓN
// ==========================================

var app = builder.Build();

// ==========================================
// CONFIGURACIÓN HTTP
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// ==========================================
// INICIAR LA API
// ==========================================

app.Run();
