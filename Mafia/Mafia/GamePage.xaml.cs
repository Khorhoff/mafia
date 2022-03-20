using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mafia
{
    public partial class GamePage : ContentPage
    {
        private Game Game;
        private List<Role> RolesList;
        private List<Role> PassedRolesList;
        private bool IsSelectOption;
        private bool IsDiscussOption;
        private int DayIt;
        private string Message;
        private int NightStep;
        private int DayStep;
        private List<SelectedPlayer> Selections;
        private Role selectedRole;
        private Player Target1;
        private Player Target2;
        private Player SelectedPlayer;
        private bool isAllowedToMove;
        private List<Player> MafiaList;
        private string TypeOfTimer;

        public GamePage(List<Player> _playersList, Options _gameOptions)
        {
            Game = new Game(_gameOptions);
            Game.Players = _playersList;
            RolesList = new List<Role>();
            PassedRolesList = new List<Role>();
            MafiaList = new List<Player>();
            DayIt = 0;
            if (Game.GameOptions.DurationOfSelectionAtNight != 0)
                IsSelectOption = true;
            else
                IsSelectOption = false;
            if (Game.GameOptions.DurationOfDiscussion != 0)
                IsDiscussOption = true;
            else
                IsDiscussOption = false;

            InitializeComponent();

            PlayersStack.Children.Clear();
            PlayersChoisesStack.Children.Clear();
            Label newL;
            Switch newS;
            bool IsFound;
            Selections = new List<SelectedPlayer>();
            foreach (var player in Game.Players)
            {
                IsFound = false;
                foreach(var role in RolesList)
                    if (role.Name == player.GameRole.Name)
                        IsFound = true;
                if (IsFound != true)
                    RolesList.Add(player.GameRole);

                newL = new Label
                {
                    Text = $"{player.Name}: {player.GameRole.Name}",
                    FontSize = 24,
                    HeightRequest = 30
                };
                newS = new Switch()
                {
                    IsToggled = false,
                    HorizontalOptions = LayoutOptions.Start,
                    HeightRequest = 30
                };
                Selections.Add(new SelectedPlayer(player, newS));
                PlayersStack.Children.Add(newL);
                PlayersChoisesStack.Children.Add(newS);
                foreach (var item in Selections)
                    item.Choise.IsEnabled = false;
                PassButton.IsEnabled = false;
            }
        }

        private async void EndGame_Clicked(object sender, EventArgs e)
        {
            TimerLabel.Text = "";
            EndGame.IsEnabled = false;
            await Navigation.PushModalAsync(new MainPage());
            EndGame.IsEnabled = true;
        }

        private async void GoButton_Clicked(object sender, EventArgs e)
        {
            int targetsCount = 0;
            //начало игры
            if (DayIt == 0)
            {
                GoButton.Text = "Продолжить";
                DayStep = 1;
                PassButton.IsEnabled = false;
            }
            //если день
            if (GameTime.Text == $"День {DayIt}")
            {
                if (DayStep != 0)
                {
                    if (Game.IsGameEnd())
                    {
                        string mes = Game.WonTeam == "Black" ? "Мафии" : (Game.WonTeam == "Red" ? "Мирных" : "Маньяка");
                        await DisplayAlert("", $"Игра окончена победой {mes}", "OK");
                    }
                    else
                        await DisplayAlert("SayIt", "Город засыпает", "ОК");
                    DayIt++;
                    NightStep = 0;
                    GameTime.Text = $"Ночь {DayIt}";
                    PassedRolesList.Clear();
                    this.BackgroundColor = (Color)Resources["Night"];
                    GameMess.Text = "";
                    foreach (var item in Selections)
                    {
                        item.Choise.IsEnabled = false;
                        item.Choise.IsToggled = false;
                    }
                }
                else
                {
                    DayStep++;
                    foreach (var item in Selections)
                        if (item.Choise.IsToggled != false)
                            targetsCount++;
                    if (targetsCount != 0)
                    {
                        foreach (var item in Selections)
                            if (item.Choise.IsToggled != false)
                                SelectedPlayer = item.Selected;
                        foreach (var player in Game.Players)
                            if (player == SelectedPlayer)
                            {
                                Game.Players.Remove(player);
                                break;
                            }
                        await DisplayAlert("SayIt", $"{SelectedPlayer.Name} изгнан посредством голосования, последнее слово?", "OK");
                    }
                    else
                    {
                        await DisplayAlert("SayIt", "На голосовании никто не был изгнан", "OK");
                    }
                    foreach (var item in Selections)
                    {
                        item.Choise.IsEnabled = false;
                    }
                    if (TimerLabel.Text != "")
                    {
                        TimerLabel.Text = "";
                    }
                }
            }
            //если ночь
            else
            if (GameTime.Text == $"Ночь {DayIt}")
            {
                //если начало хода
                if (NightStep % 2 == 0)
                {
                    selectedRole = SelectRole();
                    //если конец ночи
                    if (selectedRole.Name == "")
                    {
                        GameTime.Text = $"День {DayIt}";
                        DayStep = 0;
                        this.BackgroundColor = (Color)Resources["Day"];
                        await DisplayAlert("SayIt", "Город просыпается", "ОК");
                        await DisplayAlert("SayIt", $"{Game.ResultsOfTheNight(GameMess.Text)}", "OK");
                        if (Game.IsGameEnd())
                        {
                            string mes = Game.WonTeam == "Black" ? "Мафии" : (Game.WonTeam == "Red" ? "Мирных" : "Маньяка");
                            await DisplayAlert("", $"Игра окончена победой {mes}", "OK");
                            await Navigation.PushModalAsync(new MainPage());
                        }
                        if (IsDiscussOption)
                        {
                            TimerLabel.Text = Game.GameOptions.DurationOfDiscussion.ToString();
                            TypeOfTimer = "Discuss";
                            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
                        }

                        foreach (var item in Selections)
                            item.Choise.IsEnabled = false;

                        foreach (var item in Selections)
                            foreach (var player in Game.Players)
                                if (item.Selected == player)
                                {
                                    item.Selected = player;
                                    item.Choise.IsEnabled = true;
                                }

                        foreach (var player in Game.Players)
                        {
                            if (player.HealDelay != 0)
                                player.HealDelay--;
                            if (player.HealDelay == 0)
                                player.IsHealed = false;
                            player.IsMuted = false;
                        }
                        PassButton.IsEnabled = false;
                    }
                    //иначе
                    else
                    {
                        await DisplayAlert("SayIt", $"Просыпается {selectedRole.Name}", "OK");
                        foreach (var item in Selections)
                            item.Choise.IsEnabled = false;

                        if (selectedRole.Name == "Маньяк" || selectedRole.Name == "Журналист")
                            PassButton.IsEnabled = false;
                        else PassButton.IsEnabled = true;

                        if (selectedRole.Name == "Любовница")
                        {
                            foreach (var item in Selections)
                                if (item.Selected.GameRole == selectedRole)
                                    item.Choise.IsEnabled = false;
                                else foreach (var player in Game.Players)
                                        if (item.Selected == player)
                                            item.Choise.IsEnabled = true;
                        }
                        else if (selectedRole.Name == "Маньяк")
                        {
                            foreach (var item in Selections)
                                if (item.Selected.GameRole == selectedRole)
                                    item.Choise.IsEnabled = false;
                                else foreach (var player in Game.Players)
                                        if (item.Selected == player)
                                            item.Choise.IsEnabled = true;
                        }
                        else
                        {
                            foreach (var item in Selections)
                                foreach (var player in Game.Players)
                                    if (item.Selected == player)
                                        item.Choise.IsEnabled = true;
                        }

                        isAllowedToMove = true;
                        bool isChoise = false;
                        if (selectedRole.Name != "Мафия")
                        {
                            foreach (var player in Game.Players)
                            {
                                //если игрок жив
                                if (player.GameRole.Name == selectedRole.Name)
                                {
                                    isChoise = true;
                                    //если не может сделать ход
                                    if (player.IsMuted)
                                    {
                                        isAllowedToMove = false;
                                        await DisplayAlert("Do", $"Игрок {player.Name} не может ходить", "OK");
                                        foreach (var item in Selections)
                                        {
                                            item.Choise.IsEnabled = false;
                                        }
                                    }
                                    else
                                    {
                                        if (selectedRole.Name == "Журналист")
                                        {
                                            int countNotVerified = 0;
                                            foreach (var possible_elections in Game.Players)
                                                if (!possible_elections.IsVerifiedByJournalist)
                                                    countNotVerified++;
                                            if (countNotVerified >= 2)
                                                SelectedPlayer = player;
                                            else
                                            {
                                                isAllowedToMove = false;
                                                await DisplayAlert("Do", "Вы выполнили все возможные проверки", "OK");
                                            }
                                        }
                                        else
                                        SelectedPlayer = player;
                                    }
                                }
                            }
                            //если игрок мертв
                            if (isChoise != true)
                            {
                                isAllowedToMove = false;
                                await DisplayAlert("Do", $"Игрок мертв, подождите немного", "OK");
                                foreach (var item in Selections)
                                {
                                    item.Choise.IsEnabled = false;
                                }
                            }
                        }
                        else
                        {
                            MafiaList.Clear();
                            foreach (var player in Game.Players)
                                if (player.GameRole.Team == "Black")
                                    MafiaList.Add(player);
                            //если есть мафия
                            if (MafiaList.Count != 0)
                            {
                                foreach (var player in MafiaList)
                                    if (player.IsMuted != false)
                                    {
                                        MafiaList.Remove(player);
                                        await DisplayAlert("Do", $"Игрок {player.Name} не может ходить", "OK");
                                        break;
                                    }
                                if (MafiaList.Count == 0)
                                {
                                    isAllowedToMove = false;
                                    foreach (var item in Selections)
                                    {
                                        item.Choise.IsEnabled = false;
                                    }
                                }
                                else if (MafiaList.Count == 1)
                                {
                                    SelectedPlayer = MafiaList[0];
                                    if (MafiaList[0].GameRole.Name == "Дон мафии")
                                    {
                                        SelectedPlayer = new Player("");
                                        SelectedPlayer.GameRole = new Role("Мафия");
                                    }
                                }
                                else foreach (var player in MafiaList)
                                        if (player.GameRole.Name != "Дон мафии")
                                        {
                                            SelectedPlayer = player;
                                            break;
                                        }
                            }
                            //если нет мафии
                            else
                            {
                                await DisplayAlert("Do", $"Игроки мертвы, подождите немного", "OK");
                                isAllowedToMove = false;
                                foreach (var item in Selections)
                                {
                                    item.Choise.IsEnabled = false;
                                }
                            }
                        }
                        if (IsSelectOption && isAllowedToMove)
                        {
                            TimerLabel.Text = Game.GameOptions.DurationOfSelectionAtNight.ToString();
                            TypeOfTimer = "Select";
                            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
                        }
                    }
                    NightStep++;
                }
                //если выбор хода
                else
                {
                    bool IsComplited = false;
                    if (isAllowedToMove)
                    {
                        //если ход журналиста
                        if (selectedRole.Name == "Журналист")
                        {
                            bool IsFirstFound = false;

                            foreach (var item in Selections)
                                if (item.Choise.IsToggled != false)
                                    targetsCount++;
                            if (targetsCount == 2)
                            {
                                foreach (var item in Selections)
                                {
                                    if (item.Choise.IsToggled != false && IsFirstFound != true)
                                    {
                                        Target1 = item.Selected;
                                        IsFirstFound = true;
                                        continue;
                                    }
                                    if (item.Choise.IsToggled != false && IsFirstFound != false)
                                        Target2 = item.Selected;
                                }
                                    Message = SelectedPlayer.UseAbility(Target1, Target2);
                                    if (Message != "Невозможно сравнить\n")
                                    {
                                        IsComplited = true;
                                        GameMess.Text += Message;
                                    }
                                    else await DisplayAlert("Do", "Невозможно сравнить", "OK");

                            }
                            else await DisplayAlert("Do", "Выберите две цели", "OK");
                        }
                        //если ход доктора
                        else if (selectedRole.Name == "Доктор")
                        {
                            foreach (var item in Selections)
                                if (item.Choise.IsToggled != false)
                                    targetsCount++;
                            if (targetsCount == 1)
                            {
                                foreach (var item in Selections)
                                    if (item.Choise.IsToggled != false)
                                        Target1 = item.Selected;
                                if (Target1.Name != SelectedPlayer.Name)
                                {
                                    foreach (var target in Game.Players)
                                        if (target.Name == Target1.Name)
                                            Message = SelectedPlayer.UseAbility(target);
                                    if (Message != "Лечение невозможно\n")
                                    {
                                        IsComplited = true;
                                    }
                                    else await DisplayAlert("Do", "Лечение невозможно", "OK");
                                }
                                else
                                {
                                    foreach (var player in Game.Players)
                                        if (player.Name == SelectedPlayer.Name)
                                            Message = player.UseAbility(player);
                                    if (Message != "Лечение невозможно\n")
                                    {
                                        IsComplited = true;
                                    }
                                    else await DisplayAlert("Do", "Лечение невозможно", "OK");
                                }

                            }
                            else await DisplayAlert("Do", "Выберите одну цель", "OK");

                        }
                        //иначе
                        else
                        {
                            foreach (var item in Selections)
                                if (item.Choise.IsToggled != false)
                                    targetsCount++;
                            if (targetsCount == 1)
                            {
                                foreach (var item in Selections)
                                    if (item.Choise.IsToggled != false)
                                        Target1 = item.Selected;
                                foreach (var target in Game.Players)
                                    if (target.Name == Target1.Name)
                                        GameMess.Text += SelectedPlayer.UseAbility(target);
                                IsComplited = true;
                            }
                            else await DisplayAlert("Do", "Выберите одну цель", "OK");
                        }
                    }
                    else IsComplited = true;
                    if (IsComplited)
                    {
                        PassButton.IsEnabled = false;
                        NightStep++;
                        PassedRolesList.Add(selectedRole);
                        foreach (var item in Selections)
                            item.Choise.IsToggled = false;
                        await DisplayAlert("SayIt", $"{selectedRole.Name} засыпает", "OK");
                        foreach (var item in Selections)
                            item.Choise.IsEnabled = false;
                        if (TimerLabel.Text != "")
                        {
                            TimerLabel.Text = "";
                        }
                    }
                }
            }
        }

        private bool OnTimerTick()
        {
            if (TimerLabel.Text == "")
            {
                return false;
            }
            else
            if (TimerLabel.Text != "0")
            {
                TimerLabel.Text = (int.Parse(TimerLabel.Text) - 1).ToString();
                return true;
            }
            else
            {
                if (TypeOfTimer == "Discuss")
                {
                    GetMes();
                }
                if (TypeOfTimer == "Select")
                {
                    GetMes();
                }
                TimerLabel.Text = "";
                return false;
            }
        }

        private async void GetMes()
        {
            if (TypeOfTimer == "Discuss")
            {
                await DisplayAlert("SayIt", "Пора начать голосование!", "OK");
            }
            if (TypeOfTimer == "Select")
            {
                await NoChioseGo();
            }
        }

        private Role SelectRole()
        {
            Role Select = new Role("");
            bool isPass;

            if (Select.Name == "")
                foreach (var role in RolesList)
                {
                    if (role.Name == "Любовница")
                    {
                        isPass = false;
                        foreach (var passRole in PassedRolesList)
                            if (role == passRole)
                                isPass = true;
                        if (isPass != true)
                            Select = role;
                    }
                }
            if (Select.Name == "")
                foreach (var role in RolesList)
                {
                    if (role.Name == "Маньяк")
                    {
                        isPass = false;
                        foreach (var passRole in PassedRolesList)
                            if (role == passRole)
                                isPass = true;
                        if (isPass != true)
                            Select = role;
                    }
                }
            if (Select.Name == "")
                foreach (var role in RolesList)
                {
                    if (role.Team == "Black")
                    {
                        isPass = false;
                        foreach (var passRole in PassedRolesList)
                            if (passRole.Name == "Мафия")
                                isPass = true;
                        if (isPass != true)
                            Select = new Role("Мафия");
                    }
                }
            if (Select.Name == "")
                foreach (var role in RolesList)
                {
                    if (role.Name == "Доктор")
                    {
                        isPass = false;
                        foreach (var passRole in PassedRolesList)
                            if (role == passRole)
                                isPass = true;
                        if (isPass != true)
                            Select = role;
                    }
                }
            if (Select.Name == "")
                foreach (var role in RolesList)
                {
                    if (role.Name == "Комиссар")
                    {
                        isPass = false;
                        foreach (var passRole in PassedRolesList)
                            if (role == passRole)
                                isPass = true;
                        if (isPass != true)
                            Select = role;
                    }
                }
            if (Select.Name == "")
                foreach (var role in RolesList)
                {
                    if (role.Name == "Дон мафии")
                    {
                        isPass = false;
                        foreach (var passRole in PassedRolesList)
                            if (role == passRole)
                                isPass = true;
                        if (isPass != true)
                            Select = role;
                    }
                }
            if (Select.Name == "")
                foreach (var role in RolesList)
                {
                    if (role.Name == "Журналист")
                    {
                        isPass = false;
                        foreach (var passRole in PassedRolesList)
                            if (role == passRole)
                                isPass = true;
                        if (isPass != true)
                            Select = role;
                    }
                }

            return Select;
        }

        private async void PassButton_Clicked(object sender, EventArgs e)
        {
            await NoChioseGo();
        }

        private async Task NoChioseGo()
        {
            if (isAllowedToMove)
            {
                if (selectedRole.Name == "Маньяк")
                {
                    Target1 = new Player("");
                    Random rand = new Random();
                    int choise;
                    while (Target1.Name == SelectedPlayer.Name || Target1.Name == "")
                    {
                        choise = rand.Next(0, Game.Players.Count);
                        Target1 = Game.Players[choise];
                    }
                    SelectedPlayer.UseAbility(Target1);
                    PassButton.IsEnabled = false;
                    NightStep++;
                    PassedRolesList.Add(selectedRole);
                    foreach (var item in Selections)
                        item.Choise.IsToggled = false;
                    await DisplayAlert("SayIt", $"{selectedRole.Name} засыпает", "OK");
                    foreach (var item in Selections)
                        item.Choise.IsEnabled = false;
                }
                else
                if (selectedRole.Name == "Журналист")
                {
                    Target1 = new Player("");
                    Target2 = new Player("");
                    Random rand = new Random();
                    int choise;
                    while (Target1.Name == SelectedPlayer.Name || Target1.Name == "")
                    {
                        choise = rand.Next(0, Game.Players.Count);
                        Target1 = Game.Players[choise];
                    }
                    while (Target2.Name == SelectedPlayer.Name || Target2.Name == "" ||
                        Target1.Name == Target2.Name)
                    {
                        choise = rand.Next(0, Game.Players.Count);
                        Target2 = Game.Players[choise];
                    }
                    GameMess.Text += SelectedPlayer.UseAbility(Target1, Target2);
                    PassButton.IsEnabled = false;
                    NightStep++;
                    PassedRolesList.Add(selectedRole);
                    foreach (var item in Selections)
                        item.Choise.IsToggled = false;
                    await DisplayAlert("SayIt", $"{selectedRole.Name} засыпает", "OK");
                    foreach (var item in Selections)
                        item.Choise.IsEnabled = false;
                }
                else
                {
                    PassButton.IsEnabled = false;
                    NightStep++;
                    PassedRolesList.Add(selectedRole);
                    foreach (var item in Selections)
                        item.Choise.IsToggled = false;
                    await DisplayAlert("SayIt", $"{selectedRole.Name} засыпает", "OK");
                    foreach (var item in Selections)
                        item.Choise.IsEnabled = false;
                }
                if (TimerLabel.Text != "")
                {
                    TimerLabel.Text = "";
                }
            }
        }
    }
}