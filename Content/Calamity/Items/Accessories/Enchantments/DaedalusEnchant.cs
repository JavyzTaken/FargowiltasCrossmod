using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using CalamityMod;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using Terraria.Audio;
using FargowiltasCrossmod.Core.Calamity;
using rail;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasSouls;
using Terraria.GameContent.ItemDropRules;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DaedalusEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Color nameColor => new(132, 212, 246);
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<DaedalusEffect>(Item);
            player.AddEffect<SnowRuffianEffect>(Item);
        }
        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyDaedalusHelms", 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusBreastplate>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusLeggings>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SnowRuffianEnchant>(), 1);
            recipe.AddIngredient(ItemID.IceRod, 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.Wings.StarlightWings>(), 1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DaedalusEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Header ToggleHeader => Header.GetHeader<DevastationHeader>();
        public override int ToggleItemType => ModContent.ItemType<DaedalusEnchant>();
        public override void PreUpdate(Player player)
        {
            
        }
        public override void PostUpdateEquips(Player player)
        {
            int heightMult = 6;
            int forceWingTime = 120;

            if (player.ForceEffect<DaedalusEffect>())
            {
                heightMult = 10;
                if (player.wingTimeMax == 0)
                {
                    int starlightWings = EquipLoader.GetEquipSlot(ModCompatibility.Calamity.Mod, "StarlightWings", EquipType.Wings);
                    player.wings = starlightWings;
                    player.wingsLogic = starlightWings;
                    player.wingTimeMax = forceWingTime;
                    player.wingAccRunSpeed = player.GetWingStats(starlightWings).AccRunAccelerationMult;
                    player.noFallDmg = true;
                }
            }
            if (player.wingTime == player.wingTimeMax)
            {
                player.CalamityAddon().DaedalusHeight = (int)player.Center.Y - player.wingTimeMax * heightMult;
            }
            if (player.wingTime > 0)
            {
                player.wingTime = (player.Center.Y - player.CalamityAddon().DaedalusHeight)/heightMult;
            }
        }
    }
}
