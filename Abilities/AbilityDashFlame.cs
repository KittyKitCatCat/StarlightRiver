using Microsoft.Xna.Framework;
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
    public class AbilityDashFlame : AbilityDash
    {
        

        public AbilityDashFlame(Player player) : base(player)
        {

        }

        public override void OnCast()
        {
            Active = true;
            Main.PlaySound(SoundID.Item45);
            Main.PlaySound(SoundID.Item104);

            Projectile proj = Main.projectile[Projectile.NewProjectile(new Vector2(player.position.X - 21, player.position.Y-12), Vector2.Zero, ModContent.ProjectileType<Projectiles.Ability.DashFire>(), 10, 1f)];
            proj.owner = player.whoAmI;

            X = ((player.controlLeft) ? -1 : 0) + ((player.controlRight) ? 1 : 0);
            Y = ((player.controlUp) ? -1 : 0) + ((player.controlDown) ? 1 : 0);
            Timer = 7;
            Cooldown = 90;
        }

        public override void UseEffects()
        {
            for (int k = 0; k <= 15; k++)
            {
                //Dust.NewDust(player.Center - new Vector2(player.height / 2, player.height / 2), player.height, player.height, DustID.Fire);
                Dust.NewDustPerfect(player.Center + player.velocity * Main.rand.NextFloat(0, 2), DustID.Fire, player.velocity.RotatedBy((Main.rand.Next(2) == 0) ? 2.8f : 3.48f) * Main.rand.NextFloat(0, 0.15f), 0, default, 0.95f);
                Dust.NewDustPerfect(player.Center + Vector2.Normalize(player.velocity) * Main.rand.Next(0, 50), ModContent.DustType<Dusts.FireDust>(), -player.velocity*Main.rand.NextFloat(-2, 5) + new Vector2(Main.rand.Next(-1, 2), Main.rand.Next(-1, 2)),0, default, 2f);
            }
        }

        public override void OnExit()
        {
            player.velocity.X *= 0.15f;
            player.velocity.Y *= 0.15f;
        }
    }
}
