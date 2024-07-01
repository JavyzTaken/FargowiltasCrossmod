using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExothermalLaserDashes : ExoMechComboHandler
    {
        /// <summary>
        /// The spin angle at which the Exo Twins should orient themselves in accordance with.
        /// </summary>
        public static float ExoTwinSpinAngle
        {
            get
            {
                if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechWorm))
                    return 0f;

                NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];
                return hades.ai[0];
            }
            set
            {
                if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechWorm))
                    return;

                NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];
                hades.ai[0] = value;
            }
        }

        /// <summary>
        /// How long the Exo Twins spend redirecting before firing their laserbeams.
        /// </summary>
        public static int RedirectTime => LumUtils.SecondsToFrames(1.5f);

        /// <summary>
        /// How long it takes for the Exo Twins to spin at full angular velocity.
        /// </summary>
        public static int ExoTwinSpinWindUpTime => LumUtils.SecondsToFrames(1.25f);

        /// <summary>
        /// How much damage blazing exo laserbeams from the Exo Twins do.
        /// </summary>
        public static int BlazingLaserbeamDamage => Main.expertMode ? 550 : 400;

        /// <summary>
        /// How far the Exo Twins should be away from Hades' head when spinning.
        /// </summary>
        public static float ExoTwinSpinRadius => 408f;

        /// <summary>
        /// The angular velocity of the Exo Twins.
        /// </summary>
        public static float ExoTwinSpinAngularVelocity => MathHelper.ToRadians(2f);

        public override int[] ExpectedManagingExoMechs => [ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<Apollo>()];

        public override bool Perform(NPC npc)
        {
            if (npc.type == ExoMechNPCIDs.HadesHeadID)
            {
                Perform_Hades(npc);

                // This is executed by Hades since unless the Exo Twins there is only one instance of him, and as such he can be counted on for
                // storing and executing attack data.
                // Furthermore, parts of the state rely on him specifically, particularly the part that specifies what value the spin angle should begin at, which is related to Hades' position relative to the target.
                HandleAttackState(npc);
            }
            if (npc.type == ExoMechNPCIDs.ArtemisID || npc.type == ExoMechNPCIDs.ApolloID)
                Perform_ExoTwin(npc);

            return AITimer >= RedirectTime + BlazingExoLaserbeam.Lifetime;
        }

        /// <summary>
        /// Performs Hades' part in the ExothermalLaserDashes attack.
        /// </summary>
        /// <param name="npc">Hades' NPC instance.</param>
        public static void Perform_Hades(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out HadesHeadEternity hades))
                return;

            float angularVelocity = LumUtils.InverseLerp(200f, 500f, npc.Distance(Target.Center)) * MathHelper.ToRadians(2f);
            float idealRotation = npc.AngleTo(Target.Center) + MathF.Cos(MathHelper.TwoPi * AITimer / 90f) * 0.34f;
            float idealSpeed = Utils.Remap(npc.Distance(Target.Center), 200f, 450f, 13.75f, 25f);
            npc.Center = Vector2.Lerp(npc.Center, Target.Center, 0.007f);
            npc.velocity = npc.velocity.RotateTowards(idealRotation, angularVelocity);
            npc.velocity = npc.velocity.SafeNormalize(Vector2.UnitY) * MathHelper.Lerp(npc.velocity.Length(), idealSpeed, 0.15f);

            hades.BodyBehaviorAction = new(HadesHeadEternity.EveryNthSegment(3), HadesHeadEternity.OpenSegment(HadesHeadEternity.StandardSegmentOpenRate, 0f));

            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
        }

        /// <summary>
        /// Performs The Exo Twins' part in the ExothermalLaserDashes attack.
        /// </summary>
        /// <param name="npc">The Exo Twins' NPC instance.</param>
        public static void Perform_ExoTwin(NPC npc)
        {
            if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechWorm))
                return;

            NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];

            float hoverOffsetAngle = ExoTwinSpinAngle;
            float hoverFlySpeedInterpolant = LumUtils.InverseLerp(0f, RedirectTime * 0.9f, AITimer);
            if (npc.type == ExoMechNPCIDs.ApolloID)
                hoverOffsetAngle += MathHelper.Pi;

            Vector2 hoverDestination = hades.Center + hoverOffsetAngle.ToRotationVector2() * ExoTwinSpinRadius;
            if (hoverDestination.Y < 300f)
                hoverDestination.Y = 300f;

            npc.SmoothFlyNear(hoverDestination, hoverFlySpeedInterpolant * 0.21f, 1f - hoverFlySpeedInterpolant * 0.175f);

            npc.rotation = npc.AngleFrom(hades.Center).AngleLerp(hoverOffsetAngle, hoverFlySpeedInterpolant);

            if (AITimer == RedirectTime + 1)
            {
                ScreenShakeSystem.StartShake(9.5f);

                // TODO -- Play a laser shoot sound.
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), npc.Center, npc.rotation.ToRotationVector2(), ModContent.ProjectileType<BlazingExoLaserbeam>(), BlazingLaserbeamDamage, 0f, -1, npc.whoAmI);
            }

            if (npc.TryGetDLCBehavior(out CalDLCEmodeBehavior behavior) && behavior is IExoTwin twin)
            {
                twin.Animation = ExoTwinAnimation.ChargingUp;
                twin.Frame = twin.Animation.CalculateFrame(AITimer / 40f % 1f, twin.InPhase2);
                twin.OpticNerveAngleSensitivity = MathHelper.Lerp(-1.6f, -4f, LumUtils.Cos01(MathHelper.TwoPi * AITimer / 54f + npc.whoAmI * 4f));
            }
        }

        /// <summary>
        /// Handles general purpose state variables for the ExothermalLaserDashes attack.
        /// </summary>
        /// <param name="hades">Hades' NPC instance.</param>
        public static void HandleAttackState(NPC hades)
        {
            // Make the spin angle perpendicular to the target, to ensure that they don't get telefragged by the lasers.
            // TODO -- This probably will cause telefrags in multiplayer, however. Do we care?
            if (AITimer <= RedirectTime)
                ExoTwinSpinAngle = hades.AngleTo(Target.Center) + MathHelper.PiOver2;

            float spinWindUpInterpolant = LumUtils.InverseLerp(0f, ExoTwinSpinWindUpTime, AITimer - RedirectTime);
            ExoTwinSpinAngle += MathHelper.SmoothStep(0f, ExoTwinSpinAngularVelocity, spinWindUpInterpolant);
        }
    }
}
