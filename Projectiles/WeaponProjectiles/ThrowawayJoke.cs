using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Items.Misc;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace StarlightRiver.Projectiles.WeaponProjectiles
{
    public class ThrowawayJokeProjectile : ModProjectile
    {
        bool crit = false;
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 120;
            projectile.ignoreWater = true;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            DisplayName.SetDefault("Throwaway Joke");
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + projectile.Size / 2 + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k * 20) / projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.oldRot[k], projectile.Size / 2, projectile.scale - (k * 0.1f), SpriteEffects.None, 0f);
            }
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.velocity.X = -projectile.velocity.X;
            projectile.velocity.Y = -projectile.velocity.Y;
            this.crit = crit;
        }
        public override void AI()
        {
            projectile.rotation += 0.3f;
            if (projectile.penetrate == 1)
            {
                projectile.tileCollide = false;
                projectile.velocity += Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center);
                projectile.velocity = Vector2.Normalize(projectile.velocity) * 20f;
                Player player = Main.player[projectile.owner];
                projectile.timeLeft = 2;
                if (projectile.Hitbox.Intersects(player.Hitbox))
                {
                    projectile.Kill();
                    SmgPlayer sPlayer = player.GetModPlayer<SmgPlayer>();
                    sPlayer.reload(crit);
                }
            }
            else if (projectile.timeLeft <= 100)
            {
                projectile.velocity.Y += 0.1f;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.penetrate == 2)
            {
                return base.CanHitNPC(target);
            }
            return false;
        }
    }
}