using Delta;
using HyperCache.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DataSeeder>();

builder.Services.AddControllers();
builder.Services.AddCors();


var app = builder.Build();

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

app.UseCors(x => x.AllowAnyOrigin());
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
