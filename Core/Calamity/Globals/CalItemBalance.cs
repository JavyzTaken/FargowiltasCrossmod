using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Ammos;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.Patreon.DemonKing;
using FargowiltasSouls.Content.Patreon.Duck;
using FargowiltasSouls.Content.Patreon.GreatestKraken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using FargowiltasSouls.Content.Items.Weapons.FinalUpgrades;
using Terraria.ID;
using Terraria;
using FargowiltasSouls.Content.Items.Armor;
using CalamityMod;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.Localization;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalItemBalance : GlobalItem
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

        public static List<int> RockItems = new List<int>
        {
            ModContent.ItemType<Rock>(),
            ModContent.ItemType<EternitySoul>(),
            ModContent.ItemType<HentaiSpear>(),
            ModContent.ItemType<StyxGazer>(),
            ModContent.ItemType<SparklingLove>(),
            //ModContent.ItemType<GuardianTome>(),
            //ModContent.ItemType<PhantasmalLeashOfCthulhu>(),
            //ModContent.ItemType<SlimeRain>(),
            //ModContent.ItemType<TheBiggestSting>(),
            ModContent.ItemType<MutantPants>(),
            ModContent.ItemType<MutantBody>(),
            ModContent.ItemType<MutantMask>(),
            ModContent.ItemType<FargoArrow>(),
            ModContent.ItemType<FargoBullet>(),
        };

        //this is cloned from cal because lazy
        public static bool VanillaSummonItem(Item item) =>
            item.type == ItemID.SlimeCrown || item.type == ItemID.SuspiciousLookingEye || item.type == ItemID.BloodMoonStarter || item.type == ItemID.GoblinBattleStandard || item.type == ItemID.WormFood || item.type == ItemID.BloodySpine || item.type == ItemID.Abeemination || item.type == ItemID.DeerThing || item.type == ItemID.QueenSlimeCrystal || item.type == ItemID.PirateMap || item.type == ItemID.SnowGlobe || item.type == ItemID.MechanicalEye || item.type == ItemID.MechanicalWorm || item.type == ItemID.MechanicalSkull || item.type == ItemID.NaughtyPresent || item.type == ItemID.PumpkinMoonMedallion || item.type == ItemID.SolarTablet || item.type == ItemID.SolarTablet || item.type == ItemID.CelestialSigil;

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

            //Shadowspec items
            if (item.type == ModContent.ItemType<IridescentExcalibur>()) return 0.5f;
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
            if (item.type == ModContent.ItemType<Fabstaff>()) return 0.6f;

            //Post-Mutant items
            if (item.type == ModContent.ItemType<PhantasmalLeashOfCthulhu>()) return 0.17f;
            if (item.type == ModContent.ItemType<GuardianTome>()) return 0.17f;
            if (item.type == ModContent.ItemType<SlimeRain>()) return 0.08f;
            if (item.type == ModContent.ItemType<TheBiggestSting>()) return 0.17f;
            return 1;

        }

        public override void SetDefaults(Item item)
        {
            //Progression balance changes
            if (DLCCalamityConfig.Instance.BalanceRework)
            {
                float balance = BalanceChange(item);
                if (balance != 1)
                {
                    item.damage = (int)(item.damage * balance);
                }

                if (CalSummons.Contains(item.type) || VanillaSummonItem(item))
                {
                    item.consumable = WorldSavingSystem.EternityMode;
                    item.maxStack = WorldSavingSystem.EternityMode ? 9999 : 1;
                }

            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (CalSummons.Contains(item.type) || VanillaSummonItem(item))
            {
                item.consumable = WorldSavingSystem.EternityMode;
                item.maxStack = WorldSavingSystem.EternityMode ? 9999 : 1;
            }
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            //magic dagger not using system because needs to dynamically change and change shootspeed (setdefaults doesnt allow dynamic change)
            if (item.type == ItemID.MagicDagger && !Main.hardMode)
            {
                damage *= 0.51f;
                item.shootSpeed = 12;
            }
            else if (item.type == ItemID.MagicDagger)
            {
                item.shootSpeed = 30;

            }

        }
        public static float TrueMeleeTungstenScaleNerf(Player player)
        {
            FargoSoulsPlayer soulsPlayer = player.FargoSouls();
            return soulsPlayer.TungstenEnchantItem != null && soulsPlayer.ForceEffect(soulsPlayer.TungstenEnchantItem.type) ? 2f : 1.5f;
        }
        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            FargoSoulsPlayer soulsPlayer = player.FargoSouls();

            #region Tungsten balance changes/fixes
            if (soulsPlayer.TungstenEnchantItem != null && player.GetToggleValue("Tungsten") &&
                    !item.IsAir && item.damage > 0 && (!item.noMelee || FargoGlobalItem.TungstenAlwaysAffects.Contains(item.type)) && item.pick == 0 && item.axe == 0 && item.hammer == 0)
            {
                if (CrossplayerCalamity.TungstenExcludeWeapon.Contains(item.type))
                {
                    float tungScale = 1f + (soulsPlayer.ForceEffect(soulsPlayer.TungstenEnchantItem.type) ? 2f : 1f);
                    scale /= tungScale;
                }
                else if (item != null && (item.DamageType == ModContent.GetInstance<TrueMeleeDamageClass>() || item.DamageType == ModContent.GetInstance<TrueMeleeNoSpeedDamageClass>()))
                {
                    if (DLCCalamityConfig.Instance.BalanceRework)
                        scale /= TrueMeleeTungstenScaleNerf(player);
                }
            }
            #endregion
        }
        #region Tooltips
        public enum EModeChange
        {
            None,
            Nerf,
            Buff,
            Neutral
        }
        //Identical to Eternity nerfs from Souls Mod
        void ItemBalance(List<TooltipLine> tooltips, EModeChange change, string key, int amount = 0)
        {
            string prefix = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{change}");
            string nerf = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{key}", amount == 0 ? null : amount);
            tooltips.Add(new TooltipLine(Mod, $"{change}{key}", $"{prefix} {nerf}"));
        }

        void ItemBalance(List<TooltipLine> tooltips, EModeChange change, string key, string extra)
        {
            string prefix = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{change}");
            string nerf = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{key}");
            tooltips.Add(new TooltipLine(Mod, $"{change}{key}", $"{prefix} {nerf} {extra}"));
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            const string BalanceLine = "Cross-mod Balance: ";

            if (WorldSavingSystem.EternityMode)
            {
                if (item.type == ModContent.ItemType<NormalityRelocator>())
                {
                    ItemBalance(tooltips, EModeChange.Nerf, "RodofDiscord");
                }
            }

            if (DLCCalamityConfig.Instance.BalanceRework)
            {
                float balance = BalanceChange(item);
                const string BalanceUpLine = $"[c/00A36C:{BalanceLine}]";
                const string BalanceDownLine = $"[c/FF0000:{BalanceLine}]";
                if (balance > 1)
                {
                    tooltips.Add(new TooltipLine(Mod, "BalanceUp", $"{BalanceUpLine}Damage increased by {Math.Round((balance - 1) * 100)}%."));
                }
                else if (balance < 1)
                {
                    tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Damage decreased by {Math.Round((1 - balance) * 100)}%."));
                }
                if (item.type == ItemID.MagicDagger)
                {
                    tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Damage decreased by 50% in Pre-Hardmode."));
                }
                if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
                {
                    tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Massively reduced damage with any minions active"));
                }
                if (item.type == ModContent.ItemType<TungstenEnchant>())
                {
                    tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Less effective on true melee weapons"));
                }
                if (item.type == ModContent.ItemType<MythrilEnchant>())
                {
                    tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Less effective on rogue weapons"));
                }
                if (item.type == ModContent.ItemType<OrichalcumEnchant>())
                {
                    tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Reduced effectiveness"));
                }
                if (item.type == ModContent.ItemType<DaawnlightSpiritOrigin>())
                {
                    tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Effect disabled while Tin Enchantment effect is active"));
                }
                /*
                if (item.type == ItemID.WarmthPotion)
                {
                    tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Does not grant buff immunities"));
                }
                */
            }
        }
        #endregion
    }
}
