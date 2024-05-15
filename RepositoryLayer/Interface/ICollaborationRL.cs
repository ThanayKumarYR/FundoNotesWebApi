using ModelLayer;
using ModelLayer.Collaboration;

namespace Repository.Interface
{
    public interface ICollaborationRL
    {
        public Task<bool> AddCollaborator(int noteid, CollaborationRequestModel model, int userId);
        public Task RemoveCollaborator(int CollabId);
        public Task<IEnumerable<CollabInfoModel>> GetCollaboration();
    }
}