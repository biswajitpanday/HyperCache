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

// Use this for global Delta processing
app.UseDelta<AppDbContext>();

// Use this, If needs something like: "Request contains CustomProperty"
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
