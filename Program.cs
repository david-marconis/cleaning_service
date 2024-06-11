public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        var app = builder.Build();
        app.Map("/api", api => ConfigureApi(app, api));
        app.Map("/", context =>
        {
            context.Response.Redirect("/api/swagger");
            return Task.CompletedTask;
        });
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
