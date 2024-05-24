using Dapper;
using ModelLayer.User;
using Repository.Context;
using Repository.GlobalExceptions;
using RepositoryLayer.Interface;
using System.Data;
using RepositoryLayer.RegularExpressions;
using Repository.Entity;
using Repository.Interface;
using Microsoft.Data.SqlClient;
using Confluent.Kafka;
using RepositoryLayer.Helper;

namespace RepositoryLayer.Services
{
    public class UserServiceRepositoryLayer : IRegistrationRepositoryLayer
    {
        private readonly DapperContext _context;
        private readonly IAuthServiceRepositoryLayer _authService;
        private readonly RegexValidation _regexValidation;
        public UserServiceRepositoryLayer(DapperContext context, IAuthServiceRepositoryLayer authService, IEmailRepositoryLayer email)
        {
            _context = context;
            _authService = authService;
            _regexValidation = new RegexValidation();
        }

        private async Task EnsureTablesExistAsync()
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 1);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageUserOperations", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> RegisterUser(UserRegistrationModel userRegModel)
        {

            if (!_regexValidation.IsValidFirstName(userRegModel.FirstName))
            {
                throw new InvalidFormatException("Invalid First Name format");
            }

            if (!_regexValidation.IsValidLastName(userRegModel.LastName))
            {
                throw new InvalidFormatException("Invalid Last Name format");
            }

            if (!_regexValidation.IsValidPassword(userRegModel.Password))
            {
                throw new InvalidFormatException("Invalid Password format");
            }

            if (!_regexValidation.IsValidEmail(userRegModel.Email))
            {
                throw new InvalidFormatException("Invalid email format");
            }

            var parameters = new DynamicParameters();
            parameters.Add("Operation", 2);
            parameters.Add("FirstName", userRegModel.FirstName, DbType.String);
            parameters.Add("LastName", userRegModel.LastName, DbType.String);
            parameters.Add("Email", userRegModel.Email, DbType.String);


            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegModel.Password);

            parameters.Add("Password", hashedPassword, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await EnsureTablesExistAsync();

                    await connection.ExecuteAsync("ManageUserOperations", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 50000 && ex.Message.Contains("Duplicate Email"))
                    {
                        throw new DuplicateEmailExceptions("Email address is already in use");
                    }
                    throw;
                }
            }
            var registrationDetailsForPublishing = new RegistrationDetailsForPublishing(userRegModel);

            // Serialize registration details to a JSON string
            var registrationDetailsJson = Newtonsoft.Json.JsonConvert.SerializeObject(registrationDetailsForPublishing);

            // Get Kafka producer configuration
            var producerConfig = KafkaProducerConfig.GetProducerConfig();

            // Create a Kafka producer
            using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                try
                {
                    // Publish registration details to Kafka topic
                    await producer.ProduceAsync("Registration-topic", new Message<Null, string> { Value = registrationDetailsJson });
                    Console.WriteLine("Registration details published to Kafka topic.");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Failed to publish registration details to Kafka topic: {e.Error.Reason}");
                }
            }

            var consumerConfig = KafkaConsumerConfig.GetConsumerConfig();

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
            return true;
        }
        public async Task<string> UserLogin(UserLoginModel userLogin)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 3);
            parameters.Add("Email", userLogin.Email, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QueryFirstOrDefaultAsync<UserEntity>("ManageUserOperations", parameters, commandType: CommandType.StoredProcedure);

                if (user == null)
                {
                    throw new NotFoundException($"User with email '{userLogin.Email}' not found.");
                }

                if (!BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
                {
                    throw new InvalidPasswordException($"User with Password '{userLogin.Password}' not Found.");
                }

                //if password enterd from user and password in db match then generate Token 
                var token = _authService.GenerateJwtToken(user);
                return token;
            }
        }
        public async Task UpdatePassword(string email, string oldPassword, string newPassword)
        {
            var getUserParameters =  new DynamicParameters();
            getUserParameters.Add("Operation", 3);
            getUserParameters.Add("Email", email, DbType.String);

            var parameters = new DynamicParameters();
            parameters.Add("Operation", 4);
            parameters.Add("Email", email, DbType.String);
            parameters.Add("Password", oldPassword, DbType.String);
            parameters.Add("NewPassword", newPassword, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QueryFirstOrDefaultAsync<UserEntity>("ManageUserOperations", getUserParameters, commandType: CommandType.StoredProcedure);

                if (user == null)
                {
                    throw new NotFoundException($"User with email '{email}' not found.");
                }
                else if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
                {
                    throw new InvalidPasswordException($"Can't update, Current Password '{oldPassword}' is Invalid");
                }
                else if (!_regexValidation.IsValidPassword(newPassword))
                {
                    throw new InvalidFormatException("Format is invalid in the new Password !");
                }
                else 
                {
                    await connection.ExecuteAsync("ManageUserOperations", parameters, commandType: CommandType.StoredProcedure);
                }
            }
        }
        public async Task<UserEntity> GetByEmailAsync(string email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 3);
            parameters.Add("Email", email, DbType.String);

            using (var connection = _context.CreateConnection())
            {

                var user =  await connection.QueryFirstOrDefaultAsync<UserEntity>("ManageUserOperations", parameters, commandType: CommandType.StoredProcedure);
                return user;
            }
        }
        public async Task<int> UpdatePasswordByOtp(string mailid, string password)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 5);
            parameters.Add("Email", mailid, DbType.String);
            parameters.Add("NewPassword", password, DbType.String);
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync("ManageUserOperations", parameters, commandType: CommandType.StoredProcedure);
            }
        }

    }
}
