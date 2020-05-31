using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace StarlightRiver.Projectiles
{
    class GlowProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 2;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
        }
        float time;
        float size;
        Color color;
        Texture2D texture;
        bool additive;
        public GlowProjectile(Vector2 position, float time, float size, Color color, Texture2D texture, bool additive)
        {
            this.time = time;
            this.size = size;
            this.color = color;
            this.texture = texture;
            this.additive = additive;
            Projectile.NewProjectileDirect(position, Vector2.Zero, ModContent.ProjectileType<GlowProjectile>(), 0, 0);
        }
        public override void AI()
        {
            projectile.timeLeft = 2;
            projectile.velocity = Vector2.Zero;
            projectile.ai[0] += size / time;
            if (projectile.ai[0] >= size)
            {
                projectile.Kill();
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 position = projectile.position - Main.screenPosition;
            float scale = projectile.ai[0] / time;
            float fade = 1- scale;
            if (additive)
            {
                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.Additive);
            }
            spriteBatch.Draw(texture, position + projectile.Size / 2 * scale, texture.Frame(), color * fade, projectile.rotation, texture.Size() / 2, scale, 0, 0);
            if (additive)
            {
                spriteBatch.End();
                spriteBatch.Begin();
            }
            Lighting.AddLight(projectile.Center, color.ToVector3() * fade);
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override string Texture => "StarlightRiver/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glow");
        }
    }
}
