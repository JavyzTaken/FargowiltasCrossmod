using MonoMod.Cil;
using CalamityMod.NPCs;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Events;
using System.Reflection;
using System;
using CalamityMod.Projectiles;

namespace FargowiltasCrossmod.Core.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalamityAIOverride : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod) => ModCompatibility.Calamity.Loaded;

        public override void Load()
        {
            MonoModHooks.Modify(typeof(CalamityGlobalNPC).GetMethod(nameof(CalamityGlobalNPC.PreAI)), CalamityPreAI_ILEdit);
            MonoModHooks.Modify(typeof(CalamityGlobalProjectile).GetMethod(nameof(CalamityGlobalProjectile.PreAI)), CalamityProjectilePreAI_ILEdit);
            //MonoModHooks.Modify(typeof(CalamityGlobalProjectile).GetMethod(nameof(CalamityGlobalProjectile.AI)), CalamityProjectileAI_ILEdit);
        }

        private static void CalamityPreAI_ILEdit(ILContext il)
        {
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
            c.GotoNext(i => i.MatchBrfalse(out label));
            //go to before checks
            c.Index -= 3;
            //add new check and get label for skipping to it

            c.EmitDelegate(() => ModContent.GetInstance<CalamityConfig>().EternityPriorityOverRev);
            c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label);
            c.Index -= 4;
            var label2 = il.DefineLabel(c.Prev);

            //go to checking for queen bee and go to the skipper after it
            c.GotoPrev(i => i.MatchLdcI4(222));
            c.Index++;
            //replace skipper with my own
            c.Remove();
            c.Emit(Mono.Cecil.Cil.OpCodes.Bne_Un, label2);

            //do it again but make the check for zenith seed not skip my check
            c.GotoPrev(i => i.MatchLdsfld(typeof(Main), nameof(Main.zenithWorld)));
            c.Index++;
            c.Remove();
            c.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, label2);
            //MonoModHooks.DumpIL(Instance, il);
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
            c.EmitDelegate(() => ModContent.GetInstance<CalamityConfig>().EternityPriorityOverRev);
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
