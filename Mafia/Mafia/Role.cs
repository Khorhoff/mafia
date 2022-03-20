using System;
using System.Collections.Generic;
using System.Text;

namespace Mafia
{
    public class Role
    {
        public string Name { get; set; }
        public string Team { get; set; }

        public Role(string _name)
        {
            Name = _name;
            if (Name == "Мафия" || Name == "Дон мафии")
                Team = "Black";
            else if (Name == "Маньяк")
                Team = "Grey";
            else Team = "Red";
        }
    }
}
