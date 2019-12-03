using Microsoft.Xna.Framework;
using StarlightRiver.Projectiles;
using StarlightRiver.Projectiles.Ability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.Abilities
{
    [DataContract]
    public class AbilityPure : Ability
    {
        
        public AbilityPure(Player player) : base(4, player)
        {

        }

        public override void OnCast()
        {
            Active = true;
            Main.PlaySound(SoundID.Item37);
            Cooldown = 600;
        }

        public override void InUse()
        {
            for (float k = 0; k <= (float) Math.PI * 2; k += (float) Math.PI / 40)
            {
                int proj = Projectile.NewProjectile(player.Center + new Vector2((float)Math.Cos(k), (float)Math.Sin(k)), new Vector2((float)Math.Cos(k), (float)Math.Sin(k)) * 5, ModContent.ProjectileType<Purifier>(),0,0, player.whoAmI);
                Purifier pur = Main.projectile[proj].modProjectile as Purifier;
                pur.start = player.Center;
                LegendWorld.PureTiles.Add(player.Center/16);

            }
            Active = false;
            OnExit();

        }

        public override void OnExit()
        {

        }
    }
}
