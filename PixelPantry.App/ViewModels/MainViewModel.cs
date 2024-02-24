using PixelPantry.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelPantry.App.ViewModels
{
    public class FranchiseViewModel
    {
        public string Name { get; set; }
        public long TotalSize { get; set; }
        public List<GameViewModel> Games { get; set; } = new List<GameViewModel>();
        public double SizePercentage { get; set; } // Relative to all franchises
                                                   // Other properties...
    }

    public class GameViewModel
    {
        public string Title { get; set; }
        public long TotalSize { get; set; }
        public List<VideoFileViewModel> VideoFiles { get; set; } = new List<VideoFileViewModel>();
        public double SizePercentage { get; set; } // Relative to the franchise
                                                   // Other properties...
    }

    public class VideoFileViewModel
    {
        public string FileName { get; set; }
        public long Size { get; set; }
        public double SizePercentage { get; set; } // Relative to the game
                                                   // Other properties...
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FranchiseViewModel> Franchises { get; private set; }

        public MainViewModel()
        {
            LoadFranchises();
        }

        private void LoadFranchises()
        {
            Franchises = new ObservableCollection<FranchiseViewModel>();
            long totalSizeOfAllFranchises = 0;

            using (var ctx = new PixelPantryContext())
            {
                foreach (var franchise in ctx.Franchises.ToArray())
                {
                    var f = new FranchiseViewModel { Name = franchise.Name, TotalSize = 0 };
                    foreach (var game in franchise.Games.ToArray())
                    {
                        var g = new GameViewModel { Title = game.Title, TotalSize = 0 };
                        foreach (var videofile in game.VideoFiles.ToArray())
                        {
                            var vf = new VideoFileViewModel { FileName = videofile.FileName, Size = videofile.Size };
                            g.VideoFiles.Add(vf);
                            g.TotalSize += vf.Size;
                        }
                        f.Games.Add(g);
                        f.TotalSize += g.TotalSize;
                    }
                    Franchises.Add(f);
                    totalSizeOfAllFranchises += f.TotalSize;
                }
            }

            // Calculate percentages
            foreach (var franchise in Franchises)
            {
                franchise.SizePercentage = totalSizeOfAllFranchises > 0 ? (double)franchise.TotalSize / totalSizeOfAllFranchises : 0;

                foreach (var game in franchise.Games)
                {
                    game.SizePercentage = franchise.TotalSize > 0 ? (double)game.TotalSize / franchise.TotalSize : 0;

                    foreach (var videofile in game.VideoFiles)
                    {
                        videofile.SizePercentage = game.TotalSize > 0 ? (double)videofile.Size / game.TotalSize : 0;
                    }
                }
            }

            // Order franchises by SizePercentage in descending order
            var orderedFranchises = Franchises.OrderByDescending(f => f.SizePercentage).ToList();

            // Now update your observable collection or bindable list with orderedFranchises
            // Assuming you have an observable collection named Franchises in your ViewModel
            Franchises.Clear();
            foreach (var franchise in orderedFranchises)
            {
                Franchises.Add(franchise);
            }
        }

        public void FilterFranchises(string searchText)
        {
            // Implement your filtering logic
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
