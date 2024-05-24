using Confluent.Kafka;

namespace RepositoryLayer.Helper
{
    public class KafkaConsumerConfig
    {
        public static ConsumerConfig GetConsumerConfig()
        {
            return new ConsumerConfig
            {
                BootstrapServers = "localhost:9092", // Kafka broker(s) address
                GroupId = "my-consumer-group", // Consumer group ID
                AutoOffsetReset = AutoOffsetReset.Earliest // Reset offset to the earliest message in case no offset is committed
            };
        }

    }
}
