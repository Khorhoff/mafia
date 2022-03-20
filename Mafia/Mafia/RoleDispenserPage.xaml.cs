using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mafia
{
    public partial class RoleDispenserPage : ContentPage
    {

        private List<Role> RolesList;
        private List<Player> PlayersList;
        private Options GameOptions;
        private int it;
        private int PlayerIt;
        private Random rand;

        public RoleDispenserPage(List<Role> _rolesList, List<Player> _playersList, Options _gameOptions)
        {
            RolesList = _rolesList;
            PlayersList = _playersList;
            GameOptions = _gameOptions;
            it = 0;
            PlayerIt = 0;
            InitializeComponent();
            PlayerName.Text = $"Следующий игрок: {PlayersList[PlayerIt].Name}";
            PlayerRole.Source = "question.png";
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            BackButton.IsEnabled = false;
            await Navigation.PopModalAsync();
            BackButton.IsEnabled = true;
        }

        private async void NextRoleButton_Clicked(object sender, EventArgs e)
        {
            it++;
            if (RolesList.Count != 0)
            {
                if (it % 2 != 0)
                {
                    rand = new Random();
                    int choise = rand.Next(0, RolesList.Count);
                    PlayersList[PlayerIt].GameRole = RolesList[choise];
                    RolesList.RemoveRange(choise, 1);
                    PlayerName.Text = $"Игрок {PlayersList[PlayerIt].Name}";
                    switch (PlayersList[PlayerIt].GameRole.Name)
                    {
                        case "Мафия":
                            PlayerRole.Source = "Mafia.jpg";
                            break;
                        case "Мирный житель":
                            PlayerRole.Source = "Civilian.jpg";
                            break;
                        case "Доктор":
                            PlayerRole.Source = "Doctor.jpg";
                            break;
                        case "Дон мафии":
                            PlayerRole.Source = "Don.jpg";
                            break;
                        case "Комиссар":
                            PlayerRole.Source = "Commissioner.jpg";
                            break;
                        case "Любовница":
                            PlayerRole.Source = "Mistress.jpg";
                            break;
                        case "Маньяк":
                            PlayerRole.Source = "Maniac.jpg";
                            break;
                        case "Журналист":
                            PlayerRole.Source = "Journalist.jpg";
                            break;
                    }
                }
                else
                {
                    PlayerIt++;
                    PlayerName.Text = $"Следующий игрок: {PlayersList[PlayerIt].Name}";
                    PlayerRole.Source = "question.png";
                }
            }
            else
            {
                NextRoleButton.IsEnabled = false;
                await Navigation.PushModalAsync(new GamePage(PlayersList, GameOptions));
                NextRoleButton.IsEnabled = true;
            }
        }
    }
}