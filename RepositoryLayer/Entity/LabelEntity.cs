using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Entity
{
    public class LabelEntity
    {
        [Key]
        public int LabelId { get; set; }
        [Required]
        public string? LabelName { get; set; }
        [ForeignKey("Users")]
        public int UserId { get; set; }
    }
}