using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public sealed class HadesBodyEternity : CalDLCEmodeBehavior, IHadesSegment
    {
        internal static LazyAsset<Texture2D>[] SpineTextures;

        internal static LazyAsset<Texture2D>[][] LeftPlatingTextures;

        internal static LazyAsset<Texture2D>[][] RightPlatingTextures;

        // This uses newAI[2] of all things because that happens to coincide with the immunity timer base Hades has, which makes incoming hits basically not matter if it hasn't exceeded a certain threshold.
        // By using it for the general existence timer, that guarantees that the immunity timer doesn't stay at zero 24/7 and effectively make Hades unable to be damaged.
        /// <summary>
        /// How long this body segment has existed, in frames.
        /// </summary>
        public ref float ExistenceTimer => ref NPC.Calamity().newAI[2];

        /// <summary>
        /// The spring force to apply to this segment this frame.
        /// </summary>
        public float SpringForce
        {
            get;
            set;
        }

        /// <summary>
        /// The dampening spring offset of this segment.
        /// </summary>
        public float SpringOffset
        {
            get;
            set;
        }

        /// <summary>
        /// How open this body segment is.
        /// </summary>
        public float SegmentOpenInterpolant
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this segment should attempt to reorient itself automatically.
        /// </summary>
        public bool ShouldReorientDirection
        {
            get;
            set;
        }

        /// <summary>
        /// How much this segment's plating should be offset.
        /// </summary>
        public float PlatingOffset
        {
            get;
            set;
        }

        /// <summary>
        /// How much the <see cref="PlatingOffset"/> should be changed.
        /// </summary>
        public float PlatingOffsetVelocity
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this segment should draw as a tail.
        /// </summary>
        public bool IsTailSegment => NPC.ai[1] == 1f;

        // Due to hardcoded logic in base Calamity's Thanatos code, the ahead segment index must be ai slot 2. Otherwise, you'll run into issues where the map icons get messed up.

        /// <summary>
        /// The index to the ahead segment in the NPC array.
        /// </summary>
        public int AheadSegmentIndex => (int)NPC.ai[2];

        /// <summary>
        /// Whether this segment should be drawn as the second body variant.
        /// </summary>
        public bool IsSecondaryBodySegment => RelativeIndex % 2 == 1;

        /// <summary>
        /// The index of this segment relative to the entire worm. A value of 0 corresponds to the first body segment, a value of 1 to the second, and so on.
        /// </summary>
        public int RelativeIndex => (int)NPC.ai[3];

        /// <summary>
        /// A generic countdown variable that can be used for whatever during AI states.
        /// </summary>
        public ref float GenericCountdown => ref NPC.ai[0];

        /// <summary>
        /// The position of the turret on this index.
        /// </summary>
        public Vector2 TurretPosition
        {
            get
            {
                float forwardOffset = IsSecondaryBodySegment ? 12f : 28f;
                Vector2 forward = (NPC.rotation - MathHelper.PiOver2).ToRotationVector2();
                return NPC.Center + forward * forwardOffset;
            }
        }

        /// <summary>
        /// How long body segments wait before executing any actual AI code.
        /// </summary>
        /// 
        /// <remarks>
        /// This exists to give a short window of time in multiplayer to allow for body segments to all spawn, so that if latency occurs with any of the segments the worm doesn't become just a head due to not having a valid ahead segment yet.
        /// </remarks>
        public static readonly int ActivationDelay = Utilities.SecondsToFrames(0.25f);

        public override int NPCOverrideID => ModContent.NPCType<ThanatosBody1>();

        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            SpineTextures = new LazyAsset<Texture2D>[2];

            LeftPlatingTextures = new LazyAsset<Texture2D>[2][];
            LeftPlatingTextures[0] = new LazyAsset<Texture2D>[4];
            LeftPlatingTextures[1] = new LazyAsset<Texture2D>[3];

            RightPlatingTextures = new LazyAsset<Texture2D>[2][];
            RightPlatingTextures[0] = new LazyAsset<Texture2D>[4];
            RightPlatingTextures[1] = new LazyAsset<Texture2D>[3];

            string platingPrefix = $"FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Hades/Plates";
            for (int i = 0; i < 4; i++)
            {
                LeftPlatingTextures[0][i] = LazyAsset<Texture2D>.Request($"{platingPrefix}/HadesBody1Part{i + 1}Left");
                RightPlatingTextures[0][i] = LazyAsset<Texture2D>.Request($"{platingPrefix}/HadesBody1Part{i + 1}Right");
            }
            for (int i = 0; i < 3; i++)
            {
                LeftPlatingTextures[1][i] = LazyAsset<Texture2D>.Request($"{platingPrefix}/HadesBody2Part{i + 1}Left");
                RightPlatingTextures[1][i] = LazyAsset<Texture2D>.Request($"{platingPrefix}/HadesBody2Part{i + 1}Right");
            }

            SpineTextures[0] = LazyAsset<Texture2D>.Request($"{platingPrefix}/HadesBody1Spine");
            SpineTextures[1] = LazyAsset<Texture2D>.Request($"{platingPrefix}/HadesBody2Spine");
        }

        public override bool PreAI()
        {
            ExistenceTimer++;
            if (ExistenceTimer <= ActivationDelay)
                return false;

            if (!ValidateAheadSegment(out bool segmentIsSimplyInactive))
            {
                // If the head is still present, that means that the worm was partially destroyed.
                // Correct for this.
                if (segmentIsSimplyInactive && CalamityGlobalNPC.draedonExoMechWorm != -1)
                {
                    Main.npc[AheadSegmentIndex].realLife = NPC.realLife;
                    Main.npc[AheadSegmentIndex].life = 1;
                    Main.npc[AheadSegmentIndex].active = true;
                    return false;
                }

                NPC.active = false;
                return false;
            }

            if (NPC.realLife < 0 || NPC.realLife >= Main.maxNPCs || !Main.npc[NPC.realLife].TryGetDLCBehavior(out HadesHeadEternity head))
            {
                if (CalamityGlobalNPC.draedonExoMechWorm != -1)
                {
                    NPC.realLife = CalamityGlobalNPC.draedonExoMechWorm;
                    return false;
                }

                NPC.active = false;
                return false;
            }

            NPC aheadSegment = Main.npc[AheadSegmentIndex];
            DecidePositionAndRotation(aheadSegment, head.SegmentReorientationStrength);

            // Hack to ensure that segments retain Hades' secondary AI state, and thusly use the correct map icon.
            NPC.Calamity().newAI[1] = aheadSegment.Calamity().newAI[1];
            NPC.defDamage = HadesHeadEternity.DefaultSegmentDamage;
            NPC.damage = 0;
            NPC.Opacity = aheadSegment.Opacity;
            NPC.life = aheadSegment.lifeMax;
            NPC.dontTakeDamage = aheadSegment.dontTakeDamage;
            NPC.velocity *= 0.84f;
            ShouldReorientDirection = true;

            PlatingOffset += PlatingOffsetVelocity;
            PlatingOffsetVelocity = MathHelper.Lerp(PlatingOffsetVelocity, -PlatingOffset, 0.15f);
            if (PlatingOffset < 0.1f)
            {
                PlatingOffset = 0f;
                PlatingOffsetVelocity *= 0.3f;
            }

            // Apply spring effects.
            SpringOffset += SpringForce;
            SpringOffset *= 0.88f;
            SpringForce *= 0.7f;
            if (RelativeIndex >= 1 && aheadSegment.TryGetDLCBehavior(out HadesBodyEternity aheadSegmentBehavior))
                aheadSegmentBehavior.SpringOffset = MathHelper.Lerp(aheadSegmentBehavior.SpringOffset, SpringOffset, 0.99f);

            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "vulnerable")]
            extern static ref bool GetVulnerableField(ThanatosBody1 body);
            ref bool vulernable = ref GetVulnerableField(NPC.As<ThanatosBody1>());
            vulernable = SegmentOpenInterpolant >= 0.75f;

            ListenToHeadInstructions();
            ModifyDRBasedOnOpenInterpolant();

            if (GenericCountdown > 0f)
                GenericCountdown--;
            return false;
        }

        /// <summary>
        /// Decides the position and rotation for this segment.
        /// </summary>
        /// <param name="aheadSegment">The ahead segment relative to this one.</param>
        /// <param name="segmentReorientationStrength">The segment reorientation strength. This dictates how rigidly segments reorient into the same direction as the head.</param>
        public void DecidePositionAndRotation(NPC aheadSegment, float segmentReorientationStrength)
        {
            float offsetPerSegment = NPC.scale * 76f;

            if (RelativeIndex == 0)
            {
                NPC.rotation = aheadSegment.rotation;
                NPC.Center = aheadSegment.Center - (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * offsetPerSegment + aheadSegment.velocity;
            }

            if (RelativeIndex >= 1)
            {
                Vector2 directionalBearing = Vector2.Zero;

                // Apply secondary snaking effects.
                if (RelativeIndex >= 2 && aheadSegment.TryGetDLCBehavior(out HadesBodyEternity aheadSegmentBehavior))
                {
                    NPC aheadSegment2 = Main.npc[aheadSegmentBehavior.AheadSegmentIndex];
                    directionalBearing = (aheadSegment.Center - aheadSegment2.Center) * segmentReorientationStrength * 0.055f;
                }

                Vector2 directionToAheadSegment = (NPC.Center - aheadSegment.Center).SafeNormalize(Vector2.Zero);
                directionalBearing += directionToAheadSegment.RotatedBy(MathHelper.PiOver2) * SpringOffset;

                NPC.Center = aheadSegment.Center + (directionToAheadSegment + directionalBearing).SafeNormalize(Vector2.Zero) * offsetPerSegment;
                NPC.rotation = NPC.AngleTo(aheadSegment.Center) + MathHelper.PiOver2;
            }
        }

        public override void BossHeadSlot(ref int index)
        {
            if (IsSecondaryBodySegment)
                index = SegmentOpenInterpolant >= 0.75f ? ThanatosBody2.vulnerableIconIndex : ThanatosBody2.normalIconIndex;
            if (IsTailSegment)
                index = SegmentOpenInterpolant >= 0.75f ? ThanatosTail.vulnerableIconIndex : ThanatosTail.normalIconIndex;

            if (NPC.realLife >= 0 && Main.npc[NPC.realLife].TryGetDLCBehavior(out HadesHeadEternity hades) && hades.DisableMapIcon)
                index = -1;
        }

        /// <summary>
        /// Listens to incoming instructions from the head's <see cref="HadesHeadEternity.BodyBehaviorAction"/>.
        /// </summary>
        public void ListenToHeadInstructions()
        {
            if (CalamityGlobalNPC.draedonExoMechWorm == -1)
                return;

            NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];
            if (!hades.TryGetDLCBehavior(out HadesHeadEternity hadesAI))
                return;

            if (!hadesAI.BodyBehaviorAction?.Condition(NPC, RelativeIndex) ?? false)
                return;

            if (hades.damage <= 0)
                NPC.damage = 0;

            hadesAI.BodyBehaviorAction?.Action(this);
        }

        /// <summary>
        /// Listens to incoming instructions from the head's <see cref="HadesHeadEternity.BodyRenderAction"/> that dictate optional draw data.
        /// </summary>
        public void RenderInAccordanceWithHeadInstructions()
        {
            if (CalamityGlobalNPC.draedonExoMechWorm == -1)
                return;

            NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];
            if (!hades.TryGetDLCBehavior(out HadesHeadEternity hadesAI))
                return;

            if (!hadesAI.BodyRenderAction?.Condition(NPC, RelativeIndex) ?? false)
                return;

            hadesAI.BodyRenderAction?.Action(this);
        }

        /// <summary>
        /// Modifies the DR of this segment in accordance with the <see cref="SegmentOpenInterpolant"/> value.
        /// </summary>
        public void ModifyDRBasedOnOpenInterpolant()
        {
            float damageReduction = MathHelper.SmoothStep(0.9999f, HadesHeadEternity.StandardOpenSegmentDR, SegmentOpenInterpolant);
            CalamityGlobalNPC globalNPC = NPC.Calamity();
            globalNPC.unbreakableDR = damageReduction >= 0.999f;
            globalNPC.DR = damageReduction;
            NPC.chaseable = !globalNPC.unbreakableDR;
        }

        /// <summary>
        /// Validates whether the ahead segment is valid.
        /// </summary>
        /// <returns>Whether this segment can still exist due to having a valid ahead segment.</returns>
        public bool ValidateAheadSegment(out bool segmentIsSimplyInactive)
        {
            segmentIsSimplyInactive = false;
            if (!Main.npc.IndexInRange(AheadSegmentIndex))
                return false;

            NPC aheadSegment = Main.npc[AheadSegmentIndex];
            if (aheadSegment.type != NPCOverrideID && aheadSegment.type != ExoMechNPCIDs.HadesHeadID)
                return false;

            segmentIsSimplyInactive = true;
            if (!aheadSegment.active)
                return false;

            segmentIsSimplyInactive = false;
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // REALLY stupid hack to get the cannons to call CheckDead, since realLife having a defined value makes the CheckDead call propagate to the owner, instead of the
            // NPC that got killed.
            if (NPC.life <= 0)
            {
                NPC.life = NPC.lifeMax;
                NPC.dontTakeDamage = true;

                if (NPC.realLife >= 0 && NPC.realLife < Main.maxNPCs)
                {
                    Main.npc[NPC.realLife].checkDead();
                    NPC.realLife = -1;
                }
            }

            if (Main.netMode == NetmodeID.Server || NPC.life >= 1)
                return;

            if (IsSecondaryBodySegment)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("CalamityMod/ThanatosBody2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("CalamityMod/ThanatosBody2_2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("CalamityMod/ThanatosBody2_3").Type, NPC.scale);
            }
            else if (IsTailSegment)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("CalamityMod/ThanatosTail1").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("CalamityMod/ThanatosTail2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("CalamityMod/ThanatosTail3").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("CalamityMod/ThanatosTail4").Type, NPC.scale);
            }

            else
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("CalamityMod/ThanatosBody1").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("CalamityMod/ThanatosBody1_2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("CalamityMod/ThanatosBody1_3").Type, NPC.scale);
            }
        }

        public override bool CheckDead()
        {
            NPC.life = 1;
            NPC.dontTakeDamage = true;
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            int frame = Utils.Clamp((int)(SegmentOpenInterpolant * Main.npcFrameCount[NPC.type]), 0, Main.npcFrameCount[NPC.type] - 1);
            NPC.frame.Y = frame * frameHeight;
        }

        public override void ModifyTypeName(ref string typeName)
        {
            typeName = Language.GetTextValue("Mods.FargowiltasCrossmod.NPCs.ThanatosRename");
        }

        public void DrawManualBodyPlating(Vector2 drawPosition, Color lightColor)
        {
            int textureIndex = IsSecondaryBodySegment.ToInt();
            Color color = NPC.GetAlpha(lightColor);
            SpriteEffects direction = NPC.spriteDirection.ToSpriteDirection();
            Texture2D spine = SpineTextures[textureIndex].Value;
            Texture2D leftPlating1 = LeftPlatingTextures[textureIndex][0].Value;
            Texture2D rightPlating1 = RightPlatingTextures[textureIndex][0].Value;
            Texture2D leftPlating2 = LeftPlatingTextures[textureIndex][1].Value;
            Texture2D rightPlating2 = RightPlatingTextures[textureIndex][1].Value;
            Texture2D leftPlating3 = LeftPlatingTextures[textureIndex][2].Value;
            Texture2D rightPlating3 = RightPlatingTextures[textureIndex][2].Value;

            Vector2 Transform(Vector2 offset) => (offset * new Vector2(NPC.spriteDirection, 1f)).RotatedBy(NPC.rotation);

            if (!IsSecondaryBodySegment)
            {
                Texture2D leftPlating4 = LeftPlatingTextures[textureIndex][3].Value;
                Texture2D rightPlating4 = RightPlatingTextures[textureIndex][3].Value;

                Main.spriteBatch.Draw(leftPlating1, drawPosition + Transform(new(-13f - PlatingOffset, 20f)), null, color, NPC.rotation, leftPlating1.Size() * 0.5f, NPC.scale, direction, 0f);
                Main.spriteBatch.Draw(rightPlating1, drawPosition + Transform(new(15f + PlatingOffset, 19f)), null, color, NPC.rotation, rightPlating1.Size() * 0.5f, NPC.scale, direction, 0f);

                Main.spriteBatch.Draw(spine, drawPosition, null, color, NPC.rotation, spine.Size() * 0.5f, NPC.scale, direction, 0f);

                Main.spriteBatch.Draw(leftPlating2, drawPosition + Transform(new(-15f - PlatingOffset, 4f - PlatingOffset * 0.85f)), null, color, NPC.rotation, leftPlating2.Size() * 0.5f, NPC.scale, direction, 0f);
                Main.spriteBatch.Draw(rightPlating2, drawPosition + Transform(new(15f + PlatingOffset, 4f - PlatingOffset * 0.85f)), null, color, NPC.rotation, rightPlating2.Size() * 0.5f, NPC.scale, direction, 0f);

                Main.spriteBatch.Draw(leftPlating3, drawPosition + Transform(new(-31f - PlatingOffset * 0.5f, 13f)), null, color, NPC.rotation, leftPlating3.Size() * 0.5f, NPC.scale, direction, 0f);
                Main.spriteBatch.Draw(rightPlating3, drawPosition + Transform(new(33f + PlatingOffset * 0.5f, 13f)), null, color, NPC.rotation, rightPlating3.Size() * 0.5f, NPC.scale, direction, 0f);

                Main.spriteBatch.Draw(leftPlating4, drawPosition + Transform(new(-43f - PlatingOffset * 0.5f, -17f)), null, color, NPC.rotation, leftPlating4.Size() * 0.5f, NPC.scale, direction, 0f);
                Main.spriteBatch.Draw(rightPlating4, drawPosition + Transform(new(41f + PlatingOffset * 0.5f, -17f)), null, color, NPC.rotation, rightPlating4.Size() * 0.5f, NPC.scale, direction, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(spine, drawPosition, null, color, NPC.rotation, spine.Size() * 0.5f, NPC.scale, direction, 0f);

                Main.spriteBatch.Draw(leftPlating1, drawPosition + Transform(new(-11f - PlatingOffset, 33f)), null, color, NPC.rotation, leftPlating1.Size() * 0.5f, NPC.scale, direction, 0f);
                Main.spriteBatch.Draw(rightPlating1, drawPosition + Transform(new(11f + PlatingOffset, 33f)), null, color, NPC.rotation, rightPlating1.Size() * 0.5f, NPC.scale, direction, 0f);

                Main.spriteBatch.Draw(leftPlating2, drawPosition + Transform(new(-20f - PlatingOffset, 13f - PlatingOffset * 0.85f)), null, color, NPC.rotation, leftPlating2.Size() * 0.5f, NPC.scale, direction, 0f);
                Main.spriteBatch.Draw(rightPlating2, drawPosition + Transform(new(19f + PlatingOffset, 13f - PlatingOffset * 0.85f)), null, color, NPC.rotation, rightPlating2.Size() * 0.5f, NPC.scale, direction, 0f);

                Main.spriteBatch.Draw(leftPlating3, drawPosition + Transform(new(-36f - PlatingOffset * 0.5f, -13f)), null, color, NPC.rotation, leftPlating3.Size() * 0.5f, NPC.scale, direction, 0f);
                Main.spriteBatch.Draw(rightPlating3, drawPosition + Transform(new(36f + PlatingOffset * 0.5f, -13f)), null, color, NPC.rotation, rightPlating3.Size() * 0.5f, NPC.scale, direction, 0f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosBody1Glow").Value;
            Texture2D bloom = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            if (IsSecondaryBodySegment)
            {
                int body2ID = ModContent.NPCType<ThanatosBody2>();
                Main.instance.LoadNPC(body2ID);
                texture = TextureAssets.Npc[body2ID].Value;
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosBody2Glow").Value;
            }
            if (IsTailSegment)
            {
                int tailID = ModContent.NPCType<ThanatosTail>();
                Main.instance.LoadNPC(tailID);
                texture = TextureAssets.Npc[tailID].Value;
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosTailGlow").Value;
            }

            Vector2 drawPosition = NPC.Center - screenPos;
            int frame = NPC.frame.Y / NPC.frame.Height;
            float glowmaskOpacity = LumUtils.InverseLerp(5f, 15f, (lightColor.R + lightColor.G + lightColor.B) * 0.333f);
            Rectangle rectangleFrame = texture.Frame(1, Main.npcFrameCount[NPC.type], 0, frame);

            if (!IsTailSegment && PlatingOffset > 0f)
                DrawManualBodyPlating(drawPosition, lightColor);
            else
            {
                Color glowmaskColor = Color.White * glowmaskOpacity;
                Main.spriteBatch.Draw(texture, drawPosition, rectangleFrame, NPC.GetAlpha(lightColor), NPC.rotation, rectangleFrame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection(), 0f);
                Main.spriteBatch.Draw(glowmask, drawPosition, rectangleFrame, NPC.GetAlpha(glowmaskColor), NPC.rotation, rectangleFrame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection(), 0f);
            }

            float bloomOpacity = SegmentOpenInterpolant.Squared() * glowmaskOpacity * 0.56f;
            Vector2 bloomDrawPosition = TurretPosition - screenPos;
            Main.spriteBatch.Draw(bloom, bloomDrawPosition, null, NPC.GetAlpha(Color.Red with { A = 0 }) * bloomOpacity, 0f, bloom.Size() * 0.5f, NPC.scale * 0.4f, 0, 0f);
            Main.spriteBatch.Draw(bloom, bloomDrawPosition, null, NPC.GetAlpha(Color.Wheat with { A = 0 }) * bloomOpacity * 0.5f, 0f, bloom.Size() * 0.5f, NPC.scale * 0.2f, 0, 0f);

            RenderInAccordanceWithHeadInstructions();

            return false;
        }
    }
}
