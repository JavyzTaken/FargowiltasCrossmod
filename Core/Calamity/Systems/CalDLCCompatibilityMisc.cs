using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles;
using CalamityMod.World;
using Fargowiltas.NPCs;
using FargowiltasCrossmod.Content.Calamity.Bosses.Crabulon;
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
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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

            //Adding bosses to boss rush
            Mod cal = ModCompatibility.Calamity.Mod;
            //get the list of boss entries
            var Entries = cal.Call("GetBossRushEntries") as List<(int, int, Action<int>, int, bool, float, int[], int[])>;
            //insert trojan squirrel at the beginning
            Entries!.Insert(0, (ModContent.NPCType<TrojanSquirrel>(), 0, delegate
            {
                FargoSoulsUtil.SpawnBossNetcoded(Main.player[Main.myPlayer], ModContent.NPCType<TrojanSquirrel>());
            }, -1, false, 0f, new[] { ModContent.NPCType<TrojanSquirrelArms>(), ModContent.NPCType<TrojanSquirrelHead>(), ModContent.NPCType<TrojanSquirrelLimb>(), ModContent.NPCType<TrojanSquirrelPart>() }, Array.Empty<int>()));
            //insert deviantt before sg
            Entries.Insert(12, (ModContent.NPCType<DeviBoss>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<DeviBoss>());
            }, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            Entries.Insert(16, (ModContent.NPCType<BanishedBaron>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<BanishedBaron>());
            }, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            //insert lieflight after mechs
            Entries.Insert(23, (ModContent.NPCType<LifeChallenger>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<LifeChallenger>());
            }, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            //insert champions after provi
            Entries.Insert(39, (ModContent.NPCType<CosmosChampion>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<CosmosChampion>());
            }, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            Entries.Insert(39, (ModContent.NPCType<WillChampion>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<WillChampion>());
            }, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            Entries.Insert(39, (ModContent.NPCType<ShadowChampion>(), -1, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<ShadowChampion>());
            }, -1, false, 0f, new[] { ModContent.NPCType<ShadowOrbNPC>(), }, Array.Empty<int>()));
            Entries.Insert(39, (ModContent.NPCType<SpiritChampion>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<SpiritChampion>());
            }, -1, false, 0f, new[] { ModContent.NPCType<SpiritChampionHand>() }, Array.Empty<int>()));
            Entries.Insert(39, (ModContent.NPCType<LifeChampion>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<LifeChampion>());
            }, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
            Entries.Insert(39, (ModContent.NPCType<NatureChampion>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<NatureChampion>());
            }, -1, false, 0f, new[] { ModContent.NPCType<NatureChampionHead>() }, Array.Empty<int>()));
            Entries.Insert(39, (ModContent.NPCType<EarthChampion>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<EarthChampion>());
            }, -1, false, 0f, new[] { ModContent.NPCType<EarthChampionHand>() }, Array.Empty<int>()));
            Entries.Insert(39, (ModContent.NPCType<TerraChampion>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<TerraChampion>());

            }, -1, false, 0f, new[] { ModContent.NPCType<TerraChampionBody>(), ModContent.NPCType<TerraChampionTail>() }, Array.Empty<int>()));
            Entries.Insert(39, (ModContent.NPCType<TimberChampion>(), 0, delegate
            {
                FargoSoulsUtil.SpawnBossNetcoded(Main.player[Main.myPlayer], ModContent.NPCType<TimberChampion>());

            }, -1, false, 0f, new[] { ModContent.NPCType<TimberChampionHead>() }, new[] { ModContent.NPCType<TimberChampionHead>() }));
            //add abom before draedon
            Entries.Insert(55, (ModContent.NPCType<AbomBoss>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<AbomBoss>());
            }, -1, false, 0f, new[] { ModContent.NPCType<AbomSaucer>() }, Array.Empty<int>()));
            //add mutant boss entry to the end
            Entries.Add((ModContent.NPCType<MutantBoss>(), 0, delegate
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<MutantBoss>());
            }, -1, false, 0f, new int[] { ModContent.NPCType<MutantIllusion>() }, Array.Empty<int>()));
            for (int i = 0; i < Entries.Count; i++)
            {
                if (Entries[i].Item1 == ModContent.NPCType<Crabulon>())
                {
                    Entries[i].Item7.Append(ModContent.NPCType<FungalClump>());
                }
                if (Entries[i].Item1 == ModContent.NPCType<PerforatorHive>())
                {
                    Entries[i].Item7.Append(ModContent.NPCType<LargePerforatorHead>());
                    Entries[i].Item7.Append(ModContent.NPCType<LargePerforatorBody>());
                    Entries[i].Item7.Append(ModContent.NPCType<LargePerforatorBody2>());
                    Entries[i].Item7.Append(ModContent.NPCType<LargePerforatorTail>());
                }
                if (Entries[i].Item1 == NPCID.BrainofCthulhu)
                {
                    Entries[i].Item7.Append(ModContent.NPCType<BrainIllusion>());
                    Entries[i].Item7.Append(ModContent.NPCType<BrainIllusion2>());
                    Entries[i].Item7.Append(ModContent.NPCType<BrainIllusionAttack>());
                }
                if (Entries[i].Item1 == NPCID.QueenBee)
                {
                    Entries[i].Item7.Append(ModContent.NPCType<RoyalSubject>());
                }
                if (Entries[i].Item1 == NPCID.Plantera)
                {
                    Entries[i].Item7.Append(ModContent.NPCType<CrystalLeaf>());
                }
                //if (Entries[i].Item1 == NPCID.HallowBoss)
                //{
                //    Entries.RemoveAt(i);
                //    Entries.Insert(i, (NPCID.HallowBoss, 0, delegate { NPC.SpawnOnPlayer(0, NPCID.HallowBoss); }, -1, false, 0f, Array.Empty<int>(), Array.Empty<int>()));
                //}
            }
            //set boss rush entries to new list of entries
            cal.Call("SetBossRushEntries", Entries);
            //make scal not end the event on defeat so it continues to mutant
            DeathEffectsList.Remove(ModContent.NPCType<SupremeCalamitas>());
            #endregion bossrush
            cal.Call("RegisterModCooldowns", FargowiltasCrossmod.Instance);
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
