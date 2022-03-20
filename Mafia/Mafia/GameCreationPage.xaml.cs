
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mafia
{
    public partial class GameCreationPage : ContentPage
    {
        private Options GameOptions;
        private List<Entry> entries;
        private int playersCount;
        private List<Player> PlayersList;
        private List<Role> RolesList;
        public GameCreationPage(Options _options)
        {
            GameOptions = _options;
            InitializeComponent();
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            BackButton.IsEnabled = false;
            await Navigation.PopModalAsync();
            BackButton.IsEnabled = true;
        }

        private void PlayersCount_Completed(object sender, EventArgs e)
        {
            PlayersStack.Children.Clear();
            Label newL;
            Entry newE;
            entries = new List<Entry>();
            if (PlayersCount.Text != "")
            {
                playersCount = (int)double.Parse(PlayersCount.Text);
                if (playersCount < 0)
                {
                    playersCount = 0;
                }
                for (int i = 0; i < playersCount; i++)
                {
                    newL = new Label
                    {
                        Text = $"Игрок {i + 1}"
                    };
                    newE = new Entry();
                    entries.Add(newE);
                    PlayersStack.Children.Add(newL);
                    PlayersStack.Children.Add(newE);
                }
                RoleStack.IsEnabled = true;
            }
            else RoleStack.IsEnabled = false;

            MafiaCount.Text = "0";
            CivilianCount.Text = "0";
            IsDoctor.IsToggled = false;
            IsMistress.IsToggled = false;
            IsCommissioner.IsToggled = false;
            IsDon.IsToggled = false;
            IsManiac.IsToggled = false;
            IsJournalist.IsToggled = false;
        }

        private void MinusMaf_Clicked(object sender, EventArgs e)
        {
            if (MafiaCount.Text != "0")
                MafiaCount.Text = (int.Parse(MafiaCount.Text) - 1).ToString();
        }

        private void PlusMaf_Clicked(object sender, EventArgs e)
        {
            MafiaCount.Text = (int.Parse(MafiaCount.Text) + 1).ToString();
        }

        private void MinusCiv_Clicked(object sender, EventArgs e)
        {
            if (CivilianCount.Text != "0")
                CivilianCount.Text = (int.Parse(CivilianCount.Text) - 1).ToString();
        }

        private void PlusCiv_Clicked(object sender, EventArgs e)
        {
            CivilianCount.Text = (int.Parse(CivilianCount.Text) + 1).ToString();
        }

        private void MafiaCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuantityCheck();
        }

        private void CivilianCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuantityCheck();
        }

        private void IsDoctor_Toggled(object sender, ToggledEventArgs e)
        {
            QuantityCheck();
        }

        private void IsMistress_Toggled(object sender, ToggledEventArgs e)
        {
            QuantityCheck();
        }

        private void IsCommissioner_Toggled(object sender, ToggledEventArgs e)
        {
            QuantityCheck();
        }

        private void IsDon_Toggled(object sender, ToggledEventArgs e)
        {
            QuantityCheck();
        }

        private void IsManiac_Toggled(object sender, ToggledEventArgs e)
        {
            QuantityCheck();
        }

        private void IsJournalist_Toggled(object sender, ToggledEventArgs e)
        {
            QuantityCheck();
        }

        private int QuantityCheck()
        {
            int roleCount = 0;
            if (MafiaCount.Text != "")
                roleCount += int.Parse(MafiaCount.Text);
            if (CivilianCount.Text != "")
                roleCount += int.Parse(CivilianCount.Text);
            if (IsDoctor.IsToggled != false)
                roleCount++;
            if (IsMistress.IsToggled != false)
                roleCount++;
            if (IsCommissioner.IsToggled != false)
                roleCount++;
            if (IsDon.IsToggled != false)
                roleCount++;
            if (IsManiac.IsToggled != false)
                roleCount++;
            if (IsJournalist.IsToggled != false)
                roleCount++;

            if (roleCount >= playersCount)
            {
                PlusCiv.IsEnabled = false;
                PlusMaf.IsEnabled = false;
                if (IsDoctor.IsToggled == false)
                    IsDoctor.IsEnabled = false;
                if (IsMistress.IsToggled == false)
                    IsMistress.IsEnabled = false;
                if (IsCommissioner.IsToggled == false)
                    IsCommissioner.IsEnabled = false;
                if (IsDon.IsToggled == false)
                    IsDon.IsEnabled = false;
                if (IsManiac.IsToggled == false)
                    IsManiac.IsEnabled = false;
                if (IsJournalist.IsToggled == false)
                    IsJournalist.IsEnabled = false;
            }
            else
            {
                PlusCiv.IsEnabled = true;
                PlusMaf.IsEnabled = true;
                IsDoctor.IsEnabled = true;
                IsMistress.IsEnabled = true;
                IsCommissioner.IsEnabled = true;
                IsDon.IsEnabled = true;
                IsManiac.IsEnabled = true;
                IsJournalist.IsEnabled = true;
            }
            return roleCount;
        }

        private async void HandOutRoles_Clicked(object sender, EventArgs e)
        {
            if (playersCount <= 0)
                await DisplayAlert("Уведомление", "Игроков меньше одного", "OK");
            else
            if (QuantityCheck() == playersCount)
            {
                Entry entry;
                bool isEmpty = false;
                bool isNotRepeat = true;
                foreach (var view in PlayersStack.Children)
                    if (view.GetType() == typeof(Entry))
                    {
                        entry = (Entry)view;
                        if (entry.Text == "")
                            isEmpty = true;
                    }
                if (isEmpty != false)
                    await DisplayAlert("Уведомление", "Заполните все поля с именами!", "OK");
                else
                {
                    RolesList = new List<Role>();
                    PlayersList = new List<Player>();
                    if (MafiaCount.Text != "")
                        for (int i = 0; i < int.Parse(MafiaCount.Text); i++)
                            RolesList.Add(new Role("Мафия"));
                    if (CivilianCount.Text != "")
                        for (int i = 0; i < int.Parse(CivilianCount.Text); i++)
                            RolesList.Add(new Role("Мирный житель"));
                    if (IsDoctor.IsToggled != false)
                        RolesList.Add(new Role("Доктор"));
                    if (IsMistress.IsToggled != false)
                        RolesList.Add(new Role("Любовница"));
                    if (IsCommissioner.IsToggled != false)
                        RolesList.Add(new Role("Комиссар"));
                    if (IsDon.IsToggled != false)
                        RolesList.Add(new Role("Дон мафии"));
                    if (IsManiac.IsToggled != false)
                        RolesList.Add(new Role("Маньяк"));
                    if (IsJournalist.IsToggled != false)
                        RolesList.Add(new Role("Журналист"));
                    foreach (var view in PlayersStack.Children)
                        if (view.GetType() == typeof(Entry))
                        {
                            entry = (Entry)view;
                            PlayersList.Add(new Player(entry.Text));
                        }
                    for (int i = 0; i < PlayersList.Count; i++)
                        for (int j = 0; j < PlayersList.Count; j++)
                            if (PlayersList[i].Name == PlayersList[j].Name && i != j)
                                isNotRepeat = false;
                    if (isNotRepeat)
                    {
                        HandOutRoles.IsEnabled = false;
                        await Navigation.PushModalAsync(new RoleDispenserPage(RolesList, PlayersList, GameOptions));
                        HandOutRoles.IsEnabled = true;
                    }
                    else
                        await DisplayAlert("Уведомление", "Есть одинаковые имена!", "OK");
                }
            }
            else await DisplayAlert("Уведомление", "Выберите еще роли, количество выбранных меньше количества игроков", "OK");
        }
    }
}