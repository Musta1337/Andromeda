﻿using InfinityScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andromeda.Events.EventArguments
{
    public class WeaponTakenArgs : EventArgs
    {
        public Entity Player { get; private set; }

        public string Weapon { get; private set; }

        public WeaponTakenArgs(Entity player, string weapon)
        {
            Player = player;
            Weapon = weapon;
        }

        public void Deconstruct(out Entity player, out string weapon)
        {
            player = Player;
            weapon = Weapon;
        }
    }
}
