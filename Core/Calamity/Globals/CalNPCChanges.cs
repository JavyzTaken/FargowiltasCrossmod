﻿using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.DraedonLabThings;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Fargowiltas.NPCs;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasCrossmod.Content.Calamity.Items.Summons;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.ItemDropRules;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
using FargowiltasCrossmod.Core.Common.Systems;
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
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Ammos;
using FargowiltasSouls.Content.Items.Armor;
using FargowiltasSouls.Content.Items.Weapons.FinalUpgrades;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ItemDropRules.Conditions;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using DLCCalamityConfig = FargowiltasCrossmod.Core.Calamity.DLCCalamityConfig;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalNPCChanges : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => ModCompatibility.Calamity.Loaded;

        private static List<int> SuffocationImmune = new List<int>
        {
            ModContent.NPCType<ShockstormShuttle>(),
            ModContent.NPCType<Sunskater>(),
            ModContent.NPCType<AeroSlime>(),
            ModContent.NPCType<RepairUnitCritter>(),


        };
        private static List<int> ClippedWingsImmune = new List<int>
        {
            ModContent.NPCType<BrimstoneHeart>(),
            ModContent.NPCType<SupremeCataclysm>(),
            ModContent.NPCType<SupremeCatastrophe>(),

        };
        public override void SetStaticDefaults()
        {

            foreach (int type in SuffocationImmune)
            {
                NPCID.Sets.SpecificDebuffImmunity[type][BuffID.Suffocation] = true;
            }
            foreach (int type in ClippedWingsImmune)
            {
                NPCID.Sets.SpecificDebuffImmunity[type][ModContent.BuffType<ClippedWingsBuff>()] = true;
            }
            NPCID.Sets.SpecificDebuffImmunity[ModContent.NPCType<MutantBoss>()][ModContent.BuffType<Enraged>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[ModContent.NPCType<MutantBoss>()][ModContent.BuffType<BanishingFire>()] = true;
        }
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void SetDefaults(NPC npc)
        {
            #region Balance

            if (DLCCalamityConfig.Instance.BalanceRework)
            {

                //Events/Minibosses

                if (CalamityContentLists.AcidRainEnemies.Contains(npc.type) && DownedBossSystem.downedPolterghast)
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
                if (ContentLists.SolarEclipseEnemies.Contains(npc.type) && DownedBossSystem.downedDoG)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 4.5f);
                }
                if ((ContentLists.FrostMoonEnemies.Contains(npc.type) || ContentLists.PumpkinMoonEnemies.Contains(npc.type)) && DownedBossSystem.downedDoG)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 3f);
                }
                #region Vanilla Bosses
                if (WorldSavingSystem.EternityMode)
                {
                    switch (npc.type)
                    {
                        case NPCID.SkeletronHead:
                            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.1f);
                            break;
                        case NPCID.SkeletronHand:
                            if (DLCWorldSavingSystem.E_EternityRev)
                                npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.5f);
                            break;
                        case NPCID.QueenBee:
                            npc.lifeMax = (int)(npc.lifeMax * 1.5f);
                            break;
                        case NPCID.WallofFlesh:
                        case NPCID.WallofFleshEye:
                            npc.lifeMax = (int)(npc.lifeMax * 0.6f);
                            break;
                        case NPCID.Plantera:
                            npc.lifeMax = (int)(npc.lifeMax * 0.4f);
                            break;
                        case NPCID.Golem:
                            npc.lifeMax = (int)(npc.lifeMax * 0.5f);
                            break;
                        case NPCID.DukeFishron:
                            npc.lifeMax = (int)(npc.lifeMax * 0.5f);
                            break;
                        case NPCID.HallowBoss:
                            npc.lifeMax = (int)(npc.lifeMax * 0.45f);
                            break;
                        case NPCID.CultistBoss:
                            npc.lifeMax = (int)(npc.lifeMax * 0.6f);
                            break;
                        case NPCID.MoonLordCore:
                            npc.lifeMax = (int)(npc.lifeMax * 0.5f);
                            break;
                    }
                }
                #endregion
                #region Modded Bosses
                //champions
                if (ContentLists.Champions.Contains(npc.type))
                {
                    npc.lifeMax = (int)(npc.lifeMax * 0.9f);
                }
                //Providence and guardian minions
                if (npc.type == ModContent.NPCType<Providence>() || npc.type == ModContent.NPCType<ProvSpawnDefense>() ||
                    npc.type == ModContent.NPCType<ProvSpawnHealer>() || npc.type == ModContent.NPCType<ProvSpawnOffense>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.6f);
                }
                //profaned guardians and rock thing
                if (npc.type == ModContent.NPCType<ProfanedGuardianHealer>() || npc.type == ModContent.NPCType<ProfanedGuardianDefender>() ||
                    npc.type == ModContent.NPCType<ProfanedGuardianCommander>() || npc.type == ModContent.NPCType<ProfanedRocks>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.3f);
                }
                //dragonfolly and minion
                if (npc.type == ModContent.NPCType<Bumblefuck>() || npc.type == ModContent.NPCType<Bumblefuck2>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.3f);
                }
                //signus
                if (npc.type == ModContent.NPCType<Signus>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.2f);
                }
                //ceaseless void & dark energy
                if (npc.type == ModContent.NPCType<CeaselessVoid>() || npc.type == ModContent.NPCType<DarkEnergy>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.1f);
                }
                //storm weaver
                //sw is weird yes i need to set all segments
                if (npc.type == ModContent.NPCType<StormWeaverHead>() || npc.type == ModContent.NPCType<StormWeaverTail>() || npc.type == ModContent.NPCType<StormWeaverBody>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.4f);
                }
                //polterghast and polterclone
                if (npc.type == ModContent.NPCType<Polterghast>() || npc.type == ModContent.NPCType<PolterPhantom>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.2f);
                }
                //overdose
                if (npc.type == ModContent.NPCType<OldDuke>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.2f);
                }
                //dog
                if (npc.type == ModContent.NPCType<DevourerofGodsHead>() || npc.type == ModContent.NPCType<DevourerofGodsBody>() || npc.type == ModContent.NPCType<DevourerofGodsTail>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.2f);
                }
                //yhar
                if (npc.type == ModContent.NPCType<Yharon>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2f);
                }
                //abom
                if (npc.type == ModContent.NPCType<AbomBoss>())
                {
                    npc.lifeMax = (int)(npc.lifeMax * 3.5f);
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
                    npc.lifeMax = (int)(npc.lifeMax * 4.4f);
                }
                if (ModCompatibility.WrathoftheGods.Loaded)
                {
                    if (npc.type == ModCompatibility.WrathoftheGods.NoxusBoss1.Type ||
                        npc.type == ModCompatibility.WrathoftheGods.NoxusBoss2.Type ||
                        npc.type == ModCompatibility.WrathoftheGods.NamelessDeityBoss.Type)
                    {
                        npc.lifeMax = (int)(npc.lifeMax * 1.6f);
                    }
                }
                #endregion
                #region BRBalance
                List<int> squirrelParts = new List<int>
                {
                    ModContent.NPCType<TrojanSquirrelArms>(),
                    ModContent.NPCType<TrojanSquirrel>(),
                    ModContent.NPCType<TrojanSquirrelHead>(),
                    ModContent.NPCType<TrojanSquirrelLimb>(),
                    ModContent.NPCType<TrojanSquirrelPart>(),
                };
                List<int> KingSlime = new List<int>
                {
                    NPCID.KingSlime,
                    NPCID.BlueSlime,
                    NPCID.SlimeSpiked,
                    ModContent.NPCType<KingSlimeJewel>()
                };
                List<int> Eater = new List<int> { NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail };
                List<int> SlimeGod = new List<int> { ModContent.NPCType<EbonianPaladin>(), ModContent.NPCType<SplitEbonianPaladin>(),
                    ModContent.NPCType<CrimulanPaladin>(), ModContent.NPCType<SplitCrimulanPaladin>(),
                    };
                List<int> bossworms = new List<int>
                {

                    ModContent.NPCType<DesertScourgeHead>(), ModContent.NPCType<DesertScourgeBody>(), ModContent.NPCType<DesertScourgeTail>(),
                    NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail,

                    ModContent.NPCType<AquaticScourgeHead>(), ModContent.NPCType<AquaticScourgeBody>(),ModContent.NPCType<AquaticScourgeBodyAlt>(), ModContent.NPCType<AquaticScourgeTail>(),
                    NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail,
                    ModContent.NPCType<AstrumDeusHead>(), ModContent.NPCType<AstrumDeusBody>(), ModContent.NPCType<AstrumDeusTail>(),
                    ModContent.NPCType<StormWeaverHead>(), ModContent.NPCType<StormWeaverBody>(), ModContent.NPCType<StormWeaverTail>(),

                };
                List<int> minionworms = new List<int>()
                {
                    ModContent.NPCType<DesertNuisanceHead>(), ModContent.NPCType<DesertNuisanceBody>(), ModContent.NPCType<DesertNuisanceTail>(),
                    ModContent.NPCType<PerforatorHeadSmall>(),ModContent.NPCType<PerforatorBodySmall>(), ModContent.NPCType<PerforatorTailSmall>(),
                    ModContent.NPCType<PerforatorHeadMedium>(),ModContent.NPCType<PerforatorBodyMedium>(), ModContent.NPCType<PerforatorTailMedium>(),
                    ModContent.NPCType<PerforatorHeadLarge>(),ModContent.NPCType<PerforatorBodyLarge>(), ModContent.NPCType<PerforatorTailLarge>(),

                };
                if (BossRushEvent.BossRushActive)
                {
                    if (!npc.boss && npc.type != ModContent.NPCType<CreeperGutted>())
                    {
                        npc.lifeMax += 50000;

                    }
                    else if (npc.type != ModContent.NPCType<CreeperGutted>())
                    {
                        npc.lifeMax += 1700000;
                    }
                    if (npc.type == ModContent.NPCType<DeviBoss>())
                    {
                        npc.lifeMax += 500000;
                    }
                    if (SlimeGod.Contains(npc.type))
                    {
                        npc.lifeMax += 400000;
                    }
                    if (bossworms.Contains(npc.type) && !Eater.Contains(npc.type))
                    {
                        npc.lifeMax += 7500000;
                    }
                    else if (minionworms.Contains(npc.type) && !Eater.Contains(npc.type))
                    {
                        npc.lifeMax += 1500000;
                    }
                    if (Eater.Contains(npc.type))
                    {
                        //npc.lifeMax += 100000;
                    }
                    if (npc.damage < 200 && npc.damage != 0)
                    {
                        npc.damage = 200;
                    }
                    if (npc.type == NPCID.Golem || npc.type == NPCID.GolemFistLeft || npc.type == NPCID.GolemFistRight || npc.type == NPCID.GolemHead)
                    {
                        npc.lifeMax /= 10;
                    }
                    //reduce health of bosses that are either too tanky or impossible to dodge
                    //increase hp of bosses that die fast
                    //destroyer: tanky and incredibly difficult to dodge
                    if (npc.type == NPCID.BrainofCthulhu) npc.lifeMax /= 3;
                    if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail) npc.lifeMax /= 3;
                    //golem: flies into space and deals tons of damage and is impossible to dodge
                    //if (npc.type == NPCID.Golem) npc.lifeMax /= 10;
                    //impossible to dodge in final phase
                    if (npc.type == NPCID.DukeFishron) npc.lifeMax /= 2;
                    //dies fast because is really big
                    if (npc.type == ModContent.NPCType<Providence>()) npc.lifeMax *= 3;
                    //tanky by design of original boss
                    if (npc.type == ModContent.NPCType<LifeChampion>()) npc.lifeMax /= 10;
                    //darke energies a little tanky
                    if (npc.type == ModContent.NPCType<DarkEnergy>()) npc.lifeMax /= 3;
                    //dies a little fast
                    if (npc.type == ModContent.NPCType<DevourerofGodsHead>() || npc.type == ModContent.NPCType<DevourerofGodsBody>() || npc.type == ModContent.NPCType<DevourerofGodsTail>()) npc.lifeMax *= 2;
                    //dies too fast
                    if (npc.type == ModContent.NPCType<Yharon>()) npc.lifeMax *= 3;
                    //too tanky eyes
                    if (npc.type == NPCID.MoonLordHand || npc.type == NPCID.MoonLordHead) npc.lifeMax /= 8;
                    if (npc.type == NPCID.MoonLordCore) npc.lifeMax /= 4;
                    if (npc.type == ModContent.NPCType<MutantBoss>()) npc.lifeMax = (int)(npc.lifeMax * 0.5f);
                    if (npc.type == ModContent.NPCType<DesertScourgeHead>() || npc.type == ModContent.NPCType<DesertScourgeBody>() || npc.type == ModContent.NPCType<DesertScourgeTail>())
                    {
                        npc.lifeMax /= 4;
                    }
                }
                #endregion BRBalance

            }
            #endregion
            #region Compatibility
            if (DLCCalamityConfig.Instance.EternityPriorityOverRev)
            {
                if (npc.type >= NPCID.TheDestroyer && npc.type <= NPCID.TheDestroyerTail || npc.type == NPCID.Probe)
                {
                    if (WorldSavingSystem.EternityMode)
                        npc.scale = 1f;
                    if (DLCWorldSavingSystem.EternityDeath)
                        npc.scale = 1.4f;
                }
            }
            //setdefaultsbeforelookupsarebuilt error
            if (!Main.gameMenu && CalamityConfig.Instance != null && npc.boss && npc.ModNPC != null && npc.ModNPC.Mod != null && (npc.ModNPC.Mod == FargowiltasCrossmod.Instance || npc.ModNPC.Mod == ModCompatibility.SoulsMod.Mod))
            {
                // Boost health according to Calamity boss health boost config
                float HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01f;
                npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            }
            #endregion

        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.HasEffect<OrichalcumEffect>() && npc.lifeRegen < 0)
            {
                float modifier = 0.6f;
                if (npc.Calamity().shellfishVore > 0) //nerf with shellfish thing
                {
                    modifier = 0.5f;
                }
                if (player.FargoSouls().ForceEffect<OrichalcumEnchant>())
                {
                    modifier -= 0.1f;
                }

                npc.lifeRegen = (int)(npc.lifeRegen * modifier);
                damage = (int)(damage * modifier);


            }
            base.UpdateLifeRegen(npc, ref damage);
        }
        //all this bullshit just so tmod doesnt JITException a method that is supposed to be ignored >:(
        public IItemDropRuleCondition PostDog => DropHelper.PostDoG();

        public IItemDropRuleCondition Revenge => DropHelper.RevNoMaster;

        public bool death => CalamityWorld.death;

        private DropBasedOnExpertMode NormalVsExpertQuantity(int itemID, int droprate, int minNormal, int maxNormal, int minExpert, int maxExpert)
        {
            return DropHelper.NormalVsExpertQuantity(itemID, droprate, minNormal, maxNormal, minExpert, maxExpert);
        }
        public IItemDropRuleCondition If(Func<bool> lambda, Func<bool> ui, string dec = null)
        {
            return DropHelper.If(lambda, ui, dec);
        }

        public static List<int> DropsBoundingPotion = new()
        {
            ModContent.NPCType<AeroSlime>(),
            ModContent.NPCType<EbonianBlightSlime>(),
            ModContent.NPCType<CrimulanBlightSlime>(),
            NPCID.SpikedJungleSlime
        };
        public static List<int> DropsCalciumPotion = new()
        {
            NPCID.Skeleton,
            NPCID.ArmoredSkeleton,
            NPCID.SkeletonTopHat,
            NPCID.SkeletonAstonaut,
            NPCID.SkeletonAlien,
            NPCID.BigSkeleton,
            NPCID.SmallSkeleton,
        };
        public static List<int> DropsPhotosynthesisPotion = new()
        {
            NPCID.AngryNimbus,
            ModContent.NPCType<ThiccWaifu>(), //fuck you fabsol
            NPCID.WyvernHead
        };
        public static List<int> DropsShadowPotion = new()
        {
            ModContent.NPCType<Scryllar>(),
            ModContent.NPCType<SoulSlurper>(),
            ModContent.NPCType<HeatSpirit>(),
            ModContent.NPCType<DespairStone>(),
            ModContent.NPCType<CalamityEye>(),
            ModContent.NPCType<RenegadeWarlock>()
        };
        public static List<int> DropsSoaringPotion = new()
        {
            ModContent.NPCType<EutrophicRay>(),
            ModContent.NPCType<GhostBell>(),
            ModContent.NPCType<SeaFloaty>(),
        };
        public static List<int> DropsSulphurskinPotion = new()
        {
            ModContent.NPCType<AquaticUrchin>(),
            ModContent.NPCType<Sulflounder>(),
            ModContent.NPCType<Gnasher>(),
            ModContent.NPCType<Toxicatfish>(),
            ModContent.NPCType<Trasher>(),
        };
        public static List<int> DropsTeslaPotion = new()
        {
            NPCID.GreenJellyfish,
            ModContent.NPCType<BlindedAngler>(),
            ModContent.NPCType<ShockstormShuttle>(),
        };
        public static List<int> DropsZenPotion = new()
        {
            ModContent.NPCType<Atlas>(),
            ModContent.NPCType<AstralachneaGround>(),
            ModContent.NPCType<AstralachneaWall>(),
            ModContent.NPCType<SightseerCollider>(),
            ModContent.NPCType<StellarCulex>(),
            ModContent.NPCType<AstralSlime>()
        };
        public static List<int> DropsZergPotion = new()
        {
            ModContent.NPCType<Hadarian>(),
            ModContent.NPCType<SightseerSpitter>(),
            ModContent.NPCType<SightseerCollider>(),
            ModContent.NPCType<StellarCulex>(),
            ModContent.NPCType<FusionFeeder>(),
            ModContent.NPCType<MantisShrimp>()
        };
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!ModCompatibility.Calamity.Loaded)
            {
                return;
            }

            #region Remove Drops
            int allowedRecursionDepth = 10;
            foreach (IItemDropRule rule in npcLoot.Get())
            {
                RecursiveDropRemove(rule);
            }
            [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
            void RecursiveDropRemove(IItemDropRule dropRule)
            {
                if (--allowedRecursionDepth > 0)
                {
                    if (dropRule != null && dropRule.ChainedRules != null)
                    {
                        foreach (IItemDropRuleChainAttempt chain in dropRule.ChainedRules)
                        {
                            if (chain != null && chain.RuleToChain != null)
                                RecursiveDropRemove(chain.RuleToChain);
                        }
                    }

                    if (dropRule is DropBasedOnMasterMode dropBasedOnMasterMode)
                    {
                        if (dropBasedOnMasterMode != null && dropBasedOnMasterMode.ruleForMasterMode != null)
                            RecursiveDropRemove(dropBasedOnMasterMode.ruleForMasterMode);
                    }
                }
                allowedRecursionDepth++;

                if (dropRule is DropBasedOnExpertMode expertDrop && expertDrop.ruleForNormalMode is CommonDrop commonDrop)
                {
                    if (npc.type == NPCID.IchorSticker && commonDrop.itemId == ModContent.ItemType<IchorSpear>())
                    {
                        npcLoot.Remove(dropRule);
                    }
                    if ((npc.type == NPCID.WyvernHead || npc.type == NPCID.AngryNimbus) && commonDrop.itemId == ModContent.ItemType<EssenceofSunlight>())
                    {
                        npcLoot.Remove(dropRule);
                    }
                    if (npc.type == NPCID.SandElemental && (commonDrop.itemId == ModContent.ItemType<WifeinaBottle>() || commonDrop.itemId == ModContent.ItemType<WifeinaBottlewithBoobs>())) // ew ew e w ew
                    {
                        npcLoot.Remove(dropRule);
                    }
                }

                if (npc.type == NPCID.SeekerHead || npc.type == NPCID.SeekerBody || npc.type == NPCID.SeekerTail)
                {
                    if (dropRule is CommonDrop commonDrop2 && commonDrop2.itemId == ItemID.CursedFlame)
                    {
                        npcLoot.Remove(dropRule);
                    }
                }

                if (dropRule is ItemDropWithConditionRule itemDropWithCondition && itemDropWithCondition.condition is EModeNotMasterDropCondition && npc.ModNPC == null)
                {
                    npcLoot.Remove(dropRule);
                }
                else if (dropRule is DropPerPlayerOnThePlayer dropPerPlayer && dropPerPlayer.condition is EModeNotMasterDropCondition && npc.ModNPC == null)
                {
                    npcLoot.Remove(dropRule);
                }
                //}
            }
            #endregion Remove Drops

            //LeadingConditionRule postDoG = npcLoot.DefineConditionalDropSet(PostDog);
            LeadingConditionRule emodeRule = new(new EModeDropCondition());
            LeadingConditionRule pMoon = new LeadingConditionRule(new Conditions.PumpkinMoonDropGatingChance());
            LeadingConditionRule fMoon = new LeadingConditionRule(new Conditions.FrostMoonDropGatingChance());
            //LeadingConditionRule rev = npcLoot.DefineConditionalDropSet(Revenge);
            LeadingConditionRule hardmode = new LeadingConditionRule(Condition.Hardmode.ToDropCondition(ShowItemDropInUI.Always));

            #region Crates
            if (npc.type == ModContent.NPCType<DesertScourgeHead>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SunkenCrate>(), 1, 3, 3));
            }
            if (npc.type == ModContent.NPCType<Crabulon>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ItemID.IronCrate, 1, 3, 3));
            }
            if (npc.type == ModContent.NPCType<HiveMind>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ItemID.CorruptFishingCrate, 1, 5, 5));
            }
            if (npc.type == ModContent.NPCType<PerforatorHive>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ItemID.CrimsonFishingCrate, 1, 5, 5));
            }
            if (npc.type == ModContent.NPCType<SlimeGodCore>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SulphurousCrate>(), 1, 5, 5));
            }
            if (npc.type == ModContent.NPCType<Cryogen>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ItemID.FrozenCrateHard, 1, 5, 5));
            }
            if (npc.type == ModContent.NPCType<AquaticScourgeHead>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SulphurousCrate>(), 1, 5, 5));
            }
            if (npc.type == ModContent.NPCType<BrimstoneElemental>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<BrimstoneCrate>(), 1, 5, 5));
            }
            if (npc.type == ModContent.NPCType<CalamitasClone>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<BrimstoneCrate>(), 1, 5, 5));
            }


            if (npc.type == ModContent.NPCType<Leviathan>() || npc.type == ModContent.NPCType<Anahita>())
            {
                Func<bool> what = new Func<bool>(Leviathan.LastAnLStanding);
                LeadingConditionRule levidroprule = npcLoot.DefineConditionalDropSet(what);
                levidroprule.OnSuccess(ItemDropRule.ByCondition(emodeRule.condition, ItemID.OceanCrateHard, 1, 5, 5, 1));
                npcLoot.Add(levidroprule);
            }

            if (npc.type == ModContent.NPCType<AstrumAureus>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AstralCrate>(), 1, 5, 5));
            }
            if (npc.type == ModContent.NPCType<PlaguebringerGoliath>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ItemID.JungleFishingCrateHard, 1, 5, 5));
            }
            if (npc.type == ModContent.NPCType<RavagerBody>())
            {
                emodeRule.OnSuccess(ItemDropRule.Common(ItemID.GoldenCrateHard, 1, 5, 5));
            }
            bool AstrumDeusHeadShouldNotDropThings(NPC npc) //this being needed is crazy (jit exception when cal not loaded otherwise)
            {
                if (npc.Calamity().newAI[0] != 0f)
                {
                    if (CalamityWorld.death || BossRushEvent.BossRushActive)
                    {
                        return npc.Calamity().newAI[0] != 3f;
                    }

                    return false;
                }
                return true;
            }

            if (npc.type == ModContent.NPCType<AstrumDeusHead>())
            {
                LeadingConditionRule lastWorm = npcLoot.DefineConditionalDropSet((info) => !AstrumDeusHeadShouldNotDropThings(info.npc));
                lastWorm.OnSuccess(ItemDropRule.ByCondition(emodeRule.condition, ModContent.ItemType<AstralCrate>(), 1, 5, 5, 1));
                npcLoot.Add(lastWorm);
            }

            #endregion Crates

            #region MasterModeDropsInRev

            if (npc.type == NPCID.DD2DarkMageT3)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DarkMageMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DarkMageBookMountItem, 4));
            }
            if (npc.type == NPCID.DD2OgreT3)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.OgreMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DD2OgrePetItem, 4));

            }
            if (npc.type == NPCID.MourningWood)
            {
                pMoon.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.MourningWoodMasterTrophy));
                pMoon.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SpookyWoodMountItem, 4));
            }
            if (npc.type == NPCID.Pumpking)
            {
                pMoon.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.PumpkingMasterTrophy));
                pMoon.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.PumpkingPetItem, 4));
            }
            if (npc.type == NPCID.Everscream)
            {
                fMoon.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EverscreamMasterTrophy));
                fMoon.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EverscreamPetItem, 4));
            }
            if (npc.type == NPCID.SantaNK1)
            {
                fMoon.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SantankMasterTrophy));
                fMoon.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SantankMountItem, 4));
            }
            if (npc.type == NPCID.IceQueen)
            {
                fMoon.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.IceQueenMasterTrophy));
                fMoon.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.IceQueenPetItem, 4));
            }
            if (npc.type == NPCID.PirateShip)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.FlyingDutchmanMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.PirateShipMountItem, 4));
            }
            if (npc.type == NPCID.MartianSaucerCore)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.UFOMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.MartianPetItem, 4));
            }
            if (npc.type == NPCID.KingSlime)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.KingSlimeMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.KingSlimePetItem, 4));
            }
            if (npc.type == NPCID.EyeofCthulhu)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EyeofCthulhuMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EyeOfCthulhuPetItem, 4));

            }
            if (npc.type >= NPCID.EaterofWorldsHead && npc.type <= NPCID.EaterofWorldsTail)
            {
                LeadingConditionRule lastEater = new(new Conditions.LegacyHack_IsABoss());
                lastEater.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EaterofWorldsMasterTrophy));
                lastEater.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.EaterOfWorldsPetItem, 4));
                npcLoot.Add(lastEater);

            }
            if (npc.type == NPCID.BrainofCthulhu)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.BrainofCthulhuMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.BrainOfCthulhuPetItem, 4));
            }
            if (npc.type == NPCID.Deerclops)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DeerclopsMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DeerclopsPetItem, 4));
            }
            if (npc.type == NPCID.QueenBee)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.QueenBeeMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.QueenBeePetItem, 4));
            }
            if (npc.type == NPCID.SkeletronHead)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SkeletronMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SkeletronPetItem, 4));
            }
            if (npc.type == NPCID.WallofFlesh)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.WallofFleshMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.WallOfFleshGoatMountItem, 4));
            }
            if (npc.type == NPCID.QueenSlimeBoss)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.QueenSlimeMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.QueenSlimePetItem, 4));
            }
            if (npc.type >= NPCID.TheDestroyer && npc.type <= NPCID.TheDestroyerTail)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DestroyerMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DestroyerPetItem, 4));
            }
            if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
            {
                LeadingConditionRule noTwin = new(new Conditions.MissingTwin());
                noTwin.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.TwinsMasterTrophy));
                noTwin.OnSuccess(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.TwinsPetItem, 4));
                npcLoot.Add(noTwin);
            }
            if (npc.type == NPCID.SkeletronPrime)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SkeletronPrimeMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.SkeletronPrimePetItem, 4));
            }
            if (npc.type == NPCID.Plantera)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.PlanteraMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.PlanteraPetItem, 4));
            }
            if (npc.type == NPCID.Golem)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.GolemMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.GolemPetItem, 4));
            }
            if (npc.type == NPCID.DD2Betsy)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.BetsyMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DD2BetsyPetItem, 4));
            }
            if (npc.type == NPCID.DukeFishron)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DukeFishronMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.DukeFishronPetItem, 4));
            }
            if (npc.type == NPCID.HallowBoss)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.FairyQueenMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.FairyQueenPetItem, 4));
            }
            if (npc.type == NPCID.CultistBoss)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.LunaticCultistMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.LunaticCultistPetItem, 4));
            }
            if (npc.type == NPCID.MoonLordCore)
            {
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.MoonLordMasterTrophy));
                npcLoot.Add(ItemDropRule.ByCondition(CalamityConditions.EmodeNotRevCondition.ToDropCondition(ShowItemDropInUI.Never), ItemID.MoonLordPetItem, 4));
            }

            #endregion MasterModeDropsInRev

            #region PreHM progression break fixes
            LeadingConditionRule PreHMNotBalanced = new LeadingConditionRule(CalamityConditions.PreHardmodeAndNotBalance.ToDropCondition(ShowItemDropInUI.Always));
            if (npc.type == NPCID.WyvernHead)
            {
                hardmode.OnSuccess(NormalVsExpertQuantity(ModContent.ItemType<EssenceofSunlight>(), 1, 8, 10, 10, 12));
                PreHMNotBalanced.OnSuccess(NormalVsExpertQuantity(ModContent.ItemType<EssenceofSunlight>(), 1, 8, 10, 10, 12));
                npcLoot.Add(PreHMNotBalanced);
                npcLoot.Add(hardmode);
            }
            if (npc.type == NPCID.AngryNimbus)
            {
                npcLoot.Add(ItemDropRule.ByCondition(Condition.Hardmode.ToDropCondition(ShowItemDropInUI.Always), ModContent.ItemType<EssenceofSunlight>(), 2));
                PreHMNotBalanced.OnSuccess(ItemDropRule.Common(ModContent.ItemType<EssenceofSunlight>(), 2));
                npcLoot.Add(PreHMNotBalanced);
            }
            if (npc.type == NPCID.SeekerHead)
            {
                npcLoot.RemoveWhere(delegate (IItemDropRule rule)
                {
                    CommonDrop drop = rule as CommonDrop;
                    return drop != null && drop.itemId == ItemID.CursedFlame && Condition.Hardmode.IsMet();
                });
                npcLoot.DefineConditionalDropSet(If(() => !death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), () => !death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), "")).Add(ItemID.CursedFlame, 1, 2, 5);
                npcLoot.DefineConditionalDropSet(If(() => death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), () => death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), "")).Add(ItemID.CursedFlame, 1, 6, 15);
                npcLoot.DefineConditionalDropSet(If(() => death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), () => death && (Condition.Hardmode.IsMet() || CalamityConditions.PreHardmodeAndNotBalance.IsMet()), "In Death Mode")).Add(ItemID.SoulofNight, 1, 4, 8);
            }
            if (npc.type == NPCID.SandElemental)
            {
                hardmode.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottle>(), 5, 3));
                hardmode.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottlewithBoobs>(), 10, 6));
                PreHMNotBalanced.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottle>(), 5, 3));
                PreHMNotBalanced.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottlewithBoobs>(), 10, 6));
                npcLoot.Add(PreHMNotBalanced);
                npcLoot.Add(hardmode);
            }
            if (npc.type == NPCID.IchorSticker)
            {
                hardmode.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<IchorSpear>(), 25, 15));
                PreHMNotBalanced.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<IchorSpear>(), 25, 15));
                npcLoot.Add(PreHMNotBalanced);
                npcLoot.Add(hardmode);
            }
            #endregion
            #region Tim's Concoction drops
            void TimsConcoctionDrop(IItemDropRule rule)
            {
                TimsConcoctionDropCondition dropCondition = new();
                IItemDropRule conditionalRule = new LeadingConditionRule(dropCondition);
                conditionalRule.OnSuccess(rule);
                npcLoot.Add(conditionalRule);
            }
            if (DropsBoundingPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<BoundingPotion>(), 1, 1, 6));
            }
            if (npc.type == NPCID.BlueSlime && (npc.netID == NPCID.GreenSlime || npc.netID == NPCID.JungleSlime))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<BoundingPotion>(), 1, 1, 2));
            }
            if (DropsCalciumPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<CalciumPotion>(), 1, 1, 6));
            }
            if (DropsPhotosynthesisPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<PhotosynthesisPotion>(), 1, 2, 6));
            }
            if (DropsShadowPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<ShadowPotion>(), 1, 1, 3));
            }
            if (DropsSoaringPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<SoaringPotion>(), 1, 1, 6));
            }
            if (DropsSulphurskinPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<SulphurskinPotion>(), 1, 1, 6));
            }
            if (DropsTeslaPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<TeslaPotion>(), 1, 2, 6));
            }
            if (DropsZenPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<ZenPotion>(), 1, 1, 1));
            }
            if (DropsZergPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<ZergPotion>(), 1, 1, 1));
            }
            //if (npc.type == ModContent.NPCType<>)
            #endregion

            npcLoot.Add(emodeRule);
            npcLoot.Add(pMoon);
            npcLoot.Add(fMoon);
        }

        public static bool killedAquatic;
        public override bool PreKill(NPC npc)
        {
            #region newshopitemdisplay
            bool doDeviText = false;
            //if (npc.type == ModContent.NPCType<GreatSandShark>() && !DownedBossSystem.downedGSS)
            //{
            //    Main.NewText("A new item has been unlocked in Abominationn's shop!", Color.Orange);
            //}
            //if (npc.type == ModContent.NPCType<CragmawMire>() && !DownedBossSystem.downedCragmawMire)
            //{
            //    Main.NewText("A new item has been unlocked in Abominationn's shop!", Color.Orange);
            //}
            //if (npc.type == ModContent.NPCType<Mauler>() && !DownedBossSystem.downedMauler)
            //{
            //    Main.NewText("A new item has been unlocked in Abominationn's shop!", Color.Orange);
            //}
            //if (npc.type == ModContent.NPCType<NuclearTerror>() && !DownedBossSystem.downedNuclearTerror)
            //{
            //    Main.NewText("A new item has been unlocked in Abominationn's shop!", Color.Orange);
            //}
            if (npc.type == ModContent.NPCType<GiantClam>() && !CalamityAIOverride.DownedClam)
            {

                doDeviText = true;
            }
            if (npc.type == ModContent.NPCType<PlaguebringerMiniboss>() && !DLCWorldSavingSystem.downedMiniPlaguebringer)
            {
                doDeviText = true;
                DLCWorldSavingSystem.downedMiniPlaguebringer = true;
            }
            if (npc.type == ModContent.NPCType<ReaperShark>() && !DLCWorldSavingSystem.downedReaperShark)
            {
                doDeviText = true;
                DLCWorldSavingSystem.downedReaperShark = true;
            }
            if (npc.type == ModContent.NPCType<ColossalSquid>() && !DLCWorldSavingSystem.downedColossalSquid)
            {
                doDeviText = true;
                DLCWorldSavingSystem.downedColossalSquid = true;
            }
            if (npc.type == ModContent.NPCType<EidolonWyrmHead>() && !DLCWorldSavingSystem.downedEidolonWyrm)
            {
                doDeviText = true;
                DLCWorldSavingSystem.downedEidolonWyrm = true;
            }
            if (npc.type == ModContent.NPCType<ThiccWaifu>() && !DLCWorldSavingSystem.downedCloudElemental)
            {
                doDeviText = true;
                DLCWorldSavingSystem.downedCloudElemental = true;
            }
            if (npc.type == ModContent.NPCType<Horse>() && !DLCWorldSavingSystem.downedEarthElemental)
            {
                doDeviText = true;
                DLCWorldSavingSystem.downedEarthElemental = true;
            }
            if (npc.type == ModContent.NPCType<ArmoredDiggerHead>() && !DLCWorldSavingSystem.downedArmoredDigger)
            {
                doDeviText = true;
                DLCWorldSavingSystem.downedArmoredDigger = true;
            }
            if (doDeviText && Main.netMode != NetmodeID.Server)
            {
                Main.NewText("A new item has been unlocked in Deviantt's shop!", Color.HotPink);
            }
            #endregion
            //the thing


            if (npc.type == ModContent.NPCType<TimberChampionHead>() && BossRushEvent.BossRushActive)
            {
                for (int playerIndex = 0; playerIndex < 255; playerIndex++)
                {
                    Player p = Main.player[playerIndex];
                    if (p != null && p.active)
                    {
                        p.Calamity().BossRushReturnPosition = p.Center;
                        Vector2 underworld = new Vector2(Main.maxTilesX * 16 / 2, Main.maxTilesY * 16 - 2400);
                        CalamityPlayer.ModTeleport(p, underworld, false, 2);
                        SoundStyle teleportSound = BossRushEvent.TeleportSound;
                        teleportSound.Volume = 1.6f;
                        SoundEngine.PlaySound(teleportSound, p.Center);
                    }
                }
            }
            if (npc.type == ModContent.NPCType<NatureChampion>() && BossRushEvent.BossRushActive)
            {
                for (int playerIndex = 0; playerIndex < 255; playerIndex++)
                {
                    Player p = Main.player[playerIndex];
                    if (p != null && p.active)
                    {
                        if (p.Calamity().BossRushReturnPosition != null)
                        {
                            CalamityPlayer.ModTeleport(p, p.Calamity().BossRushReturnPosition.Value, false, 2);
                            p.Calamity().BossRushReturnPosition = null;
                        }
                        p.Calamity().BossRushReturnPosition = null;
                        SoundStyle teleportSound = BossRushEvent.TeleportSound;
                        teleportSound.Volume = 1.6f;
                        SoundEngine.PlaySound(teleportSound, p.Center);
                    }
                }
            }
            if ((npc.type == ModContent.NPCType<TrojanSquirrel>() || npc.type == ModContent.NPCType<LifeChallenger>() || ContentLists.Champions.Contains(npc.type) || npc.type == ModContent.NPCType<DeviBoss>() || npc.type == ModContent.NPCType<AbomBoss>()) && BossRushEvent.BossRushActive && npc.type != ModContent.NPCType<TimberChampion>() || npc.type == ModContent.NPCType<BanishedBaron>())
            {
                BossRushEvent.BossRushStage++;
            }
            if ((npc.type == NPCID.SolarCorite || npc.type == NPCID.SolarCrawltipedeHead || npc.type == NPCID.SolarCrawltipedeTail
                || npc.type == NPCID.StardustJellyfishBig || npc.type == NPCID.NebulaBrain || npc.type == NPCID.VortexHornetQueen) && !NPC.downedAncientCultist)
            {
                return false;
            }
            if (ContentLists.SolarEclipseEnemies.Contains(npc.type) && DownedBossSystem.downedDoG && !Main.eclipse)
            {
                return false;
            }
            if (ContentLists.FrostMoonEnemies.Contains(npc.type) && DownedBossSystem.downedDoG && !Main.snowMoon)
            {
                return false;
            }
            if (ContentLists.PumpkinMoonEnemies.Contains(npc.type) && DownedBossSystem.downedDoG && !Main.pumpkinMoon)
            {
                return false;
            }
            return base.PreKill(npc);
        }
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneSulphur)
            {
                pool[NPCID.PigronCorruption] = 0.0001f;
                pool[NPCID.PigronCrimson] = 0.0001f;
                pool[NPCID.PigronHallow] = 0.0001f;
                pool[NPCID.IchorSticker] = 0;
                for (int i = 0; i < ContentLists.SandstormEnemies.Count; i++)
                {
                    pool[ContentLists.SandstormEnemies[i]] = 0;
                }
                if (AcidRainEvent.AcidRainEventIsOngoing)
                {
                    pool[NPCID.PigronCorruption] = 0f;
                    pool[NPCID.PigronCrimson] = 0f;
                    pool[NPCID.PigronHallow] = 0f;
                }
            }
            if (!Main.hardMode && spawnInfo.Player.Calamity().ZoneSunkenSea)
            {
                pool[NPCID.Mimic] = 0f;
                pool[NPCID.DuneSplicerHead] = 0f;
            }
        }
        public override bool InstancePerEntity => true;
        private int numAI;
        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (npc.type == ModContent.NPCType<Squirrel>())
            {
                bool sellRock = false;
                bool soldRock = false;
                foreach (Player player in Main.player)
                {
                    foreach (Item item in player.inventory)
                    {
                        if (CalamityContentLists.RockItems.Contains(item.type))
                            sellRock = true;
                    }
                    foreach (Item item in player.armor)
                    {
                        if (CalamityContentLists.RockItems.Contains(item.type))
                            sellRock = true;
                    }
                    foreach (Item item in player.bank.item)
                    {
                        if (CalamityContentLists.RockItems.Contains(item.type))
                            sellRock = true;
                    }
                }
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] is null && sellRock && !soldRock)
                    {
                        items[i] = new Item(ModContent.ItemType<Rock>()) { shopCustomPrice = Item.buyPrice(platinum: 50) };
                        soldRock = true;
                    }
                }
            }
        }
        public bool droppedSummon = false;
        public static List<int> HyperNPCs = new List<int>()
        {
            ModContent.NPCType<TrojanSquirrelHead>(), ModContent.NPCType<TrojanSquirrelArms>(), ModContent.NPCType<TrojanSquirrel>(),
            NPCID.KingSlime, NPCID.EyeofCthulhu, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail,
            NPCID.BrainofCthulhu, ModContent.NPCType<BrainIllusion>(), NPCID.Creeper, NPCID.QueenBee, ModContent.NPCType<RoyalSubject>(),
            NPCID.SkeletronHead, NPCID.SkeletronHand, ModContent.NPCType<DeviBoss>(), NPCID.WallofFlesh,
             NPCID.QueenSlimeBoss, NPCID.Retinazer, NPCID.Spazmatism, NPCID.SkeletronPrime,
            NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.TheDestroyer, NPCID.TheDestroyerBody,
            NPCID.TheDestroyerTail, NPCID.Probe, NPCID.Plantera, NPCID.PlanterasHook, NPCID.PlanterasTentacle, NPCID.GolemFistLeft,
            NPCID.GolemFistRight, NPCID.GolemHead, NPCID.Golem, NPCID.GolemHeadFree,
            NPCID.CultistBoss, NPCID.MoonLordCore, NPCID.MoonLordFreeEye, NPCID.MoonLordHand, NPCID.MoonLordHead
        };
        public override bool PreAI(NPC npc)
        {
            #region SummonDrops
            if (npc.type == ModContent.NPCType<DesertScourgeHead>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "MedallionoftheDesert", DownedBossSystem.downedDesertScourge, ref droppedSummon);
            }
            else if (npc.type == ModContent.NPCType<Crabulon>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "OphiocordycipitaceaeSprout", DownedBossSystem.downedCrabulon, ref droppedSummon);

            }
            else if (npc.type == ModContent.NPCType<HiveMind>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "HiveTumor", DownedBossSystem.downedHiveMind, ref droppedSummon);
            }
            else if (npc.type == ModContent.NPCType<PerforatorHive>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "RedStainedWormFood", DownedBossSystem.downedPerforator, ref droppedSummon);
            }
            else if (npc.type == ModContent.NPCType<SlimeGodCore>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "MurkySludge", DownedBossSystem.downedSlimeGod, ref droppedSummon);
            }
            else if (npc.type == ModContent.NPCType<Cryogen>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "CryingKey", DownedBossSystem.downedCryogen, ref droppedSummon, Main.hardMode);
            }
            else if (npc.type == ModContent.NPCType<AquaticScourgeHead>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "SeeFood", DownedBossSystem.downedAquaticScourge, ref droppedSummon);
            }
            else if (npc.type == ModContent.NPCType<BrimstoneElemental>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "FriedDoll", DownedBossSystem.downedBrimstoneElemental, ref droppedSummon, Main.hardMode);
            }
            else if (npc.type == ModContent.NPCType<CalamitasClone>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "BlightedEye", DownedBossSystem.downedCalamitasClone, ref droppedSummon, Main.hardMode);
            }
            else if (npc.type == ModContent.NPCType<Anahita>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "SirensPearl", DownedBossSystem.downedLeviathan, ref droppedSummon);
            }
            else if (npc.type == ModContent.NPCType<AstrumAureus>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "ChunkyStardust", DownedBossSystem.downedAstrumAureus, ref droppedSummon, Main.hardMode);
            }
            else if (npc.type == ModContent.NPCType<PlaguebringerGoliath>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "ABombInMyNation", DownedBossSystem.downedPlaguebringer, ref droppedSummon, NPC.downedGolemBoss);
            }
            else if (npc.type == ModContent.NPCType<RavagerBody>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "NoisyWhistle", DownedBossSystem.downedRavager, ref droppedSummon, Main.hardMode);
            }
            else if (npc.type == ModContent.NPCType<AstrumDeusHead>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "AstrumCor", DownedBossSystem.downedAstrumDeus, ref droppedSummon, Main.hardMode);
            }
            else if (npc.type == ModContent.NPCType<Bumblefuck>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "BirbPheromones", DownedBossSystem.downedDragonfolly, ref droppedSummon, NPC.downedAncientCultist);
            }
            else if (npc.type == ModContent.NPCType<ProfanedGuardianCommander>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "DefiledShard", DownedBossSystem.downedGuardians, ref droppedSummon, NPC.downedMoonlord);
            }
            else if (npc.type == ModContent.NPCType<Providence>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "DefiledCore", DownedBossSystem.downedProvidence, ref droppedSummon, NPC.downedMoonlord);
            }
            else if (npc.type == ModContent.NPCType<CeaselessVoid>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "RiftofKos", DownedBossSystem.downedCeaselessVoid, ref droppedSummon, NPC.downedMoonlord);
            }
            else if (npc.type == ModContent.NPCType<StormWeaverHead>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "WormFoodofKos", DownedBossSystem.downedStormWeaver, ref droppedSummon, NPC.downedMoonlord);
            }
            else if (npc.type == ModContent.NPCType<Signus>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "LetterofKos", DownedBossSystem.downedSignus, ref droppedSummon, NPC.downedMoonlord);
            }
            else if (npc.type == ModContent.NPCType<Polterghast>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "PolterplasmicBeacon", DownedBossSystem.downedPolterghast, ref droppedSummon, NPC.downedMoonlord);
            }
            else if (npc.type == ModContent.NPCType<OldDuke>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "BloodyWorm", DownedBossSystem.downedBoomerDuke, ref droppedSummon, DownedBossSystem.downedPolterghast);
            }
            else if (npc.type == ModContent.NPCType<DevourerofGodsHead>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "SomeKindofSpaceWorm", DownedBossSystem.downedDoG, ref droppedSummon, NPC.downedMoonlord);
            }
            else if (npc.type == ModContent.NPCType<Yharon>())
            {
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "DragonEgg", DownedBossSystem.downedYharon, ref droppedSummon, NPC.downedMoonlord);
            }
            else if (npc.type == ModContent.NPCType<Draedon>())
            {
                if (Main.expertMode && Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<CalamitousPresenceBuff>(), 2);
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "ExoBattery", DownedBossSystem.downedExoMechs, ref droppedSummon, DownedBossSystem.downedYharon);
            }
            else if (npc.type == ModContent.NPCType<SupremeCalamitas>())
            {
                if (Main.expertMode && Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<CalamitousPresenceBuff>(), 2);
                DLCUtils.DropSummon(npc, FargowiltasCrossmod.Instance.Name, "EyeofExtinction", DownedBossSystem.downedCalamitas, ref droppedSummon, DownedBossSystem.downedYharon);
            }
            if (ModCompatibility.WrathoftheGods.Loaded)
            {
                if (npc.type == ModCompatibility.WrathoftheGods.NoxusBoss1.Type ||
                    npc.type == ModCompatibility.WrathoftheGods.NoxusBoss2.Type ||
                    npc.type == ModCompatibility.WrathoftheGods.NamelessDeityBoss.Type)
                {
                    if (Main.expertMode && Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
                        Main.LocalPlayer.AddBuff(ModContent.BuffType<MutantPresenceBuff>(), 2);
                }
            }
            #endregion SummonDrops
            if (BossRushEvent.BossRushActive)
            {
                if (!killedAquatic && BossRushEvent.BossRushStage > 19)
                {
                    BossRushEvent.BossRushStage = 19;
                }
                if (NPC.AnyNPCs(ModContent.NPCType<AquaticScourgeHead>()))
                {
                    killedAquatic = true;
                }
            }
            else
            {
                killedAquatic = false;
                if (DLCCalamityConfig.Instance.EternityPriorityOverRev)
                {
                    if (npc.type == NPCID.AncientLight && DLCWorldSavingSystem.EternityDeath && NPC.AnyNPCs(NPCID.CultistBoss))
                    {
                        npc.Center += npc.velocity * 0.75f;
                        npc.dontTakeDamage = true;
                    }
                }
            }

            //BossRushEvent.BossRushStage = 18;
            //BossRushEvent.BossRushStage = 36;
            if (BossRushEvent.BossRushActive)
            {

                if (npc.type == NPCID.HallowBoss && npc.ai[0] == 13)
                {
                    Main.dayTime = true;
                }
                if (npc.type == ModContent.NPCType<BanishedBaron>())
                {
                    //Fix for floppy fish in p1
                    BanishedBaron baron = npc.ModNPC as BanishedBaron;
                    Player target = Main.player[npc.target];
                    if (target != null && target.active && !target.dead)
                    {
                        if (baron.Phase == 1 && npc.Center.Y < target.Center.Y && !(Collision.WetCollision(npc.position, npc.width, npc.height) || Collision.SolidCollision(npc.position, npc.width, npc.height)))
                        {
                            npc.position.Y -= 4f;
                        }
                    }

                    foreach (Player player in Main.player)
                    {
                        if (player.active) player.buffImmune[ModContent.BuffType<BaronsBurdenBuff>()] = true;
                    }
                }
                if (npc.type == NPCID.SkeletronHead && npc.life <= 58000 && npc.life > 1000)
                {
                    npc.life = 1000;
                }
                if (numAI == 0)
                {
                    if (HyperNPCs.Contains(npc.type) && WorldSavingSystem.EternityMode && npc.type != NPCID.WallofFleshEye)
                    {
                        numAI++;
                        npc.AI();
                        float speedToAdd = 0.8f;
                        Vector2 newPos = npc.position + npc.velocity * speedToAdd;
                        if (!Collision.SolidCollision(newPos, npc.width, npc.height))
                        {
                            npc.position = newPos;
                        }
                    }
                }
            }
            if (npc.type == NPCID.DukeFishron && BossRushEvent.BossRushActive)
            {
                Main.player[Main.myPlayer].ZoneBeach = true;
            }
            if (npc.type == ModContent.NPCType<SpiritChampion>() && BossRushEvent.BossRushActive)
            {
                Main.player[Main.myPlayer].ZoneRockLayerHeight = true;
                Main.player[Main.myPlayer].ZoneUndergroundDesert = true;

            }
            if (npc.type == ModContent.NPCType<ShadowChampion>() && BossRushEvent.BossRushActive)
            {
                Main.dayTime = false;
                Main.time = Main.nightLength / 2;
            }
            if (npc.type == ModContent.NPCType<TerraChampion>() && BossRushEvent.BossRushActive)
            {
                Main.player[Main.myPlayer].ZoneUnderworldHeight = false;
            }
            if (npc.type == ModContent.NPCType<EarthChampion>() && BossRushEvent.BossRushActive)
            {
                Main.player[Main.myPlayer].ZoneUnderworldHeight = true;
            }
            if (npc.type == ModContent.NPCType<NatureChampion>() && BossRushEvent.BossRushActive)
            {
                Main.player[Main.myPlayer].ZoneUnderworldHeight = false;

            }
            if (npc.type == ModContent.NPCType<MutantBoss>() && BossRushEvent.BossRushActive)
            {
                npc.ModNPC.SceneEffectPriority = SceneEffectPriority.None;
                if (npc.ai[0] == -7 && npc.ai[1] >= 250)
                {
                    npc.StrikeInstantKill();

                    CalamityUtils.KillAllHostileProjectiles();
                    BossRushEvent.HostileProjectileKillCounter = 3;
                    DownedBossSystem.downedBossRush = true;
                    CalamityNetcode.SyncWorld();
                    if (DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(new EntitySource_WorldEvent(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BossRushEndEffectThing>(), 0, 0f, Main.myPlayer);
                    }
                }
            }
            //Main.NewText(FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode);
            #region Balance Changes config
            if (DLCCalamityConfig.Instance.BalanceRework)
            {
                //add defense damage to fargo enemies. setting this in SetDefaults crashes the game for some reason
                if (npc.ModNPC != null)
                {
                    if (npc.ModNPC.Mod == ModCompatibility.SoulsMod.Mod && npc.IsAnEnemy())
                    {
                        ModCompatibility.Calamity.Mod.Call("SetDefenseDamageNPC", npc, true);
                    }
                }
            }
            #endregion
            if (!DLCWorldSavingSystem.E_EternityRev)
            {
                return base.PreAI(npc);
            }
            if (npc.type == NPCID.MoonLordCore)
            {
                Main.player[Main.myPlayer].Calamity().infiniteFlight = true;
            }
            //queen bee no enrage during br
            if (npc.type == NPCID.QueenBee && BossRushEvent.BossRushActive)
            {
                npc.GetGlobalNPC<QueenBee>().EnrageFactor = 0;
            }
            if (npc.type == NPCID.Golem && BossRushEvent.BossRushActive)
            {
                npc.GetGlobalNPC<Golem>().IsInTemple = true;
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
            //make destroyer not invincible and normal scale
            List<int> bossworms = new List<int>
                {

                    ModContent.NPCType<DesertScourgeHead>(), ModContent.NPCType<DesertScourgeBody>(), ModContent.NPCType<DesertScourgeTail>(),
                    NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail,

                    ModContent.NPCType<AquaticScourgeHead>(), ModContent.NPCType<AquaticScourgeBody>(),ModContent.NPCType<AquaticScourgeBodyAlt>(), ModContent.NPCType<AquaticScourgeTail>(),
                    NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail,
                    /*ModContent.NPCType<AstrumDeusHead>(), ModContent.NPCType<AstrumDeusBody>(), ModContent.NPCType<AstrumDeusTail>(),*/
                    ModContent.NPCType<StormWeaverHead>(), ModContent.NPCType<StormWeaverBody>(), ModContent.NPCType<StormWeaverTail>(),

                };
            if (bossworms.Contains(npc.type) && WorldSavingSystem.EternityMode)
            {
                Mod calamity = ModCompatibility.Calamity.Mod;

                calamity.Call("SetCalamityAI", npc, 1, 600f);
                calamity.Call("SetCalamityAI", npc, 2, 0f);
                npc.SyncExtraAI();
            }
            //make plantera not summon free tentacles
            if (npc.type == ModContent.NPCType<PlanterasFreeTentacle>())
            {
                npc.StrikeInstantKill();
            }
            return base.PreAI(npc);
        }
        public override void PostAI(NPC npc)
        {

            if (numAI > 0)
            {
                numAI = 0;
            }
        }

        public int PermafrostDefeatLine = 0;
        public override void GetChat(NPC npc, ref string chat)
        {
            if (npc.type == ModContent.NPCType<DILF>())
            {
                if (PermafrostDefeatLine == 1)
                {
                    chat = "You clearly possess great skill! My thanks to you for freeing me, and providing an opportunity to defrost my fighting abilities.";
                    PermafrostDefeatLine = 0;
                }
                if (PermafrostDefeatLine == 2)
                {
                    chat = "Aah, a good round of sparring. I need the exercise, so thanks for that.";
                    PermafrostDefeatLine = 0;
                }
            }
        }
        public override void ModifyShop(NPCShop shop)
        {


            Condition killedCragmaw = new Condition("Kill a Cragmaw Mire", () => CalamityAIOverride.DownedCragmaw);
            Condition killedMauler = new Condition("Kill a Mauler", () => CalamityAIOverride.DownedMauler);
            Condition killedNuclear = new Condition("Kill a Nuclear Terror", () => CalamityAIOverride.DownedNuclear);
            Condition killedGSS = new Condition("Kill a Great Sand Shark", () => CalamityAIOverride.DownedGSS);
            if (shop.NpcType == ModContent.NPCType<Abominationn>())
            {
                shop.Add(new Item(ModContent.ItemType<SulphurBearTrap>()) { shopCustomPrice = Item.buyPrice(gold: 10) }, killedCragmaw);
                shop.Add(new Item(ModContent.ItemType<MaulerSkull>()) { shopCustomPrice = Item.buyPrice(gold: 30) }, killedMauler);
                shop.Add(new Item(ModContent.ItemType<NuclearChunk>()) { shopCustomPrice = Item.buyPrice(gold: 30) }, killedNuclear);
                shop.Add(new Item(ModContent.ItemType<SandstormsCore>()) { shopCustomPrice = Item.buyPrice(gold: 30) }, killedGSS);
            }
            base.ModifyShop(shop);
        }
    }
}
