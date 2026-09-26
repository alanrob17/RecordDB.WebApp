using RecordDB.API.Data;
using RecordDB.API.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------
// Services
// -----------------------------------------------------------------------

builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title   = "RecordDB API",
        Version = "v1",
        Description = "RESTful API for the RecordDB music collection database."
    });

    // Include XML comments from this assembly so Swagger shows <summary> docs.
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// Data layer (singleton — stateless, just holds config)
builder.Services.AddSingleton<IDataAccess, DataAccess>();

// Repositories (scoped — one per HTTP request)
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<IRecordRepository, RecordRepository>();
builder.Services.AddScoped<IDiscRepository, DiscRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();

// -----------------------------------------------------------------------
// Pipeline
// -----------------------------------------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "RecordDB API v1");
        options.RoutePrefix = "swagger"; // Swagger UI at /swagger
    });

    app.UseReDoc(options =>
    {
        options.DocumentTitle = "RecordDB API Documentation";
        options.SpecUrl = "/openapi/v1.json";
        options.RoutePrefix = "redoc"; // ReDoc UI at /redoc
    });

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("RecordDB API Reference")
               .WithTheme(ScalarTheme.Mars)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
               .WithOpenApiRoutePattern("/openapi/{documentName}.json");
    });

    // Conveniently redirect root / to /scalar/v1
    app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
