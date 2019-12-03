using StarlightRiver.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace StarlightRiver.Items.Infusions
{
    public class WispHomingItem : InfusionItem
    {
        public WispHomingItem() : base(3) { }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Feral Wisp");
            Tooltip.SetDefault("Faeflame Infusion\nRelease homing bolts that lower enemie's damage");
        }
        public override void UpdateEquip(Player player)
        {
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();
            if (!(mp.AbilityWisp is AbilityWispHoming) && !(mp.AbilityWisp is AbilityWispCombo))
            {
                if (mp.AbilityWisp is AbilityWispWip)
                {
                    mp.AbilityWisp = new AbilityWispCombo(player);
                    mp.AbilityWisp.Locked = false;
                }
                else
                {
                    mp.AbilityWisp = new AbilityWispHoming(player);
                    mp.AbilityWisp.Locked = false;
                }
            }
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();
            return !mp.AbilityWisp.Locked;
        }

        public override void Unequip(Player player)
        {
            player.GetModPlayer<AbilityHandler>().AbilityWisp = new Abilities.AbilityWisp(player);
            player.GetModPlayer<AbilityHandler>().AbilityWisp.Locked = false;
        }
    }
}
