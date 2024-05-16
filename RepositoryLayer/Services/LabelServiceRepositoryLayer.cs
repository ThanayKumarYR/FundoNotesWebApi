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

        private async Task EnsureLabelTableExistsAsync()
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 0);

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageLabelsAndNotes", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task CreateLabel(CreateLabel label, int userId)
        {
            await EnsureLabelTableExistsAsync();

            var parameters = new DynamicParameters();
            parameters.Add("Operation", 1);
            parameters.Add("LabelName", label.LabelName, DbType.String);
            parameters.Add("UserId", userId, DbType.Int64);

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageLabelsAndNotes", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteLabel(int labelId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 2);
            parameters.Add("LabelId", labelId, DbType.Int64);

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageLabelsAndNotes", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateLabel(CreateLabel label, int labelId, int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 3);
            parameters.Add("LabelId", labelId, DbType.Int64);
            parameters.Add("LabelName", label.LabelName, DbType.String);
            parameters.Add("UserId", userId, DbType.Int64);

            using (var connection = _Context.CreateConnection())
            {
                await connection.ExecuteAsync("ManageLabelsAndNotes", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<LabelEntity>> GetAllLabels()
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 4);

            using (var connection = _Context.CreateConnection())
            {
                var labels = await connection.QueryAsync<LabelEntity>("ManageLabelsAndNotes", parameters, commandType: CommandType.StoredProcedure);
                return labels.ToList();
            }
        }

        public async Task<IEnumerable<object>> GetAllNotesbyLabelId(int LabelId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Operation", 5);
            parameters.Add("LabelId", LabelId, DbType.Int64);

            using (var connection = _Context.CreateConnection())
            {
                var notes = await connection.QueryAsync<object>("ManageLabelsAndNotes", parameters, commandType: CommandType.StoredProcedure);
                return notes.ToList();
            }
        }
    }
}
