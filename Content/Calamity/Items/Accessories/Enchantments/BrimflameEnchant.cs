using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasSouls;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.ModPlayers;
using FargowiltasSouls.Core.ModPlayers;
using Terraria.Localization;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod;
using CalamityMod.Items.Armor.Brimflame;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using Terraria.Audio;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Armor.Bloodflare;
using FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Content.UI.Elements;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BrimflameEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
            //return true;
        }
        public override Color nameColor => new Color(240, 100, 75);
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 40000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<BrimflameEffect>(Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<BrimflameEffect>(item);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<BrimflameScowl>();
            recipe.AddIngredient<BrimflameRobes>(1);
            recipe.AddIngredient<BrimflameBoots>(1);
            recipe.AddIngredient<BrimroseStaff>(1);
            recipe.AddIngredient<SeethingDischarge>(1);
            recipe.AddIngredient<Brimblade>(1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BrimflameEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<BrimflameEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            CalDLCAddonPlayer modplayer = player.CalamityAddon();
            
            if (modplayer.BrimflameShootingTimer > 0 )
            {
                for (int i = 0; i < 10; i++)
                {
                    Projectile.NewProjectileDirect(player.GetSource_EffectItem<BrimflameEffect>(), player.Center, new Vector2(20, 0).RotatedBy(MathHelper.ToRadians(360f / 10 * i + modplayer.BrimflameShootingTimer*10)), ModContent.ProjectileType<BrimflameBurst>(), player.statDefense*player.statDefense/20*2, 1, player.whoAmI);
                }
            }
            if (modplayer.BrimflameDefenseTimer > 0 && modplayer.BrimflameDefenseTimer % 100 == 0)
            {
                CombatText.NewText(player.Hitbox, Color.LightGray, Language.GetTextValue("Mods.FargowiltasCrossmod.Items.BrimflameEnchant.DefenseUp", 10 / (player.ForceEffect<BrimflameEffect>() ? 2 : 1)));
            }

            player.statDefense -= (int)(modplayer.BrimflameDefenseTimer / 10) / (player.ForceEffect<BrimflameEffect>() ? 2 : 1);
        }
        public static void BrimflameTrigger(Player player)
        {
            
            CalDLCAddonPlayer modplayer = player.CalamityAddon();
            if (modplayer.BrimflameDefenseTimer == 0)
            {
                
                CombatText.NewText(player.Hitbox, Color.LightGray, Language.GetTextValue("Mods.FargowiltasCrossmod.Items.BrimflameEnchant.DefenseDown", player.statDefense));
                SoundEngine.PlaySound(BloodflareHeadRanged.ActivationSound, player.Center);
                Projectile.NewProjectileDirect(player.GetSource_EffectItem<BrimflameEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), 0, 0, player.whoAmI, 0, 5);
                modplayer.BrimflameDefenseTimer = player.statDefense * 10;
                modplayer.BrimflameShootingTimer = player.statDefense / 5;
                modplayer.MaxDefense = player.statDefense;
                CooldownBarManager.Activate("BrimflameEnchantCooldown", ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Items/Accessories/Enchantments/BrimflameEnchant").Value, Color.Lerp(Color.OrangeRed, Color.Red, 0.75f),
                () =>(modplayer.BrimflameDefenseTimer/10f) / (modplayer.MaxDefense+1), activeFunction: () => player.HasEffect<BrimflameEffect>());

            }
        }
    }
   
}
