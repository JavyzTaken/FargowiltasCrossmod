using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Artemis;
using Luminance.Common.DataStructures;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public static partial class ExoTwinsStates
    {
        /// <summary>
        /// AI update loop method for the Leave state.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        public static void DoBehavior_Leave(NPC npc, IExoTwin twinAttributes)
        {
            if (AITimer <= 5)
            {
                IProjOwnedByBoss<Artemis>.KillAll();
                IProjOwnedByBoss<Apollo>.KillAll();
            }

            npc.dontTakeDamage = true;
            npc.velocity.X *= 0.95f;
            npc.velocity.Y -= 1.32f;
            npc.rotation = npc.rotation.AngleLerp(npc.velocity.ToRotation(), 0.15f);

            if (!npc.WithinRange(Target.Center, 2400f) || Main.LocalPlayer.respawnTimer <= 30)
                npc.active = false;

            twinAttributes.Animation = ExoTwinAnimation.ChargingUp;
            twinAttributes.Frame = twinAttributes.Animation.CalculateFrame(AITimer / 30f % 1f, twinAttributes.InPhase2);
        }
    }
}
