internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddLogging();
        builder.Services.AddSwaggerGen();

        // memory cache        
        builder.Services.AddMemoryCache(
            //optins => optins.SizeLimit = 50
            );

        // response cache
        builder.Services.AddResponseCaching();

        // distributed cache
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        //app.UseResponseCaching();

        //app.Use(async (context, next) =>
        //{
        //    context.Response.GetTypedHeaders().CacheControl =
        //        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
        //        {
        //            Public = true,
        //            MaxAge = TimeSpan.FromSeconds(10)
        //        };
        //    await next();
        //});

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}