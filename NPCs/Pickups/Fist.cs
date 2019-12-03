using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using StarlightRiver.Abilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.NPCs.Pickups
{
    class Fist : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gaia's Fist");
        }
        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 32;
            npc.aiStyle = -1;
            npc.immortal = true;
            npc.lifeMax = 1;
            npc.knockBackResist = 0;
            npc.noGravity = true;
        }

        public override bool CheckActive() { return true; }

        int animate = 0;
        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();

            if (npc.Hitbox.Intersects(player.Hitbox) && mp.AbilitySmash.Locked)
            {
                mp.AbilitySmash.Locked = false;
                mp.StatStaminaMaxPerm += 1;
                animate = 300;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Pickups/get"));
            }

            if (animate >= 1)
            {
                player.position = new Vector2(npc.position.X, npc.position.Y - 16);
                player.immune = true;
                player.immuneTime = 5;
                player.immuneNoBlink = true;
                if (animate > 100 && animate < 290)
                {
                    float rot = (animate - 100) / 190f * 6.28f;
                }

                if (animate == 1)
                {
                    player.AddBuff(BuffID.Featherfall, 120);
                    Achievements.Achievements.QuickGive("Shatterer", player);

                    StarlightRiver.Instance.abilitytext.Display(mp.AbilitySmash, "Gaia's Fist", "Press " + StarlightRiver.Smash.GetAssignedKeys()[0] + " in the air to dive downwards");
                }
            }

            if (animate > 0)
            {
                animate--;
            }

        }
        float timer = 0;
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            AbilityHandler mp = Main.LocalPlayer.GetModPlayer<AbilityHandler>();

            timer += (float)(Math.PI * 2) / 120;
            if (timer >= Math.PI * 2)
            {
                timer = 0;
            }

            if (mp.AbilitySmash.Locked)
            {
                Vector2 pos = npc.position - Main.screenPosition - (new Vector2((int)((Math.Cos(timer * 3) + 1) * 4f), (int)((Math.Sin(timer * 3) + 1) * 4f)) / 2) + new Vector2(0, (float)Math.Sin(timer) * 4);

                spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Smash1"), npc.position + new Vector2(0, (float)Math.Sin(timer) * 4) - Main.screenPosition, Color.White);

                Dust.NewDustPerfect(npc.Center + Vector2.One.RotatedBy(timer) * (23 + (float)Math.Sin(timer * 10) * 4) , ModContent.DustType<Dusts.JungleEnergy>(), Vector2.Zero, 0, default, 0.8f);
                Dust.NewDustPerfect(npc.Center + Vector2.One.RotatedBy(timer) * 18, ModContent.DustType<Dusts.JungleEnergy>(), Vector2.Zero, 0, default, 0.8f);
                Dust.NewDustPerfect(npc.Center + Vector2.One.RotatedBy(timer) * 28, ModContent.DustType<Dusts.JungleEnergy>(), Vector2.Zero, 0, default, 0.8f);
            }
        }
    }
}
