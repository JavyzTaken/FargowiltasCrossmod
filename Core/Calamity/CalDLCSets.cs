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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace FargowiltasCrossmod.Core.Calamity
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCSets : ModSystem
    {
        public class Items
        {
            public static bool[] RockItem;
            public static bool[] CalBossSummon;
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
        }
        public override void PostSetupContent()
        {
            #region Items
            SetFactory itemFactory = new(ItemLoader.ItemCount);
            Items.RockItem = itemFactory.CreateBoolSet(false,
                ItemType<Rock>(),
                ItemType<EternitySoul>(),
                ItemType<HentaiSpear>(),
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
            #endregion

            #region NPCs
            SetFactory npcFactory = new(NPCLoader.NPCCount);
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
            SetFactory buffFactory = new(BuffLoader.BuffCount);
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
            SetFactory projectileFactory = new(ProjectileLoader.ProjectileCount);

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

            #endregion

        }
    }
}
