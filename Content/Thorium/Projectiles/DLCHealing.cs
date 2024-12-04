using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Cil;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public static class DLCHealing
    {
        public delegate void CustomHealing(Player player, Player target, ref int heals, ref int selfHeals);
        public static Type CustomHealingType;

        internal static MethodInfo HealMethod;
        public static void DLCHeal(this Projectile projectile,
                                    int healAmount,
                                    float radius = 30f,
                                    bool onHealEffects = true,
                                    bool bonusHealing = true,
                                    CustomHealing customHealing = null,
                                    Func<Player, bool> canHealPlayer = null,
                                    int specificPlayer = -1,
                                    bool ignoreHealer = true,
                                    bool ignoreSetTarget = false,
                                    bool statistics = true)
        {
            HealMethod.Invoke(null, new object[] { projectile, healAmount, radius, onHealEffects, bonusHealing, customHealing, canHealPlayer, specificPlayer, ignoreHealer, ignoreSetTarget, statistics });
        }



        public static void DLCOnHealEffects(Player healer, Player target, int heals)
        {
            // Main.NewText("OnHeal");
        }
    }
}
