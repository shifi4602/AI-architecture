using Confluent.Kafka;
using LogConsumerServer.Configuration;
using Microsoft.Extensions.Options;

namespace LogConsumerServer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly KafkaOptions _kafkaOptions;

    public Worker(ILogger<Worker> logger, IOptions<KafkaOptions> kafkaOptions)
    {
        _logger = logger;
        _kafkaOptions = kafkaOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(_kafkaOptions.BootstrapServers))
            throw new InvalidOperationException("Kafka:BootstrapServers is required.");

        if (string.IsNullOrWhiteSpace(_kafkaOptions.Topic))
            throw new InvalidOperationException("Kafka:Topic is required.");

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _kafkaOptions.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
            AllowAutoCreateTopics = true
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_kafkaOptions.Topic);

        _logger.LogInformation(
            "Kafka consumer started. Topic: {Topic}, GroupId: {GroupId}, BootstrapServers: {BootstrapServers}",
            _kafkaOptions.Topic,
            _kafkaOptions.GroupId,
            _kafkaOptions.BootstrapServers);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    if (consumeResult?.Message is null)
                        continue;

                    _logger.LogInformation(
                        "Kafka message received. Partition: {Partition}, Offset: {Offset}, Value: {Value}",
                        consumeResult.Partition.Value,
                        consumeResult.Offset.Value,
                        consumeResult.Message.Value);
                }
                catch (ConsumeException ex)
                    when (ex.Error.Code == ErrorCode.UnknownTopicOrPart)
                {
                    _logger.LogWarning(
                        "Topic '{Topic}' not yet available. Retrying in 5 seconds...",
                        _kafkaOptions.Topic);
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer stopping.");
        }
        finally
        {
            consumer.Close();
        }
    }
}
