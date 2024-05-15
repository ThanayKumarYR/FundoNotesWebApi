using Dapper;
using ModelLayer.Label;
using Repository.Context;
using Repository.Entity;
using Repository.Interface;
using System.Data;

namespace Repository.Service
{
    public class LabelServiceRepositoryLayer : ILabelRepositoryLayer
    {
        private readonly DapperContext _Context;
        public LabelServiceRepositoryLayer(DapperContext context)
        {
            _Context = context;
        }
        public async Task CreateLabel(CreateLabel label, int UserId)
        {

            var parameter = new DynamicParameters();
            parameter.Add("LabelName", label.LabelName, DbType.String);
            parameter.Add("UserId", UserId, DbType.Int64);

            var query = "INSERT INTO Label (LabelName,UserId) VALUES (@LabelName,@UserId);";

            using (var connection = _Context.CreateConnection())
            {
                bool tableExists = await connection.QueryFirstOrDefaultAsync<bool>(
                    @"
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME = 'Label';
                     "
                );

                if (!tableExists)
                {
                    await connection.ExecuteAsync(@"CREATE TABLE Label(
                                                    LabelId INT IDENTITY(1, 1) PRIMARY KEY,
                                                    LabelName NVARCHAR(MAX) NOT NULL,
                                                    UserId INT FOREIGN KEY REFERENCES Users(UserId) NOT NULL
                                                );"
                    );
                }
                await connection.ExecuteAsync(query, parameter);
            }
        }
        public async Task DeleteLabel(int LabelId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("LabelId", LabelId, DbType.Int64);

            var delete_notes_query = "DELETE FROM Notes WHERE LabelId = @LabelId;";

            var query = "DELETE FROM Label WHERE LabelId = @LabelId;";

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync(delete_notes_query, parameter);
                await connection.ExecuteAsync(query, parameter);
            }
        }
        public async Task UpdateLabel(CreateLabel label, int LabelId, int UserId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("LabelId", LabelId, DbType.Int64);
            parameter.Add("LabelName", label.LabelName, DbType.String);
            parameter.Add("UserId", UserId, DbType.Int64);

            var query = "UPDATE Label SET LabelName =@LabelName,UserId = @UserId WHERE LabelId = @LabelId;";

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
            }
        }
        public async Task<IEnumerable<LabelEntity>> GetAllLabels()
        {


            var query = "SELECT * FROM Label;";

            using (var connection = _Context.CreateConnection())
            {
                var Label = await connection.QueryAsync<LabelEntity>(query);
                return Label.ToList();
            }
        }
        public async Task<IEnumerable<object>> GetAllNotesbyLabelId(int LabelId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("LabelId", LabelId, DbType.Int64);

            var query = "SELECT n.NoteId,n.Title AS Notestitle,n.Description AS NotesDescription,n.Colour,l.LabelId,l.LabelName FROM Notes n INNER JOIN Label l ON n.LabelId = l.LabelId WHERE n.IsArchived = 0 AND n.IsDeleted = 0;";
            using (var connection = _Context.CreateConnection())
            {
                var Label = await connection.QueryAsync<object>(query, parameter);
                return Label.ToList();
            }
        }

    }
}