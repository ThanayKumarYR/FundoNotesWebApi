using ModelLayer;
using ModelLayer.Collaboration;
using Repository.Interface;

namespace BussinesLayer.Service
{
    public class CollaborationServicebl : Interface.ICollaboration
    {
        private readonly ICollaborationRL _collaboration;

        public CollaborationServicebl(ICollaborationRL collaboration)
        {
            _collaboration = collaboration;
        }
        public Task<bool> AddCollaborator(int noteid, CollaborationRequestModel model, int userId)
        {
            return _collaboration.AddCollaborator(noteid, model, userId);
        }
        public Task RemoveCollaborator(int CollabId)
        {
            return _collaboration.RemoveCollaborator(CollabId);
        }
        public Task<IEnumerable<CollabInfoModel>> GetCollaboration()
        {
            return _collaboration.GetCollaboration();
        }
    }
}