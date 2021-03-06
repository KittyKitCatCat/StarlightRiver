﻿using Terraria.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameInput;
using System.Linq;
using System.Runtime.Serialization;
using StarlightRiver.Abilities;
using StarlightRiver.Dragons;

namespace StarlightRiver.Abilities
{
    public class Ability
    {
        public int StaminaCost;
        public bool Active; 
        public int Timer;
        public int Cooldown;
        public bool Locked = true;
        public Player player;
        public virtual bool CanUse { get => true; }

        public Ability(int staminaCost, Player Player)
        {
            StaminaCost = staminaCost;
            player = Player;
        }

        public virtual void StartAbility(Player player)
        {
            AbilityHandler handler = player.GetModPlayer<AbilityHandler>();
            DragonHandler dragon = player.GetModPlayer<DragonHandler>();
            //if the player: has enough stamina  && unlocked && not on CD     && Has no other abilities active
            if(CanUse && handler.StatStamina >= StaminaCost && !Locked && Cooldown == 0 && !handler.Abilities.Any(a => a.Active))
            {
                handler.StatStamina -= StaminaCost; //Consume the stamina
                if (dragon.DragonMounted) OnCastDragon(); //Do what the ability should do when it starts
                else OnCast(); 
                Active = true; //Ability is activated
            }
        }

        public virtual void OnCast() { }
        public virtual void OnCastDragon() { }

        public virtual void InUse() { }
        public virtual void InUseDragon() { }

        public virtual void UseEffects() { }
        public virtual void UseEffectsDragon() { }

        public virtual void OffCooldownEffects() { }

        public virtual void OnExit() { }

    }
}
