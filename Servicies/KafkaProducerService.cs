using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Configuration;

namespace Services
{
    public class KafkaProducerService : IKafkaProducerService, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly string _topic;
        private bool _disposed;

        public KafkaProducerService(IOptions<KafkaOptions> options, ILogger<KafkaProducerService> logger)
        {
            _logger = logger;
            KafkaOptions kafkaOptions = options.Value;

            if (string.IsNullOrWhiteSpace(kafkaOptions.BootstrapServers))
                throw new ArgumentException("Kafka:BootstrapServers must be configured.", nameof(options));

            if (string.IsNullOrWhiteSpace(kafkaOptions.Topic))
                throw new ArgumentException("Kafka:Topic must be configured.", nameof(options));

            _topic = kafkaOptions.Topic;
            var config = new ProducerConfig
            {
                BootstrapServers = kafkaOptions.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public Task ProduceAsync(string message, CancellationToken cancellationToken = default)
        {
            string key = Guid.NewGuid().ToString("N");
            return ProduceAsync(key, message, cancellationToken);
        }

        public async Task ProduceAsync(string key, string message, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.", nameof(message));

            try
            {
                DeliveryResult<string, string> result = await _producer.ProduceAsync(
                    _topic,
                    new Message<string, string> { Key = key, Value = message },
                    cancellationToken);

                _logger.LogInformation(
                    "Kafka message delivered. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                    result.Topic,
                    result.Partition.Value,
                    result.Offset.Value);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Failed to deliver Kafka message to topic {Topic}", _topic);
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
            _disposed = true;
        }
    }
}