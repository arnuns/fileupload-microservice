using Amazon.S3;
using Amazon.Runtime;
using Amazon;
using KafkaWorkerService;
using KafkaWorkerService.Entities;
using KafkaWorkerService.Entities.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
.Enrich.FromLogContext()
.WriteTo.Console()
.CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<EmailSettings>(hostContext.Configuration.GetSection("EmailSettings"));
        services.Configure<KafkaSettings>(hostContext.Configuration.GetSection("KafkaSettings"));
        services.Configure<AWSSettings>(hostContext.Configuration.GetSection("AWSSettings"));

        var connectionString = Environment.GetEnvironmentVariable("PSQL_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string was not found.");
        services.AddDbContextFactory<FileUploadContext>(options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        var awsEnvVars = new Dictionary<string, string>
        {
            ["AWS_ACCESS_KEY"] = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY")!,
            ["AWS_SECRET_KEY"] = Environment.GetEnvironmentVariable("AWS_SECRET_KEY")!,
            ["S3_REGION_ENDPOINT"] = Environment.GetEnvironmentVariable("S3_REGION_ENDPOINT")!
        };

        if (awsEnvVars.Any(v => string.IsNullOrEmpty(v.Value)))
        {
            var missingVars = awsEnvVars.Where(v => string.IsNullOrEmpty(v.Value))
                                     .Select(v => v.Key)
                                     .ToList();
            throw new InvalidOperationException($"The following AWS environment variables are missing or empty: {string.Join(", ", missingVars)}");
        }

        var awsCredentials = new BasicAWSCredentials(awsEnvVars["AWS_ACCESS_KEY"], awsEnvVars["AWS_SECRET_KEY"]);
        var s3Config = new AmazonS3Config { RegionEndpoint = RegionEndpoint.GetBySystemName(awsEnvVars["S3_REGION_ENDPOINT"]) };
        services.TryAddSingleton<IAmazonS3>(sp => new AmazonS3Client(awsCredentials, s3Config));

        services.TryAddTransient<IEmailService, EmailService>();

        services.AddHostedService<KafkaFileUploadBackgroundService>();
    })
    .Build();

try
{
    await host.RunAsync();
}
finally
{
    Log.CloseAndFlush();
}
