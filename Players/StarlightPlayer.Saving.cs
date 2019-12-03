using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace StarlightRiver.Players
{
    public sealed partial class StarlightPlayer
    {
        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();

            SaveStamina(tag);
            SaveAbilities(tag);

            return tag;
        }

        public override void Load(TagCompound tag)
        {
            LoadStamina(tag);
            LoadAbilities(tag);
        }
    }
}
