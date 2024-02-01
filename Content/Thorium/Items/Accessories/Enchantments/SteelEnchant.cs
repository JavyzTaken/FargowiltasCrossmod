using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasSouls;
using ThoriumMod.Items.Steel;
using ThoriumMod.Items.Donate;
using Terraria.ID;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SteelEnchant : BaseEnchant
    {
        public override Color nameColor => Color.DarkGray;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SteelEffect>(Item);
            player.ThoriumDLC().SteelTeir = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelHelmet>()
                .AddIngredient<SteelChestplate>()
                .AddIngredient<SteelGreaves>()
                .AddIngredient<ThoriumMod.Items.NPCItems.EbonHammer>()
                .AddIngredient<BlacksmithsBarrier>()
                .AddIngredient<ThoriumMod.Items.HealerItems.WarForger>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SteelEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.SvartalfheimHeader>();
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void ParryKey()
        {
            if (Main.myPlayer != Player.whoAmI || Player.dead || !Player.active) return;
            int teir = Player.ThoriumDLC().SteelTeir;

            if (Player.ForceEffect<SteelEffect>()) teir++;

            if (teir == 0)
            {
                Main.NewText("This shouldn't be able to happen? tell ghoose");
                return;
            }

            if (!Player.HasBuff<SteelParry_CD>())
            {
                Player.AddBuff(ModContent.BuffType<SteelParry_CD>(), 900);

                float rot = Player.Center.DirectionTo(Main.MouseWorld).ToRotation();
                Projectile.NewProjectile(Player.GetSource_EffectItem<SteelEffect>(), Player.Center, Vector2.Zero, ModContent.ProjectileType<SteelParry>(), 0, 0, Player.whoAmI, teir, rot);
            }
        }
    }
}