using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.ID;
using System.Linq;
using StarlightRiver.Abilities;
using StarlightRiver.Worlds;

namespace StarlightRiver.GUI
{
    public class LinkHP : UIState
    {
        public static bool visible = false;

        UIImage border;
        public override void OnInitialize()           
        {
            border = new UIImage(ModContent.GetTexture("StarlightRiver/GUI/LinkLifeBorder"));
            border.Left.Set(-298, 1);
            border.Top.Set(20, 0);
            border.Width.Set(252, 0);
            border.Height.Set(58, 0);
            base.Append(border);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            int hp = (!Main.LocalPlayer.dead) ? (int)(LinkMode.WorldHP/ (float)LinkMode.MaxWorldHP * 232) : 0;
            Color color = new Color(232 - hp, hp, 0);
            spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/GUI/LinkLifeFill"), new Rectangle((int)border.GetDimensions().Position().X + 10, (int)border.GetDimensions().Position().Y + 10, hp, 38), color * 0.8f);
            Utils.DrawBorderString(spriteBatch, "Team HP: " + ((!Main.LocalPlayer.dead)? LinkMode.WorldHP : 0) + "/" + LinkMode.MaxWorldHP, border.GetDimensions().Position() + new Vector2(50, 18), Color.White);
            Recalculate();
        }       
    }
}
  