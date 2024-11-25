using System.Collections.Generic;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ID;
using ThoriumMod.Buffs;
using ThoriumMod.Core.Sheaths;
using FargowiltasSouls.Core.Systems;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium;

[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
public class ThoriumDetours
{
    public static void LoadDetours()
    {
        SheathData_DamageMultiplier_get_Hooks.Add(new Hook(
            typeof(GardenersSheathData).GetProperty("DamageMultiplier").GetGetMethod(),
            (GardenersSheathData_DamageMultiplier_orig orig, GardenersSheathData self) => WorldSavingSystem.EternityMode ? 6f : orig(self)));
        SheathData_DamageMultiplier_get_Hooks.Add(new Hook(
            typeof(JetstreamSheathData).GetProperty("DamageMultiplier").GetGetMethod(),
            (JetstreamSheathData_DamageMultiplier_orig orig, JetstreamSheathData self) => WorldSavingSystem.EternityMode ? 2.5f : orig(self)));
        SheathData_DamageMultiplier_get_Hooks.Add(new Hook(
            typeof(LeatherSheathData).GetProperty("DamageMultiplier").GetGetMethod(),
            (LeatherSheathData_DamageMultiplier_orig orig, LeatherSheathData self) => WorldSavingSystem.EternityMode ? 6f : orig(self)));
        SheathData_DamageMultiplier_get_Hooks.Add(new Hook(
            typeof(LeechingSheathData).GetProperty("DamageMultiplier").GetGetMethod(),
            (LeechingSheathData_DamageMultiplier_orig orig, LeechingSheathData self) => WorldSavingSystem.EternityMode ? 5f : orig(self)));
        SheathData_DamageMultiplier_get_Hooks.Add(new Hook(
            typeof(TitanSlayerSheathData).GetProperty("DamageMultiplier").GetGetMethod(),
            (TitanSlayerSheathData_DamageMultiplier_orig orig, TitanSlayerSheathData self) => WorldSavingSystem.EternityMode ? 5f : orig(self)));
        SheathData_DamageMultiplier_get_Hooks.Add(new Hook(
            typeof(WrithingSheathData).GetProperty("DamageMultiplier").GetGetMethod(),
            (WrithingData_DamageMultiplier_orig orig, WrithingSheathData self) => WorldSavingSystem.EternityMode ? 4f : orig(self)));
        
        foreach (Hook hook in SheathData_DamageMultiplier_get_Hooks) 
            hook.Apply();

        GardenersSheathData_OnHit_Hook = new Hook(typeof(GardenersSheathData).GetMethod("OnHit", BindingFlags.Instance | BindingFlags.NonPublic),
            (GardenersSheathData_OnHit_orig orig, GardenersSheathData self, Player player, NPC target, NPC.HitInfo hit,
                int damageDone, int hitCount) =>
            {
                if (!WorldSavingSystem.EternityMode) orig(self, player, target, hit, damageDone, hitCount);
                
                if (player.HasBuff(BuffID.ChaosState)) return;

                // 10 second cooldown
                player.AddBuff(BuffID.ChaosState, 1800);
                orig(self, player, target, hit, damageDone, hitCount);
            });
        GardenersSheathData_OnHit_Hook.Apply();
    }

    public static void UnlaodDetours()
    {
        foreach (Hook hook in SheathData_DamageMultiplier_get_Hooks)
            hook.Undo();
        GardenersSheathData_OnHit_Hook?.Undo();
    }
    
    private static List<Hook> SheathData_DamageMultiplier_get_Hooks = new();

    private delegate float GardenersSheathData_DamageMultiplier_orig(GardenersSheathData self);
    private delegate float JetstreamSheathData_DamageMultiplier_orig(JetstreamSheathData self);
    private delegate float LeatherSheathData_DamageMultiplier_orig(LeatherSheathData self);
    private delegate float LeechingSheathData_DamageMultiplier_orig(LeechingSheathData self);
    private delegate float TitanSlayerSheathData_DamageMultiplier_orig(TitanSlayerSheathData self);
    private delegate float WrithingData_DamageMultiplier_orig(WrithingSheathData self);
    
    private static Hook GardenersSheathData_OnHit_Hook;
    private delegate void GardenersSheathData_OnHit_orig(GardenersSheathData self, Player player, NPC target, NPC.HitInfo hit, int damageDone, int hitCount);
    
}