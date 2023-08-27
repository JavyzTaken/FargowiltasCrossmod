using FargowiltasCrossmod.Common.Systems;
using MonoMod.Cil;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity
{
    public class CalamityAIOverride : ModSystem
    {
        public override void Load()
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                Type calamityDetourClass = ModLoader.GetMod("CalamityMod").Code.GetType("CalamityMod.NPCs.CalamityGlobalNPC");
                MethodInfo detourMethod = calamityDetourClass.GetMethod("PreAI", BindingFlags.Public | BindingFlags.Instance);
                MonoModHooks.Modify(detourMethod, CalamityPreAI_ILEdit);
            }
        }
        public static void CalamityPreAI_ILEdit(ILContext il)
        {
            var BossRushEvent = ModLoader.GetMod("CalamityMod").Code.GetType("CalamityMod.Events.BossRushEvent");
            var c = new ILCursor(il);
            //go to correct boss rush check
            c.GotoNext(i => i.MatchLdsfld<CalamityMod.Events.BossRushEvent>("BossRushActive"));
            c.Index++;
            c.GotoNext(i => i.MatchLdsfld<CalamityMod.Events.BossRushEvent>("BossRushActive"));
            c.Index++;
            //get label for skipping past ai changes
            ILLabel label = null;
            c.GotoNext(i => i.MatchBrfalse(out label));
            //go to before checks
            c.Index -= 3;
            //add new check and get label for skipping to it

            c.EmitDelegate(() => ModContent.GetInstance<CalamityConfig>().RevVanillaAIDisabled);
            c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label);
            c.Index -= 4;
            ILLabel label2 = il.DefineLabel(c.Prev);

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
    }
}
