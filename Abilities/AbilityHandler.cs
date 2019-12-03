using StarlightRiver.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StarlightRiver.Abilities
{
    public partial class AbilityHandler : ModPlayer
    {
        // A list of all ability instances is kept to easily check things globally across the player's abilities.
        public List<Ability> Abilities = new List<Ability>();

        //Holds the player's wing or rocket boot timer, since they must be disabled to move upwards correctly.
        private float StoredAccessoryTime = 0;

        public override TagCompound Save()
        {
            return new TagCompound
            {
                // Ability Unlock Data
                [nameof(AbilityDash)] = AbilityDash.Locked,
                [nameof(AbilityWisp)] = AbilityWisp.Locked,
                [nameof(AbilityPure)] = AbilityPure.Locked,
                [nameof(AbilitySmash)] = AbilitySmash.Locked,
                [nameof(AbilityShadowDash)] = AbilityShadowDash.Locked,

                // Infusion Data
                [nameof(Slot1)] = Slot1,
                [nameof(Slot2)] = Slot2,

                [nameof(HasSecondSlot)] = HasSecondSlot
            };
        }

        public override void Load(TagCompound tag)
        {
            // Dash
            AbilityDash = new AbilityDash(player);
            AbilityDash.Locked = tag.GetBool(nameof(AbilityDash));
            Abilities.Add(AbilityDash);

            // Wisp
            AbilityWisp = new AbilityWisp(player);
            AbilityWisp.Locked = tag.GetBool(nameof(AbilityWisp));
            Abilities.Add(AbilityWisp);

            // Pure
            AbilityPure = new AbilityPure(player);
            AbilityPure.Locked = tag.GetBool(nameof(AbilityPure));
            Abilities.Add(AbilityPure);

            // Smash
            AbilitySmash = new AbilitySmash(player);
            AbilitySmash.Locked = tag.GetBool(nameof(AbilitySmash));
            Abilities.Add(AbilitySmash);

            // Shadow Dash
            AbilityShadowDash = new AbilityShadowDash(player);
            AbilityShadowDash.Locked = tag.GetBool(nameof(AbilityShadowDash));
            Abilities.Add(AbilityShadowDash);


            // Loads Infusion Data
            Slot1 = tag.Get<Item>(nameof(Slot1)); 

            if (string.IsNullOrWhiteSpace(Slot1.Name)) 
                Slot1 = null;

            Slot2 = tag.Get<Item>(nameof(Slot2)); 
            if (string.IsNullOrWhiteSpace(Slot2.Name))
                Slot2 = null;

            HasSecondSlot = tag.GetBool(nameof(HasSecondSlot));
        }

        // Updates the Ability list with the latest info
        public void SetList()
        {
            Abilities.Clear();
            Abilities.Add(AbilityDash);
            Abilities.Add(AbilityWisp);
            Abilities.Add(AbilityPure);
            Abilities.Add(AbilitySmash);
            Abilities.Add(AbilityShadowDash);
        }

        public override void ResetEffects()
        {
            if (Abilities.Any(ability => ability.Active))
            {
                // The player cant use items while casting an ability.
                player.noItems = true;
                player.noBuilding = true;
            }

            SetList(); //Update the list to ensure all interactions work correctly
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            
        }

        public override void PreUpdate()
        {

            //Executes the ability's use code while it's active.
            foreach (Ability ability in Abilities.Where(ability => ability.Active)) { ability.InUse(); ability.UseEffects(); }

            //Decrements internal cooldowns of abilities.
            foreach (Ability ability in Abilities.Where(ability => ability.Cooldown > 0)) { ability.Cooldown--; }

            //Ability cooldown Effects
            foreach (Ability ability in Abilities.Where(ability => ability.Cooldown == 1)) { ability.OffCooldownEffects(); }

            //Physics fuckery due to redcode being retarded
            if (Abilities.Any(ability => ability.Active))
            {
                player.velocity.Y += 0.01f; //Required to ensure that the game never thinks we hit the ground when using an ability. Thanks redcode!

                // We need to store the player's wing or rocket boot time and set the effective time to zero while an ability is active to move upwards correctly. Thanks redcode!
                if (StoredAccessoryTime == 0) { StoredAccessoryTime = ((player.wingTimeMax > 0) ? player.wingTime : player.rocketTime + 1); }
                player.wingTime = 0;
                player.rocketTime = 0;
                player.rocketRelease = true;
            }

            //This restores the player's wings or rocket boots after the ability is over.
            else if (StoredAccessoryTime > 0)
            {
                player.velocity.Y += 0.01f; //We need to do this the frame after also.

                //Makes the determination between which of the two flight accessories the player has.
                if (player.wingTimeMax > 0) { player.wingTime = StoredAccessoryTime; }
                else { player.rocketTime = (int)StoredAccessoryTime - 1; }
                StoredAccessoryTime = 0;
            }

            //Dont exceed max stamina or regenerate stamina when full.
            if(StatStamina >= StatStaminaMax)
            {
                StatStamina = StatStaminaMax;
                StatStaminaRegen = StatStaminaRegenMax;
            }

            //The player's stamina regeneration.
            if (StatStaminaRegen <= 0 && StatStamina < StatStaminaMax)
            {
                StatStamina++;
                StatStaminaRegen = StatStaminaRegenMax;
            }

            //Regenerate only when abilities are not active.
            if (!Abilities.Any(a => a.Active)) { StatStaminaRegen--; }

            //If the player is dead, drain their stamina and disable all of their abilities.
            if (player.dead)
            {
                StatStamina = 0;
                StatStaminaRegen = StatStaminaRegenMax;
                foreach (Ability ability in Abilities) { ability.Active = false; }
            }

        }

    }
}
