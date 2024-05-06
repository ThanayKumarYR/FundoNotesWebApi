using Dapper;
using ModelLayer.Registration;
using Repository.Context;
using Repository.GlobalExceptions;
using RepositoryLayer.Interface;
using System.Data;
using RepositoryLayer.RegularExpressions;

namespace RepositoryLayer.Services
{
    public class RegistrationServiceRepositoryLayer : IRegistrationRepositoryLayer
    {
        private readonly DapperContext _context;

        public RegistrationServiceRepositoryLayer(DapperContext context)
        {
            _context = context;
        }
        public async Task<bool> RegisterUser(UserRegistrationModel userRegModel)
        {
            RegexValidation regexValidation = new RegexValidation();


            if (!regexValidation.IsValidFirstName(userRegModel.FirstName))
            {
                throw new InvalidFormatException("Invalid First Name format");
            }

            if (!regexValidation.IsValidLastName(userRegModel.LastName))
            {
                throw new InvalidFormatException("Invalid Last Name format");
            }

            if (!regexValidation.IsValidPassword(userRegModel.Password))
            {
                throw new InvalidFormatException("Invalid Password format");
            }

            if (!regexValidation.IsValidEmail(userRegModel.Email))
            {
                throw new InvalidFormatException("Invalid email format");
            }


            var parametersToCheckEmailIsValid = new DynamicParameters();
            parametersToCheckEmailIsValid.Add("Email", userRegModel.Email, DbType.String);

            var querytoCheckEmailIsNotDuplicate = @"SELECT COUNT(*)FROM Users WHERE Email = @Email;";

            var query = @"INSERT INTO Users (FirstName, LastName, Email, Password)VALUES (@FirstName, @LastName, @Email, @Password);";


            var parameters = new DynamicParameters();
            parameters.Add("FirstName", userRegModel.FirstName, DbType.String);
            parameters.Add("LastName", userRegModel.LastName, DbType.String);
            parameters.Add("Email", userRegModel.Email, DbType.String);


            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegModel.Password);

            parameters.Add("Password", hashedPassword, DbType.String);

            using (var connection = _context.CreateConnection())
            {


                bool tableExists = await connection.QueryFirstOrDefaultAsync<bool>(
                    @"
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME = 'Users';
                     "
                );

                if (!tableExists)
                {
                    await connection.ExecuteAsync(
                                                    @" CREATE TABLE Users (
                                                             UserId INT IDENTITY(1, 1) PRIMARY KEY,
                                                             FirstName NVARCHAR(100) NOT NULL,
                                                             LastName NVARCHAR(100) NOT NULL,
                                                             Email NVARCHAR(100) UNIQUE NOT NULL,
                                                             Password NVARCHAR(100) UNIQUE NOT NULL )"
                                                 );
                }

                bool emailExists = await connection.QueryFirstOrDefaultAsync<bool>(querytoCheckEmailIsNotDuplicate, parametersToCheckEmailIsValid);

                if (emailExists)
                {
                    throw new DuplicateEmailExceptions("Email address is already in use");
                }

                await connection.ExecuteAsync(query, parameters);
            }
            return true;
        }
    }
}
