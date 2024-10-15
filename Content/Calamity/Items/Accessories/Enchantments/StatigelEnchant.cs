using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using System.Security.Policy;
using Terraria.Graphics.Renderers;
using CalamityMod.Graphics.Renderers;
using Terraria.Graphics;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using CalamityMod.Projectiles.Turret;
using CalamityMod.Particles;
using Terraria.Audio;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasSouls;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod.Items.Weapons.Rogue;
using FargowiltasSouls.Content.UI.Elements;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("StatigelEnchantment")]
    public class StatigelEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Color nameColor => new(89, 170, 204);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = 50000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<StatigelEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyStatisHelms");
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Statigel.StatigelArmor>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Statigel.StatigelGreaves>());
            recipe.AddIngredient(ItemID.GolfBallDyedPurple);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.OverloadedBlaster>());
            recipe.AddIngredient(ModContent.ItemType<BouncySpikyBall>(), 300);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class StatigelEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Header ToggleHeader => Header.GetHeader<CalamitySoulHeader>();
        public override int ToggleItemType => ModContent.ItemType<StatigelEnchant>();
        
        public override void PostUpdateEquips(Player player)
        {
            float DamageFormula(float x) => x / MathF.Sqrt(x * x + 1);
            float x = player.velocity.Length() / 8f;
            float bonusMultiplier = DamageFormula(x); // This function approaches y = 1 as x approaches infinity.
            float bonusDamage = bonusMultiplier * 0.35f;
            if (player.ForceEffect<StatigelEffect>())
                bonusDamage *= 2;
            player.GetDamage(DamageClass.Generic) += bonusDamage;

            CooldownBarManager.Activate("StatigelDamage", ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Items/Accessories/Enchantments/StatigelEnchant").Value, new Color(89, 170, 204), 
                () => DamageFormula(Main.LocalPlayer.velocity.Length() / 8f), true, 60, () => player.HasEffect<StatigelEffect>());

            if (player.ForceEffect<StatigelEffect>())
            {
                player.runAcceleration *= 0.9f;
                //player.maxRunSpeed *= 1.3f;
                player.accRunSpeed *= 1.2f;
                player.runSlowdown *= 0.5f;
            }
            else
            {
                player.runAcceleration *= 0.8f;
                player.accRunSpeed *= 1.12f;
                player.runSlowdown *= 0.5f;
            }
            
        }
       
    }
}
