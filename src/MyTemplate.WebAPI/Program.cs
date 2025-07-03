using Elastic.Channels;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using Serilog;
using Serilog.Events;
using Elastic.Transport;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Setup Serilog entirely in code
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()  // Optional: Write logs to console as well
    .WriteTo.Elasticsearch(new[] { new Uri("http://localhost:9200") }, opts =>
    {
        opts.DataStream = new DataStreamName("logs", "console-example", "demo");
        opts.BootstrapMethod = BootstrapMethod.Failure;
    }, transport =>
    {
        transport.Authentication(new BasicAuthentication("username", "password"));
        // transport.Authentication(new ApiKey(base64EncodedApiKey));
    })
    .CreateLogger();


// Setup OpenTelemetry Tracing with Console Exporter
services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("MyAspNetCoreApp", serviceVersion: "1.0.0"))
            .AddAspNetCoreInstrumentation()    // ✅ Captures incoming HTTP request spans
            .AddHttpClientInstrumentation()   // ✅ Captures outgoing HTTP calls
            .AddConsoleExporter();            // ✅ Export spans to console
    });


builder.Host.UseSerilog();

#if mysql
services.AddTransient<IUserRepository, MySqlUserRepository>();
#endif

#if postgres
services.AddTransient<IUserRepository, PostgresUserRepository>();
#endif


#if rabbit
services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();
#endif

#if kafka
services.AddSingleton<IKafkaProducer, KafkaProducer>();
services.AddScoped<IMessagePublisher, KafkaMessagePublisher>();
#endif


var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();
