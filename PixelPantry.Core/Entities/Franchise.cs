using System.ComponentModel.DataAnnotations;

namespace PixelPantry.Core.Entities
{
    public class Franchise
    {
        [Key]
        public int FranchiseId { get; set; }

        [Required]
        public required string Name { get; set; }

        // Navigation property
        public virtual ICollection<Game>? Games { get; set; }

        public long? TotalSize => Games?.Sum(f => f.TotalSize) ?? 0;

    }

}