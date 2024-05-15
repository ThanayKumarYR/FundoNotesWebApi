using BussinesLayer.Interface;
using ModelLayer.Label;
using Repository.Entity;
using Repository.Interface;

namespace BussinesLayer.Service
{
    public class LabelRepositoryBusinessLayer : ILabelBusinessLayer
    {
        private readonly ILabelRepositoryLayer ilabel;
        public LabelRepositoryBusinessLayer(ILabelRepositoryLayer label)
        {
            this.ilabel = label;
        }
        public Task CreateLabel(CreateLabel label, int UserId)
        {
            return ilabel.CreateLabel(label, UserId);
        }
        public Task DeleteLabel(int LabelId)
        {
            return ilabel.DeleteLabel(LabelId);
        }
        public Task UpdateLabel(CreateLabel label, int LabelId, int UserId)
        {
            return ilabel.UpdateLabel(label, LabelId, UserId);
        }
        public Task<IEnumerable<LabelEntity>> GetAllLabels()
        {
            return ilabel.GetAllLabels();
        }
        public Task<IEnumerable<object>> GetAllNotesbyLabelId(int LabelId)
        {
            return ilabel.GetAllNotesbyLabelId(LabelId);
        }
    }
}