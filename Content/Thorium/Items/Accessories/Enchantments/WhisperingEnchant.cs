using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
	[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
	public class WhisperingEnchant : BaseEnchant 
	{
		public override Color nameColor => Color.Purple;

		public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

		public override void UpdateAccessory(Player player, bool hideVisuals) 
		{
			var DLCPLayer = player.ThoriumDLC();
			DLCPLayer.WhisperingEnch = true;
			DLCPLayer.WhisperingEnchItem = Item;

			int visionType = ModContent.ProjectileType<Projectiles.WhisperingVision>();
			if (player.ownedProjectileCounts[visionType] == 0)
            {
				Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, visionType, 75, 0, player.whoAmI);
            }
		}
	}
}

namespace FargowiltasCrossmod.Content.Thorium
{
	public partial class CrossplayerThorium
    {
		public void WhisperingActivate()
        {

        }
    }
}