using Microsoft.Xna.Framework;
using StarlightRiver.Dusts;
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
    public class AbilityDashAstral : AbilityDash
    {
        

        public AbilityDashAstral(Player player) : base(player)
        {

        }

        public override void InUse()
        {
            player.maxFallSpeed = 999;

            Timer--;

            if (X != 0 || Y != 0)
            {
                player.velocity = Vector2.Normalize(new Vector2(X, Y)) * 44;
            }

            if (Vector2.Distance(player.position, player.oldPosition) < 5 && Timer < 4)
            {
                Timer = 0;
                player.velocity *= -0.2f;
            }

            if (Timer <= 0)
            {
                Active = false;
                OnExit();
            }
        }

        public override void UseEffects()
        {
            if (player.velocity.Length() > 6)
            {
                for (int k = 0; k <= 10; k++)
                {
                    Dust.NewDust(player.Center - new Vector2(player.height / 2, player.height / 2), player.height, player.height, ModContent.DustType<Starlight>(), -10 * Vector2.Normalize(player.velocity).X, -10 * Vector2.Normalize(player.velocity).Y, 0, default, 0.75f);
                    Dust.NewDustPerfect(player.Center + Vector2.Normalize(player.velocity) * Main.rand.Next(-100, 0), ModContent.DustType<Starlight>(), Vector2.Normalize(player.velocity).RotatedBy(1) * (Main.rand.Next(-20, -5) + Timer * -3), 0, default, 1 - Timer * 0.1f);
                    Dust.NewDustPerfect(player.Center + Vector2.Normalize(player.velocity) * Main.rand.Next(-100, 0), ModContent.DustType<Starlight>(), Vector2.Normalize(player.velocity).RotatedBy(-1) * (Main.rand.Next(-20, -5) + Timer * -3), 0, default, 1 - Timer * 0.1f);
                }
            }
        }
    }
}
