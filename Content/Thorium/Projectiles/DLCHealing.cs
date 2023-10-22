using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using System.Reflection;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public static class DLCHealing
    {
        public delegate void CustomHealing(Player player, Player target, ref int heals, ref int selfHeals);
        internal static Type CustomHealingType;
        //public delegate void DLCCustomHealing(Player player, Player target, ref int heals, ref int selfHeals);
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
    }
}
