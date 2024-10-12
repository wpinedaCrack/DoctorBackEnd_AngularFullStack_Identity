using API.Extensiones;
using API.Middleware;
using Data.Inicializador;
using Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Configura servicios
builder.Services.AgregaServiciosAplicacion(builder.Configuration);

// Configura CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()    // Permite cualquier origen
               .AllowAnyMethod()    // Permite cualquier método HTTP (GET, POST, etc.)
               .AllowAnyHeader();   // Permite cualquier encabezado
    });
});

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AgregaServiciosIdentidad(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<ApiResponse>();
builder.Services.AddScoped<IdbInicializador, DBInicializador>();//wpineda implementar identity 


var app = builder.Build();

// Middleware personalizado para manejo de excepciones
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errores/{0}");

// Configurar la canalización de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Asegúrate de que CORS esté antes de la autenticación y autorización
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();  // Asegúrate de usar autenticación después de CORS
app.UseAuthorization();

using (var scoped = app.Services.CreateScope())//wpineda implementar identity
{
    var services = scoped.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var inicializador = services.GetRequiredService<IdbInicializador>();
        inicializador.Inicializar();
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "Ocurrio un Error en la ejecución de la Migración");
    }
}

app.MapControllers();

app.Run();