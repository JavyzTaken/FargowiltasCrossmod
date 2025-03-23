using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Luminance.Core.Hooking;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.WrathoftheGods.Name)]
    [JITWhenModsEnabled(ModCompatibility.WrathoftheGods.Name)]
    public class WrathDLCDetours : ModSystem, ICustomDetourProvider
    {

        public delegate bool Orig_KillOldDukeWrapper(object self, NPC npc);

        public override void Load()
        {
            
        }
        void ICustomDetourProvider.ModifyMethods()
        {
            MethodInfo killOldDukeWrapper_Method = ModCompatibility.WrathoftheGods.Mod.Code.GetType("FUCKYOUOLDDUKESystem").GetMethod("KillOldDukeWrapper", LumUtils.UniversalBindingFlags);
            HookHelper.ModifyMethodWithDetour(killOldDukeWrapper_Method, KillOldDukeWrapper_Detour);
        }
        internal static bool KillOldDukeWrapper_Detour(Orig_KillOldDukeWrapper orig, object self, NPC npc)
        {
            if (CalDLCWorldSavingSystem.E_EternityRev)
                return true;
            return orig(self, npc);
        }
    }
}