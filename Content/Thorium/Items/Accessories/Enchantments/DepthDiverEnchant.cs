using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
	[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DepthDiverEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.MediumBlue;

		public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.DepthDiverEnchant = true;
            DLCPlayer.DepthDiverEnchantItem = Item;

			DepthDiverEffect(player);
		}

		public static void DepthDiverEffect(Player player)
		{
			// TODO: Test this
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				Player localPlayer = Main.LocalPlayer;
				if (localPlayer.DistanceSQ(player.Center) < 62500f)
				{
					localPlayer.AddBuff(ModContent.BuffType<ThoriumMod.Buffs.DepthBreath>(), 30, true, false);
					localPlayer.AddBuff(ModContent.BuffType<ThoriumMod.Buffs.DepthDamage>(), 30, true, false);
					localPlayer.AddBuff(ModContent.BuffType<ThoriumMod.Buffs.DepthSpeed>(), 30, true, false);
				}
			}

			player.AddBuff(ModContent.BuffType<ThoriumMod.Buffs.DepthBreath>(), 30, true, false);
			player.AddBuff(ModContent.BuffType<ThoriumMod.Buffs.DepthDamage>(), 30, true, false);
			player.AddBuff(ModContent.BuffType<ThoriumMod.Buffs.DepthSpeed>(), 30, true, false);
		}
    }
}