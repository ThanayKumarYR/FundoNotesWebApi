using ModelLayer.Notes;
namespace BussinesLayer.Interface
{
    public interface INoteBusinessLayer
    {
        public Task CreateNote(CreateNoteRequest createNoteRequest, int userid, int LabelId);
        public Task<IEnumerable<NoteResponse>> GetAllNoteAsync(int userid);
        public Task<IEnumerable<NoteResponse>> GetAllArchivedNoteAsync(int userId);
        public Task<IEnumerable<NoteResponse>> GetAllDeletedNoteAsync(int userId);
        public Task UpdateNote(int noteId, int UserId, CreateNoteRequest updatedNote);
        public Task DeleteNote(int noteId, int userId);
    }
}