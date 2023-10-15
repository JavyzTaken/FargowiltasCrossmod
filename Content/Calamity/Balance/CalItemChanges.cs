using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Fargowiltas.Common.Configs;
using FargowiltasCrossmod.Content.Calamity.Items.Summons;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using FargowiltasSouls.Common;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.Patreon.DemonKing;
using FargowiltasSouls.Content.Patreon.Duck;
using FargowiltasSouls.Content.Patreon.GreatestKraken;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Balance
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalItemChanges : GlobalItem
    {
        
        public static List<int> ChampionTierFargoWeapons = new List<int>
        {
            ModContent.ItemType<EaterLauncher>(),
            ModContent.ItemType<FleshCannon>(),
            ModContent.ItemType<HellZone>(),
            ModContent.ItemType<MechanicalLeashOfCthulhu>(),
            ModContent.ItemType<SlimeSlingingSlasher>(),
            ModContent.ItemType<TheBigSting>(),
            ModContent.ItemType<ScientificRailgun>(),
            ModContent.ItemType<VortexMagnetRitual>()
        };
        public static List<int> AbomTierFargoWeapons = new List<int>
        {
            ModContent.ItemType<DragonBreath2>(),
            ModContent.ItemType<DestroyerGun2>(),
            ModContent.ItemType<GolemTome2>(),
            ModContent.ItemType<GeminiGlaives>(),
            ModContent.ItemType<Blender>(),
            ModContent.ItemType<RefractorBlaster2>(),
            ModContent.ItemType<NukeFishron>(),
            ModContent.ItemType<StaffOfUnleashedOcean>(),
        };
        public static List<int> CalSummons = new List<int>
        {
            ModContent.ItemType<DesertMedallion>(),
            ModContent.ItemType<DecapoditaSprout>(),
            ModContent.ItemType<Teratoma>(),
            ModContent.ItemType<BloodyWormFood>(),
            ModContent.ItemType<OverloadedSludge>(),
            ModContent.ItemType<CryoKey>(),
            ModContent.ItemType<Seafood>(),
            ModContent.ItemType<CharredIdol>(),
            ModContent.ItemType<EyeofDesolation>(),
            ModContent.ItemType<AstralChunk>(),
            ModContent.ItemType<Abombination>(),
            ModContent.ItemType<DeathWhistle>(),
            ModContent.ItemType<Starcore>(),
            ModContent.ItemType<ProfanedShard>(),
            ModContent.ItemType<ExoticPheromones>(),
            ModContent.ItemType<ProfanedCore>(),
            ModContent.ItemType<RuneofKos>(),
            ModContent.ItemType<NecroplasmicBeacon>(),
            ModContent.ItemType<CosmicWorm>(),
            ModContent.ItemType<YharonEgg>(),

            ModContent.ItemType<EidolonTablet>(),
            ModContent.ItemType<Portabulb>(),
            ModContent.ItemType<SandstormsCore>(),
            ModContent.ItemType<CausticTear>(),
            ModContent.ItemType<MartianDistressRemote>(),
        };
        //this is cloned from cal because lazy
        public static bool VanillaSummonItem(Item item) => 
            (item.type == 560 || item.type == 43 || item.type == 4271 || item.type == 361 || item.type == 70 || item.type == 1331 || item.type == 1133 || item.type == 5120 || item.type == 4988 || item.type == 1315 || item.type == 602 || item.type == 544 || item.type == 556 || item.type == 557 || item.type == 1958 || item.type == 1844 || item.type == 2767 || item.type == 2767 || item.type == 3601);
        
        public float BalanceChange(Item item)
        {
            
            if (item.type == ModContent.ItemType<MechanicalLeashOfCthulhu>())
                return 0.5f;
            if (item.type == ModContent.ItemType<Blender>())
                return 1f;
            if (item.type == ModContent.ItemType<NukeFishron>() || item.type == ModContent.ItemType<GolemTome2>() || item.type == ModContent.ItemType<DestroyerGun2>() || item.type == ModContent.ItemType<RefractorBlaster2>())
                return 2f;
            if (AbomTierFargoWeapons.Contains(item.type))
                return 1.5f;
            if (ChampionTierFargoWeapons.Contains(item.type))
            {
                return 0.8f;
            }
            
            if (item.type == ModContent.ItemType<IridescentExcalibur>()) return 0.6f;
            if (item.type == ModContent.ItemType<IllustriousKnives>()) return 0.8f;
            if (item.type == ModContent.ItemType<Azathoth>()) return 0.9f;
            if (item.type == ModContent.ItemType<RedSun>()) return 1.5f;
            if (item.type == ModContent.ItemType<SomaPrime>()) return 1.2f;
            if (item.type == ModContent.ItemType<Svantechnical>()) return 1.1f;
            if (item.type == ModContent.ItemType<Voidragon>()) return 1.1f;
            if (item.type == ModContent.ItemType<StaffofBlushie>()) return 0.7f;
            if (item.type == ModContent.ItemType<Eternity>()) return 0.4f;
            if (item.type == ModContent.ItemType<TheDanceofLight>()) return 0.5f;
            if (item.type == ModContent.ItemType<RainbowPartyCannon>()) return 0.6f;
            if (item.type == ModContent.ItemType<NanoblackReaper>()) return 0.4f;
            if (item.type == ModContent.ItemType<ScarletDevil>()) return 0.4f;
            if (item.type == ModContent.ItemType<TemporalUmbrella>()) return 0.35f;
            if (item.type == ModContent.ItemType<Endogenesis>()) return 0.35f;
            if (item.type == ModContent.ItemType<UniverseSplitter>()) return 0.5f;
            if (item.type == ModContent.ItemType<Metastasis>()) return 0.5f;
            if (item.type == ModContent.ItemType<FlamsteedRing>()) return 0.45f;
            if (item.type == ModContent.ItemType<AngelicAlliance>()) return 0.2f;
            if (item.type == ModContent.ItemType<ProfanedSoulCrystal>()) return 0.4f;
            return 1;
            
        }
        public override void SetDefaults(Item item)
        {
            //Progression balance changes
            if (Core.Calamity.DLCCalamityConfig.Instance.BalanceRework)
            {
                float balance  = BalanceChange(item);
                if (balance != 1)
                {
                    item.damage = (int)(item.damage * balance);
                }
                if (CalSummons.Contains(item.type) || VanillaSummonItem(item))
                {
                    item.consumable = true;
                    item.maxStack = 9999;
                }
            }
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            //magic dagger not using system because needs to dynamically change and change shootspeed (setdefaults doesnt allow dynamic change)
            if (item.type == ItemID.MagicDagger && !Main.hardMode)
            {
                damage *= 0.51f;
                item.shootSpeed = 12;
            }else if (item.type == ItemID.MagicDagger)
            {
                item.shootSpeed = 30;
            }
           
        }
        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            FargoSoulsPlayer soulsPlayer = player.FargoSouls();
            if (player.FargoSouls().TungstenEnchantItem != null && CrossplayerCalamity.TungstenExcludeWeapon.Contains(item.type))
            {
                float tungScale = 1f + (soulsPlayer.ForceEffect(soulsPlayer.TungstenEnchantItem.type) ? 2f : 1f);
                scale /= tungScale;
            }
            if (player.FargoSouls().TungstenEnchantItem != null && (item.DamageType == ModContent.GetInstance<TrueMeleeDamageClass>() || item.DamageType == ModContent.GetInstance<TrueMeleeNoSpeedDamageClass>()))
            {
                float tungScale = 1f + (soulsPlayer.ForceEffect(soulsPlayer.TungstenEnchantItem.type) ? 2f : 1f);
                scale /= 1.75f;
            }
        }
        public override float UseSpeedMultiplier(Item item, Player player)
        {
            if (player.FargoSouls().MythrilEnchantItem != null && item.DamageType == ModContent.GetInstance<RogueDamageClass>() && item.useTime >= item.useAnimation)
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
            if (item.type == ModContent.ItemType<CounterScarf>() || item.type == ModContent.ItemType<EvasionScarf>() || item.type == ModContent.ItemType<OrnateShield>()
                || item.type == ModContent.ItemType<AsgardianAegis>() || item.type == ModContent.ItemType<ElysianAegis>() || item.type == ModContent.ItemType<AsgardsValor>()
                || item.type == ModContent.ItemType<StatisNinjaBelt>() || item.type == ModContent.ItemType<StatisVoidSash>() || item.type == ModContent.ItemType<ShieldoftheHighRuler>()
                || (item.type == ModContent.ItemType<DeepDiver>() && player.wet) || player.Calamity().plaguebringerPatronSet)
            {
                player.FargoSouls().HasDash = true;
            }
            if (item.type == ModContent.ItemType<AngelTreads>())
            {
                if (player.GetToggleValue("MasoAeolus"))
                {
                    player.GetJumpState(ExtraJump.FartInAJar).Enable();
                }
                
            }
            if (item.type == ModContent.ItemType<ColossusSoul>() || item.type == ModContent.ItemType<DimensionSoul>() || item.type == ModContent.ItemType<EternitySoul>())
            {
                if (player.GetToggleValue("Amalgam"))
                {
                    ModContent.GetInstance<TheAmalgam>().UpdateAccessory(player, hideVisual);
                }
                if (player.GetToggleValue("AsgardianAegis", false))
                {
                    ModContent.GetInstance<AsgardianAegis>().UpdateAccessory(player, hideVisual);
                    player.FargoSouls().HasDash = true;
                }
                
                if (player.GetToggleValue("RampartofDeities", false))
                {
                    ModContent.GetInstance<RampartofDeities>().UpdateAccessory(player, hideVisual);
                }
                if (player.GetToggleValue("Purity"))
                {
                    ModContent.GetInstance<Purity>().UpdateAccessory(player, hideVisual);
                }
            }
            if (item.type == ModContent.ItemType<BerserkerSoul>() || item.type == ModContent.ItemType<UniverseSoul>() || item.type == ModContent.ItemType<EternitySoul>())
            {
                //ModContent.GetInstance<ReaperToothNecklace>().UpdateAccessory(player, hideVisual);
                player.GetArmorPenetration<GenericDamageClass>() += 15f;
            }
            if (item.type == ModContent.ItemType<BerserkerSoul>() || item.type == ModContent.ItemType<UniverseSoul>() || item.type == ModContent.ItemType<EternitySoul>())
            {
                if (player.GetToggleValue("ElementalGauntlet", false))
                {
                    ModContent.GetInstance<ElementalGauntlet>().UpdateAccessory(player, hideVisual);
                }
            }
            if (item.type == ModContent.ItemType<ArchWizardsSoul>() || item.type == ModContent.ItemType<UniverseSoul>() || item.type == ModContent.ItemType<EternitySoul>())
            {
                if (player.GetToggleValue("EtherealTalisman", false))
                {
                    ModContent.GetInstance<EtherealTalisman>().UpdateAccessory(player, hideVisual);
                }
            }
            if (item.type == ModContent.ItemType<SnipersSoul>() || item.type == ModContent.ItemType<UniverseSoul>() || item.type == ModContent.ItemType<EternitySoul>())
            {
                if (player.GetToggleValue("ElementalQuiver", false))
                {
                    ModContent.GetInstance<ElementalQuiver>().UpdateAccessory(player, hideVisual);
                }
                if (player.GetToggleValue("QuiverofNihility"))
                {
                    ModContent.GetInstance<QuiverofNihility>().UpdateAccessory(player, hideVisual);
                }
                if (player.GetToggleValue("DynamoStemCells"))
                {
                    ModContent.GetInstance<DynamoStemCells>().UpdateAccessory(player, hideVisual);
                }
            }
            if (item.type == ModContent.ItemType<ConjuristsSoul>() || item.type == ModContent.ItemType<UniverseSoul>() || item.type == ModContent.ItemType<EternitySoul>())
            {
                if (player.GetToggleValue("Nucleogenesis", false))
                {
                    ModContent.GetInstance<Nucleogenesis>().UpdateAccessory(player, hideVisual);
                }
            }
            if (item.type == ModContent.ItemType<UniverseSoul>() || item.type == ModContent.ItemType<EternitySoul>())
            {
                player.Calamity().rogueVelocity += 0.15f;
                if (player.GetToggleValue("Nanotech", false))
                {
                    ModContent.GetInstance<Nanotech>().UpdateAccessory(player, hideVisual);
                }
                if (player.GetToggleValue("EclipseMirror", false))
                {
                    ModContent.GetInstance<EclipseMirror>().UpdateAccessory(player, hideVisual);
                }
                if (player.GetToggleValue("DragonScales"))
                {
                    ModContent.GetInstance<DragonScales>().UpdateAccessory(player, hideVisual);
                }
                if (player.GetToggleValue("VeneratedLocket"))
                {
                    ModContent.GetInstance<VeneratedLocket>().UpdateAccessory(player, hideVisual);
                }
            }
            if (item.type == ModContent.ItemType<TrawlerSoul>() || item.type == ModContent.ItemType<DimensionSoul>() || item.type == ModContent.ItemType<EternitySoul>())
            {
                if (player.GetToggleValue("AbyssalDivingSuit"))
                {
                    ModContent.GetInstance<AbyssalDivingSuit>().UpdateAccessory(player, hideVisual);
                }
            }
        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<Masochist>())
            {
                return false;
            }
            if (item.type == ModContent.ItemType<CelestialOnion>() && Core.Calamity.DLCCalamityConfig.Instance.BalanceRework)
            {
                return player.FargoSouls().MutantsPactSlot;
            }
            return true;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                tooltips[i].Text = tooltips[i].Text.Replace("Not consumable", "");
                /*
                if (tooltips[i].Text.Contains("Not consumable"))
                {
                    tooltips[i].Text = "";
                }
                */
            }

            if (item.type == ModContent.ItemType<Rock>())
            {
                tooltips.Add(new TooltipLine(Mod, "sqrl", $"[c/AAAAAA:Sold by Squirrel]"));
            }
            if (item.type == ModContent.ItemType<Masochist>())
            {
                
                tooltips.RemoveAll(t => t.Text != item.Name);
                tooltips.Add(new TooltipLine(Mod, "MasochistDisabled", $"[c/FF0000:Calamity Crossmod Support:] Disabled. Use Calamity's difficulty UI instead!"));
            }

            const string BalanceLine = "Cross-mod Balance: ";
            if (item.type == ModContent.ItemType<CelestialOnion>() && Core.Calamity.DLCCalamityConfig.Instance.BalanceRework)
            {
                tooltips.Add(new TooltipLine(Mod, "OnionPactUpgrade", $"[c/FF0000:{BalanceLine}]Is now an upgrade to [i:{ModContent.ItemType<MutantsPact>()}]Mutant's Pact, that allows any accessory in the extra slot."));
            }


            string key = "Mods.FargowiltasCrossmod.Items.AddedEffects.";
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
            #region Item Balance
            if (Core.Calamity.DLCCalamityConfig.Instance.BalanceRework)
            {
                float balance = BalanceChange(item);
                const string BalanceUpLine = $"[c/00A36C:{BalanceLine}]";
                const string BalanceDownLine = $"[c/FF0000:{BalanceLine}]";
                if (balance > 1)
                {
                    tooltips.Add(new TooltipLine(Mod, "DamageBalanceUp", $"{BalanceUpLine}Damage increased by {Math.Round((balance - 1) * 100)}%."));
                }
                else if (balance < 1)
                {
                    tooltips.Add(new TooltipLine(Mod, "DamageBalanceDown", $"{BalanceDownLine}Damage decreased by {Math.Round((1 - balance) * 100)}%."));
                }
                if (item.type == ItemID.MagicDagger)
                {
                    tooltips.Add(new TooltipLine(Mod, "DamageBalanceDown", $"{BalanceDownLine}Damage decreased by 50% in Pre-Hardmode."));
                }
                if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
                {
                    tooltips.Add(new TooltipLine(Mod, "DamageBalanceDown", $"{BalanceDownLine}Massively reduced damage with any minions active"));
                }
                if (item.type == ModContent.ItemType<TungstenEnchant>())
                {
                    tooltips.Add(new TooltipLine(Mod, "DamageBalanceDown", $"{BalanceDownLine}Less effective on true melee weapons"));
                }
                if (item.type == ModContent.ItemType<MythrilEnchant>())
                {
                    tooltips.Add(new TooltipLine(Mod, "DamageBalanceDown", $"{BalanceDownLine}Less effective on rogue weapons"));
                }
                if (item.type == ModContent.ItemType<OrichalcumEnchant>())
                {
                    tooltips.Add(new TooltipLine(Mod, "DamageBalanceDown", $"{BalanceDownLine}25% less effective"));
                }
                if (item.type == ModContent.ItemType<DaawnlightSpiritOrigin>())
                {
                    tooltips.Add(new TooltipLine(Mod, "DamageBalanceDown", $"{BalanceDownLine}Effect disabled while Tin Enchantment effect is active"));
                }
            }
            #endregion
        }
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalExtraSlotPlayer : ModPlayer
    {
        public bool MutantPactShouldBeEnabled;
        public override void PostUpdate()
        {
            ref bool MutantsPactSlot = ref Player.FargoSouls().MutantsPactSlot;
            if (Player.Calamity().extraAccessoryML && Core.Calamity.DLCCalamityConfig.Instance.BalanceRework)
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
