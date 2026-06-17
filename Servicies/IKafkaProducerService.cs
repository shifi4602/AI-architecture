namespace Services
{
    public interface IKafkaProducerService
    {
        Task ProduceAsync(string message, CancellationToken cancellationToken = default);

        Task ProduceAsync(string key, string message, CancellationToken cancellationToken = default);
    }
}