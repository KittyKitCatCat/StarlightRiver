using log4net.Util.TypeConverters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace StarlightRiver.Projectiles
{
    class TrailProjectile : ModProjectile
    {
        Texture2D texture;
        Color color;
        bool additive;
        public override bool Autoload(ref string name) => false;
        public enum type
        {
            normal = 0,
            tendril = 1
        }
        public TrailProjectile(Texture2D texture, Color color, bool additive)
        {
            this.texture = texture;
            this.color = color;
            this.additive = additive;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                float progress = (float)(projectile.oldPos.Length - k) / projectile.oldPos.Length;
                float scale = projectile.scale * progress;
                Color color = this.color * (progress);
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + projectile.Size / 2 + new Vector2(0f, projectile.gfxOffY);
                if (additive)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(default, BlendState.Additive);
                }
                spriteBatch.Draw(texture, drawPos, null, color, projectile.oldRot[k], projectile.Size / 2, scale, SpriteEffects.None, 0f);
                if (additive)
                {
                    spriteBatch.End();
                    spriteBatch.Begin();
                }
            }
            return base.PreDraw(spriteBatch, lightColor);
        }
    }
}
