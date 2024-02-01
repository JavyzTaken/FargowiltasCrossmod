using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class BronzeEnchant : BaseSynergyEnchant<GraniteEffect>
    {
        public override Color nameColor => Color.Gold;

        internal override int SynergyEnch => ModContent.ItemType<GraniteEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<BronzeEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CopperEnchant>(3)
                .AddIngredient<TinEnchant>()
                .AddTile(TileID.Hellforge)
                .Register()
                ;
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class BronzeEffect : SynergyEffect<GraniteEffect>
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.SvartalfheimHeader>();
        public override bool ExtraAttackEffect => true;
        public override int ToggleItemType => ModContent.ItemType<BronzeEnchant>();

        public override void TryAdditionalAttacks(Player player, int damage, DamageClass damageType)
        {
            var DLCPlayer = player.ThoriumDLC();

            if (DLCPlayer.BronzeCD <= 0 && player.whoAmI == Main.myPlayer)
            {
                Vector2 spawnPos = player.Center + Vector2.Normalize(Main.MouseWorld - player.Center).RotatedBy(MathHelper.PiOver2 * (Main.rand.NextBool() ? 1 : -1)) * Main.rand.Next(32, 64);
                bool synergy = SynergyActive(player);

                Projectile.NewProjectile(GetSource_EffectItem(player),
                                            spawnPos,
                                            Vector2.Normalize(Main.MouseWorld - spawnPos) * 12f,
                                            ModContent.ProjectileType<Projectiles.DLCLightStrike>(),
                                            (int)(damage * 0.6f),
                                            0.5f,
                                            player.whoAmI,
                                            4f,
                                            synergy ? 1f : 0f);
            }
        }
    }
}