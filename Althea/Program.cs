using Althea.Infrastructure.DependencyInjection;

using Alva.Toolkit.AspNetCore.Extensions;
using Alva.Toolkit.AspNetCore.Wrapper;

var builder       = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ModelValidFilter>();
}).AddJsonOptions(options =>
{
    options.ConfigureDefaultOptions();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeAllXmlComments();
});

builder.Services.AddTransient(typeof(Lazy<>));
builder.Services.AddHttpContextAccessor();
builder.Services.AddCorsSetting(configuration);
builder.Services.AddJwtBearer(configuration);

builder.Services.AddByLifeScope("Althea");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseBasicException();
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
