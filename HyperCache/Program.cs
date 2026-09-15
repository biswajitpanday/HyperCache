using Delta;
using HyperCache.Api.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<SeedOptions>(builder.Configuration.GetSection(SeedOptions.SectionName));
builder.Services.AddScoped<DataSeeder>();

builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddOpenApi();


var app = builder.Build();

// CORS must run before Delta: Delta short-circuits with 304 Not Modified, and that response
// still needs Access-Control-Allow-Origin for the cross-origin Blazor client.
app.UseCors(x => x.AllowAnyOrigin());

// Applies the Delta middleware to the specified AppDbContext for all incoming requests.
// This enables Delta's features such as tracking and managing changes in the specified database context Globally.
app.UseDelta<AppDbContext>();

// OR For Custom Logic ---------------------------------------------------
// Configures the Delta middleware to execute only for requests where the URL path contains "CustomProperties".
// This ensures that Delta logic is applied selectively, optimizing performance and avoiding unnecessary processing for other requests.
// app.UseDelta<AppDbContext>(shouldExecute: ctx => ctx.Request.Path.ToString().Contains("CustomProperties"));

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<DataSeeder>();
    await seeder.SeedCustomPropertiesAsync();
}

if (app.Environment.IsDevelopment())
{
    // OpenAPI document at /openapi/v1.json and a browsable reference UI at /scalar.
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposes the implicit Program class to WebApplicationFactory<Program> in the test project.
public partial class Program;
