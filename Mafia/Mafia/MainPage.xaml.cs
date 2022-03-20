using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mafia
{
    public partial class MainPage : ContentPage
    {
        private Options GameOptions = new Options("","");

        public MainPage()
        {
            InitializeComponent();
        }

        public void GetGameOptions(string _discuss, string _selection)
        {
            GameOptions = new Options(_discuss, _selection);
        }

        private async void StartButton_Clicked(object sender, EventArgs e)
        {
            StartButton.IsEnabled = false;
            await Navigation.PushModalAsync(new GameCreationPage(GameOptions));
            StartButton.IsEnabled = true;
        }

        private async void OptionsButton_Clicked(object sender, EventArgs e)
        {
            OptionsButton.IsEnabled = false;
            await Navigation.PushModalAsync(new OptionsPage(this));
            OptionsButton.IsEnabled = true;
        }

        private async void RoleButton_Clicked(object sender, EventArgs e)
        {
            RoleButton.IsEnabled = false;
            await Navigation.PushModalAsync(new RolePage());
            RoleButton.IsEnabled = true;
        }
    }
}
