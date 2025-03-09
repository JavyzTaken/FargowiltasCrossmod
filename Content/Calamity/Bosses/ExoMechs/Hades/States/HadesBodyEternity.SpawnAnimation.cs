using FargowiltasCrossmod.Core.Calamity.Globals;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades
{
    public sealed partial class HadesHeadEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// Whether Hades is within ground during his Spawn Animation state.
        /// </summary>
        public bool SpawnAnimation_InGround
        {
            get => NPC.ai[0] == 1f;
            set => NPC.ai[0] = value.ToInt();
        }

        /// AI update loop method for the Spawn Animation state.
        /// </summary>
        public void DoBehavior_SpawnAnimation()
        {
            BodyBehaviorAction = new(AllSegments(), CloseSegment());

            if (NPC.velocity.Y <= 120f)
                NPC.velocity.Y += 4f;

            DoBehavior_SpawnAnimation_HandleGroundCollision();

            if (!SpawnAnimation_InGround)
                AITimer = 0;
            else if (MathF.Abs(NPC.velocity.Y) >= 25f)
                NPC.velocity *= 0.9f;

            if (AITimer >= 60)
                SelectNewState();

            NPC.dontTakeDamage = true;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            NPC.damage = 0;
        }

        /// <summary>
        /// Handles ground collision effects during Hades' Spawn Animation state.
        /// </summary>
        public void DoBehavior_SpawnAnimation_HandleGroundCollision()
        {
            bool inGround = Collision.SolidCollision(NPC.TopLeft, NPC.width, NPC.height);
            bool whyAreYouUsingAWorldArena = NPC.Center.Y >= Main.worldSurface * 16f;
            bool shouldSlowDown = inGround || whyAreYouUsingAWorldArena;
            if (!SpawnAnimation_InGround && shouldSlowDown)
            {
                SpawnAnimation_InGround = true;
                NPC.netUpdate = true;
                CreateGroundImpactVisuals();
            }
        }
    }
}
