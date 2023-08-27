using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace FargowiltasCrossmod;

public class FargowiltasCrossmod : Mod
{
    private class ClassPreJitFilter : PreJITFilter
    {
        public override bool ShouldJIT(MemberInfo member)
        {
            return base.ShouldJIT(member) &&
                   // also check attributes on declaring class
                   member.DeclaringType?.GetCustomAttributes<MemberJitAttribute>().All(a => a.ShouldJIT(member)) != false;
        }
    }

    public FargowiltasCrossmod()
    {
        PreJITFilter = new ClassPreJitFilter();
    }
}