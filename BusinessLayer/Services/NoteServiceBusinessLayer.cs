using BussinesLayer.Interface;
using ModelLayer.Notes;
using Repository.Interface;

namespace BussinesLayer.Service
{
    public class NoteServiceBusinessLayer : INoteBusinessLayer
    {
        private readonly INotesRepositoryLayer _notesInterface;

        public NoteServiceBusinessLayer(INotesRepositoryLayer notesInterface)
        {
            _notesInterface = notesInterface;
        }
        public Task CreateNote(CreateNoteRequest createNoteRequest, int userid, int LabelId)
        {
            return _notesInterface.CreateNote(createNoteRequest, userid,LabelId);
        }
        public Task<IEnumerable<NoteResponse>> GetAllNoteAsync(int userid)
        {
            return _notesInterface.GetAllNoteAsync(userid);
        }
        public Task UpdateNote(int noteId, int UserId, CreateNoteRequest updatedNote)
        {
            return _notesInterface.UpdateNote(noteId, UserId, updatedNote);
        }
        public Task DeleteNote(int noteId, int userId)
        {
            return _notesInterface.DeleteNote(noteId, userId);
        }

        public Task<IEnumerable<NoteResponse>> GetAllArchivedNoteAsync(int userId)
        {
            return _notesInterface.GetAllArchivedNoteAsync(userId);
        }

        public Task<IEnumerable<NoteResponse>> GetAllDeletedNoteAsync(int userId)
        {
            return _notesInterface.GetAllDeletedNoteAsync(userId);
        }
    }
}