using System.ComponentModel.DataAnnotations;

namespace PixelPantry.Core.Entities
{
    public class VideoFile
    {
        [Key]
        public int VideoFileId { get; set; }
        [Required]
        public required string FileName { get; set; }
        [Required]
        public required string FilePath { get; set; }
        [Required]
        public required long Size { get; set; } // File size in bytes
        [Required]
        public required DateTime CreationDate { get; set; }

        // Quality properties
        public int Height { get; set; }
        public int Width { get; set; }
        public int Bitrate { get; set; } // Measured in kbps (kilobits per second)
        public double FPS { get; set; } // Frames per second
        public bool HDR { get; set; } // High Dynamic Range support
        public TimeSpan Length { get; set; } // Duration of the video

        public virtual required HardDrive HardDrive { get; set; }
        public virtual required Game Game { get; set; }
    }

}