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
using CalamityMod.UI.DraedonSummoning;
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
using Mono.Cecil;
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

            Bosses =
                [
                new Boss(NPCID.KingSlime, spawnContext: type => {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);

                    
                },permittedNPCs: new int[] { NPCID.BlueSlime, NPCID.YellowSlime, NPCID.PurpleSlime, NPCID.RedSlime, NPCID.GreenSlime, NPCID.RedSlime,
                    NPCID.IceSlime, NPCID.UmbrellaSlime, NPCID.Pinky, NPCID.SlimeSpiked, NPCID.RainbowSlime, ModContent.NPCType<KingSlimeJewelRuby>(),
                    ModContent.NPCType<KingSlimeJewelSapphire>(), ModContent.NPCType<KingSlimeJewelEmerald>() }),

                new Boss(NPCID.MoonLordCore, spawnContext: type =>{
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                    // When Moon Lord spawns, Boss Rush is considered to be started at least once.
                    // King Slime will then be skipped
                    DownedBossSystem.startedBossRushAtLeastOnce = true;
                }, permittedNPCs: [NPCID.MoonLordLeechBlob, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.MoonLordFreeEye]),
                new Boss(ModContent.NPCType<Providence>(), TimeChangeContext.Day, type =>{
                    SoundEngine.PlaySound(Providence.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
                    int provi = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X), (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 400), type, 1);
                    Main.npc[provi].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(provi);
                }, usesSpecialSound: true, permittedNPCs: [ModContent.NPCType<ProvSpawnDefense>(), ModContent.NPCType<ProvSpawnHealer>(), ModContent.NPCType<ProvSpawnOffense>(),
                    ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.NPCType<ProfanedGuardianHealer>()]),
                

                new Boss(ModContent.NPCType<Polterghast>(), permittedNPCs: [ModContent.NPCType<PhantomFuckYou>(), ModContent.NPCType<PolterghastHook>(), ModContent.NPCType<PolterPhantom>()]),
                new Boss(ModContent.NPCType<OldDuke>(), spawnContext: type => {
                    int od = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X + Main.rand.Next(-100, 101)), (int)Main.player[ClosestPlayerToWorldCenter].Center.Y - 300, type, 1);
                    CalamityUtils.BossAwakenMessage(od);
                    Main.npc[od].timeLeft *= 20;
                }, permittedNPCs: [ModContent.NPCType<SulphurousSharkron>(), ModContent.NPCType<OldDukeToothBall>()]),
                new Boss(ModContent.NPCType<DevourerofGodsHead>(), spawnContext: type => {
                    SoundEngine.PlaySound(DevourerofGodsHead.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true, permittedNPCs: [ModContent.NPCType<DevourerofGodsBody>(), ModContent.NPCType<DevourerofGodsTail>(), ModContent.NPCType<CosmicGuardianBody>(), ModContent.NPCType<CosmicGuardianHead>(), ModContent.NPCType<CosmicGuardianTail>(), 
                ModContent.NPCType<Signus>(), ModContent.NPCType<CeaselessVoid>(), ModContent.NPCType<StormWeaverHead>(), ModContent.NPCType<StormWeaverBody>(), ModContent.NPCType<StormWeaverTail>()]),
                new Boss(ModContent.NPCType<CosmosChampion>(), spawnContext: type => {
                    int erd = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X), (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 400), type, 1);
                    Main.npc[erd].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(erd);
                }),
                new Boss(ModContent.NPCType<Yharon>(), permittedNPCs: ModContent.NPCType<Bumblefuck>()),
                new Boss(ModContent.NPCType<AbomBoss>()),
                new Boss(ModContent.NPCType<Draedon>(), spawnContext: type =>
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<Draedon>()))
                    {
                        Player player = Main.player[ClosestPlayerToWorldCenter];

                        SoundEngine.PlaySound(CodebreakerUI.SummonSound, player.Center);
                        Vector2 spawnPos = player.Center + new Vector2(-8f, -100f);
                        int draedon = NPC.NewNPC(new EntitySource_WorldEvent("CalamityMod_BossRush"), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<Draedon>());
                        Main.npc[draedon].timeLeft *= 20;
                    }
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<Apollo>(), ModContent.NPCType<AresBody>(), ModContent.NPCType<AresGaussNuke>(), ModContent.NPCType<AresLaserCannon>(), ModContent.NPCType<AresPlasmaFlamethrower>(), ModContent.NPCType<AresTeslaCannon>(), ModContent.NPCType<Artemis>(), ModContent.NPCType<ThanatosBody1>(), ModContent.NPCType<ThanatosBody2>(), ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<ThanatosTail>() }),
                new Boss(ModContent.NPCType<SupremeCalamitas>(), spawnContext: type => {
                    SoundEngine.PlaySound(SupremeCalamitas.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
                    CalamityUtils.SpawnBossBetter(Main.player[ClosestPlayerToWorldCenter].Top - new Vector2(42, 84f), type);
                }, dimnessFactor: 0.5f, permittedNPCs: [ModContent.NPCType<SepulcherArm>(), ModContent.NPCType<SepulcherBody>(), ModContent.NPCType<SepulcherHead>(), ModContent.NPCType<SepulcherTail>(), ModContent.NPCType<SepulcherBodyEnergyBall>(), ModContent.NPCType<SoulSeekerSupreme>(), ModContent.NPCType<BrimstoneHeart>(), ModContent.NPCType<SupremeCataclysm>(), ModContent.NPCType<SupremeCatastrophe>()]),
                new Boss(ModContent.NPCType<MutantBoss>(), permittedNPCs: [ModContent.NPCType<MutantIllusion>()])
                ];
            
            
            BossDeathEffects.Remove(ModContent.NPCType<SupremeCalamitas>());
            BossDeathEffects.Remove(ModContent.NPCType<DevourerofGodsHead>());
            BossDeathEffects.Add(ModContent.NPCType<MutantBoss>(), npc => { BossRushDialogueSystem.StartDialogue(DownedBossSystem.downedBossRush ? BossRushDialoguePhase.EndRepeat : BossRushDialoguePhase.End); });

            ////Adding bosses to boss rush
            #endregion bossrush
            Mod cal = ModCompatibility.Calamity.Mod;
            cal.Call("RegisterModCooldowns", FargowiltasCrossmod.Instance);
            cal.Call("AddDifficultyToUI", new EternityRevDifficulty());
            cal.Call("AddDifficultyToUI", new EternityDeathDifficulty());

            #region CalDebuffListCompat
            List<int> calamityDebuffs = CalamityLists.debuffList.Where(i => i >= BuffID.Count).ToList();
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
            CalamityLists.debuffList.Add(ModContent.BuffType<HellFireBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<LeadPoisonBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<OriPoisonBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<PungentGazeBuff>());
            CalamityLists.debuffList.Add(ModContent.BuffType<SolarFlareBuff>());
            FieldInfo debuffIDs = typeof(FargowiltasSouls.FargowiltasSouls).GetField("DebuffIDs", LumUtils.UniversalBindingFlags);
            List<int> newDebuffIDs = (List<int>)debuffIDs.GetValue(null);
            newDebuffIDs.AddRange(calamityDebuffs);
            debuffIDs.SetValue(null, newDebuffIDs);
            #endregion CalDebuffListCompat
        }
        //make this a property instead of directly using it so tml doesnt shit itself trying to load it
        public ref Dictionary<int, Action<NPC>> DeathEffectsList => ref BossRushEvent.BossDeathEffects;
    }
}
