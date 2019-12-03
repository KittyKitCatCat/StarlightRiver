using StarlightRiver.Items.Infusions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace StarlightRiver.Abilities
{
    public partial class AbilityHandler
    {
        public Item Slot1;
        public Item Slot2;
        public bool HasSecondSlot;
        public override void PostUpdate()
        {
            if (Slot1 != null) { Slot1.modItem.UpdateEquip(player); }
            if (Slot2 != null) { Slot2.modItem.UpdateEquip(player); }       
            
        }
    }
}
