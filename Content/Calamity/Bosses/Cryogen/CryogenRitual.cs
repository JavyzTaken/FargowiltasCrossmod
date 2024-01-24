using CalamityMod.NPCs.Cryogen;
using FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CryogenRitual : BaseArena
    {
        private const float realRotation = -MathHelper.Pi / 180f;

        public override string Texture => "CalamityMod/Projectiles/Boss/IceBomb";
        public CryogenRitual() : base(realRotation, CryogenEternity.ArenaRadius, ModContent.NPCType<CalamityMod.NPCs.Cryogen.Cryogen>(), DustID.SnowflakeIce) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        protected override void Movement(NPC npc)
        {
            Projectile.velocity = npc.Center - Projectile.Center;

            Projectile.velocity /= 40f;

            rotationPerTick = realRotation;
        }

        public override void AI()
        {
            base.AI();
            Projectile.rotation -= 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor * Projectile.Opacity * (targetPlayer == Main.myPlayer ? 1f : 0.15f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Chilled, 60 * 2);
            target.AddBuff(BuffID.Frostburn, 60 * 2);
        }
    }
}