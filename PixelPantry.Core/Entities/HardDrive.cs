using System.ComponentModel.DataAnnotations;

namespace PixelPantry.Core.Entities
{
    public class HardDrive
    {
        [Key]
        public int HardDriveId { get; set; }

        [Required]
        public string Location { get; set; } // e.g., "1A", "2B"

        public string? Model { get; set; }

        [Required]
        public long TotalCapacity { get; set; }

        // Navigation property
        public virtual ICollection<VideoFile>? VideoFiles { get; set; }

        // Computed property for used space
        public long UsedSpace => VideoFiles?.Sum(vf => vf.Size) ?? 0;

        // Computed property for free space
        public long FreeSpace => TotalCapacity - UsedSpace;
        public double UsedSpacePercentage => TotalCapacity > 0 ? (UsedSpace * 1.0 / TotalCapacity) : 0;



        // Virtual properties to get distinct franchises and games
        public virtual ICollection<Franchise>? GetFranchises() => VideoFiles?.Select(vf => vf.Game?.Franchise).Where(f => f != null).Distinct().ToList() ?? new List<Franchise>();

        public virtual ICollection<Game>? GetGames() => VideoFiles?.Select(vf => vf.Game).Where(g => g != null).Distinct().ToList() ?? new List<Game>();

    }

}
