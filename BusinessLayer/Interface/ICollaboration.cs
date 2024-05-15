using ModelLayer;
using ModelLayer.Collaboration;

namespace BussinesLayer.Interface
{
    public interface ICollaboration
    {
        public Task<bool> AddCollaborator(int noteid, CollaborationRequestModel model, int userId);
        public Task RemoveCollaborator(int CollabId);
        public Task<IEnumerable<CollabInfoModel>> GetCollaboration();


    }
}