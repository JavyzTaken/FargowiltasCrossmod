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
using FargowiltasSouls.Content.Projectiles.Masomode;
using CalamityMod.Items.Accessories;
using Terraria.Localization;


namespace FargowiltasCrossmod.Content.Calamity.Items
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamityEffectAdder : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
                //angel treads effects for aeolus boots
                if (item.type == ModContent.ItemType<AeolusBoots>())
                {
                    ModContent.GetInstance<AngelTreads>().UpdateAccessory(player, hideVisual);
                }
                //angel treads effect for supersonic
                if (item.type == ModContent.ItemType<SupersonicSoul>())
                {
                    ModContent.GetInstance<AngelTreads>().UpdateAccessory(player, hideVisual);

                }
                //aeolus effects to celestial tracers
                if (item.type == ModContent.ItemType<TracersCelestial>())
                {
                    ModContent.GetInstance<AeolusBoots>().UpdateAccessory(player, hideVisual);
                }
                if (item.type == ModContent.ItemType<ColossusSoul>())
                {
                    ModContent.GetInstance<TheAmalgam>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<RampartofDeities>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<AsgardianAegis>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<TheCamper>().UpdateAccessory(player, hideVisual);
                }
                if (item.type == ModContent.ItemType<BerserkerSoul>())
                {
                    ModContent.GetInstance<ReaperToothNecklace>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<ElementalGauntlet>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<BadgeofBravery>().UpdateAccessory(player, hideVisual);
                }
                if (item.type == ModContent.ItemType<ArchWizardsSoul>())
                {
                    ModContent.GetInstance<EtherealTalisman>().UpdateAccessory(player, hideVisual);

                }
                if (item.type == ModContent.ItemType<SnipersSoul>())
                {
                    ModContent.GetInstance<ElementalQuiver>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<QuiverofNihility>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<DaawnlightSpiritOrigin>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<DynamoStemCells>().UpdateAccessory(player, hideVisual);
                }
                if (item.type == ModContent.ItemType<ConjuristsSoul>())
                {
                    ModContent.GetInstance<Nucleogenesis>().UpdateAccessory(player, hideVisual);

                }
                if (item.type == ModContent.ItemType<TrawlerSoul>())
                {
                    ModContent.GetInstance<AbyssalDivingSuit>().UpdateAccessory(player, hideVisual);

                }
            }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            string key = "Mods.FargowiltasCrossmod.Items.AddedEffects.";
            //aeolus boots has angel treads
            if (item.type == ModContent.ItemType<AeolusBoots>() && !item.social)
            {
                tooltips.Insert(4, new TooltipLine(Mod, "ATEffect", Language.GetTextValue(key + "AngelTreads")));
            }
            if (item.type == ModContent.ItemType<SupersonicSoul>() && !item.social)
            {
                tooltips.Insert(4, new TooltipLine(Mod, "CalSSSEffects", Language.GetTextValue(key + "AngelTreads")));
            }
            //Celestial tracers has aeolus boots
            if (item.type == ModContent.ItemType<TracersCelestial>() && !item.social)
            {
                tooltips.Insert(11, new TooltipLine(Mod, "CalSSSEffects", Language.GetTextValue(key + "AeolusBoots")));
            }
            //Colossus Soul
            if (item.type == ModContent.ItemType<ColossusSoul>() && !item.social)
            {
                tooltips.Insert(11, new TooltipLine(Mod, "CalColossusSoul", Language.GetTextValue(key + "CalamityColossus")));
            }
            
            if (item.type == ModContent.ItemType<BerserkerSoul>() && !item.social)
            {
                tooltips.Insert(9, new TooltipLine(Mod, "CalBerserkerSoul", Language.GetTextValue(key + "CalamityBerserker")));
            }
            
            if (item.type == ModContent.ItemType<ArchWizardsSoul>() && !item.social)
            {
                tooltips.Insert(8, new TooltipLine(Mod, "CalWizardSoul", Language.GetTextValue(key + "CalamityWizard")));
            }
            
            if (item.type == ModContent.ItemType<SnipersSoul>() && !item.social)
            {
                tooltips.Insert(8, new TooltipLine(Mod, "CalSniperSoul", Language.GetTextValue(key + "CalamitySniper")));
            }
            
            if (item.type == ModContent.ItemType<ConjuristsSoul>() && !item.social)
            {
                tooltips.Insert(7, new TooltipLine(Mod, "CalConjurSoul", Language.GetTextValue(key + "CalamityConjurist")));
            }
            
            if (item.type == ModContent.ItemType<TrawlerSoul>() && !item.social)
            {
                tooltips.Insert(8, new TooltipLine(Mod, "CalFishSoul", Language.GetTextValue(key + "CalamityTrawler")));
            }
            
            if (item.type == ModContent.ItemType<UniverseSoul>() && !item.social)
            {
                tooltips.Insert(15, new TooltipLine(Mod, "CalUniverseSoul",
                    Language.GetTextValue(key + "CalamityBerserker") + "\n" +
                    Language.GetTextValue(key + "CalamitySniper") + "\n" +
                    Language.GetTextValue(key + "CalamityWizard") + "\n" +
                    Language.GetTextValue(key + "CalamityConjurist")));
            }
            
            if (item.type == ModContent.ItemType<DimensionSoul>() && !item.social)
            {
                tooltips.Insert(21, new TooltipLine(Mod, "CalDimensionSoul",
                    Language.GetTextValue(key + "CalamityColossus") + "\n" +
                    Language.GetTextValue(key + "AngelTreads") + "\n" +
                    Language.GetTextValue(key + "CalamityTrawler")));
            }
        }
    }
}
