using Email.Application;
using Email.Infrastructure;
using Email.Worker;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger, dispose: true);

builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<EmailQueueWorker>();

var otlpEndpoint = builder.Configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://otel-collector:4317";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Email.Worker"))
    .WithMetrics(metrics =>
    {
        metrics.AddRuntimeInstrumentation()
               .AddMeter("EmailMicroservice.Email")
               .AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
    })
    .WithTracing(tracing =>
    {
        tracing.AddHttpClientInstrumentation()
               .AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
    });

builder.Services.AddHealthChecks();

var host = builder.Build();
host.Run();
