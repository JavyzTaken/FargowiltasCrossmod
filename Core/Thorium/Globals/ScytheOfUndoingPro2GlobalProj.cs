using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Buffs.Healer;
using ThoriumMod.Projectiles;
using ThoriumMod.Projectiles.Scythe;
using ThoriumMod.Utilities;

namespace FargowiltasCrossmod.Core.Thorium.Globals;

[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
public class ScytheOfUndoingPro2GlobalProj : GlobalProjectile
{
	public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
	{
		return entity.type == ModContent.ProjectileType<ScytheofUndoingPro2>();
	}

	public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
		Player player = Main.player[projectile.owner];
		if (!target.friendly && target.lifeMax > 5 && target.chaseable && (!target.dontTakeDamage) && !target.immortal)
		{
			if (!player.HasBuff<Content.Thorium.Buffs.ScytheOfUndoingLifestealCD>())
			{
				player.AddBuff(ModContent.BuffType<Content.Thorium.Buffs.ScytheOfUndoingLifestealCD>(), 30);
				int heal = 3;
				player.HealLife(heal);
				IEntitySource source_OnHit = projectile.GetSource_OnHit(target);
				Projectile.NewProjectile(source_OnHit, target.Center.X, target.Center.Y, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), ModContent.ProjectileType<VampireScepterPro2>(), 0, 0f, projectile.owner);
			}
			
			int charge = 2;
			ThoriumPlayer thoriumPlayer = player.GetThoriumPlayer();
			if (projectile.ai[0] <= 0f)
			{
				player.AddBuff(ModContent.BuffType<SoulEssence>(), 1800);
				CombatText.NewText(target.Hitbox, new Color(100, 255, 200), charge, dramatic: false, dot: true);
				thoriumPlayer.soulEssence += charge;
				projectile.ai[0] = 30f;
			}
		}
    }
}