using CalamityMod;
using CalamityMod.NPCs.OldDuke;
using FargowiltasCrossmod.Assets;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using CalamityOD = CalamityMod.NPCs.OldDuke.OldDuke;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class OldDukeEternity : CalDLCEmodeBehavior
    {
        public enum OldDukeAIState
        {
            SpawnAnimation,

            BubbleSpin,
            NormalDashes,
            PredictiveDashes,
            ConjureNuclearVortex,
            KamikazeSharks,

            Phase2Transition_Roar,
            Phase2Transition_ConsumeFuelContainers,

            SharkWaveLaunch,
            VomitRadioactiveCinders,
            ConjureNuclearHurricane,

            Leave
        }

        public enum OldDukeAnimation
        {
            IdleAnimation,
            Dash,
            OpenMouth
        }

        /// <summary>
        /// A general-purpose frame counter for the Old Duke.
        /// </summary>
        public int FrameCounter
        {
            get;
            set;
        }

        /// <summary>
        /// The Old Duke's Y frame.
        /// </summary>
        public int FrameY
        {
            get;
            set;
        }

        /// <summary>
        /// The opacity of the Old Duke's eye glow visual.
        /// </summary>
        public float EyeGlowOpacity
        {
            get;
            set;
        }

        /// <summary>
        /// The opacity of the Old Duke's afterimages.
        /// </summary>
        public float AfterimageOpacity
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of radioactive glow that the Old Duke currently has.
        /// </summary>
        public float RadioactiveGlowInterpolant
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of motion blur that the Old Duke should render with.
        /// </summary>
        public float MotionBlurInterpolant
        {
            get;
            set;
        }

        /// <summary>
        /// The Old Duke's current AI state.
        /// </summary>
        public OldDukeAIState CurrentState
        {
            get;
            set;
        }

        /// <summary>
        /// A general-purpose AI timer dedicated towards the Old Duke's states. Is automatically reset when said states naturally shift.
        /// </summary>
        public int AITimer
        {
            get;
            set;
        }

        /// <summary>
        /// The Old Duke's current phase.
        /// </summary>
        public int Phase
        {
            get;
            set;
        } = 1;

        /// <summary>
        /// The index of the attack the Old Duke is performing in his current phase's attack cycle.
        /// </summary>
        public int AttackCycleIndex
        {
            get;
            set;
        }

        /// <summary>
        /// The action that should be performed by the Old Duke's sulphurous sharkrons.
        /// </summary>
        public Action<SulphurousSharkronEternity>? SharkronPuppeteerAction
        {
            get;
            set;
        }

        /// <summary>
        /// The Old Duke's current animation.
        /// </summary>
        public OldDukeAnimation Animation
        {
            get => (OldDukeAnimation)NPC.localAI[0];
            set => NPC.localAI[0] = (int)value;
        }

        /// <summary>
        /// The position of the Old Duke's mouth.
        /// </summary>
        public Vector2 MouthPosition => NPC.Center + new Vector2(NPC.width * 0.5f + 10f, NPC.spriteDirection * 36f).RotatedBy(NPC.rotation);

        /// <summary>
        /// The position of the Old Duke's eye.
        /// </summary>
        public Vector2 EyePosition => NPC.Center + new Vector2(NPC.width * 0.5f - 8f, NPC.spriteDirection * -10f).RotatedBy(NPC.rotation);

        /// <summary>
        /// The Old Duke's player target.
        /// </summary>
        public Player Target => Main.player[NPC.target];

        /// <summary>
        /// The life ratio at which the Old Duke transitions to his second phase.
        /// </summary>
        public static float Phase2LifeRatio => 0.55f;

        /// <summary>
        /// The set of attacks the Old Duke should perform in his first phase.
        /// </summary>
        public static OldDukeAIState[] Phase1AttackCycle =>
        [
            OldDukeAIState.ConjureNuclearVortex,
            OldDukeAIState.BubbleSpin,
            OldDukeAIState.PredictiveDashes,
            OldDukeAIState.KamikazeSharks,
            OldDukeAIState.NormalDashes
        ];

        /// <summary>
        /// The set of attacks the Old Duke should perform in his second phase.
        /// </summary>
        public static OldDukeAIState[] Phase2AttackCycle =>
        [
            OldDukeAIState.ConjureNuclearHurricane,
            OldDukeAIState.VomitRadioactiveCinders,
            OldDukeAIState.PredictiveDashes,
            OldDukeAIState.KamikazeSharks,
            OldDukeAIState.NormalDashes,
            OldDukeAIState.ConjureNuclearVortex,
            OldDukeAIState.BubbleSpin,
            OldDukeAIState.PredictiveDashes,
            OldDukeAIState.NormalDashes,
            OldDukeAIState.SharkWaveLaunch
        ];

        public override int NPCOverrideID => ModContent.NPCType<CalamityOD>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.DR_NERD(0.1f, null, null, null, true);
        }

        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(AttackCycleIndex);
            binaryWriter.Write(Phase);
            binaryWriter.Write(NPC.spriteDirection);
            binaryWriter.Write(NPC.rotation);
            binaryWriter.Write(AITimer);
            binaryWriter.Write((int)CurrentState);
        }

        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            AttackCycleIndex = binaryReader.ReadInt32();
            Phase = binaryReader.ReadInt32();
            NPC.spriteDirection = binaryReader.ReadInt32();
            NPC.rotation = binaryReader.ReadSingle();
            AITimer = binaryReader.ReadInt32();
            CurrentState = (OldDukeAIState)binaryReader.ReadInt32();
        }

        public override bool PreAI()
        {
            // Necessary for NPC.oldRot to be used.
            NPCID.Sets.TrailingMode[NPC.type] = 3;

            // Choose a new target if the current one has died.
            if (NPC.target <= -1 || NPC.target >= Main.maxPlayers || !Target.active || Target.dead)
                NPC.TargetClosest();

            bool everyoneIsDead = true;
            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.dead)
                {
                    everyoneIsDead = false;
                    break;
                }
            }

            float sparkSpawnProbability = MathHelper.SmoothStep(0.2f, 1f, RadioactiveGlowInterpolant) * NPC.Opacity.Cubed();
            if (Main.netMode != NetmodeID.MultiplayerClient && Phase >= 2 && Main.rand.NextBool(sparkSpawnProbability))
            {
                int sparkLifetime = Main.rand.Next(7, 14);
                Vector2 sparkRange = NPC.rotation.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4) * -Main.rand.NextFloat(40f, 90f);
                LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), EyePosition, sparkRange, ModContent.ProjectileType<FuelSpark>(), 0, 0f, -1, sparkLifetime);
            }

            NPC.damage = NPC.defDamage;
            NPC.dontTakeDamage = false;
            NPC.Opacity = LumUtils.Saturate(NPC.Opacity + 0.15f);
            EyeGlowOpacity = LumUtils.Saturate(EyeGlowOpacity + (Phase >= 2).ToDirectionInt() * 0.02f);
            AfterimageOpacity = LumUtils.Saturate(AfterimageOpacity - 0.02f);
            RadioactiveGlowInterpolant = MathHelper.Clamp(RadioactiveGlowInterpolant - 0.041f, 0f, 5f);
            MotionBlurInterpolant = MathHelper.Clamp(MotionBlurInterpolant - 0.05f, 0f, 3f);

            if (CurrentState != OldDukeAIState.Leave && everyoneIsDead)
                SwitchStateTo(OldDukeAIState.Leave);

            switch (CurrentState)
            {
                case OldDukeAIState.SpawnAnimation:
                    DoBehavior_SpawnAnimation();
                    break;
                case OldDukeAIState.BubbleSpin:
                    DoBehavior_BubbleSpin();
                    break;
                case OldDukeAIState.NormalDashes:
                    DoBehavior_Dashes(false);
                    break;
                case OldDukeAIState.PredictiveDashes:
                    DoBehavior_Dashes(true);
                    break;
                case OldDukeAIState.ConjureNuclearVortex:
                    DoBehavior_ConjureNuclearVortex();
                    break;
                case OldDukeAIState.KamikazeSharks:
                    DoBehavior_KamikazeSharks();
                    break;
                case OldDukeAIState.Phase2Transition_Roar:
                    DoBehavior_Phase2Transition_Roar();
                    break;
                case OldDukeAIState.Phase2Transition_ConsumeFuelContainers:
                    DoBehavior_Phase2Transition_ConsumeFuelContainers();
                    break;
                case OldDukeAIState.SharkWaveLaunch:
                    DoBehavior_SharkWaveLaunch();
                    break;
                case OldDukeAIState.VomitRadioactiveCinders:
                    DoBehavior_VomitRadioactiveCinders();
                    break;
                case OldDukeAIState.ConjureNuclearHurricane:
                    DoBehavior_ConjureNuclearHurricane();
                    break;
                case OldDukeAIState.Leave:
                    DoBehavior_Leave();
                    break;
            }

            ManagedScreenFilter rainOverlay = ShaderManager.GetFilter("FargowiltasCrossmod.OldDukeRainShader");
            rainOverlay.TrySetParameter("rainColor", new Color(120, 240, 40).ToVector4() * 0.4f);
            rainOverlay.TrySetParameter("screenCoordsOffset", Main.screenPosition / Main.ScreenSize.ToVector2());
            rainOverlay.TrySetParameter("rainOpacity", OldDukeSky.RainBrightnessFactor * 0.32f);
            rainOverlay.TrySetParameter("zoom", Main.GameViewMatrix.Zoom);
            rainOverlay.TrySetParameter("rainAngle", OldDukeSky.RainAngle);
            rainOverlay.TrySetParameter("time", OldDukeSky.RainTimer);
            rainOverlay.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 1, SamplerState.LinearWrap);
            rainOverlay.Activate();

            AITimer++;
            return false;
        }

        /// <summary>
        /// Makes the Old Duke create vomit particles.
        /// </summary>
        /// <param name="vomitScaleFactor">The scale factor of the ejected vomit. Defaults to 1.</param>
        public void Vomit(float vomitScaleFactor = 1f)
        {
            if (Collision.SolidCollision(MouthPosition - Vector2.One * 36f, 18, 18))
                return;

            for (int i = 0; i < 9; i++)
            {
                float vomitSpeed = MathF.Max(Main.rand.NextFloat(9f, 14f), NPC.velocity.Length());
                Vector2 vomitVelocity = NPC.SafeDirectionTo(MouthPosition).RotatedByRandom(0.1f) * vomitSpeed + Main.rand.NextVector2Circular(5f, 5f) * vomitScaleFactor;
                ModContent.GetInstance<BileMetaball>().CreateParticle(MouthPosition, vomitVelocity, Main.rand.NextFloat(32f, 46f) * vomitScaleFactor, Main.rand.NextFloat());
            }
        }

        /// <summary>
        /// Makes the Old Duke create mouth fire particles.
        /// </summary>
        public void BreatheFire(int particleCount = 3)
        {
            float fireBreathSpeed = MathHelper.Clamp(NPC.velocity.Length() * 1.15f, 60f, 1000f);
            for (int i = 0; i < particleCount; i++)
            {
                float squish = Main.rand.NextFloat(0.4f, 0.5f);
                float fireScale = Main.rand.NextFloat(100f, 240f);
                Vector2 fireVelocity = NPC.rotation.ToRotationVector2() * fireBreathSpeed + Main.rand.NextVector2Circular(30f, 30f);
                Color fireColor = new Color(Main.rand.Next(91, 170), 255, 9);
                OldDukeFireParticleSystemManager.ParticleSystem.CreateNew(MouthPosition + NPC.velocity * 3f, fireVelocity, new Vector2(1f - squish, 1f) * fireScale, fireColor);
            }
        }

        /// <summary>
        /// Makes the Old Duke look towards a given destination by updating his rotation.
        /// </summary>
        /// <param name="destination">The position to look towards.</param>
        /// <param name="aimPrecision">The aim precision interpolant. Dictates how rapidly he updates his rotation.</param>
        public void RotateTowards(Vector2 destination, float aimPrecision)
        {
            NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(destination), aimPrecision);
            NPC.spriteDirection = (int)NPC.HorizontalDirectionTo(destination);
        }

        /// <summary>
        /// Performs the Old Duke's Spawn Animation state.
        /// </summary>
        public void DoBehavior_SpawnAnimation()
        {
            int roarDelay = 50;
            int roarTime = 45;

            NPC.damage = 0;
            NPC.dontTakeDamage = true;

            NPC.rotation = 0f;
            NPC.spriteDirection = (int)NPC.HorizontalDirectionTo(Target.Center);
            if (NPC.spriteDirection == -1)
                NPC.rotation += MathHelper.Pi;

            NPC.velocity.Y = LumUtils.InverseLerp(roarDelay, 0f, AITimer) * -6f;
            NPC.Opacity = LumUtils.InverseLerp(0f, roarDelay * 0.67f, AITimer);

            Animation = OldDukeAnimation.IdleAnimation;
            if (AITimer >= roarDelay)
            {
                Animation = OldDukeAnimation.OpenMouth;
                if (AITimer == roarDelay)
                {
                    SoundEngine.PlaySound(CalamityOD.RoarSound, NPC.Center);
                    ScreenShakeSystem.StartShakeAtPoint(NPC.Center, 5.2f);
                }
            }

            if (AITimer >= roarDelay + roarTime)
                SwitchState();
        }

        /// <summary>
        /// Performs the Old Duke's Bubble Spin state.
        /// </summary>
        public void DoBehavior_BubbleSpin()
        {
            int bubbleReleaseRate = WorldSavingSystem.MasochistModeReal ? 5 : 6;
            int spinWindupTime = 35;
            int baseSpinDuration = 38;
            int postSpinGraceTime = WorldSavingSystem.MasochistModeReal ? 25 : 36;
            float spinRevolutions = 2f;
            float maxSpinArc = MathHelper.TwoPi / baseSpinDuration;
            float desiredSpinRadius = 350f;
            float bubbleSpeed = 5.4f;
            ref float spinAngle = ref NPC.ai[0];
            ref float spinDirection = ref NPC.ai[1];

            if (Phase >= 2)
            {
                bubbleReleaseRate = WorldSavingSystem.MasochistModeReal ? 4 : 5;
                baseSpinDuration = 29;
                spinRevolutions = 3f;
                bubbleSpeed = 4f;
                desiredSpinRadius = 380f;
                postSpinGraceTime = WorldSavingSystem.MasochistModeReal ? 20 : 25;
            }

            int spinTime = (int)(baseSpinDuration * spinRevolutions);

            NPC.damage = 0;

            if (AITimer == 1)
            {
                SoundEngine.PlaySound(CalamityOD.VomitSound, NPC.Center);
                spinDirection = NPC.velocity.X.NonZeroSign();
                spinAngle = NPC.AngleTo(Target.Center) + MathHelper.PiOver2 * spinDirection;
                NPC.netUpdate = true;
            }

            Animation = OldDukeAnimation.OpenMouth;
            if (AITimer <= spinWindupTime)
                Animation = OldDukeAnimation.Dash;

            if (AITimer <= spinWindupTime + spinTime)
            {
                // Calculate a suppressed windup interpolant that determines how quickly the Old Duke should spin.
                float spinWindup = MathF.Pow(LumUtils.InverseLerp(0f, spinWindupTime, AITimer), 2.6f);

                // Determine the Old Duke's current angular velocity (aka how much it should rotate relative to the player's position this frame).
                float spinAngularVelocity = spinWindup * spinDirection * maxSpinArc;

                // Update the Old Duke's position such that it linearly interpolates towards the desired spin radius, loosely keeping his radius consistent.
                float newDistance = MathHelper.Lerp(NPC.Distance(Target.Center), desiredSpinRadius, 0.085f);

                // Lock the Old Duke's position in place.
                // Since the angles and distances are based on their previous values, this creates an illusion of motion, rather than appearing to be
                // an unnatural, hard lock-on effect.
                NPC.Center = Target.Center + Target.SafeDirectionTo(NPC.Center).RotatedBy(spinAngularVelocity) * newDistance;

                NPC.rotation = NPC.rotation.AngleLerp(Target.AngleTo(NPC.Center) + MathHelper.PiOver2 * spinDirection, spinWindup * 0.95f + 0.05f);
                AfterimageOpacity = MathHelper.SmoothStep(0f, 0.6f, spinWindup);

                // Maintain a bit of momentum based on the Old Duke's tangent direction.
                // This is important for after the spin, at which point he's decelerate in that direction a bit.
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.rotation.ToRotationVector2() * 30f, spinWindup * 0.4f);

                if (AITimer >= spinWindupTime && AITimer % bubbleReleaseRate == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 bubbleDirection = NPC.SafeDirectionTo(Target.Center).RotatedByRandom(0.2f);
                        Vector2 bubbleVelocity = bubbleDirection * bubbleSpeed;
                        LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), MouthPosition, bubbleVelocity, ModContent.ProjectileType<LingeringAcidBubble>(), 270, 0f);
                    }
                    Vomit();
                }
            }
            else
                NPC.velocity *= 0.91f;

            if (AITimer >= spinWindupTime + spinTime + postSpinGraceTime)
                SwitchState();
        }

        /// <summary>
        /// Performs an Old Duke dash sequence attack.
        /// </summary>
        /// <param name="predictive">Whether the dashes are predictive or not.</param>
        public void DoBehavior_Dashes(bool predictive)
        {
            int hoverTelegraphTime = 28;
            int recoilTime = 16;
            int dashTime = 13;
            int dashCount = 4;
            bool antiRamDashTechnology = true;
            bool hurricaneAttack = CurrentState == OldDukeAIState.ConjureNuclearHurricane;
            float dashSpeed = 119.5f;
            Vector2 dashDestination = Target.Center;
            ref float dashCounter = ref NPC.ai[0];

            if (predictive)
            {
                dashDestination += Target.velocity * 20f;
                hoverTelegraphTime = WorldSavingSystem.MasochistModeReal ? 32 : 40;
                recoilTime = 13;
                dashSpeed = 106f;
            }
            if (Phase >= 2)
            {
                hoverTelegraphTime -= 2;
                recoilTime--;
                dashSpeed += 9.6f;
                dashTime -= 2;
            }
            if (dashCounter <= 0f)
            {
                hoverTelegraphTime += 5;
                recoilTime += 2;
            }
            if (AITimer < hoverTelegraphTime)
            {
                float aimInterpolant = AITimer / (float)hoverTelegraphTime;
                float hoverOffset = MathHelper.Clamp(NPC.Distance(Target.Center), 240f, 540f);
                float flySpeedInterpolant = LumUtils.InverseLerp(0.15f, 0.9f, aimInterpolant);
                if (dashCounter >= 1f)
                    flySpeedInterpolant = MathHelper.Lerp(flySpeedInterpolant, 1f, 0.5f);
                if (hoverOffset > 540f)
                    hoverOffset = 540f;

                if (NPC.velocity.Length() > 40f)
                    NPC.velocity *= 0.9f;

                Vector2 hoverDestination = Target.Center + Target.SafeDirectionTo(NPC.Center) * hoverOffset;
                NPC.SmoothFlyNear(hoverDestination, flySpeedInterpolant * 0.19f, 1f - flySpeedInterpolant * 0.19f);
                RotateTowards(dashDestination, 1f);

                // Go nuclear as a telegraph indicator on the first dash cycle to warn the player of said dashes.
                if (dashCounter <= 0f)
                    RadioactiveGlowInterpolant = LumUtils.InverseLerp(0f, 0.6f, LumUtils.Convert01To010(AITimer / (float)hoverTelegraphTime)) * 1.56f;

                Animation = OldDukeAnimation.IdleAnimation;
            }
            else if (AITimer <= hoverTelegraphTime + recoilTime)
            {
                EyeGlowOpacity = MathF.Max(EyeGlowOpacity, LumUtils.InverseLerpBump(0f, 0.3f, 0.7f, 1f, (AITimer - hoverTelegraphTime) / (float)recoilTime));

                float recoilAcceleration = 18.5f / recoilTime;
                NPC.velocity -= NPC.SafeDirectionTo(Target.Center) * recoilAcceleration;
                Animation = OldDukeAnimation.IdleAnimation;

                RadioactiveGlowInterpolant = MathF.Max(RadioactiveGlowInterpolant, LumUtils.InverseLerp(0f, recoilTime, AITimer - hoverTelegraphTime).Squared());
            }
            else if (AITimer == hoverTelegraphTime + recoilTime + 1)
            {
                NPC.velocity = NPC.rotation.ToRotationVector2() * dashSpeed * 0.3f;
                NPC.spriteDirection = NPC.velocity.X.NonZeroSign();
                NPC.netUpdate = true;
                NPC.oldPos = new Vector2[NPC.oldPos.Length];
                RadioactiveGlowInterpolant = 1f;

                OldDukeSky.CreateLightningFlash(new Vector2(Main.rand.NextFloat(), -0.1f));

                if (antiRamDashTechnology)
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath);

                ScreenShakeSystem.StartShakeAtPoint(NPC.Center, 20f, MathHelper.TwoPi, null, 0.9f);
                EyeGlowOpacity = 1f;
            }
            else if (AITimer <= hoverTelegraphTime + recoilTime + dashTime)
            {
                float newSpeed = MathHelper.Lerp(NPC.velocity.Length(), dashSpeed, 0.2f);
                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY) * newSpeed;
                NPC.rotation = NPC.velocity.ToRotation();
                Animation = OldDukeAnimation.Dash;
                AfterimageOpacity = 1f;
                RadioactiveGlowInterpolant = 1f;

                if (antiRamDashTechnology)
                {
                    Animation = OldDukeAnimation.OpenMouth;

                    // No ramdash cheese for you!
                    if (NPC.WithinRange(Target.Center, 210f))
                        NPC.dontTakeDamage = true;

                    BreatheFire();
                }
            }
            else
            {
                dashCounter++;
                NPC.netUpdate = true;
                AITimer = 0;
                NPC.TargetClosest();

                if (dashCounter >= dashCount && !hurricaneAttack)
                    SwitchState();
            }
        }

        /// <summary>
        /// Performs the Old Duke's Conjure Nuclear Vortex state.
        /// </summary>
        public void DoBehavior_ConjureNuclearVortex()
        {
            int maxHoverRedirectTime = 25;
            int dashDelay = 16;
            int dashTime = 6;
            int dashSlowdownTime = 40;
            ref float hoverOffsetDirection = ref NPC.ai[0];

            if (Phase >= 2)
            {
                maxHoverRedirectTime = 15;
                dashDelay = 13;
                dashSlowdownTime = 30;
            }

            NPC.damage = 0;
            if (AITimer == 1)
            {
                hoverOffsetDirection = -(int)NPC.HorizontalDirectionTo(Target.Center);
                NPC.netUpdate = true;
            }

            if (AITimer < maxHoverRedirectTime)
            {
                float flySpeedInterpolant = MathF.Pow(AITimer / (float)maxHoverRedirectTime, 1.5f);
                Vector2 hoverDestination = Target.Center + new Vector2(hoverOffsetDirection * 450f, -160f);
                NPC.Center = Vector2.Lerp(NPC.Center, hoverDestination, flySpeedInterpolant * 0.12f);
                NPC.SmoothFlyNear(hoverDestination, flySpeedInterpolant * 0.3f, 1f - flySpeedInterpolant * 0.4f);
                RotateTowards(Target.Center, 0.12f);
                Animation = OldDukeAnimation.IdleAnimation;

                if (NPC.WithinRange(hoverDestination, 95f))
                {
                    AITimer = maxHoverRedirectTime;
                    NPC.netUpdate = true;
                }
            }

            // Look upwards in anticipation of the dash.
            else if (AITimer <= maxHoverRedirectTime + dashDelay)
            {
                Vector2 dashDestination = Target.Center - Vector2.UnitY * 400f;
                dashDestination.X += NPC.HorizontalDirectionTo(Target.Center) * 480f;
                RotateTowards(dashDestination, 0.096f);
                Animation = OldDukeAnimation.IdleAnimation;

                // Dash.
                if (AITimer == maxHoverRedirectTime + dashDelay)
                {
                    SoundEngine.PlaySound(CalamityOD.RoarSound, NPC.Center);
                    NPC.velocity = (dashDestination - NPC.Center) / dashTime;
                    NPC.netUpdate = true;
                    ScreenShakeSystem.StartShakeAtPoint(NPC.Center, 30f, MathHelper.TwoPi, null, 1f);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), new Vector2(Target.Center.X + Target.velocity.X * 15f, dashDestination.Y - 360f), Vector2.Zero, ModContent.ProjectileType<NuclearVortex>(), 300, 0f);
                }
            }

            // Perform dash effects.
            else if (AITimer <= maxHoverRedirectTime + dashDelay + dashTime)
            {
                AfterimageOpacity = 1f;
                RadioactiveGlowInterpolant = 1f;
                NPC.rotation = NPC.velocity.ToRotation();
                MotionBlurInterpolant = 3f;
                Animation = OldDukeAnimation.Dash;

                // Vomit acid droplets that form the vortex.
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 acidVelocity = NPC.velocity.RotatedByRandom(0.73f) * Main.rand.NextFloat(0.3f, 0.5f);
                        LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), MouthPosition, acidVelocity, ModContent.ProjectileType<VortexFormingAcidDroplet>(), 0, 0f);
                    }
                }
            }

            // Slow down after the dash and fade away.
            else if (AITimer <= maxHoverRedirectTime + dashDelay + dashTime + dashSlowdownTime)
            {
                float slowdownCompletion = LumUtils.InverseLerp(0f, dashSlowdownTime, AITimer - (maxHoverRedirectTime + dashDelay + dashTime));

                if (NPC.velocity.Length() > 16f)
                    NPC.velocity = (NPC.velocity * 0.21f).ClampLength(16f, 1000f);
                else
                    NPC.velocity *= 0.95f;

                MotionBlurInterpolant *= 0.8f;
                NPC.Opacity = LumUtils.InverseLerp(0.5f, 0f, slowdownCompletion);
                EyeGlowOpacity = MathF.Max(EyeGlowOpacity, MathF.Cbrt(1f - NPC.Opacity) * MathF.Sqrt(LumUtils.InverseLerp(1f, 0.85f, slowdownCompletion)));
                NPC.dontTakeDamage = NPC.Opacity <= 0.6f;
            }

            // Teleport to the side of the target to give a natural start to the next attack.
            else
            {
                NPC.Center = Target.Center + Main.rand.NextVector2CircularEdge(420f, 420f);
                NPC.oldPos = new Vector2[NPC.oldPos.Length];

                float rotationOffsetAngle = NPC.HorizontalDirectionTo(Target.Center) * -MathHelper.PiOver2;
                NPC.velocity = Target.SafeDirectionTo(NPC.Center).RotatedBy(rotationOffsetAngle) * 40f;
                NPC.rotation = NPC.velocity.ToRotation();
                SwitchState();
            }

            AfterimageOpacity = LumUtils.InverseLerp(20f, 60f, NPC.velocity.Length());
        }

        /// <summary>
        /// Performs the Old Duke's Kamikaze Sharks state.
        /// </summary>
        public void DoBehavior_KamikazeSharks()
        {
            int vomitDelay = WorldSavingSystem.MasochistModeReal ? 45 : 90;
            int sharksPerSide = WorldSavingSystem.MasochistModeReal ? 6 : 5;
            int sharkVomitRate = 2;
            ref float sharkHoverOffsetAngle = ref NPC.ai[0];

            if (Phase >= 2)
            {
                vomitDelay = 72;
                sharkVomitRate = 1;
            }

            int sharkVomitTime = sharksPerSide * sharkVomitRate * 2;

            NPC.damage = 0;

            // Initialize the hover offset angle of the sharks.
            if (AITimer == 1)
            {
                sharkHoverOffsetAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                NPC.netUpdate = true;
            }

            if (AITimer <= vomitDelay + sharkVomitTime)
            {
                if (AITimer == vomitDelay)
                    SoundEngine.PlaySound(CalamityOD.VomitSound, NPC.Center);

                // Hover to the side of the target.
                Vector2 hoverDestination = Target.Center + Target.SafeDirectionTo(NPC.Center) * 450f - NPC.velocity * 40f;
                Vector2 idealVelocity = NPC.SafeDirectionTo(hoverDestination) * 27f;
                NPC.Center = Vector2.Lerp(NPC.Center, hoverDestination, 0.02f);
                NPC.SimpleFlyMovement(idealVelocity, 0.51f);
                RotateTowards(Target.Center, 0.15f);

                bool vomitingSharks = AITimer >= vomitDelay;
                if (vomitingSharks)
                {
                    Vomit(Main.rand.NextFloat(0.8f, 1f));
                    if (Main.netMode != NetmodeID.MultiplayerClient && (AITimer - vomitDelay) % sharkVomitRate == 0)
                    {
                        int time = AITimer - vomitDelay - sharkVomitTime;
                        int sharkIndex = (AITimer - vomitDelay) / sharkVomitRate;
                        int sharkSide = sharkIndex % 2;
                        float offsetInterpolant = sharkIndex / (float)sharksPerSide * 0.5f;
                        NPC shark = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)MouthPosition.X, (int)MouthPosition.Y, ModContent.NPCType<SulphurousSharkron>(), NPC.whoAmI, time, sharkSide, offsetInterpolant, sharkHoverOffsetAngle, NPC.target);
                        shark.velocity = NPC.SafeDirectionTo(MouthPosition) * 11f + Main.rand.NextVector2Circular(5f, 5f);
                    }
                }

                Animation = OldDukeAnimation.OpenMouth;
            }
            else
            {
                NPC.SmoothFlyNear(Target.Center, 0.04f, 0.95f);
                RotateTowards(Target.Center, 0.17f);
                Animation = OldDukeAnimation.IdleAnimation;
            }

            if (AITimer >= vomitDelay + sharkVomitTime + 70)
                SwitchState();

            // Since I hate uncentralized behavior code spread across multiple different files, make Old Duke do all the work and simply
            // tell the sharkrons what to do in the moment, rather than doing some ugly "Check Old Duke's AI state and act accordingly" logic in the sharkron's code.
            SharkronPuppeteerAction = DoBehavior_KamikazeSharks_PuppeteerShark;
        }

        /// <summary>
        /// Performs the Old Duke's Shark Wave Launch state.
        /// </summary>
        public void DoBehavior_SharkWaveLaunch()
        {
            int dashCount = 2;
            int repositionTime = 30;
            int sharkFormationTime = 54;
            int dashTime = 60;
            int sharksOnEachSide = 8;
            float baseSpacing = 132f;
            float spacingPerShark = 122f;
            float reelBackPerShark = 81f;
            float sharkDashSpeed = 21.5f;
            float oldDukeDashSpeed = 132f;
            float dashPredictiveness = 0f;
            float oldDukeReelBackDistance = 400f; // This HEAVILY affects the difficulty of this attack since more distance from the player = less ability to enter the empty space in time.
            ref float hoverOffsetAngle = ref NPC.ai[0];
            ref float dashCounter = ref NPC.ai[1];

            if (Main.netMode != NetmodeID.MultiplayerClient && AITimer == 1)
            {
                // Initialize the hover offset angle and bias it to the side because vertical movement in this game sucks.
                hoverOffsetAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                hoverOffsetAngle = (hoverOffsetAngle.ToRotationVector2() * new Vector2(1f, 0.5f)).ToRotation();

                NPC.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && AITimer == repositionTime)
            {
                // Create sharks.
                for (int i = 1; i <= sharksOnEachSide; i++)
                {
                    float curveinStrength = WorldSavingSystem.MasochistModeReal ? 0.01f : 0.06f;
                    float angle = i * -curveinStrength;
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SulphurousSharkron>(), NPC.whoAmI, 0f, -i, -angle);
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SulphurousSharkron>(), NPC.whoAmI, 0f, i, angle);
                }
            }

            // Hover near the player as the sharks create a perpendicular formation.
            if (AITimer <= repositionTime + sharkFormationTime)
            {
                float reelBackDistance = LumUtils.InverseLerp(sharkFormationTime * 0.3f, sharkFormationTime, AITimer - repositionTime).Squared() * oldDukeReelBackDistance;
                Vector2 reelBackOffset = NPC.SafeDirectionTo(Target.Center) * -reelBackDistance;
                Vector2 hoverDestination = Target.Center + hoverOffsetAngle.ToRotationVector2() * new Vector2(500f, 300f) + reelBackOffset;
                NPC.SmoothFlyNear(hoverDestination, 0.17f, 0.9f);
                NPC.damage = 0;

                float aimInterpolant = LumUtils.InverseLerp(sharkFormationTime * 0.5f, 0f, AITimer - repositionTime).Squared();
                RotateTowards(Target.Center + Target.velocity * dashPredictiveness, aimInterpolant * 0.2f);

                Animation = OldDukeAnimation.IdleAnimation;
            }

            // Dash.
            else if (AITimer <= repositionTime + sharkFormationTime + dashTime)
            {
                NPC.damage = NPC.defDamage;
                AfterimageOpacity = 1f;
                BreatheFire();
                Animation = OldDukeAnimation.OpenMouth;

                // No ramdash cheese for you!
                if (NPC.WithinRange(Target.Center, 210f))
                    NPC.dontTakeDamage = true;

                if (AITimer == repositionTime + sharkFormationTime + 1)
                {
                    OldDukeSky.CreateLightningFlash(new Vector2(Main.rand.NextFloat(), -0.1f));
                    NPC.oldPos = new Vector2[NPC.oldPos.Length];
                    RadioactiveGlowInterpolant = 1f;

                    SoundEngine.PlaySound(CalamityOD.RoarSound, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath);
                    NPC.velocity = NPC.rotation.ToRotationVector2() * oldDukeDashSpeed;
                    NPC.netUpdate = true;
                }
            }

            else
            {
                int sharkID = ModContent.NPCType<SulphurousSharkron>();
                foreach (NPC shark in Main.ActiveNPCs)
                {
                    if (shark.type == sharkID && shark.TryGetDLCBehavior(out SulphurousSharkronEternity s))
                        s.Die(false);
                }

                // Do a loop-around teleport.
                if (!NPC.WithinRange(Target.Center, 1200f))
                {
                    NPC.velocity = NPC.velocity.ClampLength(0f, 40f);
                    NPC.Center = Target.Center - Target.SafeDirectionTo(NPC.Center) * 900f;
                    NPC.netUpdate = true;
                }

                dashCounter++;
                if (dashCounter >= dashCount)
                    SwitchState();
                else
                {
                    AITimer = 0;
                    NPC.netUpdate = true;
                }
            }

            // Handle shark motion.
            SharkronPuppeteerAction = sharkModNPC =>
            {
                NPC shark = sharkModNPC.NPC;
                int spacingIndex = (int)shark.ai[1];
                // Stay perpendicular to the Old Duke.
                if (AITimer <= repositionTime + sharkFormationTime)
                {
                    float rotationalOffset = shark.ai[2];
                    float spacingInterpolant = MathF.Pow(LumUtils.InverseLerp(0f, sharkFormationTime, AITimer - repositionTime), 0.67f);
                    float spacingOffset = MathHelper.SmoothStep(0f, spacingIndex * spacingPerShark, spacingInterpolant) + baseSpacing * MathF.Sign(spacingIndex);
                    float reelBack = MathF.Abs(shark.ai[1]) * reelBackPerShark * MathF.Pow(spacingInterpolant, 2.3f);
                    Vector2 oldDukeDirection = NPC.rotation.ToRotationVector2();
                    Vector2 perpendicular = oldDukeDirection.RotatedBy(MathHelper.PiOver2);
                    shark.Center = Vector2.Lerp(shark.Center, NPC.Center + perpendicular * spacingOffset - oldDukeDirection * reelBack, 0.3f);
                    shark.rotation = oldDukeDirection.ToRotation() + MathHelper.Pi + rotationalOffset;
                    shark.damage = 0;

                    sharkModNPC.AfterimageOpacity = 0f;
                }
                else
                {
                    shark.damage = shark.defDamage;
                    shark.dontTakeDamage = true; // No ram dash cheese.
                    shark.velocity *= 1.024f;
                }

                if (AITimer == repositionTime + sharkFormationTime + 1)
                {
                    shark.velocity = shark.rotation.ToRotationVector2() * -sharkDashSpeed * Main.rand.NextFloat(0.85f, 1f);
                    shark.netUpdate = true;
                }
            };
        }

        /// <summary>
        /// Performs the Old Duke's Vomit Radioactive Cinders state.
        /// </summary>
        public void DoBehavior_VomitRadioactiveCinders()
        {
            int hoverRedirectTime = 36;
            int slowdownTime = 10;
            int cinderCount = 20;
            int cinderReleaseRate = 4;
            int cinderReleaseTime = cinderReleaseRate * cinderCount;
            int attackTransitionDelay = 50;
            float standardCinderSpeed = 26.5f;

            Animation = OldDukeAnimation.IdleAnimation;

            if (AITimer <= hoverRedirectTime)
            {
                float flySpeedInterpolant = LumUtils.InverseLerp(0f, hoverRedirectTime, AITimer);
                Vector2 hoverDestination = Target.Center + Target.SafeDirectionTo(NPC.Center) * new Vector2(520f, 400f);
                NPC.Center = Vector2.Lerp(NPC.Center, hoverDestination, flySpeedInterpolant * 0.15f);
                NPC.SmoothFlyNear(hoverDestination, flySpeedInterpolant * 0.285f, 1f - flySpeedInterpolant * 0.19f);
                RotateTowards(Target.Center, 0.1f);

                if (NPC.WithinRange(hoverDestination, 50f))
                {
                    AITimer = hoverRedirectTime + 1;
                    NPC.netUpdate = true;
                }
            }

            // Slow down considerably in anticipation of the cinder vomit.
            else if (AITimer <= hoverRedirectTime + slowdownTime)
            {
                if (NPC.WithinRange(Target.Center, 240f))
                    NPC.Center -= NPC.SafeDirectionTo(Target.Center) * 24f;

                NPC.velocity *= 0.85f;
                RotateTowards(Target.Center, 0.14f);
            }

            // Vomit cinders.
            else if (AITimer <= hoverRedirectTime + slowdownTime + cinderReleaseTime)
            {
                Animation = OldDukeAnimation.OpenMouth;

                int relativeTimer = AITimer - hoverRedirectTime - slowdownTime;
                if (relativeTimer == 1)
                {
                    SoundEngine.PlaySound(CalamityOD.VomitSound, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
                }

                // Look towards the player.
                RotateTowards(Target.Center, 0.12f);

                NPC.velocity *= 0.93f;
                BreatheFire(1);

                if (Main.netMode != NetmodeID.MultiplayerClient && relativeTimer % cinderReleaseRate == 0)
                {
                    Vector2 vomitDirection = NPC.rotation.ToRotationVector2();
                    NPC.velocity -= vomitDirection * 0.3f;

                    Vector2 cinderVelocity = vomitDirection.RotatedByRandom(0.92f) * standardCinderSpeed * Main.rand.NextFloat(0.75f, 1.15f);
                    LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), MouthPosition, cinderVelocity, ModContent.ProjectileType<SwervingRadioactiveCinder>(), 270, 0f);

                    NPC.netUpdate = true;
                    NPC.netSpam = 0;
                }
            }

            if (AITimer >= hoverRedirectTime + slowdownTime + cinderReleaseTime + attackTransitionDelay)
                SwitchState();
        }

        /// <summary>
        /// Performs the Old Duke's Conjure Nuclear Hurricane state.
        /// </summary>
        public void DoBehavior_ConjureNuclearHurricane()
        {
            int hurricaneLifetime = 540;
            ref float actualTimer = ref NPC.ai[0];

            if (AITimer == 1 && !LumUtils.AnyProjectiles(ModContent.ProjectileType<NuclearHurricane>()))
            {
                Vector2 hurricaneSpawnPosition = Target.Center;

                // Bias the hurricane towards the world border position.
                bool left = Target.Center.X < Main.maxTilesX * 8f;
                hurricaneSpawnPosition.X -= left.ToDirectionInt() * 1100f;

                LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), hurricaneSpawnPosition, Vector2.Zero, ModContent.ProjectileType<NuclearHurricane>(), 500, 0f, -1, 0.02f, hurricaneLifetime);
            }

            actualTimer++;
            if (actualTimer >= hurricaneLifetime)
                SwitchState();

            DoBehavior_Dashes(false);
            NPC.velocity *= 0.91f;

            if (actualTimer <= 10)
                NPC.damage = 0;
        }

        /// <summary>
        /// Handles the puppeteering of sulphurous sharkrons during the Old Duke's Kamikaze Sharks state.
        /// </summary>
        /// <param name="sharkBehavior">The shark being puppeteered.</param>
        public void DoBehavior_KamikazeSharks_PuppeteerShark(SulphurousSharkronEternity sharkBehavior)
        {
            NPC shark = sharkBehavior.NPC;
            shark.scale = LumUtils.Saturate(shark.scale + 0.16f);
            shark.damage = 0;

            int sideHoverTime = 30;
            int recoilTime = 16;
            int dashTime = 18;
            float hoverAngleArc = 0.93f;

            if (Phase >= 2)
                hoverAngleArc *= 1.15f;

            int sharkSide = (int)shark.ai[1];
            float sharkOffsetInterpolant = shark.ai[2];
            float hoverAngleOffset = shark.ai[3];
            float hoverAngle = hoverAngleOffset + MathHelper.Lerp(-hoverAngleArc, hoverAngleArc, sharkOffsetInterpolant);
            if (sharkSide == 1)
                hoverAngle += MathHelper.Pi;

            if (sharkBehavior.Time < 0f)
                shark.velocity = shark.velocity.RotatedBy(MathHelper.Lerp(-0.076f, 0.076f, shark.whoAmI / 8.37f)) * 1.03f;

            // Attempt to hover to the sides.
            if (sharkBehavior.Time < sideHoverTime)
            {
                float hoverSharpness = MathHelper.SmoothStep(0.1f, 1f, MathF.Pow(LumUtils.InverseLerp(0.3f, 1f, sharkBehavior.Time / sideHoverTime), 1.9f));
                Vector2 hoverDestination = Target.Center + hoverAngle.ToRotationVector2() * 418f;
                shark.Center = Vector2.Lerp(shark.Center, hoverDestination, hoverSharpness * 0.34f + 0.03f);
                shark.SmoothFlyNear(hoverDestination, hoverSharpness * 0.27f, 1f - hoverSharpness * 0.33f);

                shark.rotation = shark.AngleTo(Target.Center) + MathHelper.Pi;
            }

            // Recoil.
            else if (sharkBehavior.Time < sideHoverTime + recoilTime)
            {
                float recoilSpeed = LumUtils.InverseLerp(0f, recoilTime, sharkBehavior.Time - sideHoverTime) * 32f;
                shark.velocity = Vector2.Lerp(shark.velocity, shark.SafeDirectionTo(Target.Center) * -recoilSpeed, 1f);
            }

            // Fly forward.
            else
            {
                shark.velocity = Vector2.Lerp(shark.velocity, shark.rotation.ToRotationVector2() * -100f, 0.1f);
                shark.damage = shark.defDamage;
                shark.dontTakeDamage = true; // No ram dash cheese.

                if (sharkBehavior.Time >= sideHoverTime + recoilTime + dashTime)
                    sharkBehavior.Die(true);
            }
        }

        /// <summary>
        /// Performs the Old Duke's phase 2 roar transition state.
        /// </summary>
        public void DoBehavior_Phase2Transition_Roar()
        {
            int roarDelay = 60;
            int roarTime = 90;
            int roarVisualSpawnRate = 8;

            // Slow down.
            NPC.velocity *= 0.93f;
            if (NPC.velocity.Length() >= 35f)
                NPC.velocity *= 0.85f;

            NPC.dontTakeDamage = true;
            NPC.spriteDirection = (int)NPC.HorizontalDirectionTo(Target.Center);

            float idealRotation = 0f;
            if (NPC.spriteDirection == -1)
                idealRotation += MathHelper.Pi;

            Animation = OldDukeAnimation.IdleAnimation;
            NPC.rotation = NPC.rotation.AngleLerp(idealRotation, 0.074f);

            if (AITimer >= roarDelay && AITimer <= roarDelay + roarTime)
            {
                Phase = 2;
                Animation = OldDukeAnimation.OpenMouth;
                RadioactiveGlowInterpolant = MathHelper.Clamp(RadioactiveGlowInterpolant + 0.2f, 0f, 1.2f);

                OldDukeSky.RainBrightnessFactor = MathHelper.Lerp(OldDukeSky.RainBrightnessFactor, 4f, 0.15f);
                OldDukeSky.RainSpeed = MathHelper.Lerp(OldDukeSky.RainSpeed, 2.5f, 0.1f);

                if (AITimer == roarDelay)
                    SoundEngine.PlaySound(CalamityOD.RoarSound, Vector2.Lerp(NPC.Center, Main.LocalPlayer.Center, 0.85f));

                if (AITimer % roarVisualSpawnRate == 0)
                {
                    BlendState burstBlend = LumUtils.SubtractiveBlending;
                    Color burstColor = Color.White;
                    if (AITimer % (roarVisualSpawnRate * 3) >= 1)
                    {
                        burstBlend = BlendState.Additive;
                        burstColor = Color.YellowGreen;
                    }

                    ChromaticBurstParticle burst = new(MouthPosition, burstColor * 0.75f, burstBlend, 0.1f, 12);
                    burst.Spawn();

                    ScreenShakeSystem.StartShake(16f, MathHelper.TwoPi, null, 4.5f);
                }
            }

            if (AITimer >= roarDelay + roarTime + 45)
                SwitchState();
        }

        /// <summary>
        /// Performs the Old Duke's phase 2 fuel container consumption transition state.
        /// </summary>
        public void DoBehavior_Phase2Transition_ConsumeFuelContainers()
        {
            int diveTime = 40;
            int containerSpawnDelay = 45;
            ref float chompCountdown = ref NPC.ai[0];

            NPC.damage = 0;
            NPC.dontTakeDamage = true;
            Animation = OldDukeAnimation.IdleAnimation;

            // Dive downwards.
            if (AITimer <= diveTime)
            {
                float speedInterpolant = LumUtils.InverseLerp(0f, 0.4f, AITimer / (float)diveTime) * 0.095f;
                Vector2 idealVelocity = NPC.SafeDirectionTo(Target.Center + new Vector2(432f, 1776f)) * 81f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, idealVelocity, speedInterpolant);
                NPC.rotation = NPC.velocity.ToRotation();
            }

            // Send fuel containers into the air from below.
            if (AITimer == diveTime + containerSpawnDelay || AITimer == diveTime + containerSpawnDelay + 10 || AITimer == diveTime + containerSpawnDelay + 20)
            {
                SoundEngine.PlaySound(SoundID.Item106, Target.Center);
                ScreenShakeSystem.StartShakeAtPoint(Target.Center, 3.2f);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 fuelSpawnPosition = Target.Center + Vector2.UnitY * 965f;
                    if (AITimer == diveTime + containerSpawnDelay + 10)
                        fuelSpawnPosition.X -= Target.velocity.X * 12f + 300f;
                    else if (AITimer == diveTime + containerSpawnDelay + 20)
                        fuelSpawnPosition.X += Target.velocity.X * 12f + 300f;
                    else
                        fuelSpawnPosition.Y -= 240f;

                    fuelSpawnPosition.Y -= (AITimer - (diveTime + containerSpawnDelay)) * 15f;

                    Vector2 fuelVelocity = -Vector2.UnitY.RotatedByRandom(0.01f) * 37f;
                    LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), fuelSpawnPosition, fuelVelocity, ModContent.ProjectileType<TastyFuelContainer>(), 0, 0f);
                }
            }

            // Eat fuel containers.
            // Yummy........
            if (AITimer >= diveTime + containerSpawnDelay + 30)
            {
                var containers = LumUtils.AllProjectilesByID(ModContent.ProjectileType<TastyFuelContainer>()).
                    OrderByDescending(c => -MathHelper.Distance(c.Center.X, NPC.Center.X)).ToList();

                if (containers.Count >= 1)
                {
                    Projectile closestContainer = containers.First();
                    Vector2 flyDestination = closestContainer.Center;
                    Vector2 velocity = closestContainer.velocity;
                    for (int i = 0; i < 4; i++)
                    {
                        flyDestination += velocity;
                        TastyFuelContainer.UpdateVelocity(ref velocity);
                    }
                    flyDestination -= MouthPosition - NPC.Center;

                    Vector2 idealVelocity = NPC.SafeDirectionTo(flyDestination) * 85f;
                    NPC.Center = Vector2.Lerp(NPC.Center, flyDestination, 0.015f);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, idealVelocity, 0.056f);
                    NPC.SmoothFlyNear(flyDestination, 0.1f, 0.905f);
                    if (Vector2.Dot(NPC.velocity, idealVelocity) < 0f)
                        NPC.velocity *= 0.9f;

                    // Open the Old Duke's mouth in anticipation of eating the fuel container.
                    if (chompCountdown >= 1f)
                        chompCountdown--;
                    else if (MouthPosition.WithinRange(closestContainer.Center, 235f))
                        Animation = OldDukeAnimation.OpenMouth;

                    // Chomp down on the container if sufficiently close.
                    if (MouthPosition.WithinRange(closestContainer.Center, 74f))
                    {
                        closestContainer.Kill();
                        chompCountdown = 5f;
                        RadioactiveGlowInterpolant = 1.5f;
                        NPC.netUpdate = true;
                    }

                    NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.5f);
                }

                else
                    SwitchStateTo(OldDukeAIState.Phase2Transition_Roar);

                AfterimageOpacity = LumUtils.InverseLerp(20f, 54f, NPC.velocity.Length());
            }
        }

        /// <summary>
        /// Performs the Old Duke's leave state, making him vanish into the clouds.
        /// </summary>
        public void DoBehavior_Leave()
        {
            float speedUpInterpolant = LumUtils.InverseLerp(0f, 30f, AITimer) * 0.14f;
            NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(NPC.spriteDirection * 56f, -150f), speedUpInterpolant);
            NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.32f);

            NPC.Opacity = LumUtils.InverseLerp(15f, 45f, AITimer);
            OldDukeSky.RainSpeed = MathHelper.SmoothStep(0.35f, 1f, NPC.Opacity);
            OldDukeSky.RainBrightnessFactor = NPC.Opacity;

            if (AITimer >= 45)
                NPC.active = false;
        }

        /// <summary>
        /// Switches the Old Duke's current state to the next one.
        /// </summary>
        public void SwitchState()
        {
            NPC.TargetClosest();

            OldDukeAIState[] attackCycle = Phase1AttackCycle;
            if (Phase >= 2)
                attackCycle = Phase2AttackCycle;

            CurrentState = attackCycle[AttackCycleIndex % attackCycle.Length];
            AttackCycleIndex++;

            float lifeRatio = NPC.GetLifePercent();
            if (lifeRatio < Phase2LifeRatio && Phase < 2)
            {
                CurrentState = OldDukeAIState.Phase2Transition_ConsumeFuelContainers;
                AttackCycleIndex = 0;
            }

            AITimer = 0;
            NPC.ai[0] = 0f;
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.netUpdate = true;
        }

        /// <summary>
        /// Forces the Old Duke to switch to a given state.
        /// </summary>
        /// <param name="newState">The state to forcefully switch to.</param>
        public void SwitchStateTo(OldDukeAIState newState)
        {
            SwitchState();
            CurrentState = newState;
            if (AttackCycleIndex >= 1)
                AttackCycleIndex--;
        }

        public override void FindFrame(int frameHeight)
        {
            switch (Animation)
            {
                case OldDukeAnimation.Dash:
                    FrameY = 2;
                    break;
                case OldDukeAnimation.OpenMouth:
                    FrameY = 6;
                    break;
                case OldDukeAnimation.IdleAnimation:
                    FrameCounter++;
                    FrameY = FrameCounter / 6 % 6;

                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Main.spriteBatch.PrepareForShaders();

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;

            // Define the Old Duke's frame.
            // Done in here instead of FindFrame because the FindFrame hook seemingly doesn't override Calamity's, resulting in conflicts.
            NPC.frame = texture.Frame(1, Main.npcFrameCount[NPC.type], 0, FrameY);

            ManagedShader overlayShader = ShaderManager.GetShader("FargowiltasCrossmod.OldDukeRadioactiveOverlayShader");
            overlayShader.TrySetParameter("blurInterpolant", MotionBlurInterpolant);
            overlayShader.TrySetParameter("textureSize", texture.Size());
            overlayShader.TrySetParameter("glowColor", new Vector3(0.53f, 1f, 0.03f));
            overlayShader.TrySetParameter("frame", new Vector4(NPC.frame.X, NPC.frame.Y, NPC.frame.Width, NPC.frame.Height));
            overlayShader.TrySetParameter("turbulence", 0.36f);
            overlayShader.TrySetParameter("translucentAccent", new Vector3(0.8f, 0f, 1.2f));
            overlayShader.TrySetParameter("glowInterpolant", RadioactiveGlowInterpolant.Squared());
            overlayShader.TrySetParameter("pixelationLevel", 105f);
            overlayShader.SetTexture(NoiseTexturesRegistry.PerlinNoise.Value, 1, SamplerState.LinearWrap);
            overlayShader.Apply();

            float rotation = NPC.rotation;
            float rotationOffset = 0f;
            SpriteEffects direction = SpriteEffects.None;
            if (NPC.spriteDirection == -1)
            {
                rotationOffset = MathHelper.Pi;
                direction = SpriteEffects.FlipHorizontally;
            }
            rotation += rotationOffset;

            for (int i = 8; i >= 0; i--)
            {
                float afterimageOpacity = (1f - i / 9f).Squared() * AfterimageOpacity;
                Vector2 afterimageDrawPosition = NPC.oldPos[i] + NPC.Size * 0.5f - Main.screenPosition;
                Main.EntitySpriteDraw(texture, afterimageDrawPosition, NPC.frame, NPC.GetAlpha(lightColor) * afterimageOpacity, NPC.oldRot[i] + rotationOffset, NPC.frame.Size() * 0.5f, NPC.scale, direction);
            }

            Texture2D glowmask = CalamityOD.GlowTexture.Value;
            Vector2 drawPosition = NPC.Center - screenPos;
            Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, NPC.GetAlpha(lightColor), rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction);
            Main.EntitySpriteDraw(glowmask, drawPosition, NPC.frame, Color.White * EyeGlowOpacity, rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction);

            Main.spriteBatch.ResetToDefault();
            return false;
        }
    }
}
