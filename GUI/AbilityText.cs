using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using StarlightRiver.Abilities;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace StarlightRiver.GUI
{
    public class AbilityText : UIState
    {
        public static bool Visible = false;

        private Ability Thisability;
        private string Title;
        private string Message;
        private int Timer = 0;
        private bool used = false;

        private int tempTime = 0;
        private int tempTimeMax = 0;

        public void Display(string title, string message, Ability ability = null, int time = 0)
        {
            Thisability = ability;
            Title = title;
            Message = message;
            Visible = true;
            used = false;
            tempTimeMax = time;
            tempTime = 0;
            Timer = 1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int TitleLength = (int)(Main.fontDeathText.MeasureString(Title).X * 0.65f) / 2;
            int MessageLength = (int)(Main.fontDeathText.MeasureString(Message).X * 0.4f) / 2;
            int Longest = MessageLength > TitleLength ? MessageLength : TitleLength;
            int startY = (int)(Main.screenHeight * Main.UIScale) / 5;
            int startX = (int)(Main.screenWidth * Main.UIScale) / 2;
            Color color = Color.White * (Timer / 120f);

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend);

            spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/VFX/Glow0"), new Rectangle(startX - Longest * 2, startY - 25, Longest * 4, 150), Color.Black * 0.6f * (Timer / 120f));

            spriteBatch.End();
            spriteBatch.Begin();

            spriteBatch.DrawString(Main.fontDeathText, Title, new Vector2(startX - TitleLength, startY + 10), color, 0f, Vector2.Zero, 0.65f, 0, 0);
            spriteBatch.DrawString(Main.fontDeathText, Message, new Vector2(startX - MessageLength, startY + 50), color, 0f, Vector2.Zero, 0.4f, 0, 0);

            spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/GUI/LineMid"), new Rectangle(startX - (int)(Longest * 1.2f), startY + 75, (int)(Longest * 2.4f), 6), color);

            Texture2D tex = ModContent.GetTexture("StarlightRiver/GUI/LineSide");
            spriteBatch.Draw(tex, new Vector2(startX - (int)(Longest * 1.2f) - tex.Width, startY + 45), color);
            spriteBatch.Draw(tex, new Rectangle(startX + (int)(Longest * 1.2f), startY + 45, tex.Width, tex.Height), tex.Frame(), color, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);

            if (Thisability != null)
            {
                if (Thisability.Active) { used = true; }

                if (used) { Timer--; }
                else if (Timer < 120) { Timer++; }

                if (Timer == 0) { Visible = false; }
            }

            else
            {
                if (tempTime < tempTimeMax) tempTime++;
                if (tempTime >= tempTimeMax) { Timer--; }
                else if (Timer < 120) { Timer++; }

                if (Timer == 0) { Visible = false; }
            }
        }
    }
}
