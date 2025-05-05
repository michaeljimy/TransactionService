using Confluent.Kafka;
using Transaction_Service.Data.Entities;
using Transaction_Service.Interfaces;
using Transaction_Service.Models.DTOs.Requests;

namespace Transaction_Service.Services
{
    public class KafkaServices : IServiceKafka
    {
        private readonly ILogger<KafkaServices> _logger;
        private readonly ProducerConfig _producerConfig;

        public KafkaServices(ILogger<KafkaServices> logger, ProducerConfig producerConfig)
        {
            _logger = logger;
            _producerConfig = producerConfig;
        }

        public async Task<string> ProduceAsync(string topic, string message)
        {
            var results = string.Empty;

            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                try
                {
                    var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                    _logger.LogInformation($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
                    results = $"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'";
                }
                catch (ProduceException<Null, string> e)
                {
                    _logger.LogError($"Delivery failed: {e.Error.Reason}");
                    results = $"Delivery failed: {e.Error.Reason}";
                }
            }
            return results;
        }

        public void UpdateWalletAsync(string topic, WalletUpdateRequest request)
        {
            var results = string.Empty;

            using (var producer = new ProducerBuilder<Null, WalletUpdateRequest>(_producerConfig)
                                         .SetValueSerializer(new WalletUpdateRequestSerializer())
                                         .Build())
            {
                try
                {
                    producer.Produce(topic,
                        new Message<Null, WalletUpdateRequest> { Value = request });
                }
                catch (ProduceException<Null, WalletUpdateRequest> e)
                {
                    _logger.LogError($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
    }

}
