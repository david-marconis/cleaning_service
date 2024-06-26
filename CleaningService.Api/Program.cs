namespace CleaningService.Api;

using CleaningService.Api.Data;
using CleaningService.Api.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton(sp =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var connectionStringOpt = configuration.GetValue<String>("ConnectionString");
                    var connectionString = connectionStringOpt
			?? throw new ArgumentNullException("Unable to load connection string from config");
                    return new Database(sp.GetRequiredService<ILogger<Database>>(), connectionString);
                });
        builder.Services.AddSingleton<NotificationService>();

        var app = builder.Build();
        app.Map("/api", api => ConfigureApi(app, api));
        app.Map("/", context =>
        {
            context.Response.Redirect("/api/swagger");
            return Task.CompletedTask;
        });
        app.Services.GetRequiredService<NotificationService>().StartListening();
        app.Run();
    }

    private static void ConfigureApi(WebApplication app, IApplicationBuilder api)
    {
        if (app.Environment.IsDevelopment())
        {
            api.UseSwagger();
            api.UseSwaggerUI();
            api.UseDeveloperExceptionPage();
        }

        api.UseHttpsRedirection();
        api.UseAuthorization();
        api.UseRouting();
        api.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
