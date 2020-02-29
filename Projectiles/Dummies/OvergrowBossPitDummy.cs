﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StarlightRiver.Projectiles.Dummies
{
    class OvergrowBossPitDummy : ModProjectile
    {
        public override string Texture => "StarlightRiver/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.timeLeft = 2;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            projectile.timeLeft = 2;

            if(projectile.ai[1] == 1)
            {
                if (projectile.ai[0] < 88) projectile.ai[0] += 4;
            }

            if(projectile.ai[1] == 2)
            {
                projectile.ai[0] -= 4;
                if (projectile.ai[0] <= 0) projectile.ai[1] = 0;
            }

            Lighting.AddLight(projectile.position + new Vector2(88, 0), new Vector3(1, 1, 0.4f) * (projectile.ai[0] / 88f));
            if(projectile.ai[0] > 0)
            {
                Dust.NewDustPerfect(new Vector2(projectile.position.X + (88 - projectile.ai[0] + Main.rand.NextFloat(projectile.ai[0] * 2)), projectile.position.Y + 56), ModContent.DustType<Dusts.Gold2>(), new Vector2(0, Main.rand.NextFloat(-3, -1)));
            }

            //lightning
            if(projectile.ai[0] == 88 && Main.rand.Next(8) == 0)
            {
                Helper.DrawElectricity(projectile.position + new Vector2(Main.rand.Next(176), 60), projectile.position + new Vector2(Main.rand.Next(2) == 0 ? 0 : 176, 0), ModContent.DustType<Dusts.Gold>(), 0.5f);
            }

            //collision
            foreach(Player player in Main.player.Where(p => p.Hitbox.Intersects(new Rectangle((int)projectile.position.X, (int)projectile.position.Y + 30, 176, 32))))
            {
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " got cooked extra crispy..."), 120, 0);
                player.velocity.Y -= 30;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 pos = projectile.position - Main.screenPosition;

            spriteBatch.End(); //We need to draw these with transparency (additive blendstate)
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
            //glow
            Texture2D tex0 = ModContent.GetTexture("StarlightRiver/Tiles/Overgrow/PitGlowBig");
            Rectangle rect = new Rectangle((int)pos.X + 88 - (int)projectile.ai[0], (int)pos.Y - 52, (int)projectile.ai[0] * 2, 116);
            spriteBatch.Draw(tex0, rect, tex0.Frame(), new Color(255, 255, 120) * (projectile.ai[0] / 88f));

            spriteBatch.End(); //Back to normal!
            spriteBatch.Begin();
            //doors
            Texture2D tex1 = ModContent.GetTexture("StarlightRiver/Tiles/Overgrow/PitCover");
            spriteBatch.Draw(tex1, pos + new Vector2(88 + projectile.ai[0], 0), tex1.Frame(), lightColor);
            spriteBatch.Draw(tex1, pos + new Vector2(-projectile.ai[0], 0), tex1.Frame(), lightColor, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);

            //warning
            if (projectile.ai[0] > 0)
            {
                Texture2D tex2 = ModContent.GetTexture("StarlightRiver/Exclamation");
                spriteBatch.Draw(tex2, pos + new Vector2(88, -100 + (float)Math.Sin(LegendWorld.rottime) * 12), tex2.Frame(), 
                    Color.White * (projectile.ai[0] / 88f) * 0.2f, 0, tex2.Frame().Size() / 2, 0.5f + (float)Math.Sin(LegendWorld.rottime * 3) * 0.05f, 0, 0);
            }
        }
    }
}