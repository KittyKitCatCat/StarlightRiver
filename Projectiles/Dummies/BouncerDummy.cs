using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Abilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.Projectiles.Dummies
{
    class BouncerDummy : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }
        public override string Texture => "StarlightRiver/Invisible";
        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.aiStyle = -1;
            projectile.timeLeft = 2;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            foreach (Player player in Main.player.Where(player => Vector2.Distance(player.Center, projectile.Center) <= 100))
                {
                AbilityHandler mp = player.GetModPlayer<AbilityHandler>();

                if (AbilityHelper.CheckDash(player, projectile.Hitbox))
                {
                    mp.AbilityDash.Active = false;

                    if (player.velocity.Length() != 0)
                    {
                        player.velocity = Vector2.Normalize(player.velocity) * -18f;
                        player.wingTime = player.wingTimeMax;
                        player.rocketTime = player.rocketTimeMax;
                        player.jumpAgainCloud = true;
                        player.jumpAgainBlizzard = true;
                        player.jumpAgainSandstorm = true;
                        player.jumpAgainFart = true;
                        player.jumpAgainSail = true;
                    }

                    Main.PlaySound(SoundID.Shatter, projectile.Center);
                    for (int k = 0; k <= 30; k++)
                    {
                        int dus = Dust.NewDust(projectile.position, 48, 32, mod.DustType("Glass"), Main.rand.Next(-16, 15), Main.rand.Next(-16, 15), 0, default, 1.3f);
                        Main.dust[dus].customData = projectile.Center;
                    }
                }
            }
            projectile.timeLeft = 2;

            if(Main.tile[(int)projectile.Center.X / 16, (int)projectile.Center.Y / 16].type != mod.TileType("Bounce"))
            {
                projectile.timeLeft = 0;
            }
        }
    }
}
