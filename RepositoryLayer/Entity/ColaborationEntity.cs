using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RepositoryLayer.Entity
{
    public class ColaborationEntity
    {
        [Key]
        public int CollaborationId;
        [ForeignKey("Users")]
        public int UserId { get; set; }
        [ForeignKey("Notes")]
        public int NoteId { get; set; }
        [Required]
        public string? CollabEmail { get; set; }
    }
}
