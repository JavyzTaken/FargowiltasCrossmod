using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades
{
    public sealed partial class HadesHeadEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// AI update loop method for the leaving state.
        /// </summary>
        public void DoBehavior_Leave()
        {
            if (AITimer <= 5)
                IProjOwnedByBoss<ThanatosHead>.KillAll();

            BodyBehaviorAction = new(AllSegments(), CloseSegment());

            Vector2 idealVelocity = new(NPC.SafeDirectionTo(Target.Center).X * -85f, 70f);
            NPC.velocity = Vector2.Lerp(NPC.velocity, idealVelocity, 0.095f);

            // This is necessary to ensure that the map icon goes away.
            NPC.As<ThanatosHead>().SecondaryAIState = (int)ThanatosHead.SecondaryPhase.PassiveAndImmune;

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            if (!NPC.WithinRange(Target.Center, 5400f) || Main.LocalPlayer.respawnTimer <= 30)
                NPC.active = false;
        }
    }
}
