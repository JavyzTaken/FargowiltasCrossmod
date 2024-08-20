using CalamityMod;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Assets.Particles.Metaballs;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public sealed partial class HadesHeadEternity : CalDLCEmodeBehavior, IHadesSegment, IExoMech
    {
        public enum HadesAIState
        {
            SpawnAnimation,
            PerpendicularBodyLaserBlasts,
            ContinuousLaserBarrage,
            MineBarrages,
            ExoEnergyBlast,
            MissileLunges,
            MissileTailSnaps,
            Inactive,
            Leave,

            DeathAnimation,

            PerformComboAttack = ExoMechComboAttackManager.ComboAttackValue
        }

        /// <summary>
        /// Whether Hades is currently performing a combo attack.
        /// </summary>
        public bool PerformingComboAttack
        {
            get => CurrentState == HadesAIState.PerformComboAttack;
            set
            {
                if (CurrentState == HadesAIState.Leave)
                    return;

                SelectNewState();
                if (value)
                    CurrentState = HadesAIState.PerformComboAttack;
            }
        }

        /// <summary>
        /// Whether Hades should be inactive, leaving the battle to let other mechs attack on their own.
        /// </summary>
        public bool Inactive
        {
            get;
            set;
        }

        /// <summary>
        /// Whether Hades is a primary mech or not, a.k.a the one that the player chose when starting the battle.
        /// </summary>
        public bool IsPrimaryMech
        {
            get;
            set;
        }

        /// <summary>
        /// Hades' current, non-combo state.
        /// </summary>
        public HadesAIState CurrentState
        {
            get;
            set;
        }

        /// <summary>
        /// Hades' current AI timer.
        /// </summary>
        public int AITimer
        {
            get;
            set;
        }

        /// <summary>
        /// Whether Hades has created his body segments yet or not.
        /// </summary>
        public bool HasCreatedSegments
        {
            get;
            set;
        }

        /// <summary>
        /// How open this head segment is.
        /// </summary>
        public float SegmentOpenInterpolant
        {
            get;
            set;
        }

        /// <summary>
        /// The opacity of the reticle on the player.
        /// </summary>
        public float ReticleOpacity
        {
            get;
            set;
        }

        /// <summary>
        /// The strength of segment reorientation across Hades.
        /// </summary>
        public float SegmentReorientationStrength
        {
            get;
            set;
        }

        /// <summary>
        /// The action Hades should perform either after his AI state execution is complete, or after combo attack puppeteering, assuming he's performing a combo attack.
        /// </summary>
        public Action ActionsToDeferAfterCombo
        {
            get;
            set;
        }

        /// <summary>
        /// The action that should be taken by body segments.
        /// </summary>
        /// 
        /// <remarks>
        /// This value can be (and usually is) null. When it is, nothing special is performed by the body segments.
        /// </remarks>
        public BodySegmentInstructions? BodyBehaviorAction
        {
            get;
            set;
        }

        /// <summary>
        /// The render action that should be taken by body segments.
        /// </summary>
        /// 
        /// <remarks>
        /// This value can be (and usually is) null. When it is, nothing special is performed by the body segments.
        /// </remarks>
        public BodySegmentInstructions? BodyRenderAction
        {
            get;
            set;
        }

        /// <summary>
        /// Unimplemented, since Hades' head doesn't have an ahead segment. Do not use.
        /// </summary>
        public int AheadSegmentIndex => throw new NotImplementedException();

        /// <summary>
        /// The rotation of Hades' jaws.
        /// </summary>
        public ref float JawRotation => ref NPC.localAI[0];

        /// <summary>
        /// The amount of damage Hades' segments do.
        /// </summary>
        public static int DefaultSegmentDamage => Main.expertMode ? 300 : 200;

        /// <summary>
        /// The target that Hades will attempt to attack.
        /// </summary>
        public static Player Target => ExoMechTargetSelector.Target;

        /// <summary>
        /// The amount of body segments Hades spawns with.
        /// </summary>
        public const int BodySegmentCount = 50;

        /// <summary>
        /// The standard segment opening rate from <see cref="OpenSegment(float)"/>.
        /// </summary>
        public const float StandardSegmentOpenRate = 0.0285f;

        /// <summary>
        /// The standard segment closing rate from <see cref="CloseSegment(float)"/>.
        /// </summary>
        public const float StandardSegmentCloseRate = 0.067f;

        /// <summary>
        /// The amount of DR that body and tail segments should have when opened.
        /// </summary>
        public const float StandardOpenSegmentDR = 0.27f;

        /// <summary>
        /// Represents an action that should be performed by segments on Hades' body.
        /// </summary>
        /// <param name="behaviorOverride">The segment's overriding instance.</param>
        public delegate void BodySegmentAction(HadesBodyEternity behaviorOverride);

        /// <summary>
        /// Represents a condition that should be applied to Hades' body segments.
        /// </summary>
        /// <param name="segment">The segment's NPC instance.</param>
        /// <param name="segmentIndex">The index of the segment being evaluated.</param>
        public delegate bool BodySegmentCondition(NPC segment, int segmentIndex);

        /// <summary>
        /// Represents an action that should be taken conditionally across Hades' body segments.
        /// </summary>
        /// <param name="Condition">The condition that dictates whether the <paramref name="Action"/> should occur.</param>
        /// <param name="Action">The action that the body segments should perform.</param>
        public record BodySegmentInstructions(BodySegmentCondition Condition, BodySegmentAction Action);

        public override int NPCOverrideID => ExoMechNPCIDs.HadesHeadID;

        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(HasCreatedSegments);
            bitWriter.WriteBit(Inactive);
            bitWriter.WriteBit(IsPrimaryMech);

            binaryWriter.Write(SegmentOpenInterpolant);
            binaryWriter.Write(SegmentReorientationStrength);
            binaryWriter.Write(AITimer);
            binaryWriter.Write((int)CurrentState);
        }

        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            HasCreatedSegments = bitReader.ReadBit();
            Inactive = bitReader.ReadBit();
            IsPrimaryMech = bitReader.ReadBit();

            SegmentOpenInterpolant = binaryReader.ReadSingle();
            SegmentReorientationStrength = binaryReader.ReadSingle();
            AITimer = binaryReader.ReadInt32();
            CurrentState = (HadesAIState)binaryReader.ReadInt32();
        }

        public override bool PreAI()
        {
            PerformPreUpdateResets();

            if (CurrentState != HadesAIState.Leave)
            {
                if (Inactive && CurrentState != HadesAIState.Inactive)
                {
                    CurrentState = HadesAIState.Inactive;
                    AITimer = 0;
                    NPC.netUpdate = true;
                }
                if (!Inactive && CurrentState == HadesAIState.Inactive)
                {
                    CurrentState = HadesAIState.MineBarrages;
                    AITimer = 0;
                    NPC.netUpdate = true;
                }
            }

            // Leave if the player is dead.
            if ((Target.dead || !Target.active) && CurrentState != HadesAIState.Leave)
            {
                SelectNewState();
                CurrentState = HadesAIState.Leave;
            }

            if (!HasCreatedSegments)
            {
                CreateSegments();
                HasCreatedSegments = true;
                NPC.netUpdate = true;
            }

            ExecuteCurrentState();

            if (CurrentState != HadesAIState.PerformComboAttack)
                ActionsToDeferAfterCombo?.Invoke();

            NPC.Opacity = 1f;
            NPC.Calamity().ShouldCloseHPBar = Inactive || CurrentState == HadesAIState.DeathAnimation;

            AITimer++;
            return false;
        }

        /// <summary>
        /// Resets various things pertaining to the fight state prior to behavior updates.
        /// </summary>
        /// <remarks>
        /// This serves as a means of ensuring that changes to the fight state are gracefully reset if something suddenly changes, while affording the ability to make changes during updates.<br></br>
        /// As a result, this alleviates behaviors AI states from the burden of having to assume that they may terminate at any time and must account for that to ensure that the state is reset.
        /// </remarks>
        public void PerformPreUpdateResets()
        {
            if (CurrentState != HadesAIState.PerformComboAttack)
                NPC.damage = NPC.defDamage;

            NPC.defense = NPC.defDefense;
            NPC.dontTakeDamage = false;
            NPC.ShowNameOnHover = true;
            NPC.HitSound = SegmentOpenInterpolant >= 0.75f ? ThanatosHead.ThanatosHitSoundOpen : ThanatosHead.ThanatosHitSoundClosed;
            ActionsToDeferAfterCombo = null;
            BodyBehaviorAction = null;
            BodyRenderAction = null;
            SegmentReorientationStrength = 1f;
            NPC.As<ThanatosHead>().SecondaryAIState = (int)ThanatosHead.SecondaryPhase.Nothing;
            SegmentOpenInterpolant = Utilities.Saturate(SegmentOpenInterpolant - StandardSegmentOpenRate);
            JawRotation = JawRotation.AngleLerp(0f, 0.01f).AngleTowards(0f, 0.02f);
            ReticleOpacity = MathHelper.Lerp(ReticleOpacity, 0f, 0.1f);

            CalamityGlobalNPC.draedonExoMechWorm = NPC.whoAmI;
        }

        /// <summary>
        /// Creates a total of <see cref="BodySegmentCount"/> segments that attach to each other similar to a linked list.
        /// </summary>
        public void CreateSegments()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            // The tail and body2 NPC variants are unused in favor of varied drawing so as to minimize the amount of code generalizations/copy-pasting necessary to get Hades working.
            int segmentID = ModContent.NPCType<ThanatosBody1>();
            int previousSegmentIndex = NPC.whoAmI;
            for (int i = 0; i < BodySegmentCount; i++)
            {
                bool tailSegment = i == BodySegmentCount - 1;
                int nextSegmentIndex = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, segmentID, NPC.whoAmI + 1, NPC.whoAmI, tailSegment.ToInt(), previousSegmentIndex, i);

                NPC nextSegment = Main.npc[nextSegmentIndex];
                nextSegment.realLife = NPC.whoAmI;

                // Immediately inform all clients of the spawning of the body segment so that there's a little latency as possible.
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, nextSegmentIndex);

                previousSegmentIndex = nextSegmentIndex;
            }
        }

        /// <summary>
        /// Performs Hades' current state.
        /// </summary>
        public void ExecuteCurrentState()
        {
            switch (CurrentState)
            {
                case HadesAIState.SpawnAnimation:
                    DoBehavior_SpawnAnimation();
                    break;
                case HadesAIState.PerpendicularBodyLaserBlasts:
                    DoBehavior_PerpendicularBodyLaserBlasts();
                    break;
                case HadesAIState.ContinuousLaserBarrage:
                    DoBehavior_ContinuousLaserBarrage();
                    break;
                case HadesAIState.MineBarrages:
                    DoBehavior_MineBarrages();
                    break;
                case HadesAIState.MissileLunges:
                    DoBehavior_MissileLunges();
                    break;
                case HadesAIState.MissileTailSnaps:
                    DoBehavior_MissileTailSnaps();
                    break;
                case HadesAIState.ExoEnergyBlast:
                    DoBehavior_ExoEnergyBlast();
                    break;
                case HadesAIState.Inactive:
                    DoBehavior_Inactive();
                    break;
                case HadesAIState.Leave:
                    DoBehavior_Leave();
                    break;
                case HadesAIState.DeathAnimation:
                    DoBehavior_DeathAnimation();
                    break;
                case HadesAIState.PerformComboAttack:
                    AITimer = ExoMechComboAttackManager.ComboAttackTimer;
                    break;
            }
        }

        /// <summary>
        /// Chains a new action to <see cref="ActionsToDeferAfterCombo"/>.
        /// </summary>
        /// <param name="action">The action to chain.</param>
        public void DeferForComboAttack(Action action) => ActionsToDeferAfterCombo += action;

        /// <summary>
        /// Selects a new state for Hades.
        /// </summary>
        public void SelectNewState()
        {
            HadesAIState oldState = CurrentState;
            do
            {
                CurrentState = Main.rand.NextFromList(HadesAIState.ContinuousLaserBarrage, HadesAIState.MineBarrages, HadesAIState.PerpendicularBodyLaserBlasts, HadesAIState.MissileLunges, HadesAIState.MissileTailSnaps);

                if (Main.rand.NextBool() && ExoMechFightStateManager.CurrentPhase >= ExoMechFightDefinitions.BerserkSoloPhaseDefinition)
                    CurrentState = HadesAIState.ExoEnergyBlast;

                // Ensure that the continuous laser barrage attack does not occur after the missile lunges attack.
                // Testing revealed that this attack combination can result in unavoidable damage if the player is near the ground due to Hades' body serving as a damaging shield that the player would need to RoD through to escape.
                if (CurrentState == HadesAIState.ContinuousLaserBarrage && oldState == HadesAIState.MissileLunges)
                    CurrentState = HadesAIState.PerpendicularBodyLaserBlasts;
            }
            while (CurrentState == oldState);

            for (int i = 0; i < NPC.maxAI; i++)
                NPC.ai[i] = 0f;

            AITimer = 0;
            NPC.netUpdate = true;
        }

        /// <summary>
        /// Generates a <see cref="BodySegmentCondition"/> that corresponds to every Nth segment. Meant to be used in conjunction with <see cref="BodyBehaviorAction"/>.
        /// </summary>
        /// <param name="n">The cycle repeat value.</param>
        /// <param name="cyclicOffset">The offset in the cycle. Defaults to 0.</param>
        public static BodySegmentCondition EveryNthSegment(int n, int cyclicOffset = 0) => new((segment, segmentIndex) => segmentIndex % n == cyclicOffset);

        /// <summary>
        /// Generates a <see cref="BodySegmentCondition"/> that corresponds only to the tail segment. Meant to be used in conjunction with <see cref="BodyBehaviorAction"/>.
        /// </summary>
        /// <param name="n">The cycle repeat value.</param>
        /// <param name="cyclicOffset">The offset in the cycle. Defaults to 0.</param>
        public static BodySegmentCondition OnlyTailSegment() => new((segment, segmentIndex) => segmentIndex == BodySegmentCount - 1);

        /// <summary>
        /// Generates a <see cref="BodySegmentCondition"/> that corresponds to every single segment. Meant to be used in conjunction with <see cref="BodyBehaviorAction"/>.
        /// </summary>
        public static BodySegmentCondition AllSegments() => new((segment, segmentIndex) => true);

        /// <summary>
        /// An action that opens a segment's vents and creates smoke. Meant to be used in conjunction with <see cref="BodyBehaviorAction"/>.
        /// </summary>
        /// <param name="segmentOpenRate">The amount by which the segment open interpolant changes every frame.</param>
        /// <param name="smokeQuantityInterpolant">A multiplier for how much smoke should be released.</param>
        /// <param name="enableContactDamage">Whether contact damage should be activated or not.</param>
        public static BodySegmentAction OpenSegment(float segmentOpenRate = StandardSegmentOpenRate, float smokeQuantityInterpolant = 1f, bool enableContactDamage = false)
        {
            return new(behaviorOverride =>
            {
                float oldInterpolant = behaviorOverride.SegmentOpenInterpolant;
                behaviorOverride.SegmentOpenInterpolant = Utilities.Saturate(behaviorOverride.SegmentOpenInterpolant + segmentOpenRate);

                bool segmentJustOpened = behaviorOverride.SegmentOpenInterpolant > 0f && oldInterpolant <= 0f;
                if (segmentJustOpened)
                    SoundEngine.PlaySound(ThanatosHead.VentSound with { MaxInstances = 8, Volume = 0.3f }, behaviorOverride.NPC.Center);

                float bigInterpolant = Utilities.InverseLerp(1f, 0.91f, behaviorOverride.SegmentOpenInterpolant);
                if (behaviorOverride.SegmentOpenInterpolant >= 0.91f && !Collision.SolidCollision(behaviorOverride.NPC.TopLeft, behaviorOverride.NPC.width, behaviorOverride.NPC.height))
                {
                    CreateSmoke(behaviorOverride, bigInterpolant, smokeQuantityInterpolant);

                    if (Main.rand.NextBool(smokeQuantityInterpolant))
                        ModContent.GetInstance<HeatDistortionMetaball>().CreateParticle(behaviorOverride.TurretPosition, Main.rand.NextVector2Circular(3f, 3f), 70f);
                }

                if (enableContactDamage)
                    behaviorOverride.NPC.damage = behaviorOverride.NPC.defDamage;
            });
        }

        /// <summary>
        /// An action that closes a segment's vents. Meant to be used in conjunction with <see cref="BodyBehaviorAction"/>.
        /// </summary>
        /// <param name="segmentCloseRate">The amount by which the segment open interpolant changes every frame.</param>
        public static BodySegmentAction CloseSegment(float segmentCloseRate = StandardSegmentCloseRate, bool enableContactDamage = false) => OpenSegment(-segmentCloseRate, 1f, enableContactDamage);

        /// <summary>
        /// Creates smoke particles perpendicular to a segment NPC.
        /// </summary>
        /// <param name="behaviorOverride">The segment.</param>
        /// <param name="bigInterpolant">How big the smoke should be.</param>
        /// <param name="quantityInterpolant">A multiplier for how much smoke should be released.</param>
        public static void CreateSmoke(HadesBodyEternity behaviorOverride, float bigInterpolant, float quantityInterpolant = 1f)
        {
            NPC npc = behaviorOverride.NPC;
            if (!npc.WithinRange(Main.LocalPlayer.Center, 1200f))
                return;

            int smokeCount = (int)MathHelper.Lerp(2f, 40f, bigInterpolant);
            for (int i = 0; i < smokeCount; i++)
            {
                if (!Main.rand.NextBool(quantityInterpolant))
                    continue;

                int smokeLifetime = Main.rand.Next(20, 30);
                float smokeSpeed = Main.rand.NextFloat(15f, 29f);
                Color smokeColor = Color.Lerp(Color.Red, Color.Gray, 0.6f);
                if (Main.rand.NextBool(4))
                    smokeColor = Color.DarkRed;
                smokeColor.A = 97;

                if (Main.rand.NextBool(bigInterpolant))
                {
                    smokeSpeed *= 1f + bigInterpolant;
                    smokeLifetime += (int)(bigInterpolant * 30f);
                }

                Vector2 perpendicular = npc.rotation.ToRotationVector2();
                Vector2 smokeVelocity = perpendicular.RotatedByRandom(0.2f) * Main.rand.NextFromList(-1f, 1f) * smokeSpeed;
                SmokeParticle smoke = new(behaviorOverride.TurretPosition, smokeVelocity, smokeColor, smokeLifetime, 0.6f, 0.18f);
                smoke.Spawn();
            }
        }

        public override bool CheckDead()
        {
            if (CurrentState == HadesAIState.DeathAnimation && AITimer >= 10)
                return true;

            NPC.life = 1;
            NPC.dontTakeDamage = true;
            SelectNewState();
            CurrentState = HadesAIState.DeathAnimation;
            ExoMechFightStateManager.ClearExoMechProjectiles();

            return false;
        }

        public override bool PreKill()
        {
            DropHelper.BlockDrops(ModContent.ItemType<SpineOfThanatos>(), ModContent.ItemType<RefractionRotor>(), ModContent.ItemType<AtlasMunitionsBeacon>(), ModContent.ItemType<DraedonBag>());
            return true;
        }

        public override void ModifyTypeName(ref string typeName)
        {
            typeName = Language.GetTextValue("Mods.FargowiltasCrossmod.NPCs.ThanatosRename");
        }

        public override void FindFrame(int frameHeight)
        {
            int frame = Utils.Clamp((int)(SegmentOpenInterpolant * Main.npcFrameCount[NPC.type]), 0, Main.npcFrameCount[NPC.type] - 1);
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            int frame = NPC.frame.Y / NPC.frame.Height;
            Texture2D texture = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Hades/HadesHead").Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Hades/HadesHeadGlow").Value;
            Texture2D leftJawTexture = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Hades/HadesJawLeft").Value;
            Texture2D rightJawTexture = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Hades/HadesJawRight").Value;

            Vector2 drawPosition = NPC.Center - screenPos;
            Color glowmaskColor = Color.White * LumUtils.InverseLerp(5f, 15f, (lightColor.R + lightColor.G + lightColor.B) * 0.333f);
            Rectangle leftJawFrame = leftJawTexture.Frame(1, Main.npcFrameCount[NPC.type], 0, frame);
            Rectangle rightJawFrame = rightJawTexture.Frame(1, Main.npcFrameCount[NPC.type], 0, frame);
            Vector2 leftJawOrigin = leftJawFrame.Size() * new Vector2(0.38f, 0.54f);
            Vector2 rightJawOrigin = rightJawFrame.Size() * new Vector2(0.62f, 0.54f);
            Vector2 leftJawPosition = drawPosition + new Vector2(-32f, 0f).RotatedBy(NPC.rotation) * NPC.spriteDirection;
            Vector2 rightJawPosition = drawPosition + new Vector2(32f, 0f).RotatedBy(NPC.rotation) * NPC.spriteDirection;
            Main.spriteBatch.Draw(leftJawTexture, leftJawPosition, leftJawFrame, NPC.GetAlpha(lightColor), NPC.rotation + JawRotation + MathHelper.Pi, leftJawOrigin, NPC.scale, SpriteEffects.FlipVertically, 0f);
            Main.spriteBatch.Draw(rightJawTexture, rightJawPosition, rightJawFrame, NPC.GetAlpha(lightColor), NPC.rotation - JawRotation + MathHelper.Pi, rightJawOrigin, NPC.scale, SpriteEffects.FlipVertically, 0f);

            Main.spriteBatch.Draw(texture, drawPosition, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, 0, 0f);
            Main.spriteBatch.Draw(glowmask, drawPosition, NPC.frame, NPC.GetAlpha(glowmaskColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, 0, 0f);

            if (ReticleOpacity >= 0.01f)
                DrawReticle(ReticleOpacity);

            return false;
        }

        /// <summary>
        /// Draws a telegraph reticle with a given opacity on Hades' target.
        /// </summary>
        /// <param name="reticleOpacity">The opacity of the reticle.</param>
        public static void DrawReticle(float reticleOpacity)
        {
            Texture2D leftReticleTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleLeft").Value;
            Texture2D rightReticleTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleRight").Value;
            Texture2D topReticleTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleTop").Value;
            Texture2D bottomReticleTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleHead").Value;
            Texture2D leftReticleProngTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleProngLeft").Value;
            Texture2D rightReticleProngTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleProngRight").Value;

            // The reticle fades away and moves farther away from the target the closer they are to the aura.
            // Once far away, the reticle will flash between red and white as an indicator.
            float reticleOffsetDistance = MathHelper.SmoothStep(300f, 0f, reticleOpacity);
            float reticleFadeToWhite = (MathF.Cos(Main.GlobalTimeWrappedHourly * 6.8f) * 0.5f + 0.5f) * reticleOpacity * 0.67f;
            Color reticleBaseColor = new Color(255, 0, 0, 127) * reticleOpacity;
            Color reticleFlashBaseColor = Color.Lerp(reticleBaseColor, new Color(255, 255, 255, 0), reticleFadeToWhite) * reticleOpacity;
            Vector2 origin = leftReticleTexture.Size() * 0.5f;

            Vector2 playerDrawPosition = Target.Center - Main.screenPosition;
            Main.spriteBatch.Draw(leftReticleTexture, playerDrawPosition - Vector2.UnitX * reticleOffsetDistance, null, reticleBaseColor, 0f, origin, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(rightReticleTexture, playerDrawPosition + Vector2.UnitX * reticleOffsetDistance, null, reticleBaseColor, 0f, origin, 1f, SpriteEffects.None, 0f);

            for (int i = 0; i < 3; i++)
            {
                float scale = 1f + i * 0.125f;
                Main.spriteBatch.Draw(leftReticleProngTexture, playerDrawPosition - Vector2.UnitX * reticleOffsetDistance, null, reticleFlashBaseColor, 0f, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(rightReticleProngTexture, playerDrawPosition + Vector2.UnitX * reticleOffsetDistance, null, reticleFlashBaseColor, 0f, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(bottomReticleTexture, playerDrawPosition + Vector2.UnitY * reticleOffsetDistance, null, reticleFlashBaseColor, 0f, origin, scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(topReticleTexture, playerDrawPosition - Vector2.UnitY * reticleOffsetDistance, null, reticleBaseColor, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
