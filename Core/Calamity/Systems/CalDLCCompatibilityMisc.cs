using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Enums;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles;
using CalamityMod.Systems;
using CalamityMod.World;
using Fargowiltas.NPCs;
using FargowiltasCrossmod.Content.Calamity.Bosses.Crabulon;
using FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen;
using FargowiltasCrossmod.Content.Calamity.Bosses.Perforators;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.BanishedBaron;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Bosses.Champions.Life;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.Champions.Spirit;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.Champions.Timber;
using FargowiltasSouls.Content.Bosses.Champions.Will;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.Lifelight;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.Events.BossRushEvent;
using static FargowiltasCrossmod.Core.Common.Globals.DevianttGlobalNPC;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCCompatibilityMisc : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod) => ModCompatibility.Calamity.Loaded;
        #region summonloadingbullshit
        public static bool DownedDS => DownedBossSystem.downedDesertScourge;
        public static bool DownedCrab => DownedBossSystem.downedCrabulon;
        public static bool DownedHM => DownedBossSystem.downedHiveMind;
        public static bool DownedPerf => DownedBossSystem.downedPerforator;
        public static bool DownedSG => DownedBossSystem.downedSlimeGod;
        public static bool DownedCryo => DownedBossSystem.downedCryogen;
        public static bool DownedAS => DownedBossSystem.downedAquaticScourge;
        public static bool DownedBE => DownedBossSystem.downedBrimstoneElemental;
        public static bool DownedCalClone => DownedBossSystem.downedCalamitasClone;
        public static bool DownedAA => DownedBossSystem.downedAstrumAureus;
        public static bool DownedLevi => DownedBossSystem.downedLeviathan;
        public static bool DownedPBG => DownedBossSystem.downedPlaguebringer;
        public static bool DownedRav => DownedBossSystem.downedRavager;
        public static bool DownedDeus => DownedBossSystem.downedAstrumDeus;
        public static bool DownedFuck => DownedBossSystem.downedDragonfolly;
        public static bool DownedGuards => DownedBossSystem.downedGuardians;
        public static bool DownedProvi => DownedBossSystem.downedProvidence;
        public static bool DownedCV => DownedBossSystem.downedCeaselessVoid;
        public static bool DownedSignus => DownedBossSystem.downedSignus;
        public static bool DownedSW => DownedBossSystem.downedStormWeaver;
        public static bool DownedPolter => DownedBossSystem.downedPolterghast;
        public static bool DownedOD => DownedBossSystem.downedBoomerDuke;
        public static bool DownedDoG => DownedBossSystem.downedDoG;
        public static bool DownedYharon => DownedBossSystem.downedYharon;
        public static bool DownedExos => DownedBossSystem.downedExoMechs;
        public static bool DownedScal => DownedBossSystem.downedCalamitas;
        public static bool DownedCragmaw => DownedBossSystem.downedCragmawMire;
        public static bool DownedMauler => DownedBossSystem.downedMauler;
        public static bool DownedNuclear => DownedBossSystem.downedNuclearTerror;
        public static bool DownedGSS => DownedBossSystem.downedGSS;
        public static bool DownedClam => DownedBossSystem.downedCLAM;
        #endregion summonloadingbullshit
        public override void PostSetupContent()
        {
            if (!ModCompatibility.Calamity.Loaded)
                return;
            #region summons
            Mod mutant = ModLoader.GetMod("Fargowiltas");
            mutant.Call("AddSummon", 1.5f, "FargowiltasCrossmod", "MedallionoftheDesert",
                () => DownedDS, Item.buyPrice(gold: 6));
            mutant.Call("AddSummon", 2.5f, "FargowiltasCrossmod", "OphiocordycipitaceaeSprout",
                () => DownedCrab, Item.buyPrice(gold: 9));
            mutant.Call("AddSummon", 3.5f, "FargowiltasCrossmod", "HiveTumor",
                () => DownedHM, Item.buyPrice(gold: 11));
            mutant.Call("AddSummon", 3.5f, "FargowiltasCrossmod", "RedStainedWormFood",
                () => DownedPerf, Item.buyPrice(gold: 11));
            mutant.Call("AddSummon", 6.5f, "FargowiltasCrossmod", "MurkySludge",
                () => DownedSG, Item.buyPrice(gold: 17));
            mutant.Call("AddSummon", 8.5f, "FargowiltasCrossmod", "CryingKey",
                () => DownedCryo, Item.buyPrice(gold: 30));
            mutant.Call("AddSummon", 9.5f, "FargowiltasCrossmod", "SeeFood",
                () => DownedAS, Item.buyPrice(gold: 40));
            mutant.Call("AddSummon", 10.5f, "FargowiltasCrossmod", "FriedDoll",
                () => DownedBE, Item.buyPrice(gold: 40));
            mutant.Call("AddSummon", 11.5f, "FargowiltasCrossmod", "BlightedEye",
                () => DownedCalClone, Item.buyPrice(gold: 45));
            mutant.Call("AddSummon", 12.5f, "FargowiltasCrossmod", "SirensPearl",
                () => DownedLevi, Item.buyPrice(gold: 55));
            mutant.Call("AddSummon", 12.75f, "FargowiltasCrossmod", "ChunkyStardust",
                () => DownedAA, Item.buyPrice(gold: 55));
            mutant.Call("AddSummon", 13.5f, "FargowiltasCrossmod", "ABombInMyNation",
                () => DownedPBG, Item.buyPrice(gold: 60));
            mutant.Call("AddSummon", 13.75f, "FargowiltasCrossmod", "NoisyWhistle",
                () => DownedRav, Item.buyPrice(gold: 60));
            mutant.Call("AddSummon", 17.5f, "FargowiltasCrossmod", "AstrumCor",
                () => DownedDeus, Item.buyPrice(gold: 85));
            mutant.Call("AddSummon", 18.006f, "FargowiltasCrossmod", "BirbPheromones",
                () => DownedFuck, Item.buyPrice(platinum: 1, gold: 10));
            mutant.Call("AddSummon", 18.007f, "FargowiltasCrossmod", "DefiledShard",
                () => DownedGuards, Item.buyPrice(platinum: 1, gold: 10));
            mutant.Call("AddSummon", 18.008f, "FargowiltasCrossmod", "DefiledCore",
                () => DownedProvi, Item.buyPrice(platinum: 1, gold: 20));
            mutant.Call("AddSummon", 18.0091f, "FargowiltasCrossmod", "RiftofKos",
                () => DownedCV, Item.buyPrice(platinum: 1, gold: 30));
            mutant.Call("AddSummon", 18.0092f, "FargowiltasCrossmod", "WormFoodofKos",
                () => DownedSW, Item.buyPrice(platinum: 1, gold: 30));
            mutant.Call("AddSummon", 18.0093f, "FargowiltasCrossmod", "LetterofKos",
                () => DownedSignus, Item.buyPrice(platinum: 1, gold: 30));
            mutant.Call("AddSummon", 18.0094f, "FargowiltasCrossmod", "PolterplasmicBeacon",
                () => DownedPolter, Item.buyPrice(platinum: 1, gold: 40));
            mutant.Call("AddSummon", 18.0095f, "FargowiltasCrossmod", "BloodyWorm",
                () => DownedOD, Item.buyPrice(platinum: 1, gold: 40));
            mutant.Call("AddSummon", 18.0096f, "FargowiltasCrossmod", "SomeKindofSpaceWorm",
                () => DownedDoG, Item.buyPrice(platinum: 2));
            mutant.Call("AddSummon", 18.0097f, "FargowiltasCrossmod", "DragonEgg",
                () => DownedYharon, Item.buyPrice(platinum: 2, gold: 50));
            mutant.Call("AddSummon", 18.012f, "FargowiltasCrossmod", "PortableCodebreaker",
                () => DownedExos, Item.buyPrice(platinum: 3));
            mutant.Call("AddSummon", 18.014f, "FargowiltasCrossmod", "EyeofExtinction",
                () => DownedScal, Item.buyPrice(platinum: 3));
            #endregion summons
            #region bossrush
            //EXPLANATION OF TUPLES
            //int: npc id
            //int: time to set to (1 = day, -1 = night, 0 = dont force time)
            //Action<int>: "boss spawning function" idfk look at cal source but dont put null though
            //int: cooldown (to spawn i think???)
            //bool: boss needs special sound effect
            //float: dimness factor to be used when boss spawned
            //int[]: list of npc ids to not delete when spawning
            //int[]: list of other npc ids that need to die to continue event

            //Bosses =
            //    [
            //    new Boss(ModContent.NPCType<TrojanSquirrel>(), spawnContext: type =>{
            //        NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
            //        DownedBossSystem.startedBossRushAtLeastOnce = true;
            //    }, permittedNPCs: [ModContent.NPCType<TrojanSquirrelArms>(), ModContent.NPCType<TrojanSquirrelHead>()]),
            //    new Boss(ModContent.NPCType<TimberChampion>(), permittedNPCs: [ModContent.NPCType<TimberChampionHead>(), ModContent.NPCType<LesserSquirrel>()]),
            //    new Boss(NPCID.KingSlime, permittedNPCs: [NPCID.BlueSlime, NPCID.SlimeSpiked, NPCID.Pinky, NPCID.SpikedIceSlime, NPCID.SpikedJungleSlime, ModContent.NPCType<KingSlimeJewelRuby>(), ModContent.NPCType<KingSlimeJewelEmerald>(), ModContent.NPCType<KingSlimeJewelSapphire>()]),
            //    new Boss(ModContent.NPCType<DesertScourgeHead>(), permittedNPCs: [ModContent.NPCType<DesertScourgeBody>(), ModContent.NPCType<DesertScourgeTail>(), ModContent.NPCType<DesertNuisanceHead>(), ModContent.NPCType<DesertNuisanceHeadYoung>(), ModContent.NPCType<DesertNuisanceBody>(), ModContent.NPCType<DesertNuisanceBodyYoung>(), ModContent.NPCType<DesertNuisanceTail>(), ModContent.NPCType<DesertNuisanceTailYoung>()]),
            //    new Boss(NPCID.EyeofCthulhu, TimeChangeContext.Night, permittedNPCs: [NPCID.ServantofCthulhu, ModContent.NPCType<BloodlettingServant>()]),
            //    new Boss(ModContent.NPCType<Crabulon>(), permittedNPCs:[ModContent.NPCType<CrabShroom>(), ModContent.NPCType<FungalClump>()]),
            //    new Boss(NPCID.EaterofWorldsHead, permittedNPCs:[NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.VileSpit]),
            //    new Boss(NPCID.BrainofCthulhu, permittedNPCs:[NPCID.Creeper, ModContent.NPCType<BrainClone>(), ModContent.NPCType<BrainIllusion>(), ModContent.NPCType<BrainIllusion2>(), ModContent.NPCType<BrainIllusionAttack>()]),
            //    new Boss(ModContent.NPCType<HiveMind>(), permittedNPCs:[ModContent.NPCType<HiveBlob>(), ModContent.NPCType<HiveBlob2>(), NPCID.EaterofSouls, NPCID.DevourerHead, NPCID.DevourerBody, NPCID.DevourerTail, ModContent.NPCType<DarkHeart>(), ModContent.NPCType<DankCreeper>()]),
            //    new Boss(ModContent.NPCType<PerforatorHive>(), permittedNPCs:[ModContent.NPCType<PerforatorHeadLarge>(), ModContent.NPCType<PerforatorHeadMedium>(), ModContent.NPCType<PerforatorHeadSmall>(), ModContent.NPCType<PerforatorBodyLarge>(), ModContent.NPCType<PerforatorBodyMedium>(), ModContent.NPCType<PerforatorBodySmall>(), ModContent.NPCType<PerforatorTailLarge>(), ModContent.NPCType<PerforatorTailMedium>(), ModContent.NPCType<PerforatorTailSmall>()]),
            //    new Boss(ModContent.NPCType<SpiritChampion>(), permittedNPCs: [ModContent.NPCType<SpiritChampionHand>()]),
            //    new Boss(ModContent.NPCType<NatureChampion>(), permittedNPCs: [ModContent.NPCType<NatureChampionHead>()]),
            //    new Boss(NPCID.QueenBee, permittedNPCs:[NPCID.Bee, NPCID.BeeSmall, NPCID.HornetHoney, NPCID.BigHornetHoney, ModContent.NPCType<RoyalSubject>()]),
            //    new Boss(NPCID.SkeletronHead, TimeChangeContext.Night, permittedNPCs:[NPCID.SkeletronHand, NPCID.Skeleton, NPCID.BoneThrowingSkeleton, NPCID.BigSkeleton, NPCID.SmallSkeleton]),
            //    new Boss(NPCID.Deerclops, TimeChangeContext.Night),
            //    new Boss(ModContent.NPCType<SlimeGodCore>(), permittedNPCs:[ModContent.NPCType<EbonianPaladin>(), ModContent.NPCType<CrimulanPaladin>(), ModContent.NPCType<SplitCrimulanPaladin>(), ModContent.NPCType<SplitEbonianPaladin>(), ModContent.NPCType<CrimsonSlimeSpawn>(), ModContent.NPCType<CrimsonSlimeSpawn2>(), ModContent.NPCType<CorruptSlimeSpawn>(), ModContent.NPCType<CorruptSlimeSpawn2>()]),
            //    new Boss(ModContent.NPCType<DeviBoss>()),
            //    new Boss(NPCID.WallofFlesh, spawnContext: type =>{
            //        Player player = Main.player[ClosestPlayerToWorldCenter];

            //        NPC.SpawnWOF(player.position);
            //    }, permittedNPCs: [NPCID.WallofFleshEye, NPCID.TheHungry, NPCID.TheHungryII, NPCID.LeechHead, NPCID.LeechBody, NPCID.LeechTail]),
            //    new Boss(ModContent.NPCType<TerraChampion>(), permittedNPCs: [ModContent.NPCType<TerraChampionBody>(), ModContent.NPCType<TerraChampionTail>()]),
            //    new Boss(ModContent.NPCType<EarthChampion>(), permittedNPCs: [ModContent.NPCType<EarthChampionHand>()]),
            //    new Boss(NPCID.QueenSlimeBoss, permittedNPCs: [NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple, ModContent.NPCType<GelatinSlime>(), ModContent.NPCType<GelatinSubject>()]),
            //    new Boss(ModContent.NPCType<Cryogen>(), spawnContext: type => {
                    
            //        NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
            //    }, permittedNPCs:[ModContent.NPCType<PermafrostBoss>(), ModContent.NPCType<CryogenShield>()]),
            //    new Boss(ModContent.NPCType<BanishedBaron>()),
            //    new Boss(ModContent.NPCType<WillChampion>()),
            //    new Boss(NPCID.Spazmatism, TimeChangeContext.Night, type =>{
            //        NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, NPCID.Spazmatism);
            //        NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, NPCID.Retinazer);
            //    }, permittedNPCs: [NPCID.Retinazer]),
            //    new Boss(ModContent.NPCType<AquaticScourgeHead>(), permittedNPCs:[ModContent.NPCType<AquaticScourgeBody>(), ModContent.NPCType<AquaticScourgeBodyAlt>(), ModContent.NPCType<AquaticScourgeTail>()]),
            //    new Boss(NPCID.TheDestroyer, TimeChangeContext.Night, permittedNPCs:[NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Probe]),
            //    new Boss(ModContent.NPCType<BrimstoneElemental>(), permittedNPCs: [ModContent.NPCType<Brimling>()]),
            //    new Boss(NPCID.SkeletronPrime, TimeChangeContext.Night, permittedNPCs:[ModContent.NPCType<SkeletronPrime2>(), NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.Probe]),
            //    new Boss(ModContent.NPCType<CalamitasClone>(), TimeChangeContext.Night, permittedNPCs:[ModContent.NPCType<Catastrophe>(), ModContent.NPCType<Cataclysm>(), ModContent.NPCType<SoulSeeker>()]),
            //    new Boss(ModContent.NPCType<LifeChallenger>(), TimeChangeContext.Day, type =>{
            //        SoundEngine.PlaySound(SoundID.ScaryScream, Main.player[ClosestPlayerToWorldCenter].Center);
            //        int lief = NPC.NewNPC(new EntitySource_WorldEvent(), (int)Main.player[ClosestPlayerToWorldCenter].Center.X, (int)Main.player[ClosestPlayerToWorldCenter].Center.Y - 300, type, 1);
            //        Main.npc[lief].timeLeft *= 20;
            //        CalamityUtils.BossAwakenMessage(lief);
            //    }, usesSpecialSound: true),
            //    new Boss(ModContent.NPCType<LifeChampion>(), TimeChangeContext.Day, type =>{
            //        SoundEngine.PlaySound(SoundID.ScaryScream, Main.player[ClosestPlayerToWorldCenter].Center);
            //        int lief = NPC.NewNPC(new EntitySource_WorldEvent(), (int)Main.player[ClosestPlayerToWorldCenter].Center.X, (int)Main.player[ClosestPlayerToWorldCenter].Center.Y - 300, type, 1);
            //        Main.npc[lief].timeLeft *= 20;
            //        CalamityUtils.BossAwakenMessage(lief);
            //    }, usesSpecialSound: true),
            //    new Boss(NPCID.Plantera, permittedNPCs: [NPCID.PlanterasHook, NPCID.PlanterasTentacle, ModContent.NPCType<PlanterasFreeTentacle>(), NPCID.Spore]),
            //    new Boss(ModContent.NPCType<Anahita>(), permittedNPCs: [ModContent.NPCType<Leviathan>(), NPCID.DetonatingBubble, ModContent.NPCType<AnahitasIceShield>()]),
            //    new Boss(ModContent.NPCType<AstrumAureus>(), TimeChangeContext.Night, permittedNPCs:[ModContent.NPCType<AureusSpawn>()]),
            //    new Boss(NPCID.Golem, TimeChangeContext.Day, type => {
            //        int golem = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].position.X + Main.rand.Next(-100, 101)), (int)(Main.player[ClosestPlayerToWorldCenter].position.Y - 400), type, 1);
            //        Main.npc[golem].timeLeft *= 20;
            //        CalamityUtils.BossAwakenMessage(golem);
            //    }, permittedNPCs:[NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree]),
            //    new Boss(ModContent.NPCType<PlaguebringerGoliath>(), permittedNPCs:[ModContent.NPCType<PlagueHomingMissile>(), ModContent.NPCType<PlagueMine>()]),
            //    new Boss(NPCID.HallowBoss, TimeChangeContext.Night, type =>{
            //        int eol = NPC.NewNPC(new EntitySource_WorldEvent(), (int)Main.player[ClosestPlayerToWorldCenter].Center.X, (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 400), type, 1);
            //        Main.npc[eol].timeLeft *= 20;
            //        CalamityUtils.BossAwakenMessage(eol);
            //    }),
            //    new Boss(ModContent.NPCType<ShadowChampion>(), TimeChangeContext.Night, permittedNPCs: [ModContent.NPCType<ShadowOrbNPC>()]),
            //    new Boss(NPCID.DukeFishron, TimeChangeContext.Day, type =>{
            //        int duke = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X + Main.rand.Next(-100, 101)), (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 300), type, 1);
            //        Main.npc[duke].timeLeft *= 20;
            //        CalamityUtils.BossAwakenMessage(duke);
            //    }, permittedNPCs: [NPCID.Sharkron, NPCID.Sharkron2, NPCID.DetonatingBubble]),
            //    new Boss(ModContent.NPCType<RavagerBody>(), spawnContext: type =>{
            //        int rav = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X + Main.rand.Next(-100, 101)), (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 400), type, 1);
            //        Main.npc[rav].timeLeft *= 20;
            //        CalamityUtils.BossAwakenMessage(rav);
            //    }, permittedNPCs: [ModContent.NPCType<RavagerClawLeft>(), ModContent.NPCType<RavagerClawRight>(), ModContent.NPCType<RavagerLegLeft>(), ModContent.NPCType<RavagerLegRight>(), ModContent.NPCType<RavagerHead>(), ModContent.NPCType<RavagerHead2>(), ModContent.NPCType<RockPillar>(), ModContent.NPCType<FlamePillar>()]),
            //    new Boss(NPCID.CultistBoss, spawnContext: type => {
            //        int lc = NPC.NewNPC(new EntitySource_WorldEvent(), (int)Main.player[ClosestPlayerToWorldCenter].Center.X, (int)Main.player[ClosestPlayerToWorldCenter].Center.Y - 400, type, 1);
            //        Main.npc[lc].timeLeft *= 20;
            //        CalamityUtils.BossAwakenMessage(lc);
            //    }, permittedNPCs:[NPCID.CultistBossClone, NPCID.CultistDragonHead, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4, NPCID.CultistDragonTail, NPCID.AncientCultistSquidhead, NPCID.AncientLight, NPCID.AncientDoom]),
            //    new Boss(ModContent.NPCType<AstrumDeusHead>(), TimeChangeContext.Night, type => {
            //        SoundEngine.PlaySound(AstrumDeusHead.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
            //        NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
            //    }, usesSpecialSound: true, permittedNPCs: [ModContent.NPCType<AstrumDeusBody>(), ModContent.NPCType<AstrumDeusTail>()]),
            //    new Boss(NPCID.MoonLordCore, spawnContext: type =>{
            //        NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
            //    }, permittedNPCs: [NPCID.MoonLordLeechBlob, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.MoonLordFreeEye]),
            //    new Boss(ModContent.NPCType<Bumblefuck>(), permittedNPCs: [ModContent.NPCType<Bumblefuck2>()]),
            //    new Boss(ModContent.NPCType<ProfanedGuardianCommander>(), TimeChangeContext.Day, permittedNPCs:[ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.NPCType<ProfanedGuardianHealer>(), ModContent.NPCType<ProfanedRocks>()]),
            //    new Boss(ModContent.NPCType<Providence>(), TimeChangeContext.Day, type =>{
            //        SoundEngine.PlaySound(Providence.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
            //        int provi = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X), (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 400), type, 1);
            //        Main.npc[provi].timeLeft *= 20;
            //        CalamityUtils.BossAwakenMessage(provi);
            //    }, usesSpecialSound: true, permittedNPCs: [ModContent.NPCType<ProvSpawnDefense>(), ModContent.NPCType<ProvSpawnHealer>(), ModContent.NPCType<ProvSpawnOffense>()]),
            //    new Boss(ModContent.NPCType<CosmosChampion>(), spawnContext: type => {
            //        int erd = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X), (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 400), type, 1);
            //        Main.npc[erd].timeLeft *= 20;
            //        CalamityUtils.BossAwakenMessage(erd);
            //    }),
            //    new Boss(ModContent.NPCType<StormWeaverHead>(), permittedNPCs: [ModContent.NPCType<StormWeaverBody>(), ModContent.NPCType<StormWeaverTail>()]),
            //    new Boss(ModContent.NPCType<CeaselessVoid>(), permittedNPCs: [ModContent.NPCType<DarkEnergy>()]),
            //    new Boss(ModContent.NPCType<Signus>(), permittedNPCs: [ModContent.NPCType<CosmicLantern>()]),
            //    new Boss(ModContent.NPCType<Polterghast>(), permittedNPCs: [ModContent.NPCType<PhantomFuckYou>(), ModContent.NPCType<PolterghastHook>(), ModContent.NPCType<PolterPhantom>()]),
            //    new Boss(ModContent.NPCType<OldDuke>(), spawnContext: type => {
            //        int od = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X + Main.rand.Next(-100, 101)), (int)Main.player[ClosestPlayerToWorldCenter].Center.Y - 300, type, 1);
            //        CalamityUtils.BossAwakenMessage(od);
            //        Main.npc[od].timeLeft *= 20;
            //    }, permittedNPCs: [ModContent.NPCType<SulphurousSharkron>(), ModContent.NPCType<OldDukeToothBall>()]),
            //    new Boss(ModContent.NPCType<DevourerofGodsHead>(), spawnContext: type => {
            //        SoundEngine.PlaySound(DevourerofGodsHead.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
            //        NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
            //    }, usesSpecialSound: true, permittedNPCs: [ModContent.NPCType<DevourerofGodsBody>(), ModContent.NPCType<DevourerofGodsTail>(), ModContent.NPCType<CosmicGuardianBody>(), ModContent.NPCType<CosmicGuardianHead>(), ModContent.NPCType<CosmicGuardianTail>()]),
            //    new Boss(ModContent.NPCType<Yharon>()),
            //    new Boss(ModContent.NPCType<AbomBoss>()),
            //    new Boss(ModContent.NPCType<Draedon>(), permittedNPCs: [ModContent.NPCType<Artemis>(), ModContent.NPCType<Apollo>(), ModContent.NPCType<ThanatosBody1>(), ModContent.NPCType<ThanatosBody2>(), ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<ThanatosTail>(), ModContent.NPCType<AresBody>(), ModContent.NPCType<AresLaserCannon>(), ModContent.NPCType<AresPlasmaFlamethrower>(), ModContent.NPCType<AresTeslaCannon>(), ModContent.NPCType<AresGaussNuke>()]),
            //    new Boss(ModContent.NPCType<SupremeCalamitas>(), spawnContext: type => {
            //        SoundEngine.PlaySound(SupremeCalamitas.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
            //        CalamityUtils.SpawnBossBetter(Main.player[ClosestPlayerToWorldCenter].Top - new Vector2(42, 84f), type);
            //    }, dimnessFactor: 0.5f, permittedNPCs: [ModContent.NPCType<SepulcherArm>(), ModContent.NPCType<SepulcherBody>(), ModContent.NPCType<SepulcherHead>(), ModContent.NPCType<SepulcherTail>(), ModContent.NPCType<SepulcherBodyEnergyBall>(), ModContent.NPCType<SoulSeekerSupreme>(), ModContent.NPCType<BrimstoneHeart>(), ModContent.NPCType<SupremeCataclysm>(), ModContent.NPCType<SupremeCatastrophe>()]),
            //    new Boss(ModContent.NPCType<MutantBoss>(), permittedNPCs: [ModContent.NPCType<MutantIllusion>()])
            //    ];
            for (int i = Bosses.Count - 1; i >= 0; i--)
            {
                if (Bosses[i].EntityID == NPCID.KingSlime)
                {
                    Bosses[i] = new Boss(NPCID.KingSlime, permittedNPCs: [NPCID.BlueSlime, NPCID.SlimeSpiked, NPCID.Pinky, NPCID.SpikedIceSlime, NPCID.SpikedJungleSlime, ModContent.NPCType<KingSlimeJewelRuby>(), ModContent.NPCType<KingSlimeJewelEmerald>(), ModContent.NPCType<KingSlimeJewelSapphire>()]);
                    Bosses.Insert(i, new Boss(ModContent.NPCType<TimberChampion>(), permittedNPCs: [ModContent.NPCType<TimberChampionHead>(), ModContent.NPCType<LesserSquirrel>()]));
                    Bosses.Insert(i, new Boss(ModContent.NPCType<TrojanSquirrel>(), spawnContext: type =>
                    {
                        NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                        DownedBossSystem.startedBossRushAtLeastOnce = true;
                    }, permittedNPCs: [ModContent.NPCType<TrojanSquirrelArms>(), ModContent.NPCType<TrojanSquirrelHead>()]));   
                }
                if (Bosses[i].EntityID == NPCID.QueenBee)
                {
                    Bosses[i].HostileNPCsToNotDelete.Add(ModContent.NPCType<RoyalSubject>());
                    Bosses.Insert(i, new Boss(ModContent.NPCType<NatureChampion>(), permittedNPCs: [ModContent.NPCType<NatureChampionHead>()]));
                    Bosses.Insert(i, new Boss(ModContent.NPCType<SpiritChampion>(), permittedNPCs: [ModContent.NPCType<SpiritChampionHand>()]));

                }
                if (Bosses[i].EntityID == ModContent.NPCType<SlimeGodCore>())
                {
                    Bosses.Insert(i, new Boss(ModContent.NPCType<DeviBoss>()));
                }
                if (Bosses[i].EntityID == NPCID.QueenSlimeBoss)
                {
                    Bosses[i].HostileNPCsToNotDelete.Add(ModContent.NPCType<GelatinSubject>());
                    Bosses[i].HostileNPCsToNotDelete.Add(ModContent.NPCType<GelatinSlime>());
                    Bosses.Insert(i, new Boss(ModContent.NPCType<EarthChampion>(), permittedNPCs: [ModContent.NPCType<EarthChampionHand>()]));
                    Bosses.Insert(i, new Boss(ModContent.NPCType<TerraChampion>(), permittedNPCs: [ModContent.NPCType<TerraChampionBody>(), ModContent.NPCType<TerraChampionTail>()]));
                
                }
                if (Bosses[i].EntityID == ModContent.NPCType<Cryogen>())
                {
                    Bosses[i].HostileNPCsToNotDelete.Add(ModContent.NPCType<PermafrostBoss>());
                }
                if (Bosses[i].EntityID == NPCID.Spazmatism)
                {
                    Bosses.Insert(i, new Boss(ModContent.NPCType<WillChampion>()));
                    Bosses.Insert(i, new Boss(ModContent.NPCType<BanishedBaron>()));
                }
                if (Bosses[i].EntityID == NPCID.Plantera)
                {
                    Bosses[i].HostileNPCsToNotDelete.Add(ModContent.NPCType<CrystalLeaf>());
                    Bosses.Insert(i, new Boss(ModContent.NPCType<LifeChampion>(), TimeChangeContext.Day, type => {
                        SoundEngine.PlaySound(SoundID.ScaryScream, Main.player[ClosestPlayerToWorldCenter].Center);
                        int lief = NPC.NewNPC(new EntitySource_WorldEvent(), (int)Main.player[ClosestPlayerToWorldCenter].Center.X, (int)Main.player[ClosestPlayerToWorldCenter].Center.Y - 300, type, 1);
                        Main.npc[lief].timeLeft *= 20;
                        CalamityUtils.BossAwakenMessage(lief);
                    }, usesSpecialSound: true));
                    Bosses.Insert(i, new Boss(ModContent.NPCType<LifeChallenger>(), TimeChangeContext.Day, type => {
                        SoundEngine.PlaySound(SoundID.ScaryScream, Main.player[ClosestPlayerToWorldCenter].Center);
                        int lief = NPC.NewNPC(new EntitySource_WorldEvent(), (int)Main.player[ClosestPlayerToWorldCenter].Center.X, (int)Main.player[ClosestPlayerToWorldCenter].Center.Y - 300, type, 1);
                        Main.npc[lief].timeLeft *= 20;
                        CalamityUtils.BossAwakenMessage(lief);
                    }, usesSpecialSound: true));
                }
                if (Bosses[i].EntityID == NPCID.DukeFishron)
                {
                    Bosses.Insert(i, new Boss(ModContent.NPCType<ShadowChampion>(), TimeChangeContext.Night, permittedNPCs: [ModContent.NPCType<ShadowOrbNPC>()]));
                }
                if (Bosses[i].EntityID == ModContent.NPCType<StormWeaverHead>())
                {
                    Bosses.Insert(i, new Boss(ModContent.NPCType<CosmosChampion>(), spawnContext: type =>
                    {
                        int erd = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X), (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 400), type, 1);
                        Main.npc[erd].timeLeft *= 20;
                        CalamityUtils.BossAwakenMessage(erd);
                    }));
                }
                if (Bosses[i].EntityID == ModContent.NPCType<Draedon>())
                {
                    Bosses.Insert(i, new Boss(ModContent.NPCType<AbomBoss>()));
                }
                if (Bosses[i].EntityID == ModContent.NPCType<SupremeCalamitas>())
                {
                    Bosses.Add(new Boss(ModContent.NPCType<MutantBoss>(), permittedNPCs: [ModContent.NPCType<MutantIllusion>()]));
                }

            }
            BossIDsAfterDeath.Add(ModContent.NPCType<TimberChampion>(), [ModContent.NPCType<TimberChampionHead>()]);
            BossIDsAfterDeath.Add(ModContent.NPCType<Cryogen>(), [ModContent.NPCType<PermafrostBoss>()]);
            
            BossDeathEffects.Remove(ModContent.NPCType<SupremeCalamitas>());
            BossDeathEffects.Add(ModContent.NPCType<MutantBoss>(), npc => { BossRushDialogueSystem.StartDialogue(DownedBossSystem.downedBossRush ? BossRushDialoguePhase.EndRepeat : BossRushDialoguePhase.End); });
            
            ////Adding bosses to boss rush
            Mod cal = ModCompatibility.Calamity.Mod;
            ////get the list of boss entries
            //var Entries = cal.Call("GetBossRushEntries") as List<(int, int, Action<int>, int, bool, float, int[], int[])>;
            ////insert trojan squirrel at the beginning
            //Entries!.Insert(0, (ModContent.NPCType<TrojanSquirrel>(), 0, delegate
            //{
            //    FargoSoulsUtil.SpawnBossNetcoded(Main.player[Main.myPlayer], ModContent.NPCType<TrojanSquirrel>());
            //}, -1, false, 0f, new[] { ModContent.NPCType<TrojanSquirrelArms>(), ModContent.NPCType<TrojanSquirrelHead>(), ModContent.NPCType<TrojanSquirrelLimb>(), ModContent.NPCType<TrojanSquirrelPart>() }, Array.Empty<int>()));
            ////insert deviantt before sg
            //Entries.Insert(12, (ModContent.NPCType<DeviBoss>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<DeviBoss>());
            //}, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            //Entries.Insert(16, (ModContent.NPCType<BanishedBaron>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<BanishedBaron>());
            //}, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            ////insert lieflight after mechs
            //Entries.Insert(23, (ModContent.NPCType<LifeChallenger>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<LifeChallenger>());
            //}, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            ////insert champions after provi
            //Entries.Insert(39, (ModContent.NPCType<CosmosChampion>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<CosmosChampion>());
            //}, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            //Entries.Insert(39, (ModContent.NPCType<WillChampion>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<WillChampion>());
            //}, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            //Entries.Insert(39, (ModContent.NPCType<ShadowChampion>(), -1, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<ShadowChampion>());
            //}, -1, false, 0f, new[] { ModContent.NPCType<ShadowOrbNPC>(), }, Array.Empty<int>()));
            //Entries.Insert(39, (ModContent.NPCType<SpiritChampion>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<SpiritChampion>());
            //}, -1, false, 0f, new[] { ModContent.NPCType<SpiritChampionHand>() }, Array.Empty<int>()));
            //Entries.Insert(39, (ModContent.NPCType<LifeChampion>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<LifeChampion>());
            //}, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            //Entries.Insert(39, (ModContent.NPCType<NatureChampion>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<NatureChampion>());
            //}, -1, false, 0f, new[] { ModContent.NPCType<NatureChampionHead>() }, Array.Empty<int>()));
            //Entries.Insert(39, (ModContent.NPCType<EarthChampion>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<EarthChampion>());
            //}, -1, false, 0f, new[] { ModContent.NPCType<EarthChampionHand>() }, Array.Empty<int>()));
            //Entries.Insert(39, (ModContent.NPCType<TerraChampion>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<TerraChampion>());

            //}, -1, false, 0f, new[] { ModContent.NPCType<TerraChampionBody>(), ModContent.NPCType<TerraChampionTail>() }, Array.Empty<int>()));
            //Entries.Insert(39, (ModContent.NPCType<TimberChampion>(), 0, delegate
            //{
            //    FargoSoulsUtil.SpawnBossNetcoded(Main.player[Main.myPlayer], ModContent.NPCType<TimberChampion>());

            //}, -1, false, 0f, new[] { ModContent.NPCType<TimberChampionHead>() }, new[] { ModContent.NPCType<TimberChampionHead>() }));
            ////add abom before draedon
            //Entries.Insert(55, (ModContent.NPCType<AbomBoss>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<AbomBoss>());
            //}, -1, false, 0f, new[] { ModContent.NPCType<AbomSaucer>() }, Array.Empty<int>()));
            ////add mutant boss entry to the end
            //Entries.Add((ModContent.NPCType<MutantBoss>(), 0, delegate
            //{
            //    NPC.SpawnOnPlayer(0, ModContent.NPCType<MutantBoss>());
            //}, -1, false, 0f, new int[] { ModContent.NPCType<MutantIllusion>() }, Array.Empty<int>()));
            //for (int i = 0; i < Entries.Count; i++)
            //{
            //    if (Entries[i].Item1 == ModContent.NPCType<Crabulon>())
            //    {
            //        Entries[i].Item7.Append(ModContent.NPCType<FungalClump>());
            //    }
            //    if (Entries[i].Item1 == ModContent.NPCType<PerforatorHive>())
            //    {
            //        Entries[i].Item7.Append(ModContent.NPCType<LargePerforatorHead>());
            //        Entries[i].Item7.Append(ModContent.NPCType<LargePerforatorBody>());
            //        Entries[i].Item7.Append(ModContent.NPCType<LargePerforatorBody2>());
            //        Entries[i].Item7.Append(ModContent.NPCType<LargePerforatorTail>());
            //    }
            //    if (Entries[i].Item1 == NPCID.BrainofCthulhu)
            //    {
            //        Entries[i].Item7.Append(ModContent.NPCType<BrainIllusion>());
            //        Entries[i].Item7.Append(ModContent.NPCType<BrainIllusion2>());
            //        Entries[i].Item7.Append(ModContent.NPCType<BrainIllusionAttack>());
            //    }
            //    if (Entries[i].Item1 == NPCID.QueenBee)
            //    {
            //        Entries[i].Item7.Append(ModContent.NPCType<RoyalSubject>());
            //    }
            //    if (Entries[i].Item1 == NPCID.Plantera)
            //    {
            //        Entries[i].Item7.Append(ModContent.NPCType<CrystalLeaf>());
            //    }
            //    //if (Entries[i].Item1 == NPCID.HallowBoss)
            //    //{
            //    //    Entries.RemoveAt(i);
            //    //    Entries.Insert(i, (NPCID.HallowBoss, 0, delegate { NPC.SpawnOnPlayer(0, NPCID.HallowBoss); }, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            //    //}
            //}
            ////set boss rush entries to new list of entries
            //cal.Call("SetBossRushEntries", Entries);
            ////make scal not end the event on defeat so it continues to mutant
            //DeathEffectsList.Remove(ModContent.NPCType<SupremeCalamitas>());
            #endregion bossrush
            cal.Call("RegisterModCooldowns", FargowiltasCrossmod.Instance);
            cal.Call("AddDifficultyToUI", new EternityRevDifficulty());
            cal.Call("AddDifficultyToUI", new EternityDeathDifficulty());
            #region CalDebuffListCompat
            CalamityLists.debuffList.Add(ModContent.BuffType<AnticoagulationBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<AntisocialBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<AtrophiedBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<BerserkedBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<BloodthirstyBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<BaronsBurdenBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<ClippedWingsBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<CrippledBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<CurseoftheMoonBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<DefenselessBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<FlamesoftheUniverseBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<FlippedBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<FusedBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<GodEaterBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<GuiltyBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<HexedBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<HypothermiaBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<InfestedBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<IvyVenomBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<JammedBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<LethargicBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<LightningRodBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<LivingWastelandBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<LovestruckBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<LowGroundBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<MarkedforDeathBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<MidasBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<MutantNibbleBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<NanoInjectionBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<NeurotoxinBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<NullificationCurseBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<OceanicMaulBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<OiledBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<PurgedBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<PurifiedBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<ReverseManaFlowBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<RottingBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<SmiteBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<ShadowflameBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<SqueakyToyBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<StunnedBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<SwarmingBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<TimeFrozenBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<UnluckyBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<UnstableBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<BerserkerInstallBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<HolyPriceBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<BrainOfConfusionBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<LihzahrdCurseBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<RushJobBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<TwinsInstallBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<SnowstormCDBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<CorruptingBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<HellFireBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<LeadPoisonBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<OriPoisonBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<PungentGazeBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<SolarFlareBuff>());
            #endregion CalDebuffListCompat
        }
        //make this a property instead of directly using it so tml doesnt shit itself trying to load it
        public ref Dictionary<int, Action<NPC>> DeathEffectsList => ref BossRushEvent.BossDeathEffects;
    }
}
