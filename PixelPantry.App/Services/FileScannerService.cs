using MediaInfoDotNet;
using PixelPantry.Core.Entities;
using PixelPantry.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelPantry.App.Services
{
    public class FileScannerService
    {
        public async Task ScanHardDrivesAsync()
        {
            await Task.Run(() =>
            {
                var ctx = new PixelPantryContext();
                var usbDrives = GetUsbDrives();
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady && usbDrives.Keys.Contains(drive.Name.Substring(0, 2)))
                    {
                        var tempDrive = JsonSerializer.Deserialize<HardDrive>(File.ReadAllText(Path.Combine(drive.RootDirectory.FullName, ".metadata.json")));

                        var thisDrive = ctx.HardDrives.FirstOrDefault(f => f.Location == tempDrive.Location);
                        if (thisDrive == null)
                        {
                            ctx.HardDrives.Add(new HardDrive { Location = tempDrive.Location });
                            ctx.SaveChanges();
                            thisDrive = ctx.HardDrives.First(f => f.Location == tempDrive.Location);
                        }
                        thisDrive.TotalCapacity = drive.TotalSize;
                        thisDrive.Model = usbDrives[drive.Name.Substring(0, 2)];

                        ScanFranchiseDirectory(drive.RootDirectory, thisDrive, ref ctx);

                    }
                }
                ctx.SaveChanges();
                MessageBox.Show("Done!");
            });
        }

        private void ScanFranchiseDirectory(DirectoryInfo rootDirectory, HardDrive hardDrive, ref PixelPantryContext ctx)
        {
            foreach (var franchiseDir in rootDirectory.GetDirectories("*", new System.IO.EnumerationOptions { IgnoreInaccessible = true }))
            {
                var thisFranchise = ctx.Franchises.FirstOrDefault(f => f.Name == franchiseDir.Name);
                if (thisFranchise == null)
                {
                    ctx.Franchises.Add(new Franchise { Name = franchiseDir.Name });
                    ctx.SaveChanges();
                    thisFranchise = ctx.Franchises.First(f => f.Name == franchiseDir.Name);
                }
                ScanGameDirectory(thisFranchise, franchiseDir, hardDrive, ref ctx);
            }
        }

        private void ScanGameDirectory(Franchise franchise, DirectoryInfo franchiseDir, HardDrive hardDrive, ref PixelPantryContext ctx)
        {
            foreach (var gameDir in franchiseDir.GetDirectories("*", new System.IO.EnumerationOptions { IgnoreInaccessible = true }))
            {
                var thisGame = ctx.Games.FirstOrDefault(g => g.Title == gameDir.Name && g.Franchise == franchise);
                if (thisGame == null)
                {
                    ctx.Games.Add(new Game
                    {
                        Franchise = franchise,
                        Title = gameDir.Name
                    });
                    ctx.SaveChanges();
                    thisGame = ctx.Games.First(g => g.Title == gameDir.Name && g.Franchise == franchise);
                }
                ScanVideoFiles(thisGame, gameDir, hardDrive, ref ctx);
            }
        }

        private void ScanVideoFiles(Game game, DirectoryInfo gameDir, HardDrive hardDrive, ref PixelPantryContext ctx)
        {
            foreach (var file in gameDir.GetFiles("*", new System.IO.EnumerationOptions { IgnoreInaccessible = true }))
            {
                if (IsVideoFile(file))
                {
                    var mediaFile = new MediaFile(file.FullName);

                    try
                    {
                        var videoFile = new VideoFile
                        {
                            FileName = file.Name,
                            FilePath = file.FullName,
                            Size = file.Length,
                            CreationDate = file.CreationTime,
                            Game = game,
                            HardDrive = hardDrive,
                            Bitrate = int.Parse(mediaFile.Video[0].BitRate.Split("/")[0]),
                            FPS = mediaFile.Video[0].FrameRate,
                            HDR = mediaFile.Video[0].BitDepth <= 10,
                            Height = mediaFile.Video[0].Height,
                            Width = mediaFile.Video[0].Width,
                            Length = TimeSpan.FromMilliseconds(mediaFile.Video[0].Duration)
                        };

                        ctx.VideoFiles.Add(videoFile);
                        ctx.SaveChanges();
                    }
                    catch
                    {

                    }
                    
                }
            }

            // If there are subfolders in the game directory, you can call ScanVideoFiles recursively on them
            foreach (var subDir in gameDir.GetDirectories())
            {
                ScanVideoFiles(game, subDir, hardDrive, ref ctx);
            }
        }

        private Dictionary<string, string> GetUsbDrives()
        {
            var usbDrives = new Dictionary<string, string>();
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'"))
            {
                foreach (var drive in searcher.Get())
                {
                    // Get the DeviceID property of the Win32_DiskDrive
                    var deviceId = drive["DeviceID"].ToString();
                    var caption = drive["Caption"].ToString();

                    // Match the Win32_DiskDrive to Win32_DiskPartition
                    using (var partitions = new ManagementObjectSearcher($"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{deviceId}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition"))
                    {
                        foreach (var partition in partitions.Get())
                        {
                            // Match the Win32_DiskPartition to Win32_LogicalDisk
                            using (var logicalDisks = new ManagementObjectSearcher($"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass = Win32_LogicalDiskToPartition"))
                            {
                                foreach (var logicalDisk in logicalDisks.Get())
                                {
                                    usbDrives.Add(logicalDisk["DeviceID"].ToString(), caption);
                                }
                            }
                        }
                    }
                }
            }
            return usbDrives;
        }

        private bool IsVideoFile(FileInfo file)
        {
            // Define your video file extensions here
            var videoExtensions = new HashSet<string> { ".mp4", ".avi", ".mov", ".mkv" };
            return videoExtensions.Contains(file.Extension.ToLower());
        }
    }
}
