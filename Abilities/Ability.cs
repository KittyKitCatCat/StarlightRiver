using Terraria;
using System.Linq;
using StarlightRiver.Players;

namespace StarlightRiver.Abilities
{
    public class Ability
    {
        public Ability(int staminaCost, Player player)
        {
            StaminaCost = staminaCost;
            Player = player;
        }


        public virtual void StartAbility(StarlightPlayer starlightPlayer)
        {
            //if the player: has enough stamina  && unlocked && not on CD     && Has no other abilities active
            if(!Locked && starlightPlayer.Stamina >= StaminaCost && Cooldown == 0 && !starlightPlayer.Abilities.Any(a => a.Active))
            {
                starlightPlayer.Stamina -= StaminaCost; //Consume the stamina
                OnCast(); //Do what the ability should do when it starts
            }
        }


        public virtual void OnCast() { }
        public virtual void OnExit() { }

        public virtual void InUse() { }

        public virtual void UseEffects() { }

        public virtual void OffCooldownEffects() { }


        public Player Player { get; }

        public int StaminaCost { get; set; }

        public bool Locked { get; set; } = true;
        public bool Active { get; set; }

        public int Timer { get; set; }
        public int Cooldown { get; set; }
    }
}
