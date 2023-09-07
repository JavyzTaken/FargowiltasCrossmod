
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.DraedonLabThings;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.ItemDropRules;
using FargowiltasCrossmod.Core.Systems;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Bosses.Champions.Life;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.Champions.Spirit;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.Champions.Timber;
using FargowiltasSouls.Content.Bosses.Champions.Will;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.Patreon.Duck;
using FargowiltasSouls.Content.Patreon.GreatestKraken;
using FargowiltasSouls.Core.ItemDropRules.Conditions;
using FargowiltasSouls.Core.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Balance
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalNPCChanges : GlobalNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.DebuffImmunitySets.Add(ModContent.NPCType<ShockstormShuttle>(), new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Suffocation
                }
            });
            NPCID.Sets.DebuffImmunitySets.Add(ModContent.NPCType<Sunskater>(), new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Suffocation
                }
            });
            NPCID.Sets.DebuffImmunitySets.Add(ModContent.NPCType<AeroSlime>(), new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Suffocation
                }
            });
            NPCID.Sets.DebuffImmunitySets.Add(ModContent.NPCType<RepairUnitCritter>(), new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Suffocation
                }
            });
        }
        public static List<int> Champions = new List<int>
        {
            ModContent.NPCType<CosmosChampion>(),
            ModContent.NPCType<EarthChampion>(),
            ModContent.NPCType<LifeChampion>(),
            ModContent.NPCType<NatureChampion>(),
            ModContent.NPCType<ShadowChampion>(),
            ModContent.NPCType<SpiritChampion>(),
            ModContent.NPCType<TerraChampion>(),
            ModContent.NPCType<TimberChampion>(),
            ModContent.NPCType<WillChampion>()
        };
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
        public static List<int> SolarEclipseEnemies = new List<int>
        {
            NPCID.Eyezor,
            NPCID.Frankenstein,
            NPCID.SwampThing,
            NPCID.Vampire,
            NPCID.CreatureFromTheDeep,
            NPCID.Fritz,
            NPCID.ThePossessed,
            NPCID.Reaper,
            NPCID.Mothron,
            NPCID.MothronEgg,
            NPCID.MothronSpawn,
            NPCID.Butcher,
            NPCID.DeadlySphere,
            NPCID.DrManFly,
            NPCID.Nailhead,
            NPCID.Psycho
        };
        public static List<int> PumpkinMoonEnemies = new List<int>
        {
            NPCID.Scarecrow1,NPCID.Scarecrow2,NPCID.Scarecrow3,NPCID.Scarecrow4,
            NPCID.Scarecrow5,NPCID.Scarecrow6,NPCID.Scarecrow7,NPCID.Scarecrow8,
            NPCID.Scarecrow9,NPCID.Scarecrow10,
            NPCID.Splinterling,
            NPCID.Hellhound,
            NPCID.Poltergeist,
            NPCID.HeadlessHorseman,
            NPCID.MourningWood,
            NPCID.Pumpking
        };
        public static List<int> FrostMoonEnemies = new List<int>
        {
            NPCID.PresentMimic,
            NPCID.Flocko,
            NPCID.GingerbreadMan,
            NPCID.ZombieElf,NPCID.ZombieElfBeard,NPCID.ZombieElfGirl,
            NPCID.ElfArcher,
            NPCID.Nutcracker,
            NPCID.Yeti,
            NPCID.ElfCopter,
            NPCID.Krampus,
            NPCID.Everscream,
            NPCID.SantaNK1,
            NPCID.IceQueen
        };
        public override void SetDefaults(NPC npc)
        {
            #region Balance
            if (ModContent.GetInstance<Core.Calamity.CalamityConfig>().BalanceRework)
            {
                //champions
                //if (Champions.Contains(npc.type))
                //{
                    //npc.lifeMax = (int)(npc.lifeMax * 0.8f);
                //}
                //Events/Minibosses
                if (AcidRainEnemies.Contains(npc.type) && DownedBossSystem.downedPolterghast)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.5f);
                }
                if ((npc.type == ModContent.NPCType<ReaperShark>() || npc.type == ModContent.NPCType<EidolonWyrmHead>()
                    || npc.type == ModContent.NPCType<ColossalSquid>() || npc.type == ModContent.NPCType<BobbitWormHead>()
                    || npc.type == ModContent.NPCType<GulperEelHead>() || npc.type == ModContent.NPCType<GulperEelBody>() || npc.type == ModContent.NPCType<GulperEelBodyAlt>() || npc.type == ModContent.NPCType<GulperEelTail>()
                        || npc.type == ModContent.NPCType<Bloatfish>()) && DownedBossSystem.downedPolterghast)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.5f);
                }
                if (SolarEclipseEnemies.Contains(npc.type) && DownedBossSystem.downedDoG) 
                {
                    npc.lifeMax = (int)(npc.lifeMax * 4.5f);
                }
                if ((FrostMoonEnemies.Contains(npc.type) || PumpkinMoonEnemies.Contains(npc.type)) && DownedBossSystem.downedDoG)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 3f);
                }
                //Plantera
                if (npc.type == NPCID.Plantera && FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 0.4f);
                }
                //golem
                if (npc.type == NPCID.Golem && FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 0.5f);
                }
                //duke fishr
                if (npc.type == NPCID.DukeFishron && FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 0.5f);
                }
                //eol
                if (npc.type == NPCID.HallowBoss && FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 0.45f);
                }
                //lunatic
                if (npc.type == NPCID.CultistBoss && FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 0.4f);
                }
                //moon lord
                if (npc.type == NPCID.MoonLordCore && FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 0.5f);
                }
                //Providence and guardian minions
                if (npc.type == ModContent.NPCType<Providence>() || npc.type == ModContent.NPCType<ProvSpawnDefense>() || 
                    npc.type == ModContent.NPCType<ProvSpawnHealer>() || npc.type == ModContent.NPCType<ProvSpawnOffense>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.7f);
                }
                //profaned guardians and rock thing
                if (npc.type == ModContent.NPCType<ProfanedGuardianHealer>() || npc.type == ModContent.NPCType<ProfanedGuardianDefender>() ||
                    npc.type == ModContent.NPCType<ProfanedGuardianCommander>() || npc.type == ModContent.NPCType<ProfanedRocks>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.6f);
                }
                //dragonfolly and minion
                if (npc.type == ModContent.NPCType<Bumblefuck>() || npc.type == ModContent.NPCType<Bumblefuck2>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.4f);
                }
                //signus
                if (npc.type == ModContent.NPCType<Signus>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.7f);
                }
                //ceaseless void & dark energy
                if (npc.type == ModContent.NPCType<CeaselessVoid>() || npc.type == ModContent.NPCType<DarkEnergy>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.4f);
                }
                //storm weaver
                //sw is weird yes i need to set all segments
                if (npc.type == ModContent.NPCType<StormWeaverHead>() || npc.type == ModContent.NPCType<StormWeaverTail>() || npc.type == ModContent.NPCType<StormWeaverBody>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.7f);
                }
                //polterghast and polterclone
                if (npc.type == ModContent.NPCType<Polterghast>() || npc.type == ModContent.NPCType<PolterPhantom>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.9f);
                }
                //overdose
                if (npc.type == ModContent.NPCType<OldDuke>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2f);
                }
                //dog
                if (npc.type == ModContent.NPCType<DevourerofGodsHead>() || npc.type == ModContent.NPCType<DevourerofGodsBody>() || npc.type == ModContent.NPCType<DevourerofGodsTail>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.8f);
                }
                //yhar
                if (npc.type == ModContent.NPCType<Yharon>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.0f);
                }
                //abom
                if (npc.type == ModContent.NPCType<AbomBoss>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.5f);
                }
                //exos
                if (npc.type == ModContent.NPCType<ThanatosBody1>() || npc.type == ModContent.NPCType<ThanatosBody2>() || npc.type == ModContent.NPCType<ThanatosHead>()
                    || npc.type == ModContent.NPCType<ThanatosTail>() || npc.type == ModContent.NPCType<AresBody>() || npc.type == ModContent.NPCType<AresGaussNuke>()
                    || npc.type == ModContent.NPCType<AresLaserCannon>() || npc.type == ModContent.NPCType<AresPlasmaFlamethrower>() || npc.type == ModContent.NPCType<AresTeslaCannon>()
                    || npc.type == ModContent.NPCType<Apollo>() || npc.type == ModContent.NPCType<Artemis>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.9f);
                }
                if (npc.type == ModContent.NPCType<SupremeCalamitas>() || npc.type == ModContent.NPCType<BrimstoneHeart>() ||
                    npc.type == ModContent.NPCType<SoulSeekerSupreme>() || npc.type == ModContent.NPCType<SupremeCataclysm>() || npc.type == ModContent.NPCType<SupremeCatastrophe>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.9f);
                }
                //mutant
                if (npc.type == ModContent.NPCType<MutantBoss>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.5f);
                }

                
            }
            #endregion

            
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            LeadingConditionRule postDoG = npcLoot.DefineConditionalDropSet(DropHelper.PostDoG(true));
            LeadingConditionRule emodeRule = new(new EModeDropCondition());
            LeadingConditionRule pMoon = new LeadingConditionRule(new Conditions.PumpkinMoonDropGatingChance());
            LeadingConditionRule fMoon = new LeadingConditionRule(new Conditions.FrostMoonDropGatingChance());
            LeadingConditionRule rev = npcLoot.DefineConditionalDropSet(DropHelper.RevNoMaster);
            LeadingConditionRule hardmode = npcLoot.DefineConditionalDropSet(DropHelper.Hardmode(true));
            
            #region MasterModeDropsInRev
            if (npc.type == NPCID.DD2DarkMageT3)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DarkMageMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DarkMageBookMountItem, 4));
            }
            if (npc.type == NPCID.DD2OgreT3)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.OgreMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DD2OgrePetItem, 4));
                
            }
            if (npc.type == NPCID.MourningWood)
            {
                Chains.OnSuccess(pMoon, ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.MourningWoodMasterTrophy));
                Chains.OnSuccess(pMoon, ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SpookyWoodMountItem, 4));
            }
            if (npc.type == NPCID.Pumpking)
            {
                Chains.OnSuccess(pMoon, ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.PumpkingMasterTrophy));
                Chains.OnSuccess(pMoon, ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.PumpkingPetItem, 4));
            }
            if (npc.type == NPCID.Everscream)
            {
                Chains.OnSuccess(fMoon, ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EverscreamMasterTrophy));
                Chains.OnSuccess(fMoon, ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EverscreamPetItem, 4));
            }
            if (npc.type == NPCID.SantaNK1)
            {
                Chains.OnSuccess(fMoon, ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SantankMasterTrophy));
                Chains.OnSuccess(fMoon, ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SantankMountItem, 4));
            }
            if (npc.type == NPCID.IceQueen)
            {
                Chains.OnSuccess(fMoon, ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.IceQueenMasterTrophy));
                Chains.OnSuccess(fMoon, ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.IceQueenPetItem, 4));
            }
            if (npc.type == NPCID.PirateShip)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.FlyingDutchmanMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.PirateShipMountItem, 4));
            }
            if (npc.type == NPCID.MartianSaucerCore)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.UFOMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.MartianPetItem, 4));
            }
            if (npc.type == NPCID.KingSlime)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.KingSlimeMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.KingSlimePetItem, 4));
            }
            if (npc.type == NPCID.EyeofCthulhu)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EyeofCthulhuMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EyeOfCthulhuPetItem, 4));
                
            }
            if (npc.type >= NPCID.EaterofWorldsHead && npc.type <= NPCID.EaterofWorldsTail)
            {
                LeadingConditionRule lastEater = new(new Conditions.LegacyHack_IsABoss());
                lastEater.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EaterofWorldsMasterTrophy));
                lastEater.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EaterOfWorldsPetItem, 4));
                
            }
            if (npc.type == NPCID.BrainofCthulhu)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.BrainofCthulhuMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.BrainOfCthulhuPetItem, 4));
            }
            if (npc.type == NPCID.Deerclops)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DeerclopsMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DeerclopsPetItem, 4));
            }
            if (npc.type == NPCID.QueenBee)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.QueenBeeMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.QueenBeePetItem, 4));
            }
            if (npc.type == NPCID.SkeletronHead)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SkeletronMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SkeletronPetItem, 4));
            }
            if (npc.type == NPCID.WallofFlesh)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.WallofFleshMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.WallOfFleshGoatMountItem, 4));
            }
            if (npc.type == NPCID.QueenSlimeBoss)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.QueenSlimeMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.QueenSlimePetItem, 4));
            }
            if (npc.type >= NPCID.TheDestroyer && npc.type <= NPCID.TheDestroyerTail)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DestroyerMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DestroyerPetItem, 4));
            }
            if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
            {
                LeadingConditionRule noTwin = new(new Conditions.MissingTwin());
                noTwin.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.TwinsMasterTrophy));
                noTwin.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.TwinsPetItem, 4));
            }
            if (npc.type == NPCID.SkeletronPrime)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SkeletronPrimeMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SkeletronPrimePetItem, 4));
            }
            if (npc.type == NPCID.Plantera)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.PlanteraMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.PlanteraPetItem, 4));
            }
            if (npc.type == NPCID.Golem)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.GolemMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.GolemPetItem, 4));
            }
            if (npc.type == NPCID.DD2Betsy)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.BetsyMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DD2BetsyPetItem, 4));
            }
            if (npc.type == NPCID.DukeFishron)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DukeFishronMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DukeFishronPetItem, 4));
            }
            if (npc.type == NPCID.HallowBoss)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.FairyQueenMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.FairyQueenPetItem, 4));
            }
            if (npc.type == NPCID.CultistBoss)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.LunaticCultistMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.LunaticCultistPetItem, 4));
            }
            if (npc.type == NPCID.MoonLordCore)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.MoonLordMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.RevNotEmodeCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.MoonLordPetItem, 4));
            }
            #endregion MasterModeDropsInRev
            LeadingConditionRule PreHMNotBalanced = new LeadingConditionRule(CalamityConditions.PreHardmodeAndNotBalance.ToDropCondition(ShowItemDropInUI.Always));
            if (npc.type == NPCID.WyvernHead)
            {
                hardmode.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<EssenceofSunlight>(), 1, 8, 10, 10, 12));
                PreHMNotBalanced.OnSuccess(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<EssenceofSunlight>(), 1, 8, 10, 10, 12));
                npcLoot.Add(PreHMNotBalanced);
            }
            if (npc.type == NPCID.AngryNimbus)
            {
                npcLoot.Add(ItemDropRule.ByCondition(Condition.Hardmode.ToDropCondition(ShowItemDropInUI.Always), ModContent.ItemType<EssenceofSunlight>(), 2));
                PreHMNotBalanced.OnSuccess(ItemDropRule.Common(ModContent.ItemType<EssenceofSunlight>(), 2, 1, 1));
                npcLoot.Add(PreHMNotBalanced);
            }
            if (npc.type == NPCID.SeekerHead)
            {
                npcLoot.RemoveWhere(delegate (IItemDropRule rule)
                {
                    CommonDrop drop = rule as CommonDrop;
                    return drop != null && drop.itemId == ItemID.CursedFlame && Condition.Hardmode.IsMet();
                }, true);
                npcLoot.DefineConditionalDropSet(DropHelper.If(() => !CalamityWorld.death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), () => !CalamityWorld.death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), "")).Add(ItemID.CursedFlame, 1, 2, 5, false);
                npcLoot.DefineConditionalDropSet(DropHelper.If(() => CalamityWorld.death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), () => CalamityWorld.death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), "")).Add(ItemID.CursedFlame, 1, 6, 15, false);
                npcLoot.DefineConditionalDropSet(DropHelper.If(() => CalamityWorld.death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), () => CalamityWorld.death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), CalamityUtils.GetTextValue("Condition.Drops.IsDeath"))).Add(ItemID.SoulofNight, 1, 4, 8, false);
            }
            if (npc.type == NPCID.SandElemental)
            {
                hardmode.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottle>(), 5, 3));
                hardmode.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottlewithBoobs>(), 10, 6));
                PreHMNotBalanced.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottle>(), 5, 3));
                PreHMNotBalanced.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottlewithBoobs>(), 10, 6));
                npcLoot.Add(PreHMNotBalanced);
            }
        }
        public override bool PreKill(NPC npc)
        {
            if ((npc.type == NPCID.SolarCorite || npc.type == NPCID.SolarCrawltipedeHead || npc.type == NPCID.SolarCrawltipedeTail
                || npc.type == NPCID.StardustJellyfishBig || npc.type == NPCID.NebulaBrain || npc.type == NPCID.VortexHornetQueen) && !NPC.downedAncientCultist)
            {
                return false;
            }
            if (SolarEclipseEnemies.Contains(npc.type) && DownedBossSystem.downedDoG && !Main.eclipse) {
                return false;
            }
            if (FrostMoonEnemies.Contains(npc.type) && DownedBossSystem.downedDoG && !Main.snowMoon)
            {
                return false;
            }
            if (PumpkinMoonEnemies.Contains(npc.type) && DownedBossSystem.downedDoG && !Main.pumpkinMoon)
            {
                return false;
            }
            return base.PreKill(npc);
        }
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneSulphur && AcidRainEvent.AcidRainEventIsOngoing)
            {
                pool[NPCID.PigronCorruption] = 0f;
                pool[NPCID.PigronCrimson] = 0f;
                pool[NPCID.PigronHallow] = 0f;
            }
        }
        public override bool PreAI(NPC npc)
        {
            //Main.NewText(FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode);
            #region Balance Changes config
            if (ModContent.GetInstance<Core.Calamity.CalamityConfig>().BalanceRework)
            {
                //add defense damage to fargo enemies. setting this in SetDefaults crashes the game for some reason
                if (npc.ModNPC != null)
                {
                    if (npc.ModNPC.Mod == ModLoader.GetMod(ModCompatibility.SoulsMod.Name) && npc.IsAnEnemy())
                    {
                        ModLoader.GetMod(ModCompatibility.Calamity.Name).Call("SetDefenseDamageNPC", npc, true);
                    }
                }
            }
            #endregion
            if (!Core.Systems.WorldSavingSystem.E_EternityRev)
            {
                return base.PreAI(npc);
            }
            if (npc.type == NPCID.MoonLordCore)
            {
                Main.player[Main.myPlayer].Calamity().infiniteFlight = true;
            }
            //make golem not fly
            if (npc.type == NPCID.Golem)
            {
                npc.noGravity = false;
            }
            if (npc.type == NPCID.GolemHeadFree)
            {
                npc.dontTakeDamage = true;
            }
            //make destroyer not invincible
            if (npc.type >= NPCID.TheDestroyer && npc.type <= NPCID.TheDestroyerTail)
            {
                Mod calamity = ModLoader.GetMod(ModCompatibility.Calamity.Name);
                calamity.Call("SetCalamityAI", npc, 1, 600f);
                calamity.Call("SetCalamityAI", npc, 2, 0f);
            }
            //make plantera not summon free tentacles
            if (npc.type == ModContent.NPCType<PlanterasFreeTentacle>())
            {
                npc.StrikeInstantKill();
            }
            return base.PreAI(npc);
        }
    }
}
