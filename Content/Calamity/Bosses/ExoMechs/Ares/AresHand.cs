using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Assets.Particles.Metaballs;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public class AresHand : ModNPC, IPixelatedPrimitiveRenderer
    {
        /// <summary>
        /// The energy drawer for this arm. Used for telegraphing.
        /// </summary>
        public AresCannonChargeParticleSet EnergyDrawer = new(-1, 15, 40f, Color.Red);

        /// <summary>
        /// The type of hand that this NPC is.
        /// </summary>
        public AresHandType HandType = AresHandType.PlasmaCannon;

        /// <summary>
        /// The local index of this arm. This is used as a means of ensuring that the arm which instructions from Ares' body should be follows.
        /// </summary>
        public int LocalIndex => (int)NPC.ai[0];

        /// <summary>
        /// Which side arms should be drawn on relative to Ares' body.
        /// </summary>
        public int ArmSide
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        /// <summary>
        /// The direction from this arm's shoulder to its hand.
        /// </summary>
        public float ShoulderToHandDirection
        {
            get;
            private set;
        }

        /// <summary>
        /// The opacity of afterimages used by the energy katana variant of this hand.
        /// </summary>
        public float KatanaAfterimageOpacity
        {
            get;
            set;
        }

        /// <summary>
        /// How much the energy katana has appeared.
        /// </summary>
        public float KatanaAppearanceInterpolant
        {
            get;
            set;
        }

        /// <summary>
        /// The frame of this arm.
        /// </summary>
        public int Frame
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this hand uses a back arm or not.
        /// </summary>
        public bool UsesBackArm
        {
            get;
            set;
        }

        /// <summary>
        /// Whether Ares' energy katanas are in use. This only applies if using the <see cref="AresHandType.EnergyKatana"/> variant.
        /// </summary>
        public bool KatanaInUse
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this hand/arm can be rendered.
        /// </summary>
        public bool CanRender
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this hand is magnetically attached to its arm or not.
        /// </summary>
        public bool AttachedToArm
        {
            get;
            set;
        }

        /// <summary>
        /// How disabled the glow masks are, as a 0-1 interpolant.
        /// </summary>
        public float GlowmaskDisabilityInterpolant
        {
            get;
            set;
        }

        /// <summary>
        /// The endpoint of this arm when drawing Ares arm.
        /// </summary>
        /// 
        /// <remarks>
        /// For most cases, this is equivalent to the hand's center. After all, one would usually want arms and hands to be attached.
        /// However, there are some circumstances, such as when a hand is being detached, that it's desirable for the two to be incongruent.
        /// </remarks>
        public Vector2 ArmEndpoint
        {
            get => new(NPC.ai[1], NPC.ai[2]);
            set
            {
                NPC.ai[1] = value.X;
                NPC.ai[2] = value.Y;
            }
        }

        /// <summary>
        /// An optional action that should be done for drawing this hand.
        /// </summary>
        public Action? OptionalDrawAction;

        /// <summary>
        /// The sound played when one of Ares' hands get swapped to a gauss nuke.
        /// </summary>
        public static readonly SoundStyle GaussNukeSwapSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/HandSwap_GaussNuke", 2);

        /// <summary>
        /// The sound played when one of Ares' hands get swapped to a laser cannon.
        /// </summary>
        public static readonly SoundStyle LaserCannonSwapSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/HandSwap_LaserCannon", 2);

        /// <summary>
        /// The sound played when one of Ares' hands get swapped to a plasma cannon.
        /// </summary>
        public static readonly SoundStyle PlasmaCannonSwapSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/HandSwap_PlasmaCannon", 2);

        /// <summary>
        /// The sound played when one of Ares' hands get swapped to a pulse cannon.
        /// </summary>
        public static readonly SoundStyle PulseCannonSwapSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/HandSwap_PulseCannon", 2);

        /// <summary>
        /// The sound played when one of Ares' hands get swapped to a tesla cannon.
        /// </summary>
        public static readonly SoundStyle TeslaCannonSwapSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/HandSwap_TeslaCannon", 2);

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public override void SetStaticDefaults()
        {
            this.ExcludeFromBestiary();
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 5f;
            NPC.damage = 350;
            NPC.width = 172;
            NPC.height = 108;
            NPC.defense = 100;
            NPC.DR_NERD(0.45f);
            NPC.LifeMaxNERB(1250000, 1495000, 650000);
            NPC.lifeMax += (int)(NPC.lifeMax * CalamityConfig.Instance.BossHealthBoost * 0.01);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.canGhostHeal = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.DeathSound = CommonCalamitySounds.ExoDeathSound;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.hide = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPCID.Sets.TrailCacheLength[NPC.type] = 18;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            HandType.WriteTo(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            HandType = AresHandType.ReadFrom(reader);
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.draedonExoMechPrime <= -1 || !Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active || !Main.npc[CalamityGlobalNPC.draedonExoMechPrime].TryGetDLCBehavior(out AresBodyEternity body))
            {
                NPC.active = false;
                return;
            }

            NPC aresBody = Main.npc[CalamityGlobalNPC.draedonExoMechPrime];

            NPC.noTileCollide = true;

            AresHandType previousHandType = HandType;

            CanRender = true;
            AttachedToArm = true;
            KatanaInUse = false;
            OptionalDrawAction = null;
            KatanaAfterimageOpacity = Utilities.Saturate(KatanaAfterimageOpacity * 0.84f - 0.07f);
            EnergyDrawer.ParticleSpawnRate = int.MaxValue;
            EnergyDrawer.ParticleColor = HandType.EnergyTelegraphColor;
            NPC.damage = 0;
            NPC.Calamity().ShouldCloseHPBar = true;
            NPC.dontTakeDamage = NPC.Opacity < 0.95f || body.NPC.dontTakeDamage || !CanRender;
            body.InstructionsForHands[LocalIndex]?.Action?.Invoke(this);

            float oldAppearanceInterpolant = KatanaAppearanceInterpolant;
            KatanaAppearanceInterpolant = LumUtils.Saturate(KatanaAppearanceInterpolant + KatanaInUse.ToDirectionInt() * 0.072f);
            if (KatanaInUse && oldAppearanceInterpolant == 0f && KatanaAppearanceInterpolant >= 0.001f)
            {
                SoundEngine.PlaySound(AresBodyEternity.KatanaUnsheatheSound, NPC.Center);
                ScreenShakeSystem.StartShakeAtPoint(NPC.Center, 4f);
            }

            EnergyDrawer.Update();

            NPC.realLife = CalamityGlobalNPC.draedonExoMechPrime;
            NPC.scale = aresBody.scale;

            ProcessSwapSounds(previousHandType, aresBody);
        }

        /// <summary>
        /// Processes swap sounds, playing them if there's a mismatch between the previous and current hand type.
        /// </summary>
        /// <param name="previousHandType">The hand type on the previous frame.</param>
        public void ProcessSwapSounds(AresHandType previousHandType, NPC aresBody)
        {
            if (HandType == previousHandType)
                return;

            SoundStyle? soundToPlay = HandType.SwapSound;
            Vector2 soundPlayPosition = Vector2.Lerp(aresBody.Center, Main.LocalPlayer.Center, 0.85f);
            if (soundToPlay is not null)
                SoundEngine.PlaySound(soundToPlay.Value with { MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew }, NPC.Center).WithVolumeBoost(1.72f);
        }

        /// <summary>
        /// Makes this hand look towards a given direction.
        /// </summary>
        /// <param name="idealRotation">The rotation to look in the direction in.</param>
        /// <param name="rotateSpeedInterpolant">The speed at which rotation occurs.</param>
        public void RotateToLookAt(float idealRotation, float rotateSpeedInterpolant = 1f)
        {
            int oldSpriteDirection = NPC.spriteDirection;
            NPC.spriteDirection = MathF.Cos(idealRotation).NonZeroSign();
            if (NPC.spriteDirection == -1)
                idealRotation += MathHelper.Pi;

            NPC.rotation = NPC.rotation.AngleLerp(idealRotation, rotateSpeedInterpolant);
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += MathHelper.Pi;
        }

        /// <summary>
        /// Makes this hand look towards a given destination point.
        /// </summary>
        /// <param name="lookDestination">The position to look at.</param>
        public void RotateToLookAt(Vector2 lookDestination) =>
            RotateToLookAt(NPC.AngleTo(lookDestination));

        public override Color? GetAlpha(Color drawColor)
        {
            if (CalamityGlobalNPC.draedonExoMechPrime == -1)
                return drawColor * NPC.Opacity;

            return Main.npc[CalamityGlobalNPC.draedonExoMechPrime].GetAlpha(drawColor) * NPC.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            if (HandType is null || CalamityGlobalNPC.draedonExoMechPrime == -1 || !CanRender)
                return false;

            Texture2D texture = ModContent.Request<Texture2D>(HandType.TexturePath).Value;
            Texture2D glowmask = ModContent.Request<Texture2D>(HandType.GlowmaskPath).Value;
            Vector2 drawPosition = NPC.Center - screenPos;

            int frameX = Frame / HandType.TotalHorizontalFrames;
            int frameY = Frame % HandType.TotalHorizontalFrames;
            NPC.frame = texture.Frame(HandType.TotalHorizontalFrames, HandType.TotalVerticalFrames, frameX, frameY);

            Color glowmaskColor = Color.Lerp(Color.White, new(25, 25, 25), GlowmaskDisabilityInterpolant);
            Main.spriteBatch.Draw(texture, drawPosition, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection(), 0f);
            Main.spriteBatch.Draw(glowmask, drawPosition, NPC.frame, NPC.GetAlpha(glowmaskColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection(), 0f);

            DrawEnergyTelegraph(texture, drawPosition);
            OptionalDrawAction?.Invoke();
            HandType?.ExtraDrawAction?.Invoke(NPC, NPC.Center - screenPos);

            return false;
        }

        public void DrawEnergyTelegraph(Texture2D texture, Vector2 drawPosition)
        {
            Main.spriteBatch.PrepareForShaders(BlendState.Additive);

            Vector2 coreSpritePosition = NPC.Center - new Vector2(NPC.spriteDirection * 36f, -6f).RotatedBy(NPC.rotation) * NPC.scale;

            // Draw a pulsing edge glow above the hand.
            if (EnergyDrawer.chargeProgress > 0f)
            {
                float pulseRatio = Main.GlobalTimeWrappedHourly * 3f % 1f;
                float pulseOpacity = MathHelper.Clamp(pulseRatio * 0.3f, 1f, 2f) * EnergyDrawer.chargeProgress;
                Main.spriteBatch.Draw(texture, drawPosition, NPC.frame, EnergyDrawer.ParticleColor * MathHelper.Lerp(1f, 0f, pulseRatio) * pulseOpacity, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale + pulseRatio * EnergyDrawer.chargeProgress, NPC.spriteDirection.ToSpriteDirection(), 0f);

                // Draw the bloom.
                EnergyDrawer.DrawBloom(coreSpritePosition);
            }

            EnergyDrawer.DrawPulses(coreSpritePosition);
            EnergyDrawer.DrawSet(coreSpritePosition);

            Main.spriteBatch.ResetToDefault();
        }

        public void DrawMagneticLine(NPC aresBody, Vector2 start, Vector2 end, float opacity = 1f)
        {
            if (!aresBody.TryGetDLCBehavior(out AresBodyEternity aresBodyBehavior) || aresBodyBehavior.SilhouetteOpacity > 0f)
                return;

            Vector2[] controlPoints = new Vector2[8];
            for (int i = 0; i < controlPoints.Length; i++)
                controlPoints[i] = Vector2.Lerp(start, end, i / 7f);

            if (!Main.gamePaused)
            {
                Vector2 distortionVelocity = (end - start).RotatedByRandom(0.4f) * 0.01f;
                ModContent.GetInstance<HeatDistortionMetaball>().CreateParticle(Vector2.Lerp(start, end, Main.rand.NextFloat(0.45f, 0.7f)), distortionVelocity, opacity * NPC.scale * 18f);
            }

            float magnetismWidthFunction(float completionRatio) => aresBody.Opacity * aresBody.scale * 12f;
            Color magnetismColorFunction(float completionRatio) => aresBody.GetAlpha(Color.Cyan) * opacity * 0.45f;

            ManagedShader magnetismShader = ShaderManager.GetShader("FargowiltasCrossmod.AresMagneticConnectionShader");
            magnetismShader.SetTexture(MiscTexturesRegistry.TurbulentNoise.Value, 1, SamplerState.PointWrap);

            PrimitiveSettings magnetismLineSettings = new(magnetismWidthFunction, magnetismColorFunction, Shader: magnetismShader);
            PrimitiveRenderer.RenderTrail(controlPoints, magnetismLineSettings, 24);

            Main.spriteBatch.ResetToDefault();
        }

        /// <summary>
        /// Draws this hand's arm.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="screenPosition"></param>
        public void DrawArm(SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            if (!CanRender)
                return;

            NPC aresBody = Main.npc[CalamityGlobalNPC.draedonExoMechPrime];

            if (UsesBackArm)
            {
                Vector2 shoulderDrawPosition = DrawBackArmShoulderAndArm(aresBody, spriteBatch, screenPosition);
                DrawBackArmForearm(aresBody, shoulderDrawPosition, spriteBatch, screenPosition);
            }
            else
            {
                Vector2 connectorDrawPosition = DrawFrontArmConnector(aresBody, spriteBatch, screenPosition);
                Vector2 elbowDrawPosition = DrawFrontArmArm(aresBody, connectorDrawPosition, spriteBatch, screenPosition);
                DrawFrontArmForearm(aresBody, elbowDrawPosition, spriteBatch, screenPosition);
            }
        }

        /// <summary>
        /// Draws the shoulder and arm of this hand's back arm.
        /// </summary>
        /// <param name="aresBody">Ares' body NPC instance.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="screenPosition">The position of the screen. Used for draw offsets.</param>
        /// <returns>The end position of the arm in screen space.</returns>
        public Vector2 DrawBackArmShoulderAndArm(NPC aresBody, SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            Texture2D shoulderTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresArmTopShoulder").Value;
            Texture2D shoulderPaddingTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmShoulder").Value;
            Texture2D shoulderPaddingTextureGlowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmShoulderGlow").Value;
            Vector2 shoulderDrawPosition = aresBody.Center + aresBody.scale * new Vector2(ArmSide * 164f, -54f).RotatedBy(aresBody.rotation) - screenPosition;
            Vector2 shoulderPaddingDrawPosition = aresBody.Center + aresBody.scale * new Vector2(ArmSide * 100f, -72f).RotatedBy(aresBody.rotation) - screenPosition;

            Color shoulderColor = aresBody.GetAlpha(Lighting.GetColor((shoulderDrawPosition + screenPosition).ToTileCoordinates()));
            Color shoulderPaddingColor = aresBody.GetAlpha(Lighting.GetColor((shoulderPaddingDrawPosition + screenPosition).ToTileCoordinates()));
            Rectangle shoulderFrame = shoulderTexture.Frame(1, 9, 0, (int)(Main.GlobalTimeWrappedHourly * 12f) % 9);
            Rectangle shoulderPadFrame = shoulderPaddingTexture.Frame(1, 9, 0, (int)(Main.GlobalTimeWrappedHourly * 12f) % 9);

            Vector2 armStart = shoulderDrawPosition + aresBody.scale * new Vector2(ArmSide * 22f, 2f);

            bool elbowPointsUp = (ArmSide == 1) ^ (ArmEndpoint.Y > armStart.Y);
            Texture2D armTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresArmTopPart1").Value;
            Texture2D forearmTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmPart2").Value;
            Vector2 elbowDrawPosition = Utilities.CalculateElbowPosition(armStart, ArmEndpoint - screenPosition, armTexture.Width * aresBody.scale, forearmTexture.Width * aresBody.scale * 1.2f, elbowPointsUp);
            Vector2 armOrigin = armTexture.Size() * new Vector2(0.81f, 0.66f);
            float shoulderRotation = aresBody.rotation;
            float armRotation = (elbowDrawPosition - armStart).ToRotation() + MathHelper.Pi + shoulderRotation;

            if (ArmSide == 1)
            {
                armRotation += MathHelper.Pi;
                armOrigin.X = armTexture.Width - armOrigin.X;
            }

            Color armColor = aresBody.GetAlpha(Lighting.GetColor((armStart + screenPosition).ToTileCoordinates()));
            Color glowmaskColor = aresBody.GetAlpha(Color.White);
            spriteBatch.Draw(armTexture, armStart, null, armColor, armRotation, armOrigin, NPC.scale, ArmSide.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(shoulderPaddingTexture, shoulderPaddingDrawPosition, shoulderPadFrame, shoulderPaddingColor, shoulderRotation, shoulderPadFrame.Size() * 0.5f, NPC.scale, ArmSide.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(shoulderPaddingTextureGlowmask, shoulderPaddingDrawPosition, shoulderPadFrame, glowmaskColor, shoulderRotation, shoulderPadFrame.Size() * 0.5f, NPC.scale, ArmSide.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(shoulderTexture, shoulderDrawPosition, shoulderFrame, shoulderColor, shoulderRotation, shoulderFrame.Size() * 0.5f, NPC.scale, ArmSide.ToSpriteDirection(), 0f);
            AresBodyEternity.DrawRGBGlowmask("AresArmTopShoulder", shoulderDrawPosition, glowmaskColor, shoulderRotation, NPC.scale, Vector2.One * 0.5f, ArmSide.ToSpriteDirection());

            ShoulderToHandDirection = (ArmEndpoint - screenPosition - elbowDrawPosition).ToRotation();

            Vector2 armEnd = armStart + armRotation.ToRotationVector2() * aresBody.scale * ArmSide * 92f;
            return armEnd;
        }

        /// <summary>
        /// Draws the forearm of this hand's front arm.
        /// </summary>
        /// <param name="aresBody">Ares' body NPC instance.</param>
        /// <param name="shoulderDrawPosition">The position of the shoulder in screen space.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="screenPosition">The position of the screen. Used for draw offsets.</param>
        public void DrawBackArmForearm(NPC aresBody, Vector2 shoulderDrawPosition, SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            Texture2D armSegmentTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresArmTopSegment").Value;
            Texture2D forearmTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresArmTopPart2").Value;
            Rectangle shoulderFrame = armSegmentTexture.Frame(1, 9, 0, (int)(Main.GlobalTimeWrappedHourly * 12f) % 9);
            Rectangle forearmFrame = forearmTexture.Frame(1, 9, 0, (int)(Main.GlobalTimeWrappedHourly * 12f) % 9);
            Vector2 forearmOrigin = forearmFrame.Size();

            Vector2 segmentDrawPosition = shoulderDrawPosition;
            Vector2 forearmDrawPosition = segmentDrawPosition;
            Color segmentColor = aresBody.GetAlpha(Lighting.GetColor((segmentDrawPosition + screenPosition).ToTileCoordinates()));
            Color glowmaskColor = aresBody.GetAlpha(Color.White);

            float segmentRotation = (ArmEndpoint - screenPosition - segmentDrawPosition).ToRotation();
            float forearmRotation = (ArmEndpoint - screenPosition - segmentDrawPosition).ToRotation() + MathHelper.Pi;
            if (ArmSide == 1)
            {
                forearmOrigin.X = forearmTexture.Width - forearmOrigin.X;
                segmentRotation += MathHelper.Pi;
                forearmRotation += MathHelper.Pi;
            }

            forearmDrawPosition += new Vector2(ArmSide * 20f, 16f).RotatedBy(forearmRotation) * aresBody.scale;

            Vector2 magnetismEnd = forearmDrawPosition + Main.screenPosition - new Vector2(-ArmSide, 0.3f).RotatedBy(forearmRotation) * aresBody.scale * 86f;
            DrawMagneticLine(aresBody, segmentDrawPosition + Main.screenPosition, magnetismEnd, NPC.Opacity.Cubed());
            if (AttachedToArm)
                DrawMagneticLine(aresBody, magnetismEnd - Vector2.UnitY.RotatedBy(forearmRotation) * aresBody.scale * 16f, ArmEndpoint, NPC.Opacity.Cubed());

            SpriteEffects direction = ArmSide.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally;
            AresBodyEternity.ApplyNormalMapShader(armSegmentTexture, shoulderFrame, true, true);
            spriteBatch.Draw(armSegmentTexture, segmentDrawPosition, shoulderFrame, segmentColor, segmentRotation, shoulderFrame.Size() * 0.5f, NPC.scale, direction, 0f);
            spriteBatch.Draw(forearmTexture, forearmDrawPosition, forearmFrame, segmentColor, forearmRotation, forearmOrigin, NPC.scale, direction, 0f);

            AresBodyEternity.DrawRGBGlowmask("AresArmTopSegment", segmentDrawPosition, glowmaskColor, segmentRotation, NPC.scale, Vector2.One * 0.5f, direction);
            AresBodyEternity.DrawRGBGlowmask("AresArmTopPart2", forearmDrawPosition, glowmaskColor, forearmRotation, NPC.scale, forearmOrigin / forearmFrame.Size(), direction);
        }

        /// <summary>
        /// Draws the connector of this hand's front arm.
        /// </summary>
        /// <param name="aresBody">Ares' body NPC instance.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="screenPosition">The position of the screen. Used for draw offsets.</param>
        /// <returns>The position of the connector in screen space.</returns>
        public Vector2 DrawFrontArmConnector(NPC aresBody, SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            Texture2D connectorTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmConnector").Value;
            Vector2 shoulderDrawPosition = aresBody.Center + aresBody.scale * new Vector2(ArmSide * 110f, -54f).RotatedBy(aresBody.rotation) - screenPosition;
            Vector2 connectorDrawPosition = shoulderDrawPosition + aresBody.scale * new Vector2(ArmSide * 4f, 32f).RotatedBy(aresBody.rotation);

            Color connecterColor = aresBody.GetAlpha(Lighting.GetColor((connectorDrawPosition + screenPosition).ToTileCoordinates()));

            spriteBatch.Draw(connectorTexture, connectorDrawPosition, null, connecterColor, 0f, connectorTexture.Size() * 0.5f, NPC.scale, ArmSide.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally, 0f);

            return connectorDrawPosition;
        }

        /// <summary>
        /// Draws the arm of this hand's front arm.
        /// </summary>
        /// <param name="aresBody">Ares' body NPC instance.</param>
        /// <param name="connectorDrawPosition">The position of the connector in screen space.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="screenPosition">The position of the screen. Used for draw offsets.</param>
        /// <returns>The position of the connector in screen space.</returns>
        public Vector2 DrawFrontArmArm(NPC aresBody, Vector2 connectorDrawPosition, SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            Vector2 armStart = connectorDrawPosition + aresBody.scale * new Vector2(ArmSide * 32f, -6f).RotatedBy(aresBody.rotation);

            Texture2D armTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmPart1").Value;
            Texture2D forearmTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmPart2").Value;

            bool elbowPointsUp = (ArmSide == 1) ^ (ArmEndpoint.Y > armStart.Y);
            Vector2 elbowDrawPosition = Utilities.CalculateElbowPosition(armStart, ArmEndpoint - screenPosition, armTexture.Width * aresBody.scale, forearmTexture.Width * aresBody.scale * 1.2f, elbowPointsUp);
            Rectangle armFrame = armTexture.Frame(1, 9, 0, (int)(Main.GlobalTimeWrappedHourly * 12f) % 9);
            Vector2 armOrigin = armFrame.Size() * new Vector2(0.81f, 0.66f);
            float armRotation = (elbowDrawPosition - armStart).ToRotation() + MathHelper.Pi;

            if (ArmSide == 1)
            {
                armRotation += MathHelper.Pi;
                armOrigin.X = armTexture.Width - armOrigin.X;
            }

            Vector2 magnetLineOffset = new Vector2(ArmSide * 50f, -22f).RotatedBy(armRotation) * NPC.scale + Main.screenPosition;
            DrawMagneticLine(aresBody, armStart + magnetLineOffset, elbowDrawPosition + magnetLineOffset);

            Color armColor = aresBody.GetAlpha(Lighting.GetColor((elbowDrawPosition + screenPosition).ToTileCoordinates()));
            Color glowmaskColor = aresBody.GetAlpha(Color.White);
            SpriteEffects direction = ArmSide.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally;

            AresBodyEternity.ApplyNormalMapShader(armTexture, armFrame, true, false);
            spriteBatch.Draw(armTexture, armStart, armFrame, armColor, armRotation, armOrigin, NPC.scale, direction, 0f);
            AresBodyEternity.DrawRGBGlowmask("AresBottomArmPart1", armStart, glowmaskColor, armRotation, NPC.scale, armOrigin / armFrame.Size(), direction);

            ShoulderToHandDirection = (ArmEndpoint - screenPosition - elbowDrawPosition).ToRotation();

            return elbowDrawPosition;
        }

        /// <summary>
        /// Draws the forearm of this hand's front arm.
        /// </summary>
        /// <param name="aresBody">Ares' body NPC instance.</param>
        /// <param name="elbowDrawPosition">The position of the elbow in screen space.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="screenPosition">The position of the screen. Used for draw offsets.</param>
        public void DrawFrontArmForearm(NPC aresBody, Vector2 elbowDrawPosition, SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            Vector2 armStart = elbowDrawPosition + aresBody.scale * new Vector2(ArmSide * 32f, -6f);

            Texture2D forearmTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmPart2").Value;
            Rectangle forearmFrame = forearmTexture.Frame(1, 9, 0, (int)(Main.GlobalTimeWrappedHourly * 12f) % 9);
            Vector2 forearmOrigin = forearmFrame.Size() * new Vector2(0.81f, 0.5f);
            float forearmRotation = (ArmEndpoint - screenPosition - armStart).ToRotation() + MathHelper.Pi;

            if (ArmSide == 1)
            {
                forearmRotation += MathHelper.Pi;
                forearmOrigin.X = forearmTexture.Width - forearmOrigin.X;
            }

            if (AttachedToArm)
                DrawMagneticLine(aresBody, armStart + Main.screenPosition, ArmEndpoint, NPC.Opacity.Cubed());

            SpriteEffects direction = ArmSide.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally;
            Color forearmColor = aresBody.GetAlpha(Lighting.GetColor((armStart + screenPosition).ToTileCoordinates()));
            Color glowmaskColor = aresBody.GetAlpha(Color.Wheat);

            AresBodyEternity.ApplyNormalMapShader(forearmTexture, forearmFrame, true, false);
            spriteBatch.Draw(forearmTexture, armStart, forearmFrame, forearmColor, forearmRotation, forearmOrigin, NPC.scale, direction, 0f);
            AresBodyEternity.DrawRGBGlowmask("AresBottomArmPart2", armStart, glowmaskColor, forearmRotation, NPC.scale, forearmOrigin / forearmFrame.Size(), direction);
        }

        /// <summary>
        /// Draws the katana on top of the actual energy katana.
        /// </summary>
        /// <param name="npc">The katana's NPC instance.</param>
        /// <param name="drawPosition">The draw position of the katana.</param>
        public static void DrawEnergyKatana(NPC npc, Vector2 drawPosition)
        {
            if (!npc.As<AresHand>().KatanaInUse)
                return;

            float appearanceInterpolant = npc.As<AresHand>().KatanaAppearanceInterpolant;
            float squishInterpolant = Utils.Remap(npc.position.Distance(npc.oldPosition), 30f, 50f, 0f, 0.6f);
            Vector2 bladeDrawPosition = drawPosition - npc.rotation.ToRotationVector2() * npc.scale * npc.spriteDirection * -48f;
            Vector2 bloomScale = new Vector2(1f, 1f + squishInterpolant * 2f) * npc.scale * appearanceInterpolant;
            Color bloomColor = Color.Lerp(Color.Crimson, Color.Wheat, squishInterpolant * 0.7f);
            SpriteEffects bladeDirection = npc.spriteDirection.ToSpriteDirection();

            float swordBloomRotation = npc.rotation - npc.As<AresHand>().ArmSide * 0.1f;
            Texture2D bloom = MiscTexturesRegistry.BloomCircleSmall.Value;
            Main.EntitySpriteDraw(bloom, bladeDrawPosition, null, npc.GetAlpha(bloomColor) with { A = 0 } * 0.6f, swordBloomRotation, bloom.Size() * new Vector2(0.25f, 0.5f), bloomScale * new Vector2(2.6f, appearanceInterpolant * 0.97f), bladeDirection, 0);
            Main.EntitySpriteDraw(bloom, bladeDrawPosition, null, npc.GetAlpha(bloomColor) with { A = 0 } * 0.7f, swordBloomRotation, bloom.Size() * new Vector2(0.25f, 0.5f), bloomScale * new Vector2(2.6f, appearanceInterpolant * 0.71f), bladeDirection, 0);
            Main.EntitySpriteDraw(bloom, bladeDrawPosition, null, npc.GetAlpha(Color.Red) with { A = 0 } * 0.7f, 0f, bloom.Size() * 0.5f, npc.scale, bladeDirection, 0);

            float katanaWidthFunction(float completionRatio) => npc.Opacity * npc.scale * MathHelper.Lerp(11f, 8f, squishInterpolant);
            Color katanaColorFunction(float completionRatio) => npc.GetAlpha(Color.Crimson);

            ManagedShader katanaShader = ShaderManager.GetShader("FargowiltasCrossmod.AresEnergyKatanaShader");
            katanaShader.TrySetParameter("flip", npc.As<AresHand>().ArmSide == 1);
            katanaShader.TrySetParameter("appearanceInterpolant", appearanceInterpolant);
            katanaShader.SetTexture(MiscTexturesRegistry.TurbulentNoise.Value, 1, SamplerState.PointWrap);

            PrimitiveSettings katanaPrimitiveSettings = new(katanaWidthFunction, katanaColorFunction, Shader: katanaShader);

            Vector2 katanaReach = npc.rotation.ToRotationVector2() * appearanceInterpolant * 274f;
            Vector2 orthogonalOffset = (npc.rotation + npc.As<AresHand>().ArmSide * -MathHelper.PiOver2).ToRotationVector2() * appearanceInterpolant * 30f;

            Vector2[] katanaPositions = new Vector2[8];
            for (int i = 0; i < katanaPositions.Length; i++)
            {
                float completionRatio = i / (float)(katanaPositions.Length - 1f);
                katanaPositions[i] = bladeDrawPosition + katanaReach * completionRatio + Main.screenPosition;
                katanaPositions[i] += orthogonalOffset * completionRatio.Squared();
            }

            PrimitiveRenderer.RenderTrail(katanaPositions, katanaPrimitiveSettings, 40);
        }

        /// <summary>
        /// The width function for slash afterimage trails used by Ares' hands when they're energy katanas.
        /// </summary>
        /// <param name="completionRatio">The completion ratio along the primitives.</param>
        public float EnergyKatanaAfterimageWidthFunction(float completionRatio)
        {
            return NPC.scale * Utilities.InverseLerp(0f, 0.23f, completionRatio) * 28f;
        }

        /// <summary>
        /// The color function for slash afterimage trails used by Ares' hands when they're energy katanas.
        /// </summary>
        /// <param name="completionRatio">The completion ratio along the primitives.</param>
        public Color EnergyKatanaAfterimageColorFunction(float completionRatio) => NPC.GetAlpha(Color.Red) * Utilities.InverseLerp(0.75f, 0.65f, completionRatio) * MathF.Pow(KatanaAfterimageOpacity, 0.3f) * NPC.scale;

        /// <summary>
        /// The width function for slash afterimage bloom trails used by Ares' hands when they're energy katanas.
        /// </summary>
        /// <param name="completionRatio">The completion ratio along the primitives.</param>
        public float EnergyKatanaBloomWidthFunction(float completionRatio) => EnergyKatanaAfterimageWidthFunction(completionRatio) * KatanaAfterimageOpacity * 2.5f;

        /// <summary>
        /// The color function for slash afterimage bloom trails used by Ares' hands when they're energy katanas.
        /// </summary>
        /// <param name="completionRatio">The completion ratio along the primitives.</param>
        public Color EnergyKatanaBloomColorFunction(float completionRatio) => NPC.GetAlpha(new(1f, 0.1f, 0f, 0f)) * Utilities.InverseLerp(0.86f, 0.6f, completionRatio) * MathF.Pow(KatanaAfterimageOpacity, 0.4f) * 0.56f;

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            if (KatanaAfterimageOpacity <= 0f || HandType != AresHandType.EnergyKatana)
                return;

            Vector2 forward = NPC.rotation.ToRotationVector2() * NPC.scale;
            Vector2[] controlPoints = new Vector2[12];
            for (int i = 0; i < controlPoints.Length; i++)
            {
                Vector2 a = NPC.oldPos[0] + forward * 256f;
                Vector2 b = NPC.oldPos[NPC.oldPos.Length / 2];
                Vector2 c = NPC.oldPos[^1];

                Vector2 drawPosition = Utilities.QuadraticBezier(a, b, c, (i / (float)(controlPoints.Length - 1f)).Squared());
                controlPoints[i] = Vector2.Lerp(drawPosition, a, 1f - KatanaAfterimageOpacity);
            }

            ManagedShader afterimageShader = ShaderManager.GetShader("FargowiltasCrossmod.AresEnergyKatanaAfterimage");
            afterimageShader.TrySetParameter("verticalFlip", ArmSide == -1);
            afterimageShader.SetTexture(MiscTexturesRegistry.DendriticNoiseZoomedOut.Value, 1, SamplerState.LinearWrap);

            PrimitiveSettings slashSettings = new(EnergyKatanaAfterimageWidthFunction, EnergyKatanaAfterimageColorFunction, _ => NPC.Size * 0.5f, true, true, afterimageShader);
            PrimitiveRenderer.RenderTrail(controlPoints, slashSettings, 34);
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (HandType is not null)
                typeName = Language.GetTextValue(HandType.NameLocalizationKey);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 3;
                SoundEngine.PlaySound(CommonCalamitySounds.ExoHitSound, NPC.Center);
            }

            // REALLY stupid hack to get the cannons to call CheckDead, since realLife having a defined value makes the CheckDead call propagate to the owner, instead of the
            // NPC that got killed.
            if (NPC.life <= 0)
            {
                NPC.life = 1;
                if (NPC.realLife.IsWithinBounds(Main.maxNPCs))
                    Main.npc[NPC.realLife].checkDead();
                NPC.realLife = -1;
            }

            if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
            {
                Mod calamity = ModCompatibility.Calamity.Mod;
                for (int i = 1; i <= 3; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, calamity.Find<ModGore>($"AresArm_Gore{i}").Type, NPC.scale);
                for (int i = 0; i < HandType.CustomGoreNames.Length; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>(HandType.CustomGoreNames[i]).Type, NPC.scale);
            }
        }

        /// <summary>
        /// Checks whether a melee hit counts against a given entity hitbox.
        /// </summary>
        /// <param name="potentialVictimHitbox">The hitbox to check collision against.</param>
        public bool MeleeHitCounts(Rectangle potentialVictimHitbox)
        {
            if (HandType != AresHandType.EnergyKatana)
                return NPC.Hitbox.Intersects(potentialVictimHitbox);

            float _ = 0f;
            Vector2 forward = NPC.rotation.ToRotationVector2() * NPC.scale;
            Vector2 start = NPC.Center + forward * 24f;
            Vector2 end = start + forward * 554f;
            return Collision.CheckAABBvLineCollision(potentialVictimHitbox.TopLeft(), potentialVictimHitbox.Size(), start, end, NPC.scale * 96f, ref _);
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            npcHitbox.Inflate(500, 500);
            immunityCooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => MeleeHitCounts(target.Hitbox);

        public override bool CanHitNPC(NPC target) => MeleeHitCounts(target.Hitbox);

        public override bool CheckActive() => false;

        public override bool CheckDead()
        {
            NPC.life = 1;
            NPC.dontTakeDamage = true;
            return false;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance * 0.8f);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }
    }
}
