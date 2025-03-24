using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.FathomSwarmer;
using CalamityMod.Items.Weapons.Summon;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.ModPlayers;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class FathomSwarmerEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
            return FargowiltasCrossmod.EnchantLoadingEnabled;
            //return true;
        }
        public override Color nameColor => new Color(153, 200, 193);
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 40000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<FathomSwarmerEffect>(Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<FathomSwarmerEffect>(item);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<FathomSwarmerVisage>(1);
            recipe.AddIngredient<FathomSwarmerBreastplate>(1);
            recipe.AddIngredient<FathomSwarmerBoots>(1);
            recipe.AddIngredient<DreadmineStaff>(1);
            recipe.AddIngredient<OrthoceraShell>(1);
            recipe.AddIngredient<LumenousAmulet>(1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class FathomSwarmerEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
            return FargowiltasCrossmod.EnchantLoadingEnabled;
            //return true;
        }
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<FathomSwarmerEnchant>();

        public override void PostUpdateEquips(Player player)
        {
       
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FathomAngler>()] == 0)
            {
                Projectile p = Projectile.NewProjectileDirect(player.GetSource_EffectItem<FathomSwarmerEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<FathomAngler>(), 0, 0, player.whoAmI);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p.whoAmI);
                }
            }
        }
    }
}
