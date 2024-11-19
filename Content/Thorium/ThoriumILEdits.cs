using System;
using System.Reflection;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Buffs;
using ThoriumMod.Projectiles;

namespace FargowiltasCrossmod.Content.Thorium;

public static class ThoriumILEdits
{
    internal static void ApplyILEdits()
    {
        Assembly ThoriumAssembly = ModLoader.GetMod("ThoriumMod").GetType().Assembly;
            
        Type thoriumProjectileFixClass = ThoriumAssembly.GetType("ThoriumMod.Projectiles.ThoriumProjectileFix", true);
        Type thoriumPlayerClass = ThoriumAssembly.GetType("ThoriumMod.ThoriumPlayer", true);
        Type thoriumGlobalItemClass = ThoriumAssembly.GetType("ThoriumMod.Items.ThoriumGlobalItem", true);
        
        MonoModHooks.Modify(thoriumProjectileFixClass.GetMethod("HealerOnHitNPC", BindingFlags.Instance | BindingFlags.NonPublic), Projectiles.DLCHealing.LifeStealNerf_ILEdit);
        MonoModHooks.Modify(thoriumPlayerClass.GetMethod("OnHitNPCWithProj", BindingFlags.Instance | BindingFlags.Public), ThoriumILEdits.ShinobiSigilCooldown_ILEdit);
        MonoModHooks.Modify(thoriumPlayerClass.GetMethod("ProcessTriggers", BindingFlags.Instance | BindingFlags.Public), ThoriumILEdits.AbyssalShellNerf_ILEdit);
        // MonoModHooks.Modify(thoriumGlobalItemClass.GetMethod("Shoot", BindingFlags.Instance | BindingFlags.Public), ThoriumILEdits.SpearTipNerf_ILEdit);
    }
    
    public static void ShinobiSigilCooldown_ILEdit(ILContext il)
    {
        FieldInfo accShinobiSigilField = typeof(ThoriumMod.ThoriumPlayer).GetField("accShinobiSigil", BindingFlags.Instance | BindingFlags.Public);
        FieldInfo accThiefsWalletField = typeof(ThoriumMod.ThoriumPlayer).GetField("accThiefsWallet", BindingFlags.Instance | BindingFlags.Public);
        
        ILCursor c = new(il);
        var label = c.DefineLabel();
        c.GotoNext(instr => instr.MatchLdfld(accShinobiSigilField));
        c.Index += 2;

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Call,
            typeof(Terraria.ModLoader.ModPlayer).GetMethod("get_Player", BindingFlags.Instance | BindingFlags.Public));
        c.Emit(OpCodes.Callvirt,
            typeof(Terraria.Player).GetMethod("HasBuff", new Type[] {}).MakeGenericMethod(typeof(ShinobiSigilCD)));
        c.Emit(OpCodes.Call,
            typeof(FargowiltasSouls.Core.Systems.WorldSavingSystem).GetProperty("EternityMode", BindingFlags.Static | BindingFlags.Public).GetGetMethod());
        c.Emit(OpCodes.And);
        c.Emit(OpCodes.Brtrue, label);
        
        c.GotoNext(instr => instr.MatchLdfld(accThiefsWalletField));
        c.Index -= 1;
        c.MarkLabel(label);
    }

    public static void AbyssalShellNerf_ILEdit(ILContext il)
    {
        ILCursor c = new(il);
        
        var label1 = c.DefineLabel();
        var label2 = c.DefineLabel();
        
        c.GotoNext(instr => instr.MatchCall(typeof(ModContent).GetMethod("BuffType", new Type[] { })
            .MakeGenericMethod(typeof(AbyssalWeight))));
        c.GotoNext(instr => instr.MatchLdcI4(1200));
        c.GotoNext();
        // check if emode
        c.Emit(OpCodes.Call,
            typeof(FargowiltasSouls.Core.Systems.WorldSavingSystem).GetProperty("EternityMode", BindingFlags.Static | BindingFlags.Public).GetGetMethod());
        // if not emode, skip to after this
        c.Emit(OpCodes.Brfalse, label1);
        // remove the base thorium time
        c.Emit(OpCodes.Pop);
        // add our own time
        c.Emit(OpCodes.Ldc_I4, 3600);
        c.MarkLabel(label1);
        
        // do the same but for the buff
        c.GotoNext(instr => instr.MatchCall(typeof(ModContent).GetMethod("BuffType", new Type[] { })
            .MakeGenericMethod(typeof(AbyssalShellBuff))));
        c.GotoNext(instr => instr.MatchLdcI4(90000000));
        c.GotoNext();
        c.Emit(OpCodes.Call,
            typeof(FargowiltasSouls.Core.Systems.WorldSavingSystem).GetProperty("EternityMode", BindingFlags.Static | BindingFlags.Public).GetGetMethod());
        c.Emit(OpCodes.Brfalse, label2);
        c.Emit(OpCodes.Pop);
        c.Emit(OpCodes.Ldc_I4, 600);
        c.MarkLabel(label2);
    }
}