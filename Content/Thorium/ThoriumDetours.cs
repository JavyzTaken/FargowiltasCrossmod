using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ID;
using ThoriumMod.Buffs;
using ThoriumMod.Core.Sheaths;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Buffs.Healer;
using ThoriumMod.Projectiles;
using ThoriumMod.Projectiles.Scythe;
using ThoriumMod.Utilities;

namespace FargowiltasCrossmod.Content.Thorium;

[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
public class ThoriumDetours
{
    public static void LoadDetours()
    {
        Assembly ThoriumAssembly = ModLoader.GetMod("ThoriumMod").GetType().Assembly;
        
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

        PlayerHelper_HealLife_Hook = new Hook(
            typeof(PlayerHelper).GetMethod("HealLife", BindingFlags.Static | BindingFlags.Public),
            (PlayerHelper_HealLife_orig orig, Player player, int healAmount, Player healer = null,
                bool healOverMax = true, bool statistics = true) =>
            {
                if (WorldSavingSystem.EternityMode && (healer == null || player.whoAmI == healer.whoAmI))
                {
                    return orig(player, (int)MathF.Ceiling(healAmount / 4f),  healer, healOverMax, statistics);
                }
                return orig(player, healAmount, healer, healOverMax, statistics);
            });
        PlayerHelper_HealLife_Hook.Apply();

        ScytheOfUndoingPro2_OnHitNPC_Hook = new Hook(
            typeof(ScytheofUndoingPro2).GetMethod("OnHitNPC", BindingFlags.Instance | BindingFlags.Public), ScytheOfUndoingPro2_OnHitNPC_Detour);
    }

    public static void UnlaodDetours()
    {
        foreach (Hook hook in SheathData_DamageMultiplier_get_Hooks)
            hook.Undo();
        GardenersSheathData_OnHit_Hook?.Undo();
        PlayerHelper_HealLife_Hook?.Undo();
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

    private static Hook PlayerHelper_HealLife_Hook;
    private delegate int PlayerHelper_HealLife_orig(Player player, int healAmount, Player healer = null, bool healOverMax = true, bool statistics = true);
    
    private static Hook ScytheOfUndoingPro2_OnHitNPC_Hook;

    private delegate void ScytheOfUndoingPro2_OnHitNPC_orig(ScytheofUndoingPro2 self, NPC target, NPC.HitInfo hit, int damageDone);

    private static void ScytheOfUndoingPro2_OnHitNPC_Detour(ScytheOfUndoingPro2_OnHitNPC_orig orig, ScytheofUndoingPro2 self, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!WorldSavingSystem.EternityMode)
        {
            orig(self, target, hit, damageDone);
            return;
        }
        
		Player player = Main.player[self.Projectile.owner];
		if (!target.friendly && target.lifeMax > 5 && target.chaseable && (!target.dontTakeDamage) && !target.immortal)
		{
			if (!player.HasBuff<Buffs.ScytheOfUndoingLifestealCD>())
			{
				player.AddBuff(ModContent.BuffType<Buffs.ScytheOfUndoingLifestealCD>(), 30);
				int heal = 3;
				player.HealLife(heal);
				IEntitySource source_OnHit = self.Projectile.GetSource_OnHit(target);
				Projectile.NewProjectile(source_OnHit, target.Center.X, target.Center.Y, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), ModContent.ProjectileType<VampireScepterPro2>(), 0, 0f, self.Projectile.owner);
			}
			
			int charge = 2;
			ThoriumPlayer thoriumPlayer = player.GetThoriumPlayer();
			if (self.Projectile.ai[0] <= 0f)
			{
				player.AddBuff(ModContent.BuffType<SoulEssence>(), 1800);
				CombatText.NewText(target.Hitbox, new Color(100, 255, 200), charge, dramatic: false, dot: true);
				thoriumPlayer.soulEssence += charge;
				self.Projectile.ai[0] = 30f;
			}
		}
    }
}