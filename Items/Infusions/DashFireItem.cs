using StarlightRiver.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace StarlightRiver.Items.Infusions
{
    public class DashFireItem : InfusionItem
    {
        public DashFireItem() : base(3) { }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flaming Slash");
            Tooltip.SetDefault("Forbidden Winds Infusion\nDeal damage to enemies when you dash into them\nDamage dealth is equal to 5% of an enemys HP (up to 200)");
        }
        public override void UpdateEquip(Player player)
        {
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();
            if (!(mp.AbilityDash is AbilityDashFlame) && !(mp.AbilityDash is AbilityDashCombo))
            {
                if (mp.AbilityDash is AbilityDashAstral) { mp.AbilityDash = new AbilityDashCombo(player); }
                else { mp.AbilityDash = new AbilityDashFlame(player); }
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
