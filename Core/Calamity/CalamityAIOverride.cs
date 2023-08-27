using MonoMod.Cil;
using CalamityMod.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamityAIOverride : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod) => ModCompatibility.Calamity.Loaded;

        public override void Load()
        {
            MonoModHooks.Modify(typeof(CalamityGlobalNPC).GetMethod(nameof(CalamityGlobalNPC.PreAI)), CalamityPreAI_ILEdit);
        }

        private static void CalamityPreAI_ILEdit(ILContext il)
        {
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
    }
}
