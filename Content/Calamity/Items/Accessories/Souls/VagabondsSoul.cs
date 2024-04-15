using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Rogue;
using Fargowiltas.Items.Tiles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Essences;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class VagabondsSoul : BaseSoul
    {
        protected override Color? nameColor => new Color(217, 144, 67);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RogueDamageClass>() += 0.3f;
            player.Calamity().rogueVelocity += 0.15f;
            player.GetCritChance<RogueDamageClass>() += 0.15f;
            if (player.AddEffect<NanotechEffect>(Item))
            {
                ModContent.GetInstance<Nanotech>().UpdateAccessory(player, hideVisual);
            }
            if (player.AddEffect<EclipseMirrorEffect>(Item))
            {
                ModContent.GetInstance<EclipseMirror>().UpdateAccessory(player, hideVisual);
            }
            if (player.AddEffect<DragonScalesEffect>(Item))
            {
                ModContent.GetInstance<DragonScales>().UpdateAccessory(player, hideVisual);
            }
            if (player.AddEffect<VeneratedLocketEffect>(Item))
            {
                ModContent.GetInstance<VeneratedLocket>().UpdateAccessory(player, hideVisual);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OutlawsEssence>()
                .AddIngredient<EclipseMirror>()
                .AddIngredient<Nanotech>()
                .AddIngredient<DragonScales>()
                .AddIngredient<VeneratedLocket>()

                .AddIngredient<WaveSkipper>()
                .AddIngredient<GraveGrimreaver>()
                .AddIngredient<SpentFuelContainer>()
                .AddIngredient<TheSyringe>()
                .AddIngredient<RegulusRiot>()
                .AddIngredient<CelestialReaper>()
                .AddIngredient<Valediction>()
                .AddIngredient<ExecutionersBlade>()
                .AddIngredient<SearedPan>()

                .AddTile<CrucibleCosmosSheet>()
                .Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public abstract class VagabondEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<VagabondsSoulHeader>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class NanotechEffect : VagabondEffect
    {
        public override int ToggleItemType => ModContent.ItemType<Nanotech>();
        public override bool IgnoresMutantPresence => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EclipseMirrorEffect : VagabondEffect
    {
        public override int ToggleItemType => ModContent.ItemType<EclipseMirror>();
        public override bool IgnoresMutantPresence => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DragonScalesEffect : VagabondEffect
    {
        public override int ToggleItemType => ModContent.ItemType<EclipseMirror>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class VeneratedLocketEffect : VagabondEffect
    {
        public override int ToggleItemType => ModContent.ItemType<VeneratedLocket>();
    }
}
