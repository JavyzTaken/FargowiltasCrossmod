using System;
using System.Reflection;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;

namespace FargowiltasCrossmod.Content.Thorium;

public class ThoriumILEdits
{
    public static void ShinobiSigilCooldown_ILEdit(ILContext il)
    {
        FieldInfo accShinobiSigilField = typeof(ThoriumMod.ThoriumPlayer).GetField("accShinobiSigil", BindingFlags.Instance | BindingFlags.Public);
        FieldInfo accThiefsWalletField = typeof(ThoriumMod.ThoriumPlayer).GetField("accThiefsWallet", BindingFlags.Instance | BindingFlags.Public);
        
        ILCursor c = new(il);
        var label = c.DefineLabel();
        c.GotoNext(instr => instr.MatchLdfld(accShinobiSigilField));
        c.Index += 2;

        c.Emit(OpCodes.Ldarg_0);
        // c.EmitDelegate<Func<Player, bool>>((player) => player.HasBuff<Buffs.ShinobiSigilCD>());
        c.Emit(OpCodes.Call,
            typeof(Terraria.ModLoader.ModPlayer).GetMethod("get_Player", BindingFlags.Instance | BindingFlags.Public));
        c.Emit(OpCodes.Callvirt,
            typeof(Terraria.Player).GetMethod("HasBuff", new Type[] {}).MakeGenericMethod(typeof(ShinobiSigilCD)));
        c.Emit(OpCodes.Brtrue, label);
        
        c.GotoNext(instr => instr.MatchLdfld(accThiefsWalletField));
        c.Index -= 1;
        c.MarkLabel(label);
    }
}