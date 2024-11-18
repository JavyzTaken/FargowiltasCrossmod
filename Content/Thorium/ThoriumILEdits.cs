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
        
        MonoModHooks.Modify(thoriumProjectileFixClass.GetMethod("HealerOnHitNPC", BindingFlags.Instance | BindingFlags.NonPublic), Projectiles.DLCHealing.LifeStealNerf_ILEdit);
        MonoModHooks.Modify(thoriumPlayerClass.GetMethod("OnHitNPCWithProj", BindingFlags.Instance | BindingFlags.Public), ThoriumILEdits.ShinobiSigilCooldown_ILEdit);
        MonoModHooks.Modify(thoriumPlayerClass.GetMethod("ProcessTriggers", BindingFlags.Instance | BindingFlags.Public), ThoriumILEdits.AbyssalShellNerf_ILEdit);
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

    public static void AbyssalShellNerf_ILEdit(ILContext il)
    {
        ILCursor c = new(il);

        c.GotoNext(instr => instr.MatchCall(typeof(ModContent).GetMethod("BuffType", new Type[] { })
            .MakeGenericMethod(typeof(AbyssalWeight))));
        c.GotoNext(instr => instr.MatchLdcI4(1200));
        c.Remove();
        c.Emit(OpCodes.Ldc_I4, 3600);
        
        c.GotoNext(instr => instr.MatchCall(typeof(ModContent).GetMethod("BuffType", new Type[] { })
            .MakeGenericMethod(typeof(AbyssalShellBuff))));
        c.GotoNext(instr => instr.MatchLdcI4(90000000));
        c.Remove();
        c.Emit(OpCodes.Ldc_I4, 600);
    }

    public static void SpearTipNerf_ILEdit(ILContext il)
    {
        ILCursor c = new(il);
        
        c.GotoNext(instruction => instruction.MatchCall(typeof(ModContent).GetMethod("ProjectileType", new Type[] {}).MakeGenericMethod(typeof(SpearExtra))));
        c.GotoNext(instruction => instruction.MatchLdcR4(0.5f));
        c.Remove();
        c.Emit(OpCodes.Ldc_R4, 0.25);
        
        c.GotoNext(instruction => instruction.MatchCall(typeof(ModContent).GetMethod("ProjectileType", new Type[] {}).MakeGenericMethod(typeof(SpearExtraFlame))));
        c.GotoNext(instruction => instruction.MatchLdcR4(0.75f));
        c.Remove();
        c.Emit(OpCodes.Ldc_R4, 0.5);
        
        c.GotoNext(instruction => instruction.MatchCall(typeof(ModContent).GetMethod("ProjectileType", new Type[] {}).MakeGenericMethod(typeof(SpearExtraCrystal))));
        c.GotoNext();
        c.Emit(OpCodes.Conv_R4);
        c.Emit(OpCodes.Ldc_R4, 0.5f);
        c.Emit(OpCodes.Mul);
        c.Emit(OpCodes.Conv_I4);
    }
}