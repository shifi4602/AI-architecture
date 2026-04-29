using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace WebApiShop.MiddleWare
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddFixedWindowRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("fixed", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 10;
                    limiterOptions.Window = TimeSpan.FromSeconds(10);
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiterOptions.QueueLimit = 2;
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }

        public static IApplicationBuilder UseFixedWindowRateLimiting(this IApplicationBuilder app)
        {
            return app.UseRateLimiter();
        }
    }
}
