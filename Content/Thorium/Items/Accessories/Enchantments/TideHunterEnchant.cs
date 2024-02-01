using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class TideHunterEnchant : BaseEnchant
    {
        public override Color nameColor => new(0, 85, 85);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TideHunterEffect>(Item);
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class TideHunterEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.JotunheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<TideHunterEnchant>();
        public override bool ExtraAttackEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            var DLCPlayer = player.ThoriumDLC();
            if (!player.FargoSouls().IsInADashState) return;

            if (!DLCPlayer.WasInDashState)
            {
                Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, player.velocity, ModContent.ProjectileType<TideTurnerWave>(), 125, 3f, player.whoAmI, 6);
            }
        }
    }
}

