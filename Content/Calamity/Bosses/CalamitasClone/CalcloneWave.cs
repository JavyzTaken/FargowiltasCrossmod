using System;
using System.IO;
using FargowiltasCrossmod.Content.Common.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Assets.ExtraTextures;
using Luminance.Assets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalcloneWave : BaseWaveExplosionProjectile
    {
        public override int Lifetime => 80;

        public override float MaxRadius => 1500f;

        public override float RadiusExpandRateInterpolant => 0.15f;

        public override string Texture => "FargowiltasSouls/Common/Graphics/Particles/ParticleTextures/SparkParticle";

        public override float DetermineScreenShakePower(float lifetimeCompletionRatio, float distanceFromPlayer)
        {
            float baseShakePower = MathHelper.Lerp(1f, 5f, MathF.Sin(MathHelper.Pi * lifetimeCompletionRatio));
            return baseShakePower * Utils.GetLerpValue(2200f, 1050f, distanceFromPlayer, true);
        }

        public override Color DetermineExplosionColor(float lifetimeCompletionRatio)
        {
            return Color.Lerp(Color.Lerp(Color.Red, Color.Magenta, 0.5f), Color.Gray, MathHelper.Clamp(lifetimeCompletionRatio * 1.5f, 0f, 1f));
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write((int)Projectile.localAI[1]);

        public override void ReceiveExtraAI(BinaryReader reader) => Projectile.localAI[1] = reader.ReadInt32();
    }
}
