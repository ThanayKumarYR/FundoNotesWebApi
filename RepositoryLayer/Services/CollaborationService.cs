using Dapper;
using ModelLayer;
using ModelLayer.Collaboration;
using Repository.Context;
using Repository.GlobalExceptions;
using Repository.Interface;
using RepositoryLayer.Interface;
using System.Data;
using System.Text.RegularExpressions;
namespace Repository.Service
{
    public class CollaborationService : ICollaborationRL
    {
        private readonly DapperContext _Context;
        private readonly IEmailRepositoryLayer EmailService;

        public CollaborationService(DapperContext context, IEmailRepositoryLayer emailService)
        {
            _Context = context;
            EmailService = emailService;
        }
        private bool Isvalid(string email)
        {
            string pattern = @"^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[a-zA-Z]{3,}$";
            return Regex.IsMatch(email, pattern);
        }
        public async Task<bool> AddCollaborator(int noteid, CollaborationRequestModel model, int userId)
        {

            var parameter = new DynamicParameters();

            parameter.Add("Noteid", noteid, DbType.Int64);
            parameter.Add("Userid", userId, DbType.Int64);

            if (!Isvalid(model.Email))
            {
                throw new InvalidFormatException("Invalid Email Format");
            }
            parameter.Add("CollabEmail", model.Email, DbType.String);

            var query = @"INSERT INTO Collaboration(UserId,NoteId,CollabEmail) VALUES (@UserId,@NoteId,@CollabEmail);";

            var emailexistquery = "SELECT COUNT(*) FROM Users where Email = @CollabEmail;";

            var emailExistparameter = new { CollabEmail = model.Email };
            using (var connection = _Context.CreateConnection())
            {
                int emailcount = await connection.ExecuteAsync(emailexistquery, emailExistparameter);
                if (emailcount == 0)
                {
                    throw new NotFoundException($"Collaborator with the Email '{model.Email}'is not registered user");
                }
                bool tableexist = await connection.QueryFirstOrDefaultAsync<bool>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Collaboration';");
                if (!tableexist)
                {

                    await connection.ExecuteAsync("CREATE TABLE Collaboration(CollaborationId int identity(1,1) primary key,UserId int FOREIGN KEY REFERENCES Users(UserId),NoteId int FOREIGN KEY REFERENCES Notes(NoteId),CollabEmail nvarchar(100));");
                }
                else
                {
                    await connection.ExecuteAsync(query, parameter);
                    var emailBody = $"You have been added as a collaborator.";
                    await EmailService.SendEmailAsync(model.Email, "Added as Collaborator", emailBody);
                }

            }
            return true;

        }
        public async Task RemoveCollaborator(int CollabId)
        {
            try
            {

                var parameter = new DynamicParameters();
                parameter.Add("Collabid", CollabId, DbType.Int64);

                var query = "DELETE FROM Collaboration WHERE CollaborationId = @Collabid;";
                using (var connection = _Context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameter);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task<IEnumerable<CollabInfoModel>> GetCollaboration()
        {
            var query = "select * from Collaboration";

            using (var connection = _Context.CreateConnection())
            {
                var collab = await connection.QueryAsync<CollabInfoModel>(query);
                return collab;
            }
        }


    }
}