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

        #region 
        // memory cache
        builder.Services.AddMemoryCache(
                    //options => options.SizeLimit = 50
                    );
        #endregion

        #region 
        // response cache
        builder.Services.AddResponseCaching();
        #endregion

        #region
        // distributed cache 
        builder.Services.AddDistributedMemoryCache();
                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = builder.Configuration.GetConnectionString("Redis");
                });
        #endregion

        var app = builder.Build(); 

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        #region 
        // response cache

        app.UseResponseCaching();

        app.Use(async (context, next) =>
        {
            context.Response.GetTypedHeaders().CacheControl =
            new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(10)
            };

            await next();
        });
        
        #endregion

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}