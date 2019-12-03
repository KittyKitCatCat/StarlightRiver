using System.Collections.Generic;
using StarlightRiver.Abilities;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace StarlightRiver.Players
{
    public sealed partial class StarlightPlayer
    {
        // A list of all ability instances is kept to easily check things globally across the player's abilities.
        public List<Ability> Abilities = new List<Ability>();


        private void ModifyDrawLayersAbilities(List<PlayerLayer> layers)
        {
            if (AbilityWisp.Active || AbilityShadowDash.Active)
                foreach (PlayerLayer layer in layers) 
                    layer.visible = false;
        }

        private void ProcessTriggersAbilities(TriggersSet triggersSet)
        {
            // TODO Change this to be somewhat dynamic, if possible. Keybinds should be obtained from the abilities themselves, not hardcoded in here.
            //Activates one of the player's abilities on the appropriate keystroke.
            if (StarlightRiver.Dash.JustPressed) AbilityDash.StartAbility(this);
            if (StarlightRiver.Wisp.JustPressed) AbilityWisp.StartAbility(this);
            if (StarlightRiver.Purify.JustPressed) AbilityPure.StartAbility(this);
            if (StarlightRiver.Smash.JustPressed) AbilitySmash.StartAbility(this);
            if (StarlightRiver.Superdash.JustPressed) AbilityShadowDash.StartAbility(this);
        }


        // All players store 1 instance of each ability. This instance is changed to the infusion variant if an infusion is equipped.
        public AbilityDash AbilityDash { get; private set; }
        public AbilityWisp AbilityWisp { get; private set; }
        public AbilityPure AbilityPure { get; private set; }
        public AbilitySmash AbilitySmash { get; private set; }
        public AbilityShadowDash AbilityShadowDash { get; private set; }

        public Item Slot1 { get; private set; }
        public Item Slot2 { get; private set; }
        public bool HasSecondSlot { get; private set; }
    }
}
