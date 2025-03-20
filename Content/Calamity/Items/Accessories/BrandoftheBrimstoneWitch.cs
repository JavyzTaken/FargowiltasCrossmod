using CalamityMod.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using FargowiltasSouls.Content.Items.Materials;
using Terraria.ID;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using Fargowiltas.Items.Tiles;
using CalamityMod.Rarities;
using Terraria.Localization;
using CalamityMod.Items.Materials;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Core;
using Terraria.DataStructures;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasCrossmod.Content.Calamity.Toggles;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrandoftheBrimstoneWitch : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(8, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.value = 1000000;
            Item.rare = ModContent.RarityType<Violet>();
            Item.defense = 50;

        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.AddEffect<HeartoftheElementsEffect>(Item))
                ModContent.GetInstance<HeartoftheElements>().UpdateAccessory(player, hideVisual);

            //if (player.AddEffect<OccultSkullCrownEffect>(Item))
            //    ModContent.GetInstance<OccultSkullCrown>().UpdateAccessory(player, hideVisual);

            if (player.AddEffect<PurityEffect>(Item))
                ModContent.GetInstance<Radiance>().UpdateAccessory(player, hideVisual);

            if (player.AddEffect<TheSpongeEffect>(Item))
                ModContent.GetInstance<TheSponge>().UpdateAccessory(player, hideVisual);

            if (player.AddEffect<ChaliceOfTheBloodGodEffect>(Item))
                ModContent.GetInstance<ChaliceOfTheBloodGod>().UpdateAccessory(player, hideVisual);

            if (player.AddEffect<NebulousCoreEffect>(Item))
                ModContent.GetInstance<NebulousCore>().UpdateAccessory(player, hideVisual);

            if (player.AddEffect<YharimsGiftEffect>(Item))
                ModContent.GetInstance<YharimsGift>().UpdateAccessory(player, hideVisual);

            if (player.AddEffect<DraedonsHeartEffect>(Item))
                ModContent.GetInstance<DraedonsHeart>().UpdateAccessory(player, hideVisual);

            if (player.AddEffect<CalamityEffect>(Item))
                ModContent.GetInstance<CalamityMod.Items.Accessories.Calamity>().UpdateAccessory(player, hideVisual);
           
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HeartoftheElements>()
                //.AddIngredient<OccultSkullCrown>()
                .AddIngredient<Radiance>()
                .AddIngredient<TheSponge>()
                .AddIngredient<ChaliceOfTheBloodGod>()
                .AddIngredient<NebulousCore>()
                .AddIngredient<YharimsGift>()
                .AddIngredient<DraedonsHeart>()
                .AddIngredient<CalamityMod.Items.Accessories.Calamity>()
                .AddIngredient<AbomEnergy>(5)
                .AddTile<CrucibleCosmosSheet>()
                .Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public abstract class BotBWEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<BrandoftheBrimstoneWitchHeader>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HeartoftheElementsEffect : BotBWEffect
    {
        public override int ToggleItemType => ModContent.ItemType<HeartoftheElements>();

        public override bool MinionEffect => true;
    }
    /*
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class OccultSkullCrownEffect : BotBWEffect
    {
        public override int ToggleItemType => ModContent.ItemType<OccultSkullCrown>();
    }
    */
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PurityEffect : BotBWEffect
    {
        public override int ToggleItemType => ModContent.ItemType<Radiance>();
        public override bool MutantsPresenceAffects => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class TheSpongeEffect : BotBWEffect
    {
        public override int ToggleItemType => ModContent.ItemType<TheSponge>();
        public override bool MutantsPresenceAffects => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ChaliceOfTheBloodGodEffect : BotBWEffect
    {
        public override int ToggleItemType => ModContent.ItemType<ChaliceOfTheBloodGod>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class NebulousCoreEffect : BotBWEffect
    {
        public override int ToggleItemType => ModContent.ItemType<NebulousCore>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class YharimsGiftEffect : BotBWEffect
    {
        public override int ToggleItemType => ModContent.ItemType<YharimsGift>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DraedonsHeartEffect : BotBWEffect
    {
        public override int ToggleItemType => ModContent.ItemType<DraedonsHeart>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalamityEffect : BotBWEffect
    {
        public override int ToggleItemType => ModContent.ItemType<CalamityMod.Items.Accessories.Calamity>();
    }
}