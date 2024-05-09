using CalamityMod.Items;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.SummonItems;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Ammos;
using FargowiltasSouls.Content.Items.Armor;
using FargowiltasSouls.Content.Items.Weapons.FinalUpgrades;
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
using CalamityMod.NPCs.AcidRain;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Bosses.Champions.Life;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.Champions.Spirit;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.Champions.Timber;
using FargowiltasSouls.Content.Bosses.Champions.Will;
using Terraria.ID;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Alcohol;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Buffs.Souls;

namespace FargowiltasCrossmod.Core.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public static class CalamityContentLists
    {

        #region Items
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

        public static List<int> CalBossSummons = new List<int>
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
        #endregion
        #region NPCs
        public static List<int> AcidRainEnemies = new List<int>
        {
            ModContent.NPCType<AcidEel>(),
            ModContent.NPCType<NuclearToad>(),
            ModContent.NPCType<Radiator>(),
            ModContent.NPCType<Skyfin>(),
            ModContent.NPCType<FlakCrab>(),
            ModContent.NPCType<IrradiatedSlime>(),
            ModContent.NPCType<Orthocera>(),
            ModContent.NPCType<SulphurousSkater>(),
            ModContent.NPCType<Trilobite>(),
            ModContent.NPCType<CragmawMire>(),
            ModContent.NPCType<GammaSlime>(),
            ModContent.NPCType<Mauler>(),
            ModContent.NPCType<NuclearTerror>(),
        };
        #endregion
        #region Buffs
        //excludes debuffs that are from projectiles attached to npcs
        public static List<int> DoTDebuffs = new List<int>
        {
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
            ModContent.BuffType<Nightwither>(),
            ModContent.BuffType<BanishingFire>(),
            ModContent.BuffType<BrimstoneFlames>(),
            ModContent.BuffType<WeakBrimstoneFlames>(),
            ModContent.BuffType<GodSlayerInferno>(),
            ModContent.BuffType<HolyFlames>(),
            ModContent.BuffType<Dragonfire>(),
            ModContent.BuffType<VulnerabilityHex>(),
            ModContent.BuffType<AbsorberAffliction>(),
            ModContent.BuffType<AstralInfectionDebuff>(),
            ModContent.BuffType<Plague>(),
            ModContent.BuffType<SulphuricPoisoning>(),
            ModContent.BuffType<SagePoison>(),
            ModContent.BuffType<KamiFlu>(),
            ModContent.BuffType<CrushDepth>(),
            ModContent.BuffType<RiptideDebuff>(),
            ModContent.BuffType<BrainRot>(),
            ModContent.BuffType<BurningBlood>(),
            ModContent.BuffType<HolyInferno>(),
            ModContent.BuffType<Irradiated>(),
            ModContent.BuffType<MiracleBlight>(),
            ModContent.BuffType<AlcoholPoisoning>(),
            ModContent.BuffType<ManaBurn>(),
            ModContent.BuffType<SearingLava>(),
            ModContent.BuffType<ElementalMix>(),
            ModContent.BuffType<Shred>(),
            ModContent.BuffType<Vaporfied>(),
            ModContent.BuffType<Shadowflame>(),

            ModContent.BuffType<AnticoagulationBuff>(),
            ModContent.BuffType<CurseoftheMoonBuff>(),
            ModContent.BuffType<FlamesoftheUniverseBuff>(),
            ModContent.BuffType<GodEaterBuff>(),
            ModContent.BuffType<InfestedBuff>(),
            ModContent.BuffType<IvyVenomBuff>(),
            ModContent.BuffType<NanoInjectionBuff>(),
            ModContent.BuffType<NeurotoxinBuff>(),
            ModContent.BuffType<ShadowflameBuff>(),
            ModContent.BuffType<TwinsInstallBuff>(),
            ModContent.BuffType<HellFireBuff>(),
            ModContent.BuffType<LeadPoisonBuff>(),
            ModContent.BuffType<OriPoisonBuff>(),
            ModContent.BuffType<SolarFlareBuff>(),

        };
        #endregion
    }
}
