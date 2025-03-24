using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Summon;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Projectiles.Masomode;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using CalamityMod.Items;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Ammos;
using FargowiltasSouls.Content.Items.Weapons.FinalUpgrades;
using FargowiltasSouls.Content.Items.Armor;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Typeless;
using Fargowiltas;
using CalamityMod.Items.Placeables.Furniture;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using FargowiltasSouls.Content.Bosses.CursedCoffin;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.BanishedBaron;
using FargowiltasSouls.Content.Bosses.Lifelight;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Typeless;

namespace FargowiltasCrossmod.Core.Calamity
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCSets : ModSystem
    {
        /// <summary>
        /// Get boolean value, false if set is null. Necessary to alleviate SetDefaultsBeforeLookupsAreBuilt error.
        /// </summary>
        public static bool GetValue(bool[] set, int index) => set != null && set[index];
        public class Items
        {
            public static bool[] RockItem;
            public static bool[] CalBossSummon;

            public static bool[] AdamantiteExclude;
            public static bool[] TungstenExclude;
            public static bool[] MarniteExclude;

            /// <summary>
            /// Items for which emode changes should be explicitly disabled. Exists to remove conflicts with Calamity item changes for the same item.
            /// </summary>
            public static bool[] DisabledEmodeChanges; 
        }
        public class NPCs
        {
            public static bool[] AcidRainEnemy;
        }
        public class Buffs
        {
            public static bool[] DoTDebuff; //excludes debuffs that are from projectiles attached to npcs
        }
        public class Projectiles
        {
            public static bool[] TungstenExclude;
            public static bool[] MultipartShredder;
            public static bool[] ProfanedCrystalProj;
            public static bool[] EternityBookProj;
            public static bool[] AngelAllianceProj;
            public static bool[] DefenseDamage;
        }
        public override void PostSetupContent()
        {
            #region Items
            FargoSets.Items.BuffStation[ItemType<ResilientCandle>()] = true;
            FargoSets.Items.BuffStation[ItemType<SpitefulCandle>()] = true;
            FargoSets.Items.BuffStation[ItemType<VigorousCandle>()] = true;
            FargoSets.Items.BuffStation[ItemType<WeightlessCandle>()] = true;

            SetFactory itemFactory = ItemID.Sets.Factory;
            Items.RockItem = itemFactory.CreateBoolSet(false,
                ItemType<Rock>(),
                ItemType<EternitySoul>(),
                ItemType<Penetrator>(),
                ItemType<StyxGazer>(),
                ItemType<SparklingLove>(),
                //ItemType<GuardianTome>(),
                //ItemType<PhantasmalLeashOfCthulhu>(),
                //ItemType<SlimeRain>(),
                //ItemType<TheBiggestSting>(),
                ItemType<MutantPants>(),
                ItemType<MutantBody>(),
                ItemType<MutantMask>(),
                ItemType<FargoArrow>(),
                ItemType<FargoBullet>()
                );
            Items.CalBossSummon = itemFactory.CreateBoolSet(false,
                ItemType<DesertMedallion>(),
                ItemType<DecapoditaSprout>(),
                ItemType<Teratoma>(),
                ItemType<BloodyWormFood>(),
                ItemType<OverloadedSludge>(),
                ItemType<CryoKey>(),
                ItemType<Seafood>(),
                ItemType<CharredIdol>(),
                ItemType<EyeofDesolation>(),
                ItemType<AstralChunk>(),
                ItemType<Abombination>(),
                ItemType<DeathWhistle>(),
                ItemType<Starcore>(),
                ItemType<ProfanedShard>(),
                ItemType<ExoticPheromones>(),
                ItemType<ProfanedCore>(),
                ItemType<RuneofKos>(),
                ItemType<NecroplasmicBeacon>(),
                ItemType<CosmicWorm>(),
                ItemType<YharonEgg>(),

                ItemType<EidolonTablet>(),
                ItemType<Portabulb>(),
                ItemType<SandstormsCore>(),
                ItemType<CausticTear>(),
                ItemType<MartianDistressRemote>()
                );

            Items.AdamantiteExclude = itemFactory.CreateBoolSet(false,
                ItemType<HeavenlyGale>(),
                ItemType<TheSevensStriker>(),
                ItemType<Phangasm>(),
                ItemType<TheJailor>(),
                ItemType<AetherfluxCannon>(),
                ItemType<TheAnomalysNanogun>(),
                ItemType<ClockworkBow>(),
                ItemType<NebulousCataclysm>(),
                ItemType<Eternity>(), //fargo reference
                ItemType<CalamityMod.Items.Weapons.Magic.Vehemence>(),
                ItemType<Phaseslayer>(),
                ItemType<FracturedArk>(),
                ItemType<TrueArkoftheAncients>(),
                ItemType<ArkoftheElements>(),
                ItemType<ArkoftheCosmos>(),
                ItemType<Animosity>()
            );
            Items.TungstenExclude = itemFactory.CreateBoolSet(false,
                ItemType<OldLordClaymore>(),
                ItemType<BladecrestOathsword>()
            );
            Items.MarniteExclude = itemFactory.CreateBoolSet(false,  // set of boss viable tools
              //ItemID.RodofDiscord, // this is intentional
                ItemID.Rockfish,
                ItemID.ButchersChainsaw,
                ItemID.LucyTheAxe,
                ItemType<FellerofEvergreens>(),
                ItemType<AxeofPurity>(),
                ItemType<HydraulicVoltCrasher>(),
                ItemType<InfernaCutter>(),
                ItemType<Respiteblock>(),
                ItemType<RelicOfConvergence>(),
                ItemType<RelicOfResilience>(),
                ItemType<Grax>(),
                ItemType<PhotonRipper>()
            );

            Items.DisabledEmodeChanges = itemFactory.CreateBoolSet(false,
                ItemID.StarCannon,
                ItemID.SuperStarCannon,
                ItemID.VampireKnives,
                ItemID.IceBlade,
                ItemID.FrozenTurtleShell,
                ItemID.FrozenShield,
                ItemID.HallowedGreaves,
                ItemID.HallowedHeadgear,
                ItemID.HallowedHelmet,
                ItemID.HallowedHood,
                ItemID.HallowedMask,
                ItemID.HallowedPlateMail,
                ItemID.AncientHallowedGreaves,
                ItemID.AncientHallowedHeadgear,
                ItemID.AncientHallowedHelmet,
                ItemID.AncientHallowedHood,
                ItemID.AncientHallowedMask,
                ItemID.AncientHallowedPlateMail,
                ItemID.BeeGun,
                ItemID.MonkStaffT1,
                ItemID.MonkStaffT2,
                ItemID.MonkStaffT3,
                ItemID.MoltenFury,
                ItemID.DaedalusStormbow,
                ItemID.Razorpine,
                ItemID.BlizzardStaff,
                ItemID.LaserMachinegun,
                ItemID.DD2SquireBetsySword
            );
            #endregion

            #region NPCs
            SetFactory npcFactory = NPCID.Sets.Factory;
            NPCs.AcidRainEnemy = npcFactory.CreateBoolSet(false,
                NPCType<AcidEel>(),
                NPCType<NuclearToad>(),
                NPCType<Radiator>(),
                NPCType<Skyfin>(),
                NPCType<FlakCrab>(),
                NPCType<IrradiatedSlime>(),
                NPCType<Orthocera>(),
                NPCType<SulphurousSkater>(),
                NPCType<Trilobite>(),
                NPCType<CragmawMire>(),
                NPCType<GammaSlime>(),
                NPCType<Mauler>(),
                NPCType<NuclearTerror>()
            );
            #endregion

            #region Buffs
            SetFactory buffFactory = BuffID.Sets.Factory;
            Buffs.DoTDebuff = buffFactory.CreateBoolSet(false,
                BuffID.Bleeding,
                BuffID.Poisoned,
                BuffID.OnFire,
                BuffID.Venom,
                BuffID.CursedInferno,
                BuffID.Frostburn,
                BuffID.Electrified,
                BuffID.Burning,
                BuffID.ShadowFlame,
                BuffID.Daybreak,
                BuffID.OnFire3,
                BuffID.Frostburn2,
                BuffType<Nightwither>(),
                BuffType<BanishingFire>(),
                BuffType<BrimstoneFlames>(),
                BuffType<WeakBrimstoneFlames>(),
                BuffType<GodSlayerInferno>(),
                BuffType<HolyFlames>(),
                BuffType<Dragonfire>(),
                BuffType<VulnerabilityHex>(),
                BuffType<AbsorberAffliction>(),
                BuffType<AstralInfectionDebuff>(),
                BuffType<Plague>(),
                BuffType<SulphuricPoisoning>(),
                BuffType<SagePoison>(),
                BuffType<KamiFlu>(),
                BuffType<CrushDepth>(),
                BuffType<RiptideDebuff>(),
                BuffType<BrainRot>(),
                BuffType<BurningBlood>(),
                BuffType<HolyInferno>(),
                BuffType<Irradiated>(),
                BuffType<MiracleBlight>(),
                BuffType<AlcoholPoisoning>(),
                BuffType<ManaBurn>(),
                BuffType<SearingLava>(),
                BuffType<ElementalMix>(),
                BuffType<Shred>(),
                BuffType<Vaporfied>(),
                BuffType<Shadowflame>(),

                BuffType<AnticoagulationBuff>(),
                BuffType<CurseoftheMoonBuff>(),
                BuffType<FlamesoftheUniverseBuff>(),
                BuffType<GodEaterBuff>(),
                BuffType<InfestedBuff>(),
                BuffType<IvyVenomBuff>(),
                BuffType<NanoInjectionBuff>(),
                BuffType<NeurotoxinBuff>(),
                BuffType<ShadowflameBuff>(),
                BuffType<TwinsInstallBuff>(),
                BuffType<HellFireBuff>(),
                BuffType<LeadPoisonBuff>(),
                BuffType<OriPoisonBuff>(),
                BuffType<SolarFlareBuff>()
                );
            #endregion

            #region Projectiles
            SetFactory projectileFactory = ProjectileID.Sets.Factory;

            Projectiles.TungstenExclude = projectileFactory.CreateBoolSet(false,
                    ProjectileType<BladecrestOathswordProj>(),
                    ProjectileType<OldLordClaymoreProj>()
                );

            Projectiles.MultipartShredder = projectileFactory.CreateBoolSet(false,
                ProjectileType<CelestialRuneFireball>()
                );
            Projectiles.ProfanedCrystalProj = projectileFactory.CreateBoolSet(false,
                ProjectileType<ProfanedCrystalMageFireball>(),
                ProjectileType<ProfanedCrystalMageFireballSplit>(),
                ProjectileType<ProfanedCrystalMeleeSpear>(),
                ProjectileType<ProfanedCrystalRangedHuges>(),
                ProjectileType<ProfanedCrystalRangedSmalls>(),
                ProjectileType<ProfanedCrystalRogueShard>(),
                ProjectileType<ProfanedCrystalWhip>(),
                ProjectileType<MiniGuardianAttack>(),
                ProjectileType<MiniGuardianDefense>(),
                ProjectileType<MiniGuardianFireball>(),
                ProjectileType<MiniGuardianFireballSplit>(),
                ProjectileType<MiniGuardianHealer>(),
                ProjectileType<MiniGuardianHolyRay>(),
                ProjectileType<MiniGuardianRock>(),
                ProjectileType<MiniGuardianSpear>(),
                ProjectileType<MiniGuardianStars>()
                );
            Projectiles.EternityBookProj = projectileFactory.CreateBoolSet(false,
                ProjectileType<EternityCircle>(),
                ProjectileType<EternityCrystal>(),
                ProjectileType<EternityHex>(),
                ProjectileType<EternityHoming>()
                );
            Projectiles.AngelAllianceProj = projectileFactory.CreateBoolSet(false,
                ProjectileType<AngelBolt>(),
                ProjectileType<AngelicAllianceArchangel>(),
                ProjectileType<AngelOrb>(),
                ProjectileType<AngelRay>()
                );
            Projectiles.DefenseDamage = projectileFactory.CreateBoolSet(false,
                ProjectileID.DD2ExplosiveTrapT3Explosion, // trojan
                ProjectileType<TrojanHook>(),
                ProjectileType<FallingSandstone>(),
                ProjectileType<DestroyerLaser>(), // BoC
                ProjectileType<DeviAxe>(),
                ProjectileType<DeviBigMimic>(),
                ProjectileType<DeviHammer>(),
                ProjectileType<DeviHammerHeld>(),
                ProjectileType<DeviSparklingLove>(),
                ProjectileType<DeviSparklingLoveSmall>(),
                ProjectileType<CursedFlamethrower>(),
                ProjectileType<GoldenShowerWOF>(),
                ProjectileType<BaronNuke>(),
                ProjectileType<BaronMine>(),
                ProjectileType<BaronWhirlpool>(),
                ProjectileType<PrimeGuardian>(),
                ProjectileType<LifeRuneExplosion>(),
                ProjectileType<LifeRuneHitbox>(),
                ProjectileType<LifeRuneRetractHitbox>(),
                ProjectileType<LifeRunespearExplosion>(),
                ProjectileType<LifeRunespearHitbox>(),
                ProjectileType<LifeScar>(),
                ProjectileType<PlanteraSpikevine>(),
                ProjectileType<PlanteraTentacle>(),
                ProjectileType<GolemBoulder>(),
                ProjectileType<GolemSpikeBallBig>(),
                ProjectileType<GolemGeyser>(),
                ProjectileType<GolemGeyser2>(),
                ProjectileType<FishronFishron>(),
                ProjectileType<BetsyFury>(),
                ProjectileType<BetsyFury2>(),
                ProjectileType<CelestialPillar>(),
                ProjectileType<MoonLordSun>(),
                ProjectileType<MoonLordSunBlast>(),
                ProjectileType<MoonLordMoon>(),
                ProjectileType<CosmosMeteor>()
                );

            #endregion

        }
    }
}
