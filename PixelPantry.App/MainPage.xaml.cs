
using PixelPantry.App.ViewModels;

namespace PixelPantry.App
{
    public partial class MainPage : ContentPage
    {

        private MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            BindingContext = _viewModel;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            // Implement filtering logic based on e.NewTextValue
            _viewModel.FilterFranchises(e.NewTextValue);
        }
    }

}
