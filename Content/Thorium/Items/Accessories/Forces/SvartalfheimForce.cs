using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Forces
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SvartalfheimForce : BaseForce
    {

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();

            DLCPlayer.DuraSteelEnch = true;
            DLCPlayer.SteelEnchItem = Item;

            DLCPlayer.ConduitEnch = true;
            DLCPlayer.ConduitEnchItem = Item;
            DLCPlayer.AstroEnch = true;
            DLCPlayer.AstroEnchItem = Item;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.ConduitSaucer>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.ConduitSaucer>(), 0, 0, player.whoAmI);
            }

            DLCPlayer.TitanEnch = true;
            player.GetArmorPenetration(DamageClass.Generic) += (player.statDefense * 0.15f);
            player.statDefense *= 0.8f;

            DLCPlayer.BronzeEnch = true;
            DLCPlayer.BronzeEnchItem = Item;

            DLCPlayer.GraniteEnch = true;
            DLCPlayer.GraniteEnchItem = Item;
        }
    }
}