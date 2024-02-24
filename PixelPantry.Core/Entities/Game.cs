using System.ComponentModel.DataAnnotations;

namespace PixelPantry.Core.Entities
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public virtual required Franchise Franchise { get; set; }
        public virtual ICollection<VideoFile>? VideoFiles { get; set; }
        public long? TotalSize => VideoFiles?.Sum(f => f.Size) ?? 0;
    }

}