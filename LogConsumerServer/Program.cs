using LogConsumerServer;
using LogConsumerServer.Configuration;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<KafkaOptions>(
	builder.Configuration.GetSection(KafkaOptions.SectionName));
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
