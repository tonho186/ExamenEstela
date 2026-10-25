Examen para Analista Programador Semi Senior 

Sección 1: Evaluación Teórica (45 minutos, 25 puntos) 

1. Seguridad en el Desarrollo (4 puntos) 
Validación de entradas: Es fundamental validar y sanear todas las entradas del usuario para prevenir ataques como inyección SQL (SQLi) o cross-site scripting (XSS). Esto incluye verificar tipos de datos, longitudes, formatos, y eliminar cualquier contenido malicioso antes de procesar o almacenar la información. Utilizar bibliotecas de validación específicas para el lenguaje de programación en uso es una buena práctica.
Autenticación y autorización (JWT, OAuth): Para garantizar que solo usuarios autorizados accedan a ciertos recursos, se deben implementar mecanismos de autenticación robustos. JSON Web Tokens (JWT) permiten manejar sesiones sin estado en APIs REST, mientras que OAuth 2.0 es útil para delegar autenticación a proveedores externos. Además, se debe verificar que el usuario tenga los permisos necesarios antes de permitirle realizar ciertas acciones.
 2. Revisión de Código (4 puntos) 
Revisar el cumplimiento de estándares de codificación
Propósito: Asegurar que el código siga las convenciones definidas por el equipo o comunidad. Esto mejora la consistencia y facilita que cualquier desarrollador pueda entender y mantener el código en el futuro.
Detectar errores lógicos y posibles vulnerabilidades
Propósito: Analizar el código en busca de fallos que puedan causar comportamientos incorrectos o generar riesgos de seguridad, como inyecciones SQL, falta de validaciones, exposición de datos sensibles, entre otros. La detección temprana reduce el costo de corrección y mejora la confiabilidad del software.
Ofrecer feedback claro y constructivo
Propósito: Promover un entorno colaborativo en el que se ayuda a mejorar el código sin desmotivar al autor. El feedback debe ser específico, respetuoso y orientado a buenas prácticas, fomentando así la mejora continua de habilidades y la calidad del trabajo en equipo.

 3. .NET Core (6 puntos)
a. Manejo de errores centralizado en una API REST con .NET Core
1. Crear un middleware personalizado para manejo de errores
En .NET Core, puedo crear un middleware para capturar y manejar excepciones globalmente. Esto permite centralizar la lógica de manejo de errores y evitar la repetición en cada controlador.
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ocurrió un error: {ex.Message}");
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await httpContext.Response.WriteAsync("Ocurrió un error en el servidor.");
        }
    }
}
2. Registrar el middleware en Startup.cs
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
    }

    // Otros middlewares...
}
3. Uso de logging para registrar errores
Utilizo el sistema de logging integrado en .NET Core para registrar detalles sobre las excepciones y otros eventos importantes.
public class MyController : ControllerBase
{
    private readonly ILogger<MyController> _logger;

    public MyController(ILogger<MyController> logger)
    {
        _logger = logger;
    }

    public IActionResult Get()
    {
        try
        {
            // Lógica del controlador...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar la solicitud.");
            return StatusCode(500, "Ocurrió un error en el servidor.");
        }
    }
}

b. Inyección de dependencias y duración de vida de los objetos en .NET Core
La inyección de dependencias es un patrón de diseño que permite que las clases reciban sus dependencias desde el exterior en lugar de crearlas internamente. En .NET Core, esto se logra mediante el contenedor de servicios integrado.
Duración de vida de los objetos
.NET Core ofrece tres tipos de duración de vida para los servicios registrados en el contenedor de dependencias:
1.	Transient: Un nuevo objeto es creado cada vez que se solicita. Es útil para servicios ligeros y sin estado.
services.AddTransient<IMiServicio, MiServicio>();
2.	Scoped: Un objeto es creado una vez por cada solicitud HTTP. Es adecuado para servicios que deben mantener el estado durante una solicitud, como los servicios que interactúan con la base de datos.
services.AddScoped<IMiServicio, MiServicio>();
3.	Singleton: Un único objeto es creado y compartido durante toda la vida de la aplicación. Es ideal para servicios que no mantienen estado y son costosos de crear.
services.AddSingleton<IMiServicio, MiServicio>();
Y al final configuraría en la clase Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<IMiServicio, MiServicioTransient>();
    services.AddScoped<IMiServicio, MiServicioScoped>();
    services.AddSingleton<IMiServicio, MiServicioSingleton>();
}


 4. Manejo de Conexiones a SQL Server 
La conexión a una base de datos SQL Server desde una aplicación .NET la he manejado usando ADO.NET y Entity Framework.
Conexión a SQL Server con Entity Framework Core
1. Configuración de la cadena de conexión
En EF Core, la cadena de conexión se define típicamente en el archivo appsettings.json para facilitar su gestión y evitar la codificación rígida:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MiBaseDeDatos;Integrated Security=True;"
  }
}
2. Configuración del DbContext
El DbContext se configura en el método ConfigureServices de Startup.cs para establecer la conexión con la base de datos:
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
}
3. Ejemplo de uso del DbContext
Para realizar consultas a la base de datos, se puede utilizar LINQ a través del DbContext:
public class EmpleadoService
{
    private readonly ApplicationDbContext _context;

    public EmpleadoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Empleado>> ObtenerEmpleadosAsync()
    {
        return await _context.Empleados.ToListAsync();
    }
}
4. Manejo de transacciones
EF Core permite manejar transacciones de manera explícita utilizando BeginTransaction:
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    // Operaciones de base de datos
    await _context.SaveChangesAsync();
    await transaction.CommitAsync();
}
catch (Exception)
{
    await transaction.RollbackAsync();
    throw;
}
Este enfoque asegura la atomicidad de las operaciones y permite un manejo adecuado de errores.
Conexión a SQL Server con ADO.NET
1. Establecimiento de la conexión
ADO.NET proporciona un control más granular sobre las operaciones de base de datos. La conexión se establece utilizando SqlConnection:
using (var connection = new SqlConnection("Server=localhost;Database=MiBaseDeDatos;Integrated Security=True;"))
{
    await connection.OpenAsync();
    // Operaciones con la base de datos
}
2. Ejecución de consultas
Las consultas se ejecutan utilizando SqlCommand y SqlDataReader:
using (var command = new SqlCommand("SELECT * FROM Empleados", connection))
using (var reader = await command.ExecuteReaderAsync())
{
    while (await reader.ReadAsync())
    {
        var nombre = reader.GetString(0);
        // Procesar datos
    }
}
3. Manejo de transacciones
ADO.NET permite un control explícito de las transacciones:
using (var transaction = await connection.BeginTransactionAsync())
{
    try
    {
        // Operaciones de base de datos
        await transaction.CommitAsync();
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw;
    }
}
Este enfoque proporciona flexibilidad y control detallado sobre las operaciones de base de datos.
Buenas prácticas implementadas
Uso de async/await: Para operaciones asincrónicas, mejorando la escalabilidad y rendimiento de la aplicación.
Manejo adecuado de excepciones: Implementación de bloques try-catch para capturar y manejar errores de manera efectiva.
Uso de transacciones: Garantiza la atomicidad de las operaciones y mantiene la integridad de los datos.
Separación de responsabilidades: Implementación de servicios dedicados para manejar la lógica de acceso a datos, promoviendo la mantenibilidad y claridad del código.
 5. Desarrollo de API en .NET
a. Desarrollo end-to-end de una API REST en .NET Core
1. Estructura en capas
La arquitectura en capas ayuda a separar las responsabilidades y facilita el mantenimiento y escalabilidad de la aplicación. En este caso, se emplea una estructura comúnmente utilizada:
API Layer: Contiene los controladores que exponen los endpoints REST.
Service Layer: Contiene la lógica de negocio.
Repository Layer: Interactúa directamente con la base de datos.
2. Implementación de las capas
 API Layer

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoService _empleadoService;

    public EmpleadosController(IEmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmpleadoDto>>> GetEmpleados()
    {
        var empleados = await _empleadoService.ObtenerEmpleadosAsync();
        return Ok(empleados);
    }
}
Service Layer

public interface IEmpleadoService
{
    Task<IEnumerable<EmpleadoDto>> ObtenerEmpleadosAsync();
}

public class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _empleadoRepository;

    public EmpleadoService(IEmpleadoRepository empleadoRepository)
    {
        _empleadoRepository = empleadoRepository;
    }

    public async Task<IEnumerable<EmpleadoDto>> ObtenerEmpleadosAsync()
    {
        var empleados = await _empleadoRepository.GetAllAsync();
        return empleados.Select(e => new EmpleadoDto { Nombre = e.Nombre, Cargo = e.Cargo });
    }
}
Repository Layer

public interface IEmpleadoRepository
{
    Task<IEnumerable<Empleado>> GetAllAsync();
}

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly ApplicationDbContext _context;

    public EmpleadoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Empleado>> GetAllAsync()
    {
        return await _context.Empleados.ToListAsync();
    }
}
3. Configuración del DbContext
public class ApplicationDbContext : DbContext
{
    public DbSet<Empleado> Empleados { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }
}
4. Configuración en Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

    services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
    services.AddScoped<IEmpleadoService, EmpleadoService>();

    services.AddControllers();
}
5. Pruebas de funcionalidad
Para probar la funcionalidad de la API, se pueden utilizar herramientas como Postman. Estas herramientas permiten enviar solicitudes HTTP a los endpoints de la API y verificar las respuestas, asegurando que la lógica de negocio y la interacción con la base de datos funcionen correctamente.
b. Verbos HTTP en servicios REST
Los verbos HTTP son fundamentales en el diseño de APIs REST, ya que definen las operaciones que se pueden realizar sobre los recursos. A continuación, se describen los verbos más comunes y su propósito:
•	GET: Recupera información de un recurso. Es idempotente y seguro, lo que significa que no modifica el estado del servidor y puede repetirse sin efectos secundarios. 
•	POST: Crea un nuevo recurso. No es idempotente, por lo que múltiples solicitudes POST pueden crear múltiples recursos. 
•	PUT: Actualiza o reemplaza un recurso existente. Es idempotente, lo que significa que realizar la misma solicitud PUT varias veces tendrá el mismo efecto. 
•	PATCH: Aplica modificaciones parciales a un recurso. No es necesariamente idempotente, ya que puede modificar el estado del recurso de manera diferente en cada solicitud. 
•	DELETE: Elimina un recurso. Es idempotente, ya que eliminar un recurso varias veces tendrá el mismo efecto que eliminarlo una vez.
Sección 2: Desarrollo Práctico (2 hora y 15 minutos, 65 puntos)
2. Análisis de Solución (5 puntos)
Se eligió Entity Framework Core para simplificar el acceso a datos y mantener integridad referencial. Se usaron transacciones para asegurar consistencia al crear órdenes, especialmente al disminuir el stock. La separación entre entidades y DTOs permite un control claro de los datos expuestos. Las validaciones previenen errores comunes, y los endpoints están diseñados para ser fácilmente consumidos desde herramientas como Postman.
