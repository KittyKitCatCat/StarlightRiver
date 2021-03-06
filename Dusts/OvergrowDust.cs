﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace StarlightRiver.Dusts
{
    class OvergrowDust : ModDust
    {
        int time = 0;
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "StarlightRiver/Dusts/DragonFire";
            return base.Autoload(ref name, ref texture);
        }
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.customData = 550;
            dust.scale = 1.8f;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }
        public override bool Update(Dust dust)
        {
            if (dust.customData is int)
            {
                dust.customData = (int)dust.customData - 1;
                if ((int)dust.customData == 0) dust.active = false;
                if ((int)dust.customData >= 100)
                {
                    if (dust.color.R < 100) dust.color *= 1.53f;
                    dust.scale *= 1.025f;
                }
                else
                {
                    dust.color *= 0.94f;
                    dust.scale *= 0.99f;
                }

                dust.rotation = LegendWorld.rottime;
                dust.position.X += (float)Math.Sin(-dust.scale * 3);
                dust.position.Y += (float)Math.Cos(-dust.scale * 3);
            }
            return true;
        }
    }
}
