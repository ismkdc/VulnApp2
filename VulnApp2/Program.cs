using Microsoft.EntityFrameworkCore;
using VulnApp2;

var builder = WebApplication.CreateBuilder(args);

const string connectionString =
    "Server=mysql;Port=3306;Database=your_database_name;User Id=your_user;Password=your_password;";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        connectionString, ServerVersion.AutoDetect(connectionString)
    );
});

var app = builder.Build();

using var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope();
var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.Database.MigrateAsync();
if (!await context.Users.AnyAsync())
{
    context.Users.Add(new User { Username = "admin", Password = "hidden" });
    context.Users.Add(new User { Username = "user", Password = "passwd" });

    await context.SaveChangesAsync();
}

app.UseHttpsRedirection();

app.MapGet("/login/{username}", (string username) =>
{
    username = username.ToLower().Trim();
    
    if (username.Contains("admin"))
        return Results.Unauthorized();

    var user = context.Users.FirstOrDefault(u => u.Username == username);

    if (user == null)
        return Results.NotFound();

    return Results.Ok(user);
});

app.Run();
