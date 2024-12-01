using CalamityMod.NPCs.ExoMechs.Artemis;
using Luminance.Common.Utilities;
using Terraria;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public static partial class ExoTwinsStates
    {
        /// <summary>
        /// AI update loop method for the SimpleLoopDashes attack.
        /// </summary>
        /// <param name="npc">Apollo's NPC instance.</param>
        /// <param name="apolloAttributes">Apollo's designated generic attributes.</param>
        /// <param name="localAITimer">Apollo's local AI timer.</param>
        public static void DoBehavior_SimpleLoopDashes(NPC npc, IExoTwin apolloAttributes, ref int localAITimer)
        {
            if (!npc.WithinRange(Target.Center, 1200f))
                npc.SmoothFlyNearWithSlowdownRadius(Target.Center, 0.05f, 0.85f, 900f);

            if (npc.WithinRange(Target.Center, 400f))
            {
                if (npc.velocity.AngleBetween(npc.SafeDirectionTo(Target.Center)) < 0.4f)
                {
                    if (npc.soundDelay <= 0)
                    {
                        SoundEngine.PlaySound(Artemis.ChargeSound, npc.Center);
                        npc.soundDelay = 60;
                    }

                    npc.velocity *= 1.15f;
                }

                if (npc.velocity.Length() <= 2f)
                    npc.velocity.Y -= 20f;
            }
            else
            {
                float idealDirection = npc.AngleTo(Target.Center);
                npc.velocity += idealDirection.ToRotationVector2() * 1.015f;
                if (npc.velocity.Length() > 20.5f)
                    npc.velocity *= 0.98f;
                if (npc.velocity.Length() > 34f)
                    npc.velocity *= 0.97f;
                if (npc.velocity.Length() > 50f)
                    npc.velocity *= 0.97f;
            }

            npc.rotation = npc.velocity.ToRotation();

            if (localAITimer >= 60)
                npc.damage = npc.defDamage;

            apolloAttributes.WingtipVorticesOpacity = Utilities.InverseLerp(16f, 32f, npc.velocity.Length());
            apolloAttributes.ThrusterBoost = Utilities.InverseLerp(20f, 30f, npc.velocity.Length());
            apolloAttributes.Animation = ExoTwinAnimation.Attacking;
            apolloAttributes.Frame = apolloAttributes.Animation.CalculateFrame(localAITimer / 40f % 1f, apolloAttributes.InPhase2);
        }
    }
}
