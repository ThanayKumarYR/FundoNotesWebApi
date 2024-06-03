using Confluent.Kafka;
using ModelLayer.User;
using Repository.GlobalExceptions;

namespace RepositoryLayer.Helper
{
    public class KafkaConsumer
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

        public static async void consumeTopic(ConsumerConfig consumerConfig)
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                consumer.Subscribe("Registration-topic"); // Subscribe to Kafka topic
                try
                {
                    var consumeResult = consumer.Consume(); // Consume message
                    Console.WriteLine($"Consumed message: {consumeResult.Message.Value}");

                    var registrationDetailsObject = Newtonsoft.Json.JsonConvert.DeserializeObject<UserRegistrationModel>(consumeResult.Message.Value);

                    string Email = registrationDetailsObject.Email;
                    string subject = $"Hello {registrationDetailsObject.FirstName}, Welcome to Fundo Notes.";
                    string message = $"You have registered to Fundo Notes with name as {registrationDetailsObject.FirstName} {registrationDetailsObject.LastName} and email id as {registrationDetailsObject.Email}";

                    MailSender.sendMail(Email, subject, message);

                }
                catch (ConsumeException e)
                {
                    throw new KafkaConsumerException(e.Message);
                }
            }
        }

    }
}
