using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SmallHadesSegmentExplosion : ModProjectile, IExoMechProjectile, IProjOwnedByBoss<ThanatosHead>
    {
        public ExoMechDamageSource DamageType => ExoMechDamageSource.Internal;

        /// <summary>
        /// The general purpose frame timer of this sphere.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        /// How long this sphere should exist for, in frames.
        /// </summary>
        public static int Lifetime => Utilities.SecondsToFrames(0.3f);

        public override void SetStaticDefaults() => Main.projFrames[Type] = 7;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.scale = Main.rand?.NextFloat(2.2f, 2.75f) ?? 2.2f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (Time == 1f)
                SoundEngine.PlaySound(AresTeslaCannon.TeslaOrbShootSound with { MaxInstances = 0, Volume = 0.5f }, Projectile.Center);

            Projectile.frame = (int)MathF.Round(Time / Lifetime * (Main.projFrames[Type] - 1f));
            Time++;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
    }
}
