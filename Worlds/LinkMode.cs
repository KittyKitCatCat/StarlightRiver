using System;
using System.Linq;
using StarlightRiver.GUI;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StarlightRiver.Worlds
{
    public class LinkMode : ModWorld
    {
        public static bool Enabled = false;
        public static int MaxWorldHP = 1;
        public static int WorldHP = 100;
        int ticker = 0;

        public override void PreUpdate()
        {
            //Enabled = true;
            MaxWorldHP = 0;
            foreach(Player player in Main.player.Where(player => player.active))
            {
                MaxWorldHP += player.statLifeMax2;
            }

            if (Enabled && WorldHP < 0)
            {
               WorldHP = 0;
               LinkPlayer.sendpacket();
            }

            if(WorldHP > MaxWorldHP && MaxWorldHP != 0)
            {
                WorldHP = MaxWorldHP;
                LinkPlayer.sendpacket();
            }

            if (Main.player.Any(player => player.active && player.respawnTimer == 60))
            {
                WorldHP = MaxWorldHP / 2;
                LinkPlayer.sendpacket();
            }

            int heal = 0;
            foreach(Player player in Main.player.Where(player => player.active && player.lifeRegen/2 > 0))
            {
                heal += (int)(player.lifeRegen / 2);
            }
            if (ticker++ >= 60 && heal > 0 && WorldHP < MaxWorldHP)
            {
                WorldHP += heal;
                LinkPlayer.sendpacket();
                ticker = 0;
            }
        }


        public override TagCompound Save()
        {
            return new TagCompound
            {
                [nameof(Enabled)] = Enabled
            };           
        }
        public override void Load(TagCompound tag)
        {
            Enabled = tag.GetBool(nameof(Enabled));
            LinkPlayer.sendpacket();
        }
    }
    public class LinkPlayer : ModPlayer
    {
        public override void PlayerConnect(Player player)
        {
            if (LinkMode.Enabled)
            {
                LinkMode.WorldHP += player.statLife;
                sendpacket();
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (LinkMode.Enabled && LinkMode.WorldHP > 0)
            {
                return false;
            }
            return true;
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (LinkMode.Enabled && LinkMode.WorldHP > 0)
            {
                int hitfor = damage - (int)(player.statDefense * ((Main.expertMode) ? 0.75f : 0.5f));
                if (hitfor >= 1)
                {
                    LinkMode.WorldHP -= hitfor;
                    player.statLife += hitfor;
                }
                else
                {
                    LinkMode.WorldHP -= 1;
                    player.statLife += 1;
                }

                sendpacket();
            }
            return true;
        }

        int healCD = 60;
        public override void PostUpdate()
        {
            
            if (LinkMode.Enabled)
            {
                player.statLife = player.statLifeMax2 - 1;
                if (LinkMode.WorldHP <= 0)
                {
                    player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " died with their teammates..."), 99999, 0);
                    sendpacket();
                }

                if(healCD-- <= 0)
                {
                    healCD = 0;
                }

                LinkHP.visible = true;
            }
            else
            {
                LinkHP.visible = false;
            }
        }

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            if ((player.controlUseItem || player.controlQuickHeal) && LinkMode.Enabled && healCD == 0)
            {
                LinkMode.WorldHP += healValue;
                //Main.NewText("Before the Packer", 255, 0, 0);
                Console.WriteLine("PreHeal");
                sendpacket();
                //Main.NewText("After the Packer", 0, 255, 0);
                Console.WriteLine("PostHeal");
            }
        }

        public override void OnRespawn(Player player)
        {
            sendpacket();
        }

        public static void sendpacket()
        {
            //Main.NewText("The Packet", 255, 255, 0);
            Console.WriteLine("Server Packet Sent!");
            if (Main.netMode == 2)
            {
                ModPacket packet = StarlightRiver.Instance.GetPacket();
                packet.Write(LinkMode.Enabled);
                packet.Write(LinkMode.MaxWorldHP);
                packet.Write(LinkMode.WorldHP);
                packet.Send();
            }
        }
    }
}