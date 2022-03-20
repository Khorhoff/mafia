using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Mafia
{
    public class SelectedPlayer
    {
        public Player Selected { get; set; }
        public Switch Choise { get; set; }

        public SelectedPlayer(Player _player, Switch _choise)
        {
            Selected = _player;
            Choise = _choise;
        }
    }
}
