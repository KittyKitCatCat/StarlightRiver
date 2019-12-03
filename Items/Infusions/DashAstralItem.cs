using StarlightRiver.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace StarlightRiver.Items.Infusions
{
    public class DashAstralItem : InfusionItem
    {
        public DashAstralItem() : base(3) { }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Comet Rush");
            Tooltip.SetDefault("Forbidden Winds Infusion\nDash farther and faster");
        }
        public override void UpdateEquip(Player player)
        {
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();
            if (!(mp.AbilityDash is AbilityDashAstral) && !(mp.AbilityDash is AbilityDashCombo))
            {
                if (mp.AbilityDash is AbilityDashFlame) { mp.AbilityDash = new AbilityDashCombo(player); }
                else { mp.AbilityDash = new AbilityDashAstral(player); }
                mp.AbilityDash.Locked = false;
                mp.AbilityDash.Cooldown = 90;
            }
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();
            return !mp.AbilityDash.Locked;

        }

        public override void Unequip(Player player)
        {
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();
            mp.AbilityDash = new AbilityDash(player);
            mp.AbilityDash.Locked = false;
            mp.AbilityDash.Cooldown = 90;
        }
    }
}
