using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Content.Thorium.Projectiles;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LifeBloomEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Green;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.LifeBloomEnch = true;
            DLCPlayer.LifeBloomEnchItem = Item;
        }

        public static void LifeBloomEffect(Player player, Item item)
        {
                        
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void LifeBloomKeyPress()
        {
            int treeType = ModContent.ProjectileType<LifeBloomTree>();
            if (Player.ownedProjectileCounts[treeType] > 0)
            {
                Player.KillOwnedProjectilesOfType(treeType);
                Player.KillOwnedProjectilesOfType(ModContent.ProjectileType<LifeBloomBallista>());
            }
            else
            {
                Projectile.NewProjectileDirect(Player.GetSource_Accessory(LifeBloomEnchItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<LifeBloomTree>(), 0, 0f, Player.whoAmI); 
            }
        }
    }
}