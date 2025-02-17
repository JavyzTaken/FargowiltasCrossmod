using System;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Placeables.Furniture.Fountains;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.Furniture;
using Fargowiltas;
using Fargowiltas.Common.Configs;
using Fargowiltas.Items.Misc;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using FargowiltasSouls.Common;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Misc;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCItemChanges : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            CalamityPlayer cplayer = player.Calamity();
            if (item.type == ModContent.ItemType<DeerSinew>())
            {
                player.SetToggleValue<DeerSinewEffect>(false);
            }
            if (item.type == ModContent.ItemType<DeathFruit>())
            {
                if (cplayer.dFruit)
                {
                    cplayer.dFruit = false;
                    player.ConsumedLifeFruit = 20;
                }
                else if (cplayer.eBerry)
                {
                    cplayer.eBerry = false;
                    player.ConsumedLifeFruit = 20;
                }
                else if (cplayer.mFruit)
                {
                    cplayer.mFruit = false;
                    player.ConsumedLifeFruit = 20;
                }
                else if (cplayer.bOrange)
                {
                    cplayer.bOrange = false;
                    player.ConsumedLifeFruit = 20;
                }
            }
            return base.UseItem(item, player);
        }
        public override float UseSpeedMultiplier(Item item, Player player)
        {
            //Mythril stealth fix
            if (player.HasEffect<MythrilEffect>() && item.DamageType == ModContent.GetInstance<RogueDamageClass>() && item.useTime >= item.useAnimation)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                float ratio = Math.Max((float)modPlayer.MythrilTimer / modPlayer.MythrilMaxTime, 0);
                if (ratio > 0.3)
                {
                    return 0.75f;
                }
            }
            return base.UseSpeedMultiplier(item, player);
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            CalamityPlayer calPlayer = player.Calamity();

            if (item.type == ModContent.ItemType<EternitySoul>())
            {
                ModContent.GetInstance<BrandoftheBrimstoneWitch>().UpdateAccessory(player, hideVisual);
            }
            if (item.type == ModContent.ItemType<EternitySoul>() || item.type == ModContent.ItemType<TerrariaSoul>())
            {
                ModContent.GetInstance<ExplorationForce>().UpdateAccessory(player, hideVisual);
            }
            if (calPlayer.HasCustomDash || item.type == ModContent.ItemType<CounterScarf>() || item.type == ModContent.ItemType<EvasionScarf>() || item.type == ModContent.ItemType<OrnateShield>()
                || item.type == ModContent.ItemType<AsgardianAegis>() || item.type == ModContent.ItemType<ElysianAegis>() || item.type == ModContent.ItemType<AsgardsValor>()
                || item.type == ModContent.ItemType<StatisNinjaBelt>() || item.type == ModContent.ItemType<StatisVoidSash>() || item.type == ModContent.ItemType<ShieldoftheHighRuler>()
                || item.type == ModContent.ItemType<DeepDiver>() && player.wet || player.Calamity().plaguebringerPatronSet)
            {
                fargoPlayer.HasDash = true;
            }
            if (item.type == ModContent.ItemType<AngelTreads>())
            {
                if (player.AddEffect<ZephyrJump>(item))
                {
                    player.GetJumpState(ExtraJump.FartInAJar).Enable();
                }
            }
            if (item.type == ModContent.ItemType<AeolusBoots>()) // add angel treads effects
            {
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.angelTreads = true;
                player.accRunSpeed = 7.5f;
                player.moveSpeed += 0.04f; // angel provides 0.12. aeolus provides 0.08. leftover is 0.04
                player.iceSkate = true;
                player.waterWalk = true;
                player.fireWalk = true;
                player.buffImmune[BuffID.OnFire] = true;
            }
            bool dimSoul = item.type == ModContent.ItemType<DimensionSoul>() || item.type == ModContent.ItemType<EternitySoul>();
            bool uniSoul = item.type == ModContent.ItemType<UniverseSoul>() || item.type == ModContent.ItemType<EternitySoul>();

            if (item.type == ModContent.ItemType<SupersonicSoul>() || dimSoul)
            {
                if (player.AddEffect<StatisVoidSashEffect>(item))
                {
                    ModContent.GetInstance<StatisVoidSash>().UpdateAccessory(player, hideVisual);
                }
            }
            if (item.type == ModContent.ItemType<ColossusSoul>() || dimSoul)
            {
                if (player.AddEffect<AmalgamEffect>(item))
                {
                    ModContent.GetInstance<TheAmalgam>().UpdateAccessory(player, hideVisual);
                }
                if (player.AddEffect<AsgardianAegisEffect>(item))
                {
                    ModContent.GetInstance<AsgardianAegis>().UpdateAccessory(player, hideVisual);
                    fargoPlayer.HasDash = true;
                }

                ModContent.GetInstance<RampartofDeities>().UpdateAccessory(player, hideVisual);
            }
            if (item.type == ModContent.ItemType<TrawlerSoul>() || dimSoul)
            {
                if (player.AddEffect<AbyssalDivingSuitEffect>(item))
                {
                    ModContent.GetInstance<AbyssalDivingSuit>().UpdateAccessory(player, hideVisual);
                }
            }
            if (item.type == ModContent.ItemType<WorldShaperSoul>() || dimSoul)
            {
                MarniteEnchant.AddEffects(player, item);
            }

            if (item.type == ModContent.ItemType<BerserkerSoul>() || uniSoul)
            {
                if (player.AddEffect<ElementalGauntletEffect>(item))
                {
                    ModContent.GetInstance<ElementalGauntlet>().UpdateAccessory(player, hideVisual);
                }
            }
            if (item.type == ModContent.ItemType<ArchWizardsSoul>() || uniSoul)
            {
                if (player.AddEffect<EtherealTalismanEffect>(item))
                {
                    ModContent.GetInstance<EtherealTalisman>().UpdateAccessory(player, hideVisual);
                }
            }
            if (item.type == ModContent.ItemType<SnipersSoul>() || uniSoul)
            {
                if (player.AddEffect<ElementalQuiverEffect>(item))
                {
                    ModContent.GetInstance<ElementalQuiver>().UpdateAccessory(player, hideVisual);
                }
                if (player.AddEffect<QuiverofNihilityEffect>(item))
                {
                    ModContent.GetInstance<QuiverofNihility>().UpdateAccessory(player, hideVisual);
                }
            }
            if (item.type == ModContent.ItemType<ConjuristsSoul>() || uniSoul)
            {
                if (player.AddEffect<NucleogenesisEffect>(item))
                {
                    ModContent.GetInstance<Nucleogenesis>().UpdateAccessory(player, hideVisual);
                }
            }
            if (uniSoul)
            {
                player.Calamity().rogueVelocity += 0.15f;
                if (player.AddEffect<NanotechEffect>(item))
                {
                    ModContent.GetInstance<Nanotech>().UpdateAccessory(player, hideVisual);
                }
                if (player.AddEffect<EclipseMirrorEffect>(item))
                {
                    ModContent.GetInstance<EclipseMirror>().UpdateAccessory(player, hideVisual);
                }
            }

            // toggles to Cal accs
            if (calPlayer.rampartOfDeities)
            {
                player.AddEffect<RampartofDeitiesEffect>(item);
                if (!player.HasEffect<RampartofDeitiesEffect>())
                    calPlayer.rampartOfDeities = false;
                player.AddEffect<DefenseStarEffect>(item);
                if (!player.HasEffect<DefenseStarEffect>())
                    player.starCloakItem = null;
                player.AddEffect<FrozenTurtleEffect>(item);
                if (!player.HasEffect<FrozenTurtleEffect>())
                    player.ClearBuff(BuffID.IceBarrier);


            }
        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<Masochist>())
                return false;

            if (item.type == ModContent.ItemType<Terminus>())
                return WorldSavingSystem.DownedMutant;

            if (item.type == ModContent.ItemType<CelestialOnion>() && WorldSavingSystem.EternityMode)
                return player.FargoSouls().MutantsPactSlot;

            return true;
        }

        //this is cloned from cal because lazy
        public static bool VanillaSummonItem(Item item) =>
            item.type == ItemID.SlimeCrown || item.type == ItemID.SuspiciousLookingEye || item.type == ItemID.BloodMoonStarter || item.type == ItemID.GoblinBattleStandard || item.type == ItemID.WormFood || item.type == ItemID.BloodySpine || item.type == ItemID.Abeemination || item.type == ItemID.DeerThing || item.type == ItemID.QueenSlimeCrystal || item.type == ItemID.PirateMap || item.type == ItemID.SnowGlobe || item.type == ItemID.MechanicalEye || item.type == ItemID.MechanicalWorm || item.type == ItemID.MechanicalSkull || item.type == ItemID.NaughtyPresent || item.type == ItemID.PumpkinMoonMedallion || item.type == ItemID.SolarTablet || item.type == ItemID.SolarTablet || item.type == ItemID.CelestialSigil;
        public override void SetDefaults(Item item)
        {
            if (CalDLCConfig.Instance.ConsumableSummons && CalDLCSets.GetValue(CalDLCSets.Items.CalBossSummon, item.type) || VanillaSummonItem(item))
            {
                item.consumable = WorldSavingSystem.EternityMode;
                item.maxStack = WorldSavingSystem.EternityMode ? 9999 : 1;
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (CalDLCConfig.Instance.ConsumableSummons && CalDLCSets.Items.CalBossSummon[item.type] || VanillaSummonItem(item))
            {
                item.consumable = WorldSavingSystem.EternityMode;
                item.maxStack = WorldSavingSystem.EternityMode ? 9999 : 1;
            }
        }
        // Copied from Mutant Mod
        static string ExpandedTooltipLoc(string line) => Language.GetTextValue($"Mods.FargowiltasCrossmod.ExpandedTooltips.{line}");
        TooltipLine FountainTooltip(string biome) => new TooltipLine(Mod, "Tooltip0", $"[i:909] [c/AAAAAA:{ExpandedTooltipLoc($"Fountain{biome}")}]");
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var mutantServerConfig = FargoServerConfig.Instance;

            if (WorldSavingSystem.EternityMode)
            {
                for (int i = 0; i < tooltips.Count; i++)
                {
                    tooltips[i].Text = tooltips[i].Text.Replace("\nNot consumable", "");
                    tooltips[i].Text = tooltips[i].Text.Replace("Not consumable", "");
                }
            }
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Text.Contains("30") && item.type == ModContent.ItemType<AstralInjection>())
                {
                    tooltips[i].Text = "";
                }
            }
            if (item.type == ModContent.ItemType<Rock>())
            {
                tooltips.Add(new TooltipLine(Mod, "sqrl", $"[i:{ModContent.ItemType<TopHatSquirrelCaught>()}] [c/AAAAAA:" + Language.GetTextValue($"Mods.Fargowiltas.ExpandedTooltips.SoldBySquirrel") + $"]"));
            }
            if (item.type == ModContent.ItemType<Masochist>())
            {

                tooltips.RemoveAll(t => t.Text != item.Name);
                tooltips.Add(new TooltipLine(Mod, "MasochistDisabled", ExpandedTooltipLoc("MasoModeDisabled")));
            }
            if (item.type == ModContent.ItemType<Terminus>())
            {
                tooltips.Add(new TooltipLine(Mod, "PostMutant", ExpandedTooltipLoc("UsablePostMutant")));
            }
            if (item.type == ModContent.ItemType<DeerSinew>())
            {
                tooltips.Add(new TooltipLine(Mod, "ToggleDisabledByDefault", ExpandedTooltipLoc("DisabledByDefault")));
            }

            string BalanceLine = Language.GetTextValue($"Mods.FargowiltasCrossmod.EModeBalance.CrossBalanceGeneric");
            if (item.type == ModContent.ItemType<CelestialOnion>() && !Main.masterMode && WorldSavingSystem.EternityMode)
            {
                tooltips.Add(new TooltipLine(Mod, "OnionPactUpgrade", $"[c/FF0000:{BalanceLine}]" + Language.GetTextValue($"Mods.FargowiltasCrossmod.EModeBalance.OnionPackUpgrade")));
            }


            string key = "Mods.FargowiltasCrossmod.Items.AddedEffects.";
            if (item.type == ModContent.ItemType<AeolusBoots>() && !item.social)
            {
                tooltips.Insert(11, new TooltipLine(Mod, "CalAeolus", Language.GetTextValue(key + "AngelTreads")));
            }
            if (item.type == ModContent.ItemType<SupersonicSoul>() && !item.social)
            {
                tooltips.Insert(12, new TooltipLine(Mod, "CalSupersonicSoul", Language.GetTextValue(key + "CalamitySupersonic")));
            }
            //Colossus Soul
            if (item.type == ModContent.ItemType<ColossusSoul>() && !item.social)
            {
                tooltips.Insert(8, new TooltipLine(Mod, "CalColossusSoul", Language.GetTextValue(key + "CalamityColossus")));
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
                    Language.GetTextValue(key + "CalamityConjurist") + "\n" +
                    Language.GetTextValue(key + "Vagabond")));
            }

            if (item.type == ModContent.ItemType<DimensionSoul>() && !item.social)
            {
                tooltips.Insert(21, new TooltipLine(Mod, "CalDimensionSoul",
                    Language.GetTextValue(key + "CalamityColossus") + "\n" +
                    Language.GetTextValue(key + "AngelTreads") + "\n" +
                    Language.GetTextValue(key + "CalamityTrawler")));
            }

            if (FargoClientConfig.Instance.ExpandedTooltips)
            {
                if (mutantServerConfig.Fountains)
                {
                    if (item.type == ModContent.ItemType<AstralFountainItem>())
                        tooltips.Add(FountainTooltip("Astral"));
                    if (item.type == ModContent.ItemType<BrimstoneLavaFountainItem>())
                        tooltips.Add(FountainTooltip("Crags"));
                    if (item.type == ModContent.ItemType<SulphurousFountainItem>())
                        tooltips.Add(FountainTooltip("Sulphur"));
                    if (item.type == ModContent.ItemType<SunkenSeaFountain>())
                        tooltips.Add(FountainTooltip("Sunken"));
                }
            }
        }
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalExtraSlotPlayer : ModPlayer
    {
        public bool MutantPactShouldBeEnabled;
        public override void PostUpdate()
        {
            ref bool MutantsPactSlot = ref Player.FargoSouls().MutantsPactSlot;
            if (Player.Calamity().extraAccessoryML && !Main.masterMode && WorldSavingSystem.EternityMode)
            {
                if (MutantsPactSlot)
                {
                    MutantPactShouldBeEnabled = true; //store if the slot is enabled
                    DropPactSlot();
                    MutantsPactSlot = false; //turn it off since celestial onion slot is replacing it
                }
            }
            else if (MutantPactShouldBeEnabled)
            {
                MutantsPactSlot = true;
            }
        }
        public override void SaveData(TagCompound tag)
        {
            tag.Add($"{Mod.Name}.{Player.name}.MutantPactShouldBeEnabled", MutantPactShouldBeEnabled);
        }
        public override void LoadData(TagCompound tag)
        {
            MutantPactShouldBeEnabled = tag.GetBool($"{Mod.Name}.{Player.name}.MutantPactShouldBeEnabled");
        }

        private void DropPactSlot()
        {
            //this is ugly but it has to be like this
            void DropItem(Item item)
            {
                Item.NewItem(Player.GetSource_DropAsItem(), Player.Center, item);
            }
            void DropSlot(ref ModAccessorySlot slot)
            {
                DropItem(slot.FunctionalItem);
                slot.FunctionalItem = new();
                DropItem(slot.VanityItem);
                slot.VanityItem = new();
                DropItem(slot.DyeItem);
                slot.DyeItem = new();
                //making dummy items because nulling ModAccessorySlot items crashes the game because of course it does
            }
            ModAccessorySlot eSlot0 = LoaderManager.Get<AccessorySlotLoader>().Get(ModContent.GetInstance<EModeAccessorySlot0>().Type, Player);
            ModAccessorySlot eSlot1 = LoaderManager.Get<AccessorySlotLoader>().Get(ModContent.GetInstance<EModeAccessorySlot1>().Type, Player);
            ModAccessorySlot eSlot2 = LoaderManager.Get<AccessorySlotLoader>().Get(ModContent.GetInstance<EModeAccessorySlot2>().Type, Player);
            DropSlot(ref eSlot0);
            DropSlot(ref eSlot1);
            DropSlot(ref eSlot2);
        }
    }
}
