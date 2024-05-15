using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RepositoryLayer.Entity
{
    public class NoteEntity
    {
        [Key]
        public int NoteId { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string Colour { get; set; } = string.Empty;
        public bool IsArchived { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("Users")]
        public int UserId { get; set; }

        [ForeignKey("Label")]
        public int LabelId { get; set; }

    }
}