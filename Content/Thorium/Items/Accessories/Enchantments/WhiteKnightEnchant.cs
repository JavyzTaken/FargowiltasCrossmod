using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class WhiteKnightEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Silver;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerThorium>().WhiteKnightEnch = true;

            WhiteKnightEffect(player);
        }

        public static void WhiteKnightEffect(Player player)
        {
            float boost = player.GetModPlayer<FargowiltasSouls.Core.ModPlayers.FargoSoulsPlayer>().ForceEffect(ModContent.ItemType<WhiteKnightEnchant>()) ? 0.1f : 0.05f;
            player.GetDamage(DamageClass.Generic) += boost * player.townNPCs;

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                int teamPlayers = 0;
                foreach (Player other in Main.player)
                {
                    if (other.team == player.team && other.statDefense < player.statDefense && other.Center.Distance(player.Center) < 1440f) teamPlayers++;
                }
                player.GetDamage(DamageClass.Generic) += boost * teamPlayers;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.MagicItems.WhiteKnightMask>()
                .AddIngredient<ThoriumMod.Items.MagicItems.WhiteKnightTabard>()
                .AddIngredient<ThoriumMod.Items.MagicItems.WhiteKnightLeggings>()
                .Register();
        }
    }
}
