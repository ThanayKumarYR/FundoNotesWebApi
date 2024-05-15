using Dapper;
using ModelLayer.Notes;
using Repository.Context;
using System.Data;
using System.Text;
using Repository.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Data.SqlClient;

namespace Repository.Service
{
    public class NoteServiceRepositoryLayer : INotesRepositoryLayer
    {
        private readonly DapperContext _Context;
        private readonly IDistributedCache _cache;

        public NoteServiceRepositoryLayer(DapperContext context, IDistributedCache cache)
        {
            _Context = context;
            _cache = cache;
        }
        public bool RemoveData(string key)
        {

            var cachedData = _cache.GetString(key);
            if (cachedData != null)
            {
                _cache.Remove(key);
                return true;
            }
            return false;
        }

        public async Task CreateNote(CreateNoteRequest createNoteRequest, int Userid,int LabelId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("Description", createNoteRequest.Description, DbType.String);
            parameter.Add("Title", createNoteRequest.Title, DbType.String);
            parameter.Add("Colour", createNoteRequest.Colour, DbType.String);
            parameter.Add("IsArchived", createNoteRequest.IsArchived, DbType.Boolean);
            parameter.Add("IsDeleted", createNoteRequest.IsDeleted, DbType.Boolean);
            parameter.Add("UserId", Userid, DbType.Int64);
            if (LabelId == default)
            {
                parameter.Add("LabelId", null);
            }
            else
            {
                parameter.Add("LabelId", LabelId, DbType.Int64);
            }

            var insertquery = @"INSERT INTO Notes (Description, [Title], Colour, IsArchived, IsDeleted, UserId,LabelId) VALUES (@Description, @Title, @Colour, @IsArchived, @IsDeleted, @UserId, @LabelId);";

            using (var connection = _Context.CreateConnection())
            {
                bool tableExists = await connection.QueryFirstOrDefaultAsync<bool>(
                    @"
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME = 'Notes';
                    "
                );

                if (!tableExists)
                {
                    await connection.ExecuteAsync(@" CREATE TABLE Notes (
                                                    NoteId INT IDENTITY(1, 1) PRIMARY KEY,
                                                    Title NVARCHAR(MAX) NOT NULL,
                                                    Description NVARCHAR(MAX) NOT NULL,
                                                    Colour NVARCHAR(MAX) NOT NULL,
                                                    IsArchived BIT NOT NULL,
                                                    IsDeleted BIT NOT NULL,
                                                    UserId INT FOREIGN KEY REFERENCES Users(UserId) NOT NULL,
                                                    LabelId INT FOREIGN KEY REFERENCES Label(LabelId)
                                                  );"
                    );
                }
                await connection.ExecuteAsync(insertquery, parameter);

            }
            await _cache.RemoveAsync($"Notes_{Userid}");
        }

        public async Task UpdateNote(int NoteId, int UserId, CreateNoteRequest updatedNote)
        {
            var parameter = new DynamicParameters();
            parameter.Add("Description", updatedNote.Description, DbType.String);
            parameter.Add("Title", updatedNote.Title, DbType.String);
            parameter.Add("Colour", updatedNote.Colour, DbType.String);
            parameter.Add("IsArchived", updatedNote.IsArchived, DbType.Boolean);
            parameter.Add("IsDeleted", updatedNote.IsDeleted, DbType.Boolean);
            parameter.Add("NoteId", NoteId, DbType.Int64);
            parameter.Add("UserId", UserId, DbType.Int64);
            var query = "UPDATE Notes SET Description=@Description,Title=@Title,Colour= @Colour,IsArchived=@IsArchived,IsDeleted=@Isdeleted WHERE UserId=@UserId AND NoteId = @NoteId";
            using (var connection = _Context.CreateConnection())
            {

                var note = await connection.ExecuteAsync(query, parameter);

            }
            await _cache.RemoveAsync($"Notes_{UserId}");


        }

        public async Task DeleteNote(int noteId, int userId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("Noteid", noteId, DbType.Int64);
            parameter.Add("userid", userId, DbType.Int64);
            var query = "DELETE FROM Notes where NoteId=@noteId and UserId = @userId;";
            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);

            }

            await _cache.RemoveAsync($"Notes_{userId}");

        }
        public async Task<IEnumerable<NoteResponse>> GetAllNoteAsync(int userId)
        {
            var selectQuery = "SELECT * FROM Notes WHERE UserId = @UserId AND IsDeleted = 0 AND IsArchived = 0 AND LabelId is null";

            using (var connection = _Context.CreateConnection())
            {
                try
                {
                    var notes = await connection.QueryAsync<NoteResponse>(selectQuery, new { UserId = userId });
                    var notesList = notes.Reverse().ToList();
                    if (notesList.Count() != 0) return notesList;
                    else throw new ApplicationException("No Notes to display");
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        public async Task<IEnumerable<NoteResponse>> GetAllArchivedNoteAsync(int userId)
        {
            var selectQuery = "SELECT * FROM Notes WHERE UserId = @UserId AND IsDeleted = 0 AND IsArchived = 1";


            using (var connection = _Context.CreateConnection())
            {
                try
                {
                    var notes = await connection.QueryAsync<NoteResponse>(selectQuery, new { UserId = userId });
                    var notesList = notes.Reverse().ToList();
                    if (notesList.Count() != 0) return notesList;
                    else throw new ApplicationException("No Notes to display in the archive");
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        public async Task<IEnumerable<NoteResponse>> GetAllDeletedNoteAsync(int userId)
        {
            var selectQuery = "SELECT * FROM Notes WHERE UserId = @UserId AND IsDeleted = 1";

            using (var connection = _Context.CreateConnection())
            {
                try
                {
                    var notes = await connection.QueryAsync<NoteResponse>(selectQuery, new { UserId = userId });
                    var notesList = notes.Reverse().ToList();
                    if (notesList.Count() != 0) return notesList;
                    else throw new ApplicationException("No Notes to display in the Trash");
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}