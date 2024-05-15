using ModelLayer.Label;
using Repository.Entity;

namespace Repository.Interface
{
    public interface ILabelRepositoryLayer
    {
        public Task CreateLabel(CreateLabel label, int UserId);
        public Task DeleteLabel(int LabelId);
        public Task UpdateLabel(CreateLabel label, int LabelId, int UserId);
        public Task<IEnumerable<LabelEntity>> GetAllLabels();
        public Task<IEnumerable<object>> GetAllNotesbyLabelId(int LabelId);

    }
}