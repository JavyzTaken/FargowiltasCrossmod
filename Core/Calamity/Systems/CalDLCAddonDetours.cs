using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Mono.Cecil.Cil;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using Luminance.Core.Hooking;
using FargowiltasSouls.Content.Items;
using System.Reflection;
using CalamityMod.CalPlayer;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCAddonDetours : ModSystem, ICustomDetourProvider
    {
        //private static readonly MethodInfo AdrenalineEnabled_Method = typeof(CalamityPlayer).GetProperty("AdrenalineEnabled").GetGetMethod();

        //public delegate bool Orig_AdrenalineEnabled(CalamityPlayer self);

        public override void Load()
        {
            On_Player.RefreshDoubleJumps += RefreshDoubleJumps_Detour;
        }

        void ICustomDetourProvider.ModifyMethods()
        {
            //HookHelper.ModifyMethodWithDetour(AdrenalineEnabled_Method, AdrenalineEnabled_Detour);
        }

        private void RefreshDoubleJumps_Detour(On_Player.orig_RefreshDoubleJumps orig, Player self)
        {
            AerospecJumpEffect.ResetAeroCrit(self);
            orig(self);
        }
        /*
         * for some reason, this Kind of works but doesn't have the intended effect at all, 
         * and i don't know exactly how to fix that,
         * so instead i'll just wait for Calamity to update and make this easy to modify
        private bool AdrenalineEnabled_Detour(Orig_AdrenalineEnabled orig, CalamityPlayer self)
        {
            bool value = orig(self);
            if (self.Player.HasEffect<TitanHeartEffect>())
                value = true;
            return value;
        }
        */
    }
}
