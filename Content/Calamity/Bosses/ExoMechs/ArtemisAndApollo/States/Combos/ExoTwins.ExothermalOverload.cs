using CalamityMod.NPCs.ExoMechs.Artemis;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core.Calamity;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public static partial class ExoTwinsStates
    {
        /// <summary>
        /// AI update loop method for the ExothermalOverload attack.
        /// </summary>
        /// <param name="npc">The Twins' NPC instance.</param>
        /// <param name="twinAttributes">The Twins' designated generic attributes.</param>
        public static void DoBehavior_ExothermalOverload(NPC npc, IExoTwin twinAttributes)
        {
            if (npc.type == ExoMechNPCIDs.ArtemisID)
                DoBehavior_ExothermalOverload_ArtemisDeathrays(npc, twinAttributes);
            else
                DoBehavior_ExothermalOverload_ApolloHaloPlasmaDashes(npc, twinAttributes);
        }

        /// <summary>
        /// AI update loop method for the ExothermalOverload attack for Artemis specifically.
        /// </summary>
        /// <param name="npc">Artemis' NPC instance.</param>
        /// <param name="artemisAttributes">Artemis' designated generic attributes.</param>
        public static void DoBehavior_ExothermalOverload_ArtemisDeathrays(NPC npc, IExoTwin artemisAttributes)
        {
        }

        /// <summary>
        /// AI update loop method for the ExothermalOverload attack for Apollo specifically.
        /// </summary>
        /// <param name="npc">Apollo's NPC instance.</param>
        /// <param name="apolloAttributes">Apollo's designated generic attributes.</param>
        public static void DoBehavior_ExothermalOverload_ApolloHaloPlasmaDashes(NPC npc, IExoTwin apolloAttributes)
        {
            int spinTime = 45;
            int dashDelay = 10;
            int dashRepositionTime = 5;
            int dashTime = 26;
            ref float spinOffsetAngle = ref SharedState.Values[0];
            ref float spinRadius = ref SharedState.Values[1];

            if (AITimer == 1)
            {
                float snapAngle = MathHelper.TwoPi / 4f;
                spinOffsetAngle = MathF.Round(Utilities.WrapAngle360(npc.AngleFrom(Target.Center) + MathHelper.PiOver2) / snapAngle) * snapAngle;

                spinRadius = 200f;
                npc.netUpdate = true;
            }

            // ∫(0, 1) sin(pi * x^0.7)^13 dx. There's almost certainly some a/(b * pi) ratio that this corresponds to but it's easier asking the computer for a numerical approximation.
            float areaUnderBump = 0.20183f;

            float spinArc = MathHelper.PiOver2;
            float spinBump = MathF.Pow(Utilities.Convert01To010(MathF.Pow(Utilities.InverseLerp(0f, spinTime, AITimer), 0.7f)), 13f);
            spinOffsetAngle += spinBump / areaUnderBump * spinArc / spinTime;

            // Spin around the player.
            if (AITimer <= spinTime)
            {
                if (AITimer <= spinTime - 14)
                {
                    float hoverSpeedInterpolant = Utilities.InverseLerp(0f, 15f, AITimer);
                    Vector2 hoverDestination = Target.Center + spinOffsetAngle.ToRotationVector2() * spinRadius;
                    npc.SmoothFlyNear(hoverDestination, hoverSpeedInterpolant * 0.2f, 1f - hoverSpeedInterpolant * 0.32f);
                }
                else
                    npc.velocity *= 0.7f;

                if (AITimer == 12)
                    SoundEngine.PlaySound(Artemis.ChargeTelegraphSound, Target.Center);

                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.5f);

                spinRadius += 14.5f;
            }

            // Slow down before dashing.
            else if (AITimer <= spinTime + dashDelay)
            {
                npc.velocity *= 0.8f;
                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.5f);
            }

            // Begin the dash, repositioning the direction over the course of a few frames.
            else if (AITimer <= spinTime + dashDelay + dashRepositionTime)
            {
                npc.GetDLCBehavior<ApolloEternity>().FlameEngulfInterpolant = Utilities.Saturate(npc.GetDLCBehavior<ApolloEternity>().FlameEngulfInterpolant + 0.2f);

                npc.velocity = Vector2.Lerp(npc.velocity, npc.SafeDirectionTo(Target.Center) * 75f, 0.2f);
                if (AITimer == spinTime + dashDelay + 1)
                {
                    npc.velocity += npc.SafeDirectionTo(Target.Center) * 41f;
                    SoundEngine.PlaySound(Artemis.ChargeSound, Target.Center);
                    ScreenShakeSystem.StartShake(15f, shakeStrengthDissipationIncrement: 1.02f);
                }

                if (npc.velocity.Length() >= 8f)
                    npc.rotation = npc.rotation.AngleLerp(npc.velocity.ToRotation(), 0.2f);
            }

            // Accelerate and handle post-dash behavoirs.
            else if (AITimer <= spinTime + dashDelay + dashRepositionTime + dashTime)
            {
                npc.GetDLCBehavior<ApolloEternity>().FlameEngulfInterpolant = 1f;

                npc.velocity *= 1.04f;
                npc.rotation = npc.rotation.AngleLerp(npc.velocity.ToRotation(), 0.4f);
                npc.damage = npc.defDamage;
            }

            // Teleport back around after the dash.
            if (AITimer >= spinTime + dashDelay + dashRepositionTime + dashTime)
            {
                npc.Center = Target.Center - Target.SafeDirectionTo(npc.Center) * 1600f;
                npc.velocity *= 0.1f;
                npc.oldPos = new Vector2[npc.oldPos.Length];

                AITimer = 0;
            }

            apolloAttributes.Animation = ExoTwinAnimation.Attacking;
            apolloAttributes.Frame = apolloAttributes.Animation.CalculateFrame(AITimer / 30f % 1f, apolloAttributes.InPhase2);

            if (npc.GetDLCBehavior<ApolloEternity>().FlameEngulfInterpolant >= 0.67f)
                DoBehavior_ExothermalOverload_ReleasePlasmaFireParticles(npc);
        }

        /// <summary>
        /// Releases plasma fire particles from Apollo as he flies.
        /// </summary>
        public static void DoBehavior_ExothermalOverload_ReleasePlasmaFireParticles(NPC npc)
        {
            for (int i = 0; i < 7; i++)
            {
                Vector2 fireSpawnPosition = npc.Center + (npc.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2).ToRotationVector2() * 60f + Main.rand.NextVector2Circular(24f, 24f);
                Vector2 fireVelocity = -npc.rotation.ToRotationVector2() * Main.rand.NextFloat(7.5f, 27.5f) + Main.rand.NextVector2Circular(14f, 14f);
                Color fireGlowColor = Utilities.MulticolorLerp(Main.rand.NextFloat(), Color.Yellow, Color.YellowGreen, Color.Lime) * Main.rand.NextFloat(0.42f, 0.68f);
                Vector2 fireGlowScaleFactor = Vector2.One * Main.rand.NextFloat(0.12f, 0.24f);
                BloomPixelParticle fire = new(fireSpawnPosition, fireVelocity, Color.White, fireGlowColor, Main.rand.Next(11, 30), Vector2.One * 2f, null, fireGlowScaleFactor);
                fire.Spawn();
            }

            for (int i = 0; i < 3; i++)
            {
                float smokeSizeInterpolant = Main.rand.NextFloat();
                float smokeScale = MathHelper.Lerp(0.7f, 1.45f, smokeSizeInterpolant);
                Color smokeColor = Color.Lerp(Color.DarkSlateGray, Color.LightGray, Main.rand.NextFloat()) * MathHelper.Lerp(1f, 0.2f, smokeSizeInterpolant);
                Vector2 smokeVelocity = -npc.rotation.ToRotationVector2() * Main.rand.NextFloat(7.5f, 27.5f) + Main.rand.NextVector2Circular(14f, 14f);
                SmokeParticle smoke = new(npc.Center, smokeVelocity, smokeColor, 25, smokeScale, 0.04f);
                smoke.Spawn();
            }
        }
    }
}
