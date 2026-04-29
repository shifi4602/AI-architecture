using Enteties.Controllers;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;
using WebApiShop.MiddleWare;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure the HTTP request pipeline.
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<IProductReposetory, ProductReposetory>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IpasswordServices, passwordServices>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProtuctService, ProtuctService>();
builder.Services.AddScoped<IRatingService, RatingService>();

builder.Services.AddDbContext<ApiShopContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("School")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

var redisConnectionString = builder.Configuration["RedisCache:ConnectionString"] ?? "localhost:6379,password=S9v#3mL@8qT!2xN$7rKpZ4dH,abortConnect=false";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "WebApiShop:";
});

builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(
    StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddFixedWindowRateLimiting();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
    });
}

app.UseHttpsRedirection();

app.UseFixedWindowRateLimiting();

app.UseErrorHandling();

app.UseRating();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers().RequireRateLimiting("fixed");

app.Run();


