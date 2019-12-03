using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Abilities;
using StarlightRiver.Gases;
using Terraria;
using Terraria.ModLoader;

namespace StarlightRiver.Tiles.Interactive
{
    class VoidGoo : ModTile
    {
        int Frame = 0;
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
            drop = ModContent.ItemType<Items.VoidGooItem>();
            dustType = ModContent.DustType<Dusts.Void>();
            AddMapEntry(new Color(0, 0, 0));
            
            animationFrameHeight = 88;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();
            if (mp.AbilityShadowDash.Active)
            {
                Main.tile[i, j].inActive(true);
            }
            else
            {
                Main.tile[i, j].inActive(false);
            }
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 5)
            {
                frameCounter = 0;
                if (++frame >= 3)
                {
                    frame = 0;
                }
            }
            Frame = frame;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/Tiles/Interative/VoidGooGlow"), new Vector2((i + 12) * 16, (j + 12) * 16) - Main.screenPosition, new Rectangle(Main.tile[i,j].frameX, Main.tile[i, j].frameY + 88*Frame, 16, 16), Color.White);
        }
    }
}
