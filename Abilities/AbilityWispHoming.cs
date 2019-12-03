using Microsoft.Xna.Framework;
using StarlightRiver.Dusts;
using StarlightRiver.Projectiles.Ability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.Abilities
{
    [DataContract]
    public class AbilityWispHoming : AbilityWisp
    {        
        public AbilityWispHoming(Player player) : base(player)
        {

        }

        public override void UseEffects()
        {
            if (Timer > -1)
            {
                if (Timer % 2 == 0) { Projectile.NewProjectile(player.Center, Vector2.One.RotatedByRandom(6.28f) * 3f, ModContent.ProjectileType<WispBolt>(), 1, 1f, player.whoAmI); }

                Dust.NewDust(player.Center - new Vector2(4, 4), 8, 8, ModContent.DustType<Stamina>(), 0f, 0f, 0, default, 3.5f);
            }
            else
            {
                for (int k = 0; k <= 2; k++)
                {
                    Dust.NewDust(player.Center - new Vector2(4, 4), 8, 8, ModContent.DustType<Dusts.Void>());
                }
            }
        }
    }
}
