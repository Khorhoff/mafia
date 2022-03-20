using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mafia
{
    public partial class OptionsPage : ContentPage
    {
        MainPage HomePage;
        public OptionsPage(MainPage _mainPage)
        {
            HomePage = _mainPage;
            InitializeComponent();
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            HomePage.GetGameOptions(DiscussionDuration.Text, SelectionDuration.Text);
            SaveButton.IsEnabled = false;
            await Navigation.PopModalAsync();
            SaveButton.IsEnabled = true;
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            BackButton.IsEnabled = false;
            await Navigation.PopModalAsync();
            BackButton.IsEnabled = true;
        }
    }
}