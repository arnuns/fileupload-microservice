using FileUploadService.Entities;
using FileUploadService.Entities.DbContexts;
using FileUploadService.Entities.Repositories;
using FileUploadService.Models;
using FileUploadService.Middlewares;
using FileUploadService.Services;
using FileUploadService.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Setup Serilog configuration
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions<FileSettings>().BindConfiguration("FileSettings");

var connectionString = Environment.GetEnvironmentVariable("PSQL_CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string was not found.");
builder.Services.AddDbContext<FileUploadContext>(opt => opt.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

builder.Services.Configure<FileSettings>(builder.Configuration.GetSection("FileSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new UtcPlus7Converter());
    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.TryAddScoped<IFileStorageService, FileStorageService>();
builder.Services.TryAddTransient<IEmailService, EmailService>();

builder.Services.TryAddScoped<IFileUploadRepository, FileUploadRepository>();
builder.Services.TryAddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();
