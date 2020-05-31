using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace StarlightRiver.Keys
{
    internal class OvergrowKey : Key
    {
        public OvergrowKey() : base("Overgrowth Key", "StarlightRiver/Keys/OvergrowKey") { }
        public override bool ShowCondition => Main.LocalPlayer.GetModPlayer<BiomeHandler>().ZoneOvergrow;
        public override void PreDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);

            Texture2D tex = ModContent.GetTexture("StarlightRiver/VFX/Glow0");
            spriteBatch.Draw(tex, Position + Vector2.One * 16 - Main.screenPosition, tex.Frame(), new Color(255, 255, 200) * 0.3f, LegendWorld.rottime, tex.Frame().Size() / 2, 1 + (float)Math.Cos(LegendWorld.rottime) * 0.25f, 0, 0);
            spriteBatch.Draw(tex, Position + Vector2.One * 16 - Main.screenPosition, tex.Frame(), new Color(255, 255, 200) * 0.5f, LegendWorld.rottime, tex.Frame().Size() / 2, 0.7f + (float)Math.Cos(LegendWorld.rottime + 0.5f) * 0.15f, 0, 0);
            spriteBatch.Draw(tex, Position + Vector2.One * 16 - Main.screenPosition, tex.Frame(), new Color(255, 255, 200) * 0.7f, LegendWorld.rottime, tex.Frame().Size() / 2, 0.5f + (float)Math.Cos(LegendWorld.rottime + 1) * 0.1f, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin();
        }
        public override void PreUpdate()
        {
            if (Main.rand.Next(4) == 0)
            {
                Dust.NewDust(Position + new Vector2(0, (float)Math.Sin(LegendWorld.rottime) * 5), 32, 32, ModContent.DustType<Dusts.Gold2>(), 0, 0, 0, default, 0.5f);
            }

            Lighting.AddLight(Position, new Vector3(1, 1, 0.8f) * 0.6f);
        }
        public override void OnPickup()
        {
            CombatText.NewText(Hitbox, Color.White, "Got: Overgrowth Key");
        }
    }
}
