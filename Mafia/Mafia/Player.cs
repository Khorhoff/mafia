using System;
using System.Collections.Generic;
using System.Text;

namespace Mafia
{
    public class Player
    {
        public string Name { get; set; }
        public Role GameRole { get; set; }
        public bool IsMuted { get; set; }
        public bool IsAlive { get; set; }
        public bool IsVerifiedByJournalist { get; set; }
        public bool IsHealed { get; set; }
        public int HealDelay { get; set; }
        public int NumberOfVotes { get; set; }

        public Player(string _name)
        {
            Name = _name;
            IsMuted = false;
            IsAlive = true;
            IsVerifiedByJournalist = false;
            IsHealed = false;
            HealDelay = 0;
            NumberOfVotes = 0;
        }

        public string UseAbility(Player _player)
        {
            string _message = "";

            if (GameRole.Name == "Мафия")
            {
                _player.IsAlive = false;
            }
            if (GameRole.Name == "Дон мафии")
            {
                if (_player.GameRole.Name == "Комиссар")
                {
                    _message = "Вы нашли комиссара\n";
                }
                else
                {
                    _message = "Вы не попали\n";
                }
            }
            if (GameRole.Name == "Маньяк")
            {
                _player.IsAlive = false;
            }
            if (GameRole.Name == "Комиссар")
            {
                if (_player.GameRole.Team == "Black")
                {
                    _message = "Вы нашли мафию\n";
                }
                else
                {
                    _message = "Вы не попали\n";
                }
            }
            if (GameRole.Name == "Доктор")
            {
                if (_player.IsHealed == false)
                {
                    _player.IsAlive = true;
                    _player.IsHealed = true;
                    if (_player == this)
                    {
                        _player.HealDelay = 999;
                    }
                    else _player.HealDelay = 2;
                }
                else
                {
                    _message = "Лечение невозможно\n";
                }
            }
            if (GameRole.Name == "Любовница")
            {
                _player.IsMuted = true;
            }

            return _message;
        }

        public string UseAbility(Player _firts, Player _second)
        {
            string _message;
            if (!_firts.IsVerifiedByJournalist && !_second.IsVerifiedByJournalist)
            {
                _message = $"{_firts.Name} и {_second.Name} играют в";

                if (_firts.GameRole.Team == _second.GameRole.Team ||
                    (_firts.GameRole.Team != _second.GameRole.Team && _firts.GameRole.Team != "Black" && _second.GameRole.Team != "Black"))
                {
                    _message += " одной команде\n";
                }
                if (_firts.GameRole.Team != _second.GameRole.Team && (_firts.GameRole.Team == "Black" || _second.GameRole.Team == "Black"))
                {
                    _message += " разных командах\n";
                }
                _firts.IsVerifiedByJournalist = true;
                _second.IsVerifiedByJournalist = true;
            }
            else
                _message = "Невозможно сравнить\n";

            return _message;
        }
    }
}
