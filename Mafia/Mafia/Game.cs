using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mafia
{
    public class Game
    {
        public List<Player> Players { get; set; }
        public Options GameOptions { get; set; }
        public string WonTeam { get; set; }

        public Game(Options _gameOptions)
        {
            GameOptions = _gameOptions;
        }

        public string ResultsOfTheNight(string _message)
        {
            bool _isThereDead = false;
            string _target = "";
            Regex _regex = new Regex(@"Вы не попали\n");
            _message = _regex.Replace(_message, _target);
            _regex = new Regex(@"Лечение невозможно\n");
            _message = _regex.Replace(_message, _target);
            _regex = new Regex(@"Вы нашли мафию\n");
            _message = _regex.Replace(_message, _target);
            _regex = new Regex(@"Вы нашли комиссара\n");
            _message = _regex.Replace(_message, _target);
            _regex = new Regex(@"Невозможно сравнить\n");
            _message = _regex.Replace(_message, _target);
            List<Player> newPlayers = new List<Player>();
            foreach (var player in Players)
            {
                if (player.IsAlive == false)
                {
                    _message += $"{player.Name} мертв\n";
                    _isThereDead = true;
                }
                if (player.IsMuted == true)
                {
                    _message += $"{player.Name} пропускает дневное обсуждение\n";
                }
            }
            foreach (var player in Players)
                if (player.IsAlive != false)
                {
                    newPlayers.Add(player);
                }
            Players = newPlayers;
            foreach (var player in Players)
                if (player.IsMuted == true)
                    _message += $"{player.Name} не учавствует в обсуждении и голосовании\n";
            if (_isThereDead)
                _message += "Послушаем последние слова\n";
            if (_message == "")
                _message = "Ночью ничего не произошло";
            return _message;
        }

        public bool IsGameEnd()
        {
            bool isEnd = false, isMiss = false;
            int blackPl = 0, redPl = 0, greyPl = 0;

            foreach (var player in Players)
            {
                if (player.GameRole.Team == "Black")
                    blackPl++;
                if (player.GameRole.Team == "Red")
                    redPl++;
                if (player.GameRole.Team == "Grey")
                    greyPl++;
                if (player.GameRole.Name == "Любовница")
                    isMiss = true;
            }
            if ((blackPl == 1 && redPl == 1 && isMiss && greyPl == 0) || redPl == Players.Count)
            {
                WonTeam = "Red";
                isEnd = true;
            }
            if ((blackPl >= redPl && greyPl == 0 && !isMiss) || blackPl == Players.Count || (blackPl > greyPl && redPl == 0) || (blackPl > redPl && greyPl == 0))
            {
                WonTeam = "Black";
                isEnd = true;
            }
            if (greyPl == 1 && Players.Count == 2 && !isMiss)
            {
                WonTeam = "Grey";
                isEnd = true;
            }

            return isEnd;
        }
    }
}
