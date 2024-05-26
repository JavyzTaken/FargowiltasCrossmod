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

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AerospecEnchantment : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Color nameColor => new Color(153, 200, 193);
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 10);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<AerospecJumpEffect>(Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<AerospecJumpEffect>(item);
            player.Calamity().aeroStone = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyAerospecHelms");
            recipe.AddIngredient<CalamityMod.Items.Armor.Aerospec.AerospecBreastplate>(1);
            recipe.AddIngredient<CalamityMod.Items.Armor.Aerospec.AerospecLeggings>(1);
            recipe.AddIngredient<CalamityMod.Items.Weapons.Rogue.Turbulance>(1);
            recipe.AddIngredient<CalamityMod.Items.Weapons.Magic.SkyGlaze>(1);
            recipe.AddIngredient<CalamityMod.Items.Accessories.AeroStone>(1);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AerospecJumpEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Header ToggleHeader => Header.GetHeader<ExplorationHeader>();
        public override int ToggleItemType => ModContent.ItemType<AerospecEnchantment>();
        
        public override void PostUpdateEquips(Player player)
        {
            int critPerJump = player.ForceEffect<AerospecJumpEffect>() ? 10 : 5;
            int maxCritJumps = 6;

            CalDLCAddonPlayer mplayer = player.GetModPlayer<CalDLCAddonPlayer>();
            float extraCrit = (mplayer.NumJumpsUsed > maxCritJumps ? maxCritJumps : mplayer.NumJumpsUsed) * critPerJump;
            player.GetCritChance(DamageClass.Generic) += extraCrit;
            for (int i = 0; i < extraCrit / 5; i++)
            {
                if (Main.rand.NextBool())
                    Dust.NewDustDirect(player.position, player.width, player.height, DustID.UnusedWhiteBluePurple, player.velocity.X, player.velocity.Y);
            }
            //if (Collision.SolidCollision(player.BottomLeft, player.width, 6, true) && player.velocity.Y == 0)
            //{
            //    mplayer.AeroCritBoost = 0;
            //}
           
            
            
                
            
            if (player.jump != 0 && mplayer.AllowJumpsUsedInc)
            {
                mplayer.NumJumpsUsed++;
                mplayer.AllowJumpsUsedInc = false;
                CombatText.NewText(player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasCrossmod.Items.AerospecEnchantment.CritUp", critPerJump));
            }
            mplayer.AllowJumpsUsedInc = player.jump == 0;
        }
    }
   
}
