using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Abilities;
using System;
using System.Linq;
using StarlightRiver.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace StarlightRiver.Tiles.Interactive
{
    class GravTile1 : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLavaDeath[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileBlockLight[Type] = false;
            TileID.Sets.DrawsWalls[Type] = true;

            drop = mod.ItemType("Fluff");
            dustType = mod.DustType("Air");
            AddMapEntry(new Color(200, 100, 160));
            animationFrameHeight = 88;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            foreach (Player player in Main.player)
            {
                if (player.Hitbox.Intersects(new Rectangle(i * 16, j * 16, 16, 16)))
                {
                    if(player.GetModPlayer<StarlightPlayer>().InvertGrav == 0 && player.velocity.Y < 5 && player.velocity.Y > -5)
                    {
                        player.velocity.Y = 0;
                    }
                    player.GetModPlayer<StarlightPlayer>().InvertGrav = 6;
                    //Main.NewText("grav");
                }
            }
        }
        /*public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 5)
            {
                frameCounter = 0;
                if (++frame >= 3)
                {
                    frame = 0;
                }
            }
        }*/
        /*public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            var deathShader = GameShaders.Misc["ExampleMod:DeathAnimation"];
            deathShader.UseOpacity(1f);
            deathShader.Apply(null);
            return true;
        }*/

        /*public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // As mentioned above, be sure not to forget this step.
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }*/
    }
}
