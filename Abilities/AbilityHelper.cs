using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace StarlightRiver.Abilities
{
    //This class serves to simplify implementing ability interactions
    class AbilityHelper
    {
        public static bool CheckDash(Player player, Rectangle hitbox)
        {
            if(Collision.CheckAABBvAABBCollision(player.Hitbox.TopLeft(), player.Hitbox.Size(), hitbox.TopLeft(), hitbox.Size()) && player.GetModPlayer<AbilityHandler>().AbilityDash.Active)
            {
                return true;
            }
            return false;
        }

        public static bool CheckWisp(Player player, Rectangle hitbox)
        {
            if (Collision.CheckAABBvAABBCollision(player.Hitbox.TopLeft(), player.Hitbox.Size(), hitbox.TopLeft(), hitbox.Size()) && player.GetModPlayer<AbilityHandler>().AbilityWisp.Active)
            {
                return true;
            }
            return false;
        }
    }
}
