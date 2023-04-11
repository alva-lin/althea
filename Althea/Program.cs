using Althea;
using Althea.Controllers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OpenAI.GPT3.Extensions;
using Serilog;

Environment.CurrentDirectory = AppContext.BaseDirectory;

var originConfiguration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(path: "appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
        true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(originConfiguration)
    .CreateBootstrapLogger();

try
{
    Log.Information("Application starting up...");

    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    builder.Host.UseSerilog(
        (context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
    );

    // Add services to the container.
    builder.Services.AddControllers(options => { options.Filters.Add<ModelValidFilter>(); }).AddJsonOptions(options =>
    {
        JsonExtension.SetDefaultJsonSerializerOptions(options.JsonSerializerOptions);
    });
    builder.Services.AddSignalR().AddJsonProtocol(options =>
    {
        JsonExtension.SetDefaultJsonSerializerOptions(options.PayloadSerializerOptions);
    });
    builder.Services.Configure<HubOptions>(options =>
    {
        // configure hub options
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options => { options.IncludeAllXmlComments(); });

    builder.Services.AddTransient(typeof(Lazy<>));
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddCorsSetting(configuration);
    builder.Services.AddJwtBearer(configuration);

    builder.Services.AddByLifeScope("Althea");

    builder.Services.AddScoped<IAuthInfoProvider, JwtAuthInfoProvider>();
    builder.Services.AddDbContext<AltheaDbContext>(optionsBuilder =>
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Althea")));

    builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(AltheaDbContext).Assembly);

    builder.Services.Configure<SystemOption>(configuration.GetSection(nameof(SystemOption)));

    builder.Services.AddOpenAIService(options => { options.ApiKey = configuration["OpenAI:ApiKey"]!; });

    var app = builder.Build();

    // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
    Log.Information($"running in {app.Environment.EnvironmentName} environment");

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseBasicException();
    app.UseCors();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseJwtParsing();

    app.MapControllers();
    app.MapHub<ChatHub>("/hub/chat");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Application terminated gracefully");
    Log.CloseAndFlush();
}
