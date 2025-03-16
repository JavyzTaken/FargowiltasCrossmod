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
using FargowiltasSouls.Content.UI.Elements;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [LegacyName("AerospecEnchantment")]
    public class AerospecEnchant : BaseEnchant
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
            Item.value = 40000;
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
            recipe.AddIngredient<CalamityMod.Items.Weapons.Magic.VeeringWind>(1);
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
        public override Header ToggleHeader => Header.GetHeader<ElementsHeader>();
        public override int ToggleItemType => ModContent.ItemType<AerospecEnchant>();
        
        public override void PostUpdateEquips(Player player)
        {
            var mplayer = player.GetModPlayer<CalDLCAddonPlayer>();
            if (player.HasEffect<ElementsForceEffect>())
            {
                if (mplayer.ReaverToggle)
                {
                    if (player.velocity.Y != 0)
                    {
                        mplayer.ElementsAirTime++;
                        static float DamageFormula(float x) => x / MathF.Sqrt(x * x + 1);
                        float x = mplayer.ElementsAirTime / 420f;
                        float bonusMultiplier = DamageFormula(x); // This function approaches y = 1 as x approaches infinity.
                        float bonusDamage = bonusMultiplier * 0.5f;
                        player.GetDamage(DamageClass.Generic) += bonusDamage;

                        CooldownBarManager.Activate("AerospecDamage", ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Items/Accessories/Enchantments/AerospecEnchant").Value, new Color(153, 200, 193),
                        () => LumUtils.Saturate(DamageFormula(Main.LocalPlayer.CalamityAddon().ElementsAirTime / 420f)), true, activeFunction: player.HasEffect<ElementsForceEffect>);
                    }
                    else
                        mplayer.ElementsAirTime = 0;
                }
                return;
            }
            int critPerJump = player.ForceEffect<AerospecJumpEffect>() ? 10 : 5;
            int maxCritJumps = 6;


            float extraCrit = (mplayer.NumJumpsUsed > maxCritJumps ? maxCritJumps : mplayer.NumJumpsUsed) * critPerJump;
            player.GetCritChance(DamageClass.Generic) += extraCrit;
            player.GetDamage(DamageClass.Summon) += extraCrit / 100f;
            for (int i = 0; i < extraCrit / 5; i++)
            {
                if (Main.rand.NextBool())
                    Dust.NewDustDirect(player.position, player.width, player.height, DustID.UnusedWhiteBluePurple, player.velocity.X, player.velocity.Y);
            }
            //if (Collision.SolidCollision(player.BottomLeft, player.width, 6, true) && player.velocity.Y == 0)
            //{
            //    mplayer.AeroCritBoost = 0;
            //}


            if (mplayer.NumJumpsUsed > 0 && player.mount != null && player.mount.Active)
                ResetAeroCrit(player);
                
            
            if (player.jump != 0 && mplayer.AllowJumpsUsedInc && mplayer.NumJumpsUsed < maxCritJumps)
            {
                mplayer.NumJumpsUsed++;
                mplayer.AllowJumpsUsedInc = false;
                CombatText.NewText(player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasCrossmod.Items.AerospecEnchant.CritUp", critPerJump));
            }
            mplayer.AllowJumpsUsedInc = player.jump == 0;
        }

        public static void ResetAeroCrit(Player player)
        {
            CalDLCAddonPlayer addonPlayer = player.CalamityAddon();
            if (addonPlayer.NumJumpsUsed > 0)
            {
                int critPerJump = player.ForceEffect<AerospecJumpEffect>() ? 10 : 5;
                int critLost = critPerJump * addonPlayer.NumJumpsUsed;
                addonPlayer.NumJumpsUsed = 0;
                CombatText.NewText(player.Hitbox, Color.OrangeRed, Language.GetTextValue("Mods.FargowiltasCrossmod.Items.AerospecEnchant.CritReset", critLost), true);
            }
        }
    }
   
}
