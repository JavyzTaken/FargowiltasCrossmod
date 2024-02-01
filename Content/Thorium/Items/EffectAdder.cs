using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories.Wings;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.Toggler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items.Terrarium;
using ThoriumMod.Utilities;
using FargowiltasSouls.Content.Projectiles.Masomode;
using CalamityMod.Items.Accessories;
using ThoriumMod.Items.Donate;
using ThoriumMod.Items.BasicAccessories;
using ThoriumMod.Items.BossLich;
using ThoriumMod.Items.Depths;
using ThoriumMod.Items.NPCItems;
using ThoriumMod.Items.MagicItems;
using ThoriumMod.Items.SummonItems;
using ThoriumMod.Items.ThrownItems;
using Terraria.Localization;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;

namespace FargowiltasCrossmod.Content.Thorium.Items
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ThoriumEffectAdder : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            
            //Add aeolus boots effects to terrarium boots
            if (item.type == ModContent.ItemType<TerrariumParticleSprinters>())
            {
                ModContent.GetInstance<AeolusBoots>().UpdateAccessory(player, hideVisual);
            }
            //add air walker boots, survivalist boots, and weighted winglets effects to super sonic soul
            if (item.type == ModContent.ItemType<SupersonicSoul>())
            {
                ModContent.GetInstance<AirWalkers>().UpdateAccessory(player, hideVisual);
                ModContent.GetInstance<SurvivalistBoots>().UpdateAccessory(player, hideVisual);
                ModContent.GetInstance<WeightedWinglets>().UpdateAccessory(player, hideVisual);
            }
            if (item.type == ModContent.ItemType<ColossusSoul>())
            {
                    ModContent.GetInstance<SweetVengeance>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<ObsidianScale>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<TerrariumDefender>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<Phylactery>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<HeartOfStone>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<SpinyShell>().UpdateAccessory(player, hideVisual);
            }
            if (item.type == ModContent.ItemType<BerserkerSoul>())
            {
                ModContent.GetInstance<RapierBadge>().UpdateAccessory(player, hideVisual);
            }
            if (item.type == ModContent.ItemType<ArchWizardsSoul>())
            {
                ModContent.GetInstance<MurkyCatalyst>().UpdateAccessory(player, hideVisual);

            }
            if (item.type == ModContent.ItemType<SnipersSoul>())
            {
                ModContent.GetInstance<ConcussiveWarhead>().UpdateAccessory(player, hideVisual);

            }
            if (item.type == ModContent.ItemType<ConjuristsSoul>())
            {
                ModContent.GetInstance<CrystalScorpion>().UpdateAccessory(player, hideVisual);
                ModContent.GetInstance<YumasPendant>().UpdateAccessory(player, hideVisual);
            }
            if (item.type == ModContent.ItemType<UniverseSoul>())
            {
                player.ThrownVelocity += 0.15f;
                ModContent.GetInstance<ThrowingGuideVolume3>().UpdateAccessory(player, hideVisual);
                ModContent.GetInstance<MermaidCanteen>().UpdateAccessory(player, hideVisual);
                ModContent.GetInstance<DeadEyePatch>().UpdateAccessory(player, hideVisual);
            }
            if (item.type == ModContent.ItemType<WorldShaperSoul>())
            {
                player.AddEffect<GeodeEffect>(item);
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            
            string key = "Mods.FargowiltasCrossmod.Items.AddedEffects.";
            
            //terrarium boots has aeolus boots
            if (item.type == ModContent.ItemType<TerrariumParticleSprinters>() && !item.social)
            {
                tooltips.Insert(7, new TooltipLine(Mod, "AeolusEffects", Language.GetTextValue(key + "AeolusBoots")));
            }
            //super sonic soul
            if (item.type == ModContent.ItemType<SupersonicSoul>() && !item.social)
            {
                tooltips.Insert(4, new TooltipLine(Mod, "ThoriumSSSEffects", Language.GetTextValue(key + "ThoriumSuperSonic")));
            }
            
            
            if (item.type == ModContent.ItemType<ColossusSoul>() && !item.social)
            {
                tooltips.Insert(11, new TooltipLine(Mod, "ThorColossusSoul", Language.GetTextValue(key + "ThoriumColossus")));
            }
            
            if ( item.type == ModContent.ItemType<BerserkerSoul>() && !item.social)
            {
                tooltips.Insert(9, new TooltipLine(Mod, "ThorBerserkerSoul", Language.GetTextValue(key + "ThoriumBerserker")));
            }
            
            if (item.type == ModContent.ItemType<ArchWizardsSoul>() && !item.social)
            {
                tooltips.Insert(8, new TooltipLine(Mod, "ThorWizardSoul", Language.GetTextValue(key + "ThoriumWizard")));
            }
            
            if ( item.type == ModContent.ItemType<SnipersSoul>() && !item.social)
            {
                tooltips.Insert(8, new TooltipLine(Mod, "ThorSniperSoul", Language.GetTextValue(key + "ThoriumSniper")));
            }
            
            if ( item.type == ModContent.ItemType<ConjuristsSoul>() && !item.social)
            {
                tooltips.Insert(7, new TooltipLine(Mod, "ThorConjurSoul", Language.GetTextValue(key + "ThoriumConjurist")));
            }
            
            if ( item.type == ModContent.ItemType<UniverseSoul>() && !item.social)
            {
                tooltips.Insert(15, new TooltipLine(Mod, "ThorUniverseSoul", Language.GetTextValue(key + "ThoriumUniverse") + "\n" +
                    Language.GetTextValue(key + "ThoriumBerserker") + "\n" +
                    Language.GetTextValue(key + "ThoriumSniper") + "\n" +
                    Language.GetTextValue(key + "ThoriumWizard") + "\n" +
                    Language.GetTextValue(key + "ThoriumConjurist")));
            }
            
            if ( item.type == ModContent.ItemType<DimensionSoul>() && !item.social)
            {
                tooltips.Insert(21, new TooltipLine(Mod, "ThorDimensionSoul",
                    Language.GetTextValue(key + "ThoriumColossus") + "\n" +
                    Language.GetTextValue(key + "ThoriumSuperSonic")));
            }
        }
    }
}
