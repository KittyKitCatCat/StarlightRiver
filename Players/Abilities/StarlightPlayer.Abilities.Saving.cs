using StarlightRiver.Abilities;
using Terraria;
using Terraria.ModLoader.IO;

namespace StarlightRiver.Players
{
    public sealed partial class StarlightPlayer
    {
        public const string ABILITIES_TAG_NAME = "Abilities";


        private void LoadAbilities(TagCompound tag)
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

        private void SaveAbilities(TagCompound tag)
        {
            TagCompound abilitiesTag = new TagCompound()
            {
                // Ability unlock data
                [nameof(AbilityDash)] = AbilityDash.Locked,
                [nameof(AbilityWisp)] = AbilityWisp.Locked,
                [nameof(AbilityPure)] = AbilityPure.Locked,
                [nameof(AbilitySmash)] = AbilitySmash.Locked,
                [nameof(AbilityShadowDash)] = AbilityShadowDash.Locked,

                // Infusion data
                [nameof(Slot1)] = Slot1,
                [nameof(Slot2)] = Slot2,

                [nameof(HasSecondSlot)] = HasSecondSlot
            };

            tag.Add(ABILITIES_TAG_NAME, abilitiesTag);
        }
    }
}
