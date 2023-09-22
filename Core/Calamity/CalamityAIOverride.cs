using FargowiltasCrossmod.Common.Systems;
using MonoMod.Cil;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.NPCs;
using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using CalamityMod.Events;

using CalamityMod.Projectiles;
using FargowiltasSouls.Core.Systems;
using Mono.Cecil.Cil;
using System.ComponentModel.DataAnnotations;
using CalamityMod.BiomeManagers;

using Fargowiltas.NPCs;
using Terraria.ID;
using FargowiltasSouls.Content.Bosses.MutantBoss;

using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod;
using CalamityMod.Projectiles.Typeless;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using FargowiltasSouls.Content.Bosses.Lifelight;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.Champions.Will;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.Champions.Life;
using FargowiltasSouls.Content.Bosses.Champions.Spirit;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.Champions.Timber;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.BanishedBaron;
using FargowiltasCrossmod.Content.Calamity.Toggles;

namespace FargowiltasCrossmod.Core.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamityAIOverride : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod) => ModCompatibility.Calamity.Loaded;
        
        public override void PostSetupContent()
        {
            
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
            Mod cal = ModLoader.GetMod("CalamityMod");
            //get the list of boss entries
            List < (int, int, Action<int>, int, bool, float, int[], int[]) > Entries = cal.Call("GetBossRushEntries") as List<(int, int, Action<int>, int, bool, float, int[], int[])>;
            //insert trojan squirrel at the beginning
            Entries.Insert(0, (ModContent.NPCType<TrojanSquirrel>(), 0, delegate (int type)
            {
                FargoSoulsUtil.SpawnBossNetcoded(Main.player[Main.myPlayer], ModContent.NPCType<TrojanSquirrel>());
            }, -1, false, 0f, new int[] {ModContent.NPCType<TrojanSquirrelArms>(), ModContent.NPCType<TrojanSquirrelHead>(), ModContent.NPCType<TrojanSquirrelLimb>(), ModContent.NPCType<TrojanSquirrelPart>() }, new int[] { }));
            //insert deviantt before sg
            Entries.Insert(12, (ModContent.NPCType<DeviBoss>(), 0, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<DeviBoss>());
            }, -1, false, 0f, new int[] { }, new int[] { }));
            Entries.Insert(16, (ModContent.NPCType<BanishedBaron>(), 0, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<BanishedBaron>());
            }, -1, false, 0f, new int[] { }, new int[] { }));
            //insert lieflight after mechs
            Entries.Insert(23, (ModContent.NPCType<LifeChallenger>(), 0, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<LifeChallenger>());
            }, -1, false, 0f, new int[] { }, new int[] { }));
            //insert champions after provi
            Entries.Insert(39, (ModContent.NPCType<CosmosChampion>(), 0, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<CosmosChampion>());
            }, -1, false, 0f, new int[] { }, new int[] { }));
            Entries.Insert(39, (ModContent.NPCType<WillChampion>(), 0, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<WillChampion>());
            }, -1, false, 0f, new int[] { }, new int[] { }));
            Entries.Insert(39, (ModContent.NPCType<ShadowChampion>(), -1, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<ShadowChampion>());
            }, -1, false, 0f, new int[] {ModContent.NPCType<ShadowOrbNPC>(),  }, new int[] { }));
            Entries.Insert(39, (ModContent.NPCType<SpiritChampion>(), 0, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<SpiritChampion>());
            }, -1, false, 0f, new int[] {ModContent.NPCType<SpiritChampionHand>() }, new int[] { }));
            Entries.Insert(39, (ModContent.NPCType<LifeChallenger>(), 0, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<LifeChallenger>());
            }, -1, false, 0f, new int[] { }, new int[] { }));
            Entries.Insert(39, (ModContent.NPCType<NatureChampion>(), 0, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<NatureChampion>());
            }, -1, false, 0f, new int[] {ModContent.NPCType<NatureChampionHead>() }, new int[] { }));
            Entries.Insert(39, (ModContent.NPCType<EarthChampion>(), 0, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<EarthChampion>());
            }, -1, false, 0f, new int[] {ModContent.NPCType<EarthChampionHand>() }, new int[] { }));
            Entries.Insert(39, (ModContent.NPCType<TerraChampion>(), 0, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<TerraChampion>());

            }, -1, false, 0f, new int[] {ModContent.NPCType<TerraChampionBody>(), ModContent.NPCType<TerraChampionTail>() }, new int[] { }));
            Entries.Insert(39, (ModContent.NPCType<TimberChampion>(), 0, delegate (int type)
            {
                FargoSoulsUtil.SpawnBossNetcoded(Main.player[Main.myPlayer], ModContent.NPCType<TimberChampion>());
                
            }, -1, false, 0f, new int[] {ModContent.NPCType<TimberChampionHead>() }, new int[] {ModContent.NPCType<TimberChampionHead>() }));
            //add abom before draedon
            Entries.Insert(55, (ModContent.NPCType<AbomBoss>(), 0, delegate (int type)
            {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<AbomBoss>());
            }, -1, false, 0f, new int[] { ModContent.NPCType<AbomSaucer>()}, new int[] { }));
            //add mutant boss entry to the end
            Entries.Add((ModContent.NPCType<MutantBoss>(), 0, delegate (int type) {
                NPC.SpawnOnPlayer(0, ModContent.NPCType<MutantBoss>());
            }, -1, false, 0f, new int[] { }, new int[] { }));
            //set boss rush entries to new list of entries
            cal.Call("SetBossRushEntries", Entries);
            //make scal not end the event on defeat so it continues to mutant
            DeathEffectsList.Remove(ModContent.NPCType<SupremeCalamitas>());
            
        }
        //make this a property instead of directly using it so tml doesnt shit itself trying to load it
        public ref Dictionary<int, Action<NPC>> DeathEffectsList
        {
            get { return ref BossRushEvent.BossDeathEffects; }
        }
        public override void Load()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                calamity.Call("AddDifficultyToUI", new EternityRevDifficulty());
                FargowiltasCrossmod.LoadTogglesFromType(typeof(CalamityToggles));
                FargowiltasCrossmod.LoadTogglesFromType(typeof(CalamityEnchToggles));
            }
            //Disable rev+ enemy/boss ai in emode
            MonoModHooks.Modify(typeof(CalamityGlobalNPC).GetMethod(nameof(CalamityGlobalNPC.PreAI)), CalamityPreAI_ILEdit);
            //Disable rev+ projectile ai in emode
            MonoModHooks.Modify(typeof(CalamityGlobalProjectile).GetMethod(nameof(CalamityGlobalProjectile.PreAI)), CalamityProjectilePreAI_ILEdit);
            //Disable cal drops (readd later with conditions that arent dumb)
            MonoModHooks.Modify(typeof(CalamityGlobalNPC).GetMethod(nameof(CalamityGlobalNPC.ModifyNPCLoot)), CalamityRemoveDrops_ILEdit);
            //MonoModHooks.Modify(typeof(CalamityGlobalProjectile).GetMethod(nameof(CalamityGlobalProjectile.AI)), CalamityProjectileAI_ILEdit);
        }
        private static void CalamityRemoveDrops_ILEdit(ILContext il)
        {
            #region RevMasterDropsRemoval
            var c = new ILCursor(il);
            //dark mage
            c.GotoNext(i => i.MatchLdcI4(4946));
            c.Index--;
            c.RemoveRange(16);
            ILLabel label = c.MarkLabel();
            c.GotoPrev(i => i.MatchLdcI4(565));
            c.Index++;
            c.Remove();
            c.Emit(OpCodes.Beq, label);
            //ogre
            c.GotoNext(i => i.MatchLdcI4(4947));
            c.Index--;
            c.RemoveRange(16);
            label = c.MarkLabel();
            c.GotoPrev(i => i.MatchLdcI4(577));
            c.Index++; c.Remove();
            c.Emit(OpCodes.Beq, label);
            //flying dutchman
            c.GotoNext(i => i.MatchLdcI4(4940));
            c.Index--;
            
            c.RemoveRange(16);
            label = c.MarkLabel();
            ILLabel[] labels = null;
            c.GotoPrev(i => i.MatchSwitch(out labels));
            labels[31] = label;
            c.Remove();
            c.EmitSwitch(labels);

            //morning wood
            c.GotoNext(i => i.MatchLdcI4(4941));
            c.Index -= 2;
            c.RemoveRange(22);
            //pumpking
            c.GotoNext(i => i.MatchLdcI4(4942));
            c.Index -= 2;
            c.RemoveRange(22);
            //evercream
            c.GotoNext(i => i.MatchLdcI4(4944));
            c.Index -= 2;
            c.RemoveRange(22);
            //santank
            c.GotoNext(i => i.MatchLdcI4(4945));
            c.Index -= 2;
            c.RemoveRange(22);
            //ice queen
            c.GotoNext(i => i.MatchLdcI4(4943));
            c.Index -= 2;
            c.RemoveRange(22);
            //flying saucer
            c.GotoNext(i => i.MatchLdcI4(4939));
            c.Index -= 40;
            c.RemoveRange(55);
            //ks
            c.GotoNext(i => i.MatchLdcI4(4929));
            c.Index--;
            c.RemoveRange(16);
            //eoc
            c.GotoNext(i => i.MatchLdcI4(4924));
            c.Index--;
            c.RemoveRange(16);
            //eow
            c.GotoNext(i => i.MatchLdcI4(4925));
            c.Index -= 10;
            c.RemoveRange(36);
            //boc
            c.GotoNext(i => i.MatchLdcI4(4926));
            c.Index--;
            c.RemoveRange(16);
            //deer
            c.GotoNext(i => i.MatchLdcI4(5110));
            c.Index--;
            c.RemoveRange(16);
            //qb
            c.GotoNext(i => i.MatchLdcI4(4928));
            c.Index--;
            c.RemoveRange(16);
            //skeletron
            c.GotoNext(i => i.MatchLdcI4(4927));
            c.Index--;
            c.RemoveRange(16);
            //wof
            c.GotoNext(i => i.MatchLdcI4(4930));
            c.Index--;
            c.RemoveRange(16);
            //qs
            c.GotoNext(i => i.MatchLdcI4(4950));
            c.Index--;
            c.RemoveRange(16);
            //destroyer
            c.GotoNext(i => i.MatchLdcI4(4932));
            c.Index--;
            c.RemoveRange(16);
            //twins
            c.GotoNext(i => i.MatchLdcI4(4931));
            c.Index -= 10;
            c.RemoveRange(36);
            //prime
            c.GotoNext(i => i.MatchLdcI4(4933));
            c.Index--;
            c.RemoveRange(16);
            //plant
            c.GotoNext(i => i.MatchLdcI4(4934));
            c.Index--;
            c.RemoveRange(16);
            //golem
            c.GotoNext(i => i.MatchLdcI4(4935));
            c.Index--;
            c.RemoveRange(16);
            //betsy
            c.GotoNext(i => i.MatchLdcI4(4948));
            c.Index -= 47;
            c.RemoveRange(62);
            //duke
            c.GotoNext(i => i.MatchLdcI4(4936));
            c.Index--;
            c.RemoveRange(16);
            //eol
            c.GotoNext(i => i.MatchLdcI4(4949));
            c.Index--;
            c.RemoveRange(16);
            //cultist
            c.GotoNext(i => i.MatchLdcI4(4937));
            c.Index--;
            c.RemoveRange(16);
            label = c.MarkLabel();
            c.GotoPrev(i => i.MatchLdcI4(439));
            c.Index++;
            c.Remove();
            c.EmitBeq(label);
            //ml
            c.GotoNext(i => i.MatchLdcI4(4938));
            c.Index--;
            c.RemoveRange(16);
            #endregion RevMasterDropsRemoval
            //remove essence of sunlight from wyverns
            var d = new ILCursor(il);
            d.GotoNext(i => i.MatchLdcI4(10));
            d.Index -= 4;
            d.RemoveRange(10);
            label = d.MarkLabel();
            d.GotoPrev(i => i.MatchLdcI4(87));
            d.Index++;
            d.Remove();
            d.EmitBeq(label);
            //remove essence of sunlight from nimbus
            d.GotoNext(i => i.MatchLdcI4(2));
            d.Index++;
            d.GotoNext(i => i.MatchLdcI4(2));
            d.Index -= 3;
            d.RemoveRange(8);
            label = d.MarkLabel();
            labels = null;
            d.GotoPrev(i => i.MatchSwitch(out labels));
            labels = null;
            d.Index--;
            d.GotoPrev(i => i.MatchSwitch(out labels));
            labels = null;
            d.Index--;
            d.GotoPrev(i => i.MatchSwitch(out labels));
            labels = null;
            d.Index--;
            d.GotoPrev(i => i.MatchSwitch(out labels));
            labels[8] = label;
            //remove cursed flames from world feeder
            var e = new ILCursor(il);
            e.GotoNext(i => i.MatchLdcI4(522));
            e.Index -= 35;
            e.RemoveRange(103);
            label = e.MarkLabel();
            e.GotoPrev(i => i.MatchLdcI4(113));
            labels = null;
            e.GotoPrev(i => i.MatchSwitch(out labels));
            labels[0] = label;
            //remove elemental in a bottles from sand elemental
            e.GotoNext(i => i.MatchLdcI4(5));
            
            e.Index -= 2;
            e.RemoveRange(14);
            label = e.MarkLabel();
            e.GotoPrev(i => i.MatchLdcI4(541));
            e.Index++;
            e.Remove();
            e.EmitBeq(label);
            MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasCrossmod>(), il);
        }
        private static void CalamityPreAI_ILEdit(ILContext il)
        {
            //var BossRushEvent = ModLoader.GetMod("CalamityMod").Code.GetType("CalamityMod.Events.BossRushEvent");
            var c = new ILCursor(il);
            Mod calamity = ModLoader.GetMod("CalamityMod");
            Type BossRushEvent = calamity.Code.GetType("CalamityMod.Events.BossRushEvent");
            FieldInfo BossRushEvent_BossRushActive = BossRushEvent.GetField("BossRushActive", BindingFlags.Public | BindingFlags.Static);
            //go to correct boss rush check
            c.GotoNext(i => i.MatchLdsfld(BossRushEvent_BossRushActive));
            c.Index++;
            c.GotoNext(i => i.MatchLdsfld(BossRushEvent_BossRushActive));
            c.Index++;
            //get label for skipping past ai changes
            ILLabel label = null;
            c.GotoPrev(i => i.MatchBrtrue(out label));
            c.Index += 3;
            c.EmitDelegate(() => ModContent.GetInstance<CalamityConfig>().EternityPriorityOverRev && FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode && !(BossRushEvent_BossRushActive.GetValue(null) is bool fard && fard));
            c.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, label);
            c.EmitDelegate(() => ModContent.GetInstance<CalamityConfig>().EternityPriorityOverRev && FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode && !(BossRushEvent_BossRushActive.GetValue(null) is bool fard && fard));
            c.Emit(Mono.Cecil.Cil.OpCodes.Ret);
            c.Index -= 9;
            var label2 = il.DefineLabel(c.Prev);
            c.Index -= 4;
            c.Remove();
            c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label2);

            //old method (only excludes boss ais)
            ////go to before checks
            //c.Index -= 3;
            ////add new check and get label for skipping to it

            //c.EmitDelegate(() => ModContent.GetInstance<CalamityConfig>().EternityPriorityOverRev);
            //c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label);
            //c.Index -= 4;
            //var label2 = il.DefineLabel(c.Prev);

            ////go to checking for queen bee and go to the skipper after it
            //c.GotoPrev(i => i.MatchLdcI4(222));
            //c.Index++;
            ////replace skipper with my own
            //c.Remove();
            //c.Emit(Mono.Cecil.Cil.OpCodes.Bne_Un, label2);

            ////do it again but make the check for zenith seed not skip my check
            //c.GotoPrev(i => i.MatchLdsfld(typeof(Main), nameof(Main.zenithWorld)));
            //c.Index++;
            //c.Remove();
            //c.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, label2);
            //MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasCrossmod>(), il);
        }
        public ref bool br
        {
            get { return ref CalamityMod.Events.BossRushEvent.BossRushActive; }
        }
        private static void CalamityProjectilePreAI_ILEdit(ILContext il)
        {
            Mod calamity = ModLoader.GetMod("CalamityMod");
            Type BossRushEvent = calamity.Code.GetType("CalamityMod.Events.BossRushEvent");
            FieldInfo BossRushEvent_BossRushActive = BossRushEvent.GetField("BossRushActive", BindingFlags.Public | BindingFlags.Static);
            
            var c = new ILCursor(il);
            c.GotoNext(i => i.MatchLdsfld(BossRushEvent_BossRushActive));
            c.Index++;
            ILLabel label = null;
            c.GotoNext(i => i.MatchBrfalse(out label));
            c.Index -= 3;
            c.EmitDelegate(() => ModContent.GetInstance<CalamityConfig>().EternityPriorityOverRev && FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode && !(BossRushEvent_BossRushActive.GetValue(null) is bool fard && fard));
            c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label);
            c.Index -= 4;
            var label2 = il.DefineLabel(c.Prev);
            c.GotoPrev(i => i.MatchLdfld(typeof(Projectile), nameof(Projectile.friendly)));
            c.Index++;
            c.Remove();
            c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label2);

            c.GotoPrev(i => i.MatchLdcI4(466));
            c.Index++;
            c.Remove();
            c.Emit(Mono.Cecil.Cil.OpCodes.Bne_Un, label2);

            Type CalamityPlayer = calamity.Code.GetType("CalamityMod.CalPlayer.CalamityPlayer");
            FieldInfo CalamityPlayer_areThereAnyDamnBosses = CalamityPlayer.GetField("areThereAnyDamnBosses", BindingFlags.Public | BindingFlags.Static);
            c.GotoPrev(i => i.MatchLdsfld(CalamityPlayer_areThereAnyDamnBosses));
            c.Index++;
            c.Remove();
            c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label2);

            Type CalamityWorld = calamity.Code.GetType("CalamityMod.World.CalamityWorld");
            FieldInfo CalamityWorld_death = CalamityWorld.GetField("death", BindingFlags.Public | BindingFlags.Static);
            c.GotoPrev(i => i.MatchLdsfld(CalamityWorld_death));
            c.Index++;
            c.Remove();
            c.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, label2);
            //MonoModHooks.DumpIL(mod: Mod, il);

            
        }
        //private static void CalamityProjectileAI_ILEdit(ILContext il)
        //{
        //    //make everlasting rainbow not fuck off forever
        //    var c = new ILCursor(il);
        //    c.GotoNext(i => i.MatchLdcI4(872));
        //    ILLabel label3 = null;
        //    c.GotoNext(i => i.MatchBneUn(out label3));
        //    c.Index -= 3;
        //    c.EmitDelegate(() => ModContent.GetInstance<CalamityConfig>().RevVanillaAIDisabled);
        //    c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label3);
        //    c.Index -= 4;
        //    var label4 = il.DefineLabel(c.Prev);

        //    c.GotoPrev(i => i.MatchLdcI4(95));
        //    c.Index++;
        //    c.Remove();
        //    c.Emit(Mono.Cecil.Cil.OpCodes.Blt, label4);

        //    c.GotoPrev(i => i.MatchLdsfld(typeof(Main), nameof(Main.zenithWorld)));
        //    c.Index++;
        //    c.Remove();
        //    c.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, label4);

        //    c.GotoPrev(i => i.MatchLdcI4(1013));
        //    c.Index++;
        //    c.Remove();
        //    c.Emit(Mono.Cecil.Cil.OpCodes.Bne_Un, label4);
        //    MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasCrossmod>(), il);
        //}
    }
}
