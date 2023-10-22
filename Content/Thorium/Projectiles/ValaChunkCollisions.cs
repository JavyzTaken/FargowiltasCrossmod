using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    internal static class ValaChunkCollisions
    {
        private static Dictionary<int, Action<Projectile, Projectile>> Collisions = new()
        {
            { 11, MergeWith }, { 12, SplitKill }, { 13, MergeWith },
            { 21, BounceOff }, { 22, MergeWith }, { 23, SplitKill },
            { 31, BounceOff }, { 32, SplitKill }, { 33, Annihilate },
        };

        private const float AnnihilationVelocityMin = 64;
        private const float CollisionVelocityMin = 16;
        internal static void Collide(Projectile ChunkA, Projectile ChunkB)
        {
            int SizeA = (int)ChunkA.ai[0];
            int SizeB = (int)ChunkB.ai[0];
            float RelativeVelocitySQ = (ChunkA.velocity - ChunkB.velocity).LengthSquared();

            if (RelativeVelocitySQ < CollisionVelocityMin || SizeA == 3 && SizeB == 3 && RelativeVelocitySQ < AnnihilationVelocityMin)
            {
                BounceOff(ChunkA, ChunkB);
                return;
            }

            if (SizeA == SizeB)
            {
                Collisions[SizeA * 11](ChunkA, ChunkB);
                return;
            }

            float VelA = ChunkA.velocity.LengthSquared();
            float VelB = ChunkB.velocity.LengthSquared();

            if (VelA >= VelB) // A hits B
            {
                Collisions[SizeA * 10 + SizeB](ChunkA, ChunkB);
            }
            else if (VelA < VelB) // B hits A
            {
                Collisions[SizeB * 10 + SizeA](ChunkB, ChunkA);
            }
        }

        private static void SplitKill(Projectile ChunkA, Projectile ChunkB)
        {

        }

        private static void BounceOff(Projectile ChunkA, Projectile ChunkB)
        {

        }

        private static void MergeWith(Projectile ChunkA, Projectile ChunkB)
        {

        }

        private static void Annihilate(Projectile ChunkA, Projectile ChunkB)
        {

        }
    }
}
