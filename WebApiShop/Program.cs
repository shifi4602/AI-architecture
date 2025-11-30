using Enteties.Controllers;
using Repositories;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure the HTTP request pipeline.
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddScoped<IpasswordServices, passwordServices>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();


