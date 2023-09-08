using FileUploadService.Entities;
using FileUploadService.Entities.DbContexts;
using FileUploadService.Entities.Repositories;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("PSQL_CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string was not found.");
builder.Services.AddDbContext<FileUploadContext>(opt => opt.UseNpgsql(connectionString));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapControllers();

app.Run();
