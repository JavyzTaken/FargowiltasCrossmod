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

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DaedalusEnchantment : BaseEnchant
    {

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
        }
        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyDaedalusHelms", 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusBreastplate>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusLeggings>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SnowRuffianEnchantment>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.Wings.SoulofCryogen>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.Wings.StarlightWings>(), 1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    public class DaedalusEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<DaedalusEnchantment>();
        public override void PostUpdateEquips(Player player)
        {
            if (player.wingTime == 10 && !player.HasCooldown("DaedalusPlatform"))
            {
                int proj = Projectile.NewProjectile(player.GetSource_EffectItem<DaedalusEffect>(), player.Center + new Vector2(0, 20), Vector2.Zero, ModContent.ProjectileType<CrystalPlatform>(), 0, 0, Main.myPlayer);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                }
                SoundEngine.PlaySound(SoundID.Item101, player.Center);
                player.AddCooldown("DaedalusPlatform", 100);
            }
            if (player.wings == 0)
            {
                player.wings = EquipLoader.GetEquipSlot(ModCompatibility.Calamity.Mod, "StarlightWings", EquipType.Wings);
            }
        }
        
    }
}
