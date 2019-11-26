using StarlightRiver.Players;
using Terraria;
using Terraria.ModLoader;

namespace StarlightRiver.Buffs
{
	public class DarkSlow : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Grasping Darkness");
			Description.SetDefault("Your movement is inhibited!");
			Main.debuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<StarlightPlayer>().DarkSlow = true;
		}

        
	}
}
