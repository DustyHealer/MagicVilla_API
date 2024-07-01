
using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add DbContext to the DI Container
builder.Services.AddDbContext<ApplicationDbContext>(options => 
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

// Register automapper
builder.Services.AddAutoMapper(typeof(MappingConfig));

// Log using serilog - Confifuration for serilog
//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
//    .WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();
//builder.Host.UseSerilog();

// Json support so that we can patch the json
builder.Services.AddControllers(option =>
{
    //option.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Custom Logging
//builder.Services.AddSingleton<ILogging, Logging>(); // Singleton is used for logging

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
