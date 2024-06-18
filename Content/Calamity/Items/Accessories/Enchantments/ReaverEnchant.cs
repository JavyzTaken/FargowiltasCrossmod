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
using Luminance.Core.Graphics;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("ReaverEnchantment")]
    public class ReaverEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }

        public override Color nameColor => new Color(145, 203, 102);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Lime;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ReaverEffect>(Item);
            player.CalamityAddon().ReaverHide = hideVisual;
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyReaverHelms", 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Reaver.ReaverScaleMail>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Reaver.ReaverCuisses>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Tools.BeastialPickaxe>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.NecklaceofVexation>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.SpelunkersAmulet>(), 1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ReaverEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Header ToggleHeader => Header.GetHeader<DevastationHeader>();
        public override int ToggleItemType => ModContent.ItemType<ReaverEnchant>();
        public override void OnHitByEither(Player player, NPC npc, Projectile proj)
        {
            player.CalamityAddon().ReaverBuff /= 2;
        }
        
        public override void PostUpdateEquips(Player player)
        {
            //Lots of stats here that can be balanced.
            int MaxReaver = 1000;
            float lerper = player.CalamityAddon().ReaverBuff / (float)MaxReaver;
            if (player.ForceEffect<ReaverEffect>())
            {
                player.lifeRegen += (int)MathHelper.Lerp(-20, 35, lerper);
                player.statDefense += (int)MathHelper.Lerp(-30, 30, lerper);
                player.statLifeMax2 += (int)MathHelper.Lerp(-110, 180, lerper);
                player.GetDamage(DamageClass.Generic) *= MathHelper.Lerp(0.6f, 1.4f, lerper);
            }
            else
            {
                player.lifeRegen += (int)MathHelper.Lerp(-10, 20, lerper);
                player.statDefense += (int)MathHelper.Lerp(-10, 20, lerper);
                player.statLifeMax2 += (int)MathHelper.Lerp(-50, 70, lerper);
                player.GetDamage(DamageClass.Generic) *= MathHelper.Lerp(0.8f, 1.2f, lerper);
            }
            if (player.CalamityAddon().ReaverBuff < MaxReaver)
            {
                player.CalamityAddon().ReaverBuff++;
            }
            if (Main.rand.NextBool(30) && !player.CalamityAddon().ReaverHide)
            {
                ReaverSpark spark = new(new Vector2(player.Center.X + Main.rand.NextFloat(-10, 10), player.CalamityAddon().ReaverBuff > 400 ? player.Bottom.Y : player.Top.Y), new Vector2(0, player.CalamityAddon().ReaverBuff > 400 ? -1.6f: 1.6f), Color.Lerp(Color.Red, Color.Green, lerper), 0.3f, 20, 10, player.whoAmI, player.CalamityAddon().ReaverBuff < 400);
                spark.Spawn();
            }
        }
    }
}
