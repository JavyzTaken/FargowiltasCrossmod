using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Forces
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SvartalfheimForce : BaseForce
    {

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();

            player.AddEffect<SteelEffect>(Item);
            DLCPlayer.SteelTeir = 3;
            //player.AddEffect<conduitEffect>(Item);
            player.AddEffect<AstroEffect>(Item);
            player.AddEffect<BronzeEffect>(Item);
            player.AddEffect<GraniteEffect>(Item);

            //if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.ConduitSaucer>()] < 1)
            //{
            //    Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.ConduitSaucer>(), 0, 0, player.whoAmI);
            //}

            //DLCPlayer.TitanEnch = true;
            //player.GetArmorPenetration(DamageClass.Generic) += (player.statDefense * 0.15f);
            //player.statDefense *= 0.8f;

        }
    }
}