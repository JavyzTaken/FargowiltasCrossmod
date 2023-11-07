using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    public class BronzeEnchant : BaseSynergyEnchant
    {
        protected override Color nameColor => Color.Gold;
        internal override bool SynergyActive(CrossplayerThorium DLCPlayer) => DLCPlayer.BronzeEnch && DLCPlayer.GraniteEnch;

        protected override Color SynergyColor1 => Color.DarkBlue;
        protected override Color SynergyColor2 => Color.DarkBlue with { A = 0 };
        internal override int SynergyEnch => ModContent.ItemType<GraniteEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.BronzeEnch = true;
            DLCPlayer.BronzeEnchItem = Item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FargowiltasSouls.Content.Items.Accessories.Enchantments.CopperEnchant>()
                .AddIngredient<FargowiltasSouls.Content.Items.Accessories.Enchantments.TinEnchant>()
                .AddTile(TileID.Hellforge)
                .Register()
                ;
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void BronzeEffect(Item item, Vector2 pos, int damage)
        {
            Vector2 spawnPos = Player.Center + Vector2.Normalize(Main.MouseWorld - Player.Center).RotatedBy(MathHelper.PiOver2 * (Main.rand.NextBool() ? 1 : -1)) * Main.rand.Next(32, 64);
            bool synergy = SynergyEffect(BronzeEnchItem.type);

            if (Main.rand.NextBool(synergy ? 8 : 4))
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(BronzeEnchItem),
                                         spawnPos,
                                         Vector2.Normalize(Main.MouseWorld - spawnPos) * 12f,
                                         ModContent.ProjectileType<Projectiles.DLCLightStrike>(),
                                         (int)(damage * 0.6f),
                                         0.5f,
                                         Player.whoAmI,
                                         4f,
                                         synergy ? 1f : 0f);
            }
        }
    }
}