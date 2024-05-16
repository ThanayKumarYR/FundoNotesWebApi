using Dapper;
using ModelLayer.Notes;
using Repository.Context;
using System.Data;
using Repository.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

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

        private async Task EnsureNotesTableExistsAsync()
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 0);

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageNotes", parameters, commandType: CommandType.StoredProcedure);
            }
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

        public async Task CreateNote(CreateNoteRequest createNoteRequest, int userId, int labelId)
        {
            await EnsureNotesTableExistsAsync();

            var parameters = new DynamicParameters();
            parameters.Add("Operation", 1);
            parameters.Add("Description", createNoteRequest.Description, DbType.String);
            parameters.Add("Title", createNoteRequest.Title, DbType.String);
            parameters.Add("Colour", createNoteRequest.Colour, DbType.String);
            parameters.Add("IsArchived", createNoteRequest.IsArchived, DbType.Boolean);
            parameters.Add("IsDeleted", createNoteRequest.IsDeleted, DbType.Boolean);
            parameters.Add("UserId", userId, DbType.Int32);
            parameters.Add("LabelId", labelId == default ? (int?)null : labelId, DbType.Int32);

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageNotes", parameters, commandType: CommandType.StoredProcedure);
            }
            await _cache.RemoveAsync($"Notes_{userId}");
            await _cache.RemoveAsync($"ArchivedNotes_{userId}");
            await _cache.RemoveAsync($"DeletedNotes_{userId}");
        }

        public async Task UpdateNote(int noteId, int userId, CreateNoteRequest updatedNote)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 2);
            parameters.Add("NoteId", noteId, DbType.Int32);
            parameters.Add("UserId", userId, DbType.Int32);
            parameters.Add("Description", updatedNote.Description, DbType.String);
            parameters.Add("Title", updatedNote.Title, DbType.String);
            parameters.Add("Colour", updatedNote.Colour, DbType.String);
            parameters.Add("IsArchived", updatedNote.IsArchived, DbType.Boolean);
            parameters.Add("IsDeleted", updatedNote.IsDeleted, DbType.Boolean);

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageNotes", parameters, commandType: CommandType.StoredProcedure);
            }

            await _cache.RemoveAsync($"Notes_{userId}");
            await _cache.RemoveAsync($"ArchivedNotes_{userId}");
            await _cache.RemoveAsync($"DeletedNotes_{userId}");
        }

        public async Task DeleteNote(int noteId, int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 3);
            parameters.Add("NoteId", noteId, DbType.Int32);
            parameters.Add("UserId", userId, DbType.Int32);

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageNotes", parameters, commandType: CommandType.StoredProcedure);
            }

            await _cache.RemoveAsync($"Notes_{userId}");
            await _cache.RemoveAsync($"ArchivedNotes_{userId}");
            await _cache.RemoveAsync($"DeletedNotes_{userId}");

        }

        public async Task<IEnumerable<NoteResponse>> GetAllNoteAsync(int userId)
        {
            string cacheKey = $"Notes_{userId}";
            string cachedNotes = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedNotes))
            {
                return JsonConvert.DeserializeObject<IEnumerable<NoteResponse>>(cachedNotes);
            }

            var parameters = new DynamicParameters();
            parameters.Add("Operation", 4);
            parameters.Add("UserId", userId, DbType.Int32);

            using (var connection = _Context.CreateConnection())
            {
                try
                {
                    var notes = await connection.QueryAsync<NoteResponse>("ManageNotes", parameters, commandType: CommandType.StoredProcedure);
                    var notesList = notes.Reverse().ToList();

                    if (notesList.Any())
                    {
                        await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(notesList));
                        return notesList;
                    }
                    else
                    {
                        throw new ApplicationException("No Notes to display");
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<NoteResponse>> GetAllArchivedNoteAsync(int userId)
        {
            string cacheKey = $"ArchivedNotes_{userId}";
            string cachedNotes = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedNotes))
            {
                return JsonConvert.DeserializeObject<IEnumerable<NoteResponse>>(cachedNotes);
            }

            var parameters = new DynamicParameters();
            parameters.Add("Operation", 5);
            parameters.Add("UserId", userId, DbType.Int32);

            using (var connection = _Context.CreateConnection())
            {
                try
                {
                    var notes = await connection.QueryAsync<NoteResponse>("ManageNotes", parameters, commandType: CommandType.StoredProcedure);
                    var notesList = notes.Reverse().ToList();

                    if (notesList.Any())
                    {
                        await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(notesList));
                        return notesList;
                    }
                    else
                    {
                        throw new ApplicationException("No Notes to display in the archive");
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<NoteResponse>> GetAllDeletedNoteAsync(int userId)
        {
            string cacheKey = $"DeletedNotes_{userId}";
            string cachedNotes = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedNotes))
            {
                return JsonConvert.DeserializeObject<IEnumerable<NoteResponse>>(cachedNotes);
            }

            var parameters = new DynamicParameters();
            parameters.Add("Operation", 6);
            parameters.Add("UserId", userId, DbType.Int32);

            using (var connection = _Context.CreateConnection())
            {
                try
                {
                    var notes = await connection.QueryAsync<NoteResponse>("ManageNotes", parameters, commandType: CommandType.StoredProcedure);
                    var notesList = notes.Reverse().ToList();

                    if (notesList.Any())
                    {
                        await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(notesList));
                        return notesList;
                    }
                    else
                    {
                        throw new ApplicationException("No Notes to display in the Trash");
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
