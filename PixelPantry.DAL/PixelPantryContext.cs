using Microsoft.EntityFrameworkCore;
using PixelPantry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelPantry.DAL
{
    public class PixelPantryContext : DbContext
    {
        public PixelPantryContext() : base ()
        {
        }

        public PixelPantryContext(DbContextOptions<PixelPantryContext> options)
        : base(options)
        {
        }
        public DbSet<Franchise> Franchises { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<HardDrive> HardDrives { get; set; }
        public DbSet<VideoFile> VideoFiles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseSqlServer("Server=mycloudpr4100;Database=PixelPantry;User Id=sa;Password=e3RV9kFu;TrustServerCertificate=True;");
            }
        }
    }
}
