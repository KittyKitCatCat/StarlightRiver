using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Dusts;
using System;
using System.Xml.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.Projectiles
{
    public class ShadowflameTendril : ModProjectile
    {
        public override string Texture => "StarlightRiver/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Texture2D texture = ModContent.GetTexture("StarlightRiver/VFX/Trail1");
                float progress = (float)(projectile.oldPos.Length - k) / projectile.oldPos.Length;
                float scale = projectile.scale * progress;
                Color color = Color.Purple * (progress);
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + projectile.Size / 2 + new Vector2(0f, projectile.gfxOffY);
                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.Additive);
                spriteBatch.Draw(texture, drawPos, null, color, projectile.oldRot[k], projectile.Size / 2, scale, SpriteEffects.None, 0f);
                spriteBatch.End();
                spriteBatch.Begin();
            }
            return false;
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.ranged = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 200);
        }
        float randomRotation = Main.rand.NextFloat(0.1f) - 0.05f;
        float randomSpeed = Main.rand.NextFloat(8, 12);

        bool picked = false;
        NPC target = Main.npc[0];
        public override void AI()
        {
            if (!picked)
            {
                for (int k = 0; k < Main.npc.Length; k++)
                {
                    if (Helper.IsTargetValid(Main.npc[k]))
                    {
                        if (Vector2.Distance(Main.npc[k].Center, projectile.Center) < Vector2.Distance(target.Center, projectile.Center))
                        {
                            target = Main.npc[k];
                        }
                    }
                }
            }
            picked = true;
            if (Vector2.Distance(target.Center, projectile.Center) < 400)
            {
                projectile.velocity += Vector2.Normalize(target.Center - projectile.Center) * 0.7f;
                projectile.velocity = Vector2.Normalize(projectile.velocity) * randomSpeed;
            }
            projectile.velocity = Vector2.Normalize(projectile.velocity).RotatedBy(randomRotation) * randomSpeed;
            projectile.rotation = projectile.velocity.ToRotation() + 1.57f;
            projectile.width = (int)(20f * projectile.scale);
            projectile.height = projectile.width;
            projectile.position.X = projectile.Center.X - (projectile.width / 2);
            projectile.position.Y = projectile.Center.Y - (projectile.height / 2);
            /*if (projectile.scale > 0.9)
            {
                projectile.scale -= 0.01f;
            }
            else
            {
                projectile.scale -= 0.025f;
            }
            if (projectile.scale <= 0f)
            {
                projectile.Kill();
            }
            for (int k = 0; k < projectile.scale * 5f; k++)
            {
                Dust dust = Main.dust[Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, ModContent.DustType<Shadowflame>(), projectile.velocity.X, projectile.velocity.Y, 100, default, 2f)];
                dust.position = (dust.position + projectile.Center) / 2f;
                dust.velocity *= 0.1f;
                dust.velocity -= projectile.velocity * (1.3f - projectile.scale);
                dust.scale += projectile.scale * 1.25f;
            }*/
        }
    }
}