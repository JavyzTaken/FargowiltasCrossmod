using FargowiltasSouls.Content.Items.Accessories.Forces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using CalamityMod.Items.Materials;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Assets.Particles;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using CalamityMod;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ElementsForce : BaseForce
    {
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<ElementsForceEffect>()];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Purple;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ElementsForceEffect>(Item);
            player.AddEffect<AerospecJumpEffect>(Item);
            player.AddEffect<HydrothermicEffect>(Item);
            player.AddEffect<AerospecJumpEffect>(Item);
            player.AddEffect<DaedalusEffect>(Item);
            if (player.CalamityAddon().ReaverToggle)
                player.FargoSouls().WingTimeModifier += 0.25f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<AerospecEnchant>();
            recipe.AddIngredient<DaedalusEnchant>();
            recipe.AddIngredient<ReaverEnchant>();
            recipe.AddIngredient<HydrothermicEnchant>();
            recipe.AddTile(ModContent.TileType<Fargowiltas.Items.Tiles.CrucibleCosmosSheet>());
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ElementsHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<ElementsForce>();
        public override float Priority => 0.91f;
    }
    public class ElementsForceEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            var addon = player.CalamityAddon();
            addon.ReaverToggle = !addon.ReaverToggle;
            SoundEngine.PlaySound(SoundID.Item4, player.Center);
            Color color = addon.ReaverToggle ? Color.LightSteelBlue : Color.Orange;
            for (int i = 0; i < 14; i++)
            {
                ReaverSpark spark = new(new Vector2(player.Center.X + Main.rand.NextFloat(-10, 10), player.Center.Y + Main.rand.NextFloat(-10, 10)), Main.rand.NextVector2Circular(4, 4),
                    color, 0.3f, 20, 10, player.whoAmI);
                spark.Spawn();
            }
        }
        public override void PostUpdateEquips(Player player)
        {
            var addon = player.CalamityAddon();
            if (addon.ReaverToggle) // blizzard mode
            {
                player.lifeRegen += 15;
                player.moveSpeed += 0.3f;
                if (player.miscCounter % 3 == 2 && player.dashDelay > 0)
                    player.dashDelay--;
                player.Calamity().reaverSpeed = true;
            }
            else // magma mode
            {
                player.Calamity().reaverSpeed = true;
                player.endurance += 0.3f;
                player.statDefense += 30;
            }
        }
    }
}
