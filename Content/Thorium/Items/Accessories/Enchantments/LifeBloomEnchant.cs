using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LifeBloomEnchant : BaseEnchant
    {
        public override Color nameColor => Color.Green;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<LifeBloomEffect>(Item);
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LifeBloomEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.MuspelheimHeader>();
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void LifeBloomKey()
        {
            int treeType = ModContent.ProjectileType<LifeBloomTree>();
            if (Player.ownedProjectileCounts[treeType] > 0)
            {
                Player.KillOwnedProjectilesOfType(treeType);
                Player.KillOwnedProjectilesOfType(ModContent.ProjectileType<LifeBloomBallista>());
            }
            else
            {
                Projectile.NewProjectileDirect(Player.GetSource_EffectItem<LifeBloomEffect>(), Player.Center, Vector2.Zero, ModContent.ProjectileType<LifeBloomTree>(), 0, 0f, Player.whoAmI); 
            }
        }
    }
}