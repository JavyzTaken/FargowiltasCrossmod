using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Items.EarlyMagic;
using ThoriumMod.Items.Icy;
using ThoriumMod.Items.BardItems;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class IcyEnchant : BaseSynergyEnchant<DepthDiverEffect>
    {
        public override Color nameColor => Color.DarkBlue;
        internal override int SynergyEnch => ModContent.ItemType<DepthDiverEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<IcyEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcyHeadgear>()
                .AddIngredient<IcyMail>()
                .AddIngredient<IcyGreaves>()
                .AddIngredient<Flurry>()
                .AddIngredient<IceShaver>()
                .AddIngredient<IcyPiccolo>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class IcyEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.JotunheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<IcyEnchant>();
    }
}
