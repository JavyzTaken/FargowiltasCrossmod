using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.ThoriumMod.Name)]
    public class DreadEnchant : BaseEnchant
    {
        public override Color nameColor => Color.DarkOliveGreen;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }

        //public static void DreadEffect(Player player, Item item)
        //{
        //    int headType = ModContent.ProjectileType<DragonMinionHead>();

        //    if (player.ownedProjectileCounts[headType] != 1)
        //    {
        //        player.KillOwnedProjectilesOfType(headType);

        //        Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, headType, 45, 0.2f, player.whoAmI);
        //    }
        //}
    }

    [ExtendsFromMod(ModCompatibility.ThoriumMod.Name)]
    public class DreadEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.HelheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<DreadEnchant>();
    }
}