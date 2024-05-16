using Dapper;
using ModelLayer;
using ModelLayer.Collaboration;
using Repository.Context;
using Repository.GlobalExceptions;
using Repository.Interface;
using System.Data;
using System.Text.RegularExpressions;
using RepositoryLayer.Interface;

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

        private bool IsValid(string email)
        {
            string pattern = @"^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[a-zA-Z]{3,}$";
            return Regex.IsMatch(email, pattern);
        }

        private async Task EnsureCollaborationTableExistsAsync()
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 0);

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageCollaboration", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> AddCollaborator(int noteId, CollaborationRequestModel model, int userId)
        {
            if (!IsValid(model.Email))
            {
                throw new InvalidFormatException("Invalid Email Format");
            }

            await EnsureCollaborationTableExistsAsync();

            var parameters = new DynamicParameters();
            parameters.Add("Operation", 1);
            parameters.Add("NoteId", noteId, DbType.Int64);
            parameters.Add("UserId", userId, DbType.Int64);
            parameters.Add("CollabEmail", model.Email, DbType.String);

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageCollaboration", parameters, commandType: CommandType.StoredProcedure);
                var emailBody = $"You have been added as a collaborator.";
                await EmailService.SendEmailAsync(model.Email, "Added as Collaborator", emailBody);
            }

            return true;
        }

        public async Task RemoveCollaborator(int collabId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 2);
            parameters.Add("CollabId", collabId, DbType.Int64);

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageCollaboration", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<CollabInfoModel>> GetCollaboration()
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 3);

            using (var connection = _Context.CreateConnection())
            {
                var collabs = await connection.QueryAsync<CollabInfoModel>("ManageCollaboration", parameters, commandType: CommandType.StoredProcedure);
                return collabs.ToList();
            }
        }
    }
}
