
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.Yharon;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
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
                    npc.lifeMax = (int)(npc.lifeMax * 1.8f);
                }
                //ceaseless void & dark energy
                if (npc.type == ModContent.NPCType<CeaselessVoid>() || npc.type == ModContent.NPCType<DarkEnergy>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.5f);
                }
                //storm weaver
                //sw is weird yes i need to set all segments
                if (npc.type == ModContent.NPCType<StormWeaverHead>() || npc.type == ModContent.NPCType<StormWeaverTail>() || npc.type == ModContent.NPCType<StormWeaverBody>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.8f);
                }
                //polterghast and polterclone
                if (npc.type == ModContent.NPCType<Polterghast>() || npc.type == ModContent.NPCType<PolterPhantom>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.2f);
                }
                //overdose
                if (npc.type == ModContent.NPCType<OldDuke>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.25f);
                }
                //dog
                if (npc.type == ModContent.NPCType<DevourerofGodsHead>() || npc.type == ModContent.NPCType<DevourerofGodsBody>() || npc.type == ModContent.NPCType<DevourerofGodsTail>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.7f);
                }
                //yhar
                if (npc.type == ModContent.NPCType<Yharon>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.1f);
                }
                //abom
                if (npc.type == ModContent.NPCType<AbomBoss>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.6f);
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


            #region DoubleRelicFix
            //Commented out for now because this shit gets complex for a bug that doesnt affect gameplay like at all
            //Condition emodeRule = new Condition("EmodeNotRevenge", ()=> (!CalamityWorld.revenge && FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode) || (Main.masterMode && !FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode));
            //switch (npc.type)
            //{
            //    case NPCID.KingSlime:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.KingSlimeMasterTrophy);
            //        npcLoot.Add(ItemDropRule.ByCondition(emodeRule.ToDropCondition(ShowItemDropInUI.Never), ItemID.KingSlimeMasterTrophy));
            //        break;
            //    case NPCID.EyeofCthulhu:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.EyeofCthulhuMasterTrophy);
            //        npcLoot.Add(ItemDropRule.ByCondition(emodeRule.ToDropCondition(ShowItemDropInUI.Never), ItemID.EyeofCthulhuMasterTrophy));
            //        break;
            //    case NPCID.EaterofWorldsHead:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.EaterofWorldsMasterTrophy);
            //        npcLoot.Add(ItemDropRule.ByCondition(emodeRule.ToDropCondition(ShowItemDropInUI.Never), ItemID.EaterofWorldsMasterTrophy));
            //        break;
            //    case NPCID.BrainofCthulhu:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.BrainofCthulhuMasterTrophy);
            //        npcLoot.Add(ItemDropRule.ByCondition(emodeRule.ToDropCondition(ShowItemDropInUI.Never), ItemID.BrainofCthulhuMasterTrophy));
            //        break;
            //    case NPCID.QueenBee:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.QueenBeeMasterTrophy);
            //        break;
            //    case NPCID.SkeletronHead:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.SkeletronMasterTrophy);
            //        break;
            //    case NPCID.Deerclops:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.DeerclopsMasterTrophy);
            //        break;
            //    case NPCID.WallofFlesh:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.WallofFleshMasterTrophy);
            //        break;
            //    case NPCID.QueenSlimeBoss:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.QueenSlimeMasterTrophy);
            //        break;
            //    case NPCID.Spazmatism:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.TwinsMasterTrophy);
            //        break;
            //    case NPCID.Retinazer:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.TwinsMasterTrophy);
            //        break;
            //    case NPCID.TheDestroyer:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.DestroyerMasterTrophy);
            //        break;
            //    case NPCID.SkeletronPrime:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.SkeletronPrimeMasterTrophy);
            //        break;
            //    case NPCID.Plantera:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.PlanteraMasterTrophy);
            //        break;
            //    case NPCID.Golem:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.GolemMasterTrophy);
            //        break;
            //    case NPCID.DukeFishron:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.DukeFishronMasterTrophy);
            //        break;
            //    case NPCID.HallowBoss:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.FairyQueenMasterTrophy);
            //        break;
            //    case NPCID.CultistBoss:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.LunaticCultistMasterTrophy);
            //        break;
            //    case NPCID.MoonLordCore:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.MoonLordMasterTrophy);
            //        break;
            //    case NPCID.DD2DarkMageT3:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.DarkMageMasterTrophy);
            //        break;
            //    case NPCID.DD2OgreT3:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.OgreMasterTrophy);
            //        break;
            //    case NPCID.DD2Betsy:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.BetsyMasterTrophy);
            //        break;
            //    case NPCID.PirateShip:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.FlyingDutchmanMasterTrophy);
            //        break;
            //    case NPCID.MartianSaucerCore:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.UFOMasterTrophy);
            //        break;
            //    case NPCID.MourningWood:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.MourningWoodMasterTrophy);
            //        break;
            //    case NPCID.Pumpking:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.PumpkingMasterTrophy);
            //        break;
            //    case NPCID.Everscream:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.EverscreamMasterTrophy);
            //        break;
            //    case NPCID.SantaNK1:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.SantankMasterTrophy);
            //        break;
            //    case NPCID.IceQueen:
            //        npcLoot.RemoveWhere(rule => rule is CommonDrop j && j.itemId == ItemID.IceQueenMasterTrophy);
            //        break;
            //}
            #endregion DoubleRelicFix
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
            if (!WorldSavingSystem.E_EternityRev)
            {
                return base.PreAI(npc);
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
