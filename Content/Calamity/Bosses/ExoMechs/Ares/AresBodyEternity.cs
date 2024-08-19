using CalamityMod;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior, IExoMech
    {
        public enum AresAIState
        {
            SpawnAnimation,
            LargeTeslaOrbBlast,
            DetachHands,
            NukeAoEAndPlasmaBlasts,
            AimedLaserBursts,
            KatanaSlashes,
            KatanaCycloneDashes,

            BackgroundCoreLaserBeams,

            Inactive,
            ReturnToBeingActive,
            Leave,

            DeathAnimation,

            PerformComboAttack = ExoMechComboAttackManager.ComboAttackValue
        }

        public enum AresFrameAnimationState
        {
            Default,
            Laugh
        }

        /// <summary>
        /// Whether Ares is currently performing a combo attack.
        /// </summary>
        public bool PerformingComboAttack
        {
            get => CurrentState == AresAIState.PerformComboAttack;
            set
            {
                if (CurrentState == AresAIState.Leave)
                    return;

                if (value && CurrentState == AresAIState.SpawnAnimation)
                {
                    WaitingToStartComboAttack = true;
                    return;
                }

                SelectNewState();
                if (value)
                    CurrentState = AresAIState.PerformComboAttack;
            }
        }

        /// <summary>
        /// Whether Ares is waiting for his current state to conclude before he performs combo attacks.
        /// </summary>
        public bool WaitingToStartComboAttack
        {
            get;
            set;
        }

        /// <summary>
        /// Whether Ares should be inactive, leaving the battle to let other mechs attack on their own.
        /// </summary>
        public bool Inactive
        {
            get;
            set;
        }

        /// <summary>
        /// Whether Ares is a primary mech or not, a.k.a the one that the player chose when starting the battle.
        /// </summary>
        public bool IsPrimaryMech
        {
            get;
            set;
        }

        /// <summary>
        /// Ares' current, non-combo state.
        /// </summary>
        public AresAIState CurrentState
        {
            get;
            set;
        }

        /// <summary>
        /// The previous non-combo state Ares performed.
        /// </summary>
        public AresAIState PreviousState
        {
            get;
            set;
        }

        /// <summary>
        /// Ares' current frame animation state.
        /// </summary>
        public AresFrameAnimationState AnimationState
        {
            get;
            set;
        }

        /// <summary>
        /// Ares' current frame.
        /// </summary>
        public int CurrentFrame
        {
            get;
            set;
        }

        /// <summary>
        /// Ares' current AI timer.
        /// </summary>
        public int AITimer
        {
            get;
            set;
        }

        /// <summary>
        /// Ares' Z position.
        /// </summary>
        public float ZPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Whether Ares has created his arms yet or not.
        /// </summary>
        public bool HasCreatedArms
        {
            get;
            set;
        }

        /// <summary>
        /// Whether Ares should use his standard rotation method, based on his horizontal speed.
        /// </summary>
        public bool UseStandardRotation
        {
            get;
            set;
        }

        /// <summary>
        /// Ares' silhouette opacity. If this is 0, no silhouette is drawn.
        /// </summary>
        public float SilhouetteOpacity
        {
            get;
            set;
        }

        /// <summary>
        /// How much Ares' silhouette show dissolve.
        /// </summary>
        public float SilhouetteDissolveInterpolant
        {
            get;
            set;
        }

        /// <summary>
        /// The sound responsible for Ares' idle hum.
        /// </summary>
        public LoopedSoundInstance HumLoopedSound;

        /// <summary>
        /// The set of attacks that Ares will do in the future.
        /// </summary>
        public Queue<AresAIState> StateQueue = [];

        /// <summary>
        /// Whether Ares and his hands need to be rendered to a render target for secondary draw operations, such as his silhouette.
        /// </summary>
        public bool NeedsToBeDrawnToRenderTarget => SilhouetteOpacity > 0f;

        /// <summary>
        /// The set of instructions that should be performed by each of Ares' arms.
        /// </summary>
        public HandInstructions[] InstructionsForHands
        {
            get;
            set;
        } = new HandInstructions[ArmCount];

        /// <summary>
        /// An optional draw action that is applied to Ares' body after everything else when in use.
        /// </summary>
        public Action OptionalDrawAction
        {
            get;
            set;
        }

        /// <summary>
        /// The center position of Ares' core.
        /// </summary>
        public Vector2 CorePosition => NPC.Center + Vector2.UnitY.RotatedBy(NPC.rotation) * NPC.scale * 22f;

        /// <summary>
        /// The target that Ares will attempt to attack.
        /// </summary>
        public static Player Target => ExoMechTargetSelector.Target;

        /// <summary>
        /// The amount of arms that Ares should have.
        /// </summary>
        public const int ArmCount = 4;

        /// <summary>
        /// Represents an action that should be taken for one of Ares' hands.
        /// </summary>
        /// <param name="Action">The action that the hand should perform.</param>
        public record HandInstructions(AresHandAction Action);

        /// <summary>
        /// The sound played when Ares laughs.
        /// </summary>
        public static readonly SoundStyle LaughSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/Laugh") with { Volume = 1.4f };

        /// <summary>
        /// The sound played idly by Ares.
        /// </summary>
        public static readonly SoundStyle HumSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/HumLoop");

        /// <summary>
        /// Represents an action that should be performed by hands attached to Ares.
        /// </summary>
        /// <param name="hand">The hand's ModNPC instance..</param>
        public delegate void AresHandAction(AresHand hand);

        public override int NPCOverrideID => ExoMechNPCIDs.AresBodyID;

        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(HasCreatedArms);
            bitWriter.WriteBit(Inactive);
            bitWriter.WriteBit(IsPrimaryMech);
            bitWriter.WriteBit(WaitingToStartComboAttack);

            binaryWriter.Write(ZPosition);
            binaryWriter.Write(AITimer);
            binaryWriter.Write((int)CurrentState);
            binaryWriter.Write((int)PreviousState);

            binaryWriter.Write(StateQueue.Count);
            for (int i = 0; i < StateQueue.Count; i++)
                binaryWriter.Write((int)StateQueue.ElementAt(i));

            binaryWriter.WriteVector2(AimedLaserBursts_AimOffset);
        }

        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            HasCreatedArms = bitReader.ReadBit();
            Inactive = bitReader.ReadBit();
            IsPrimaryMech = bitReader.ReadBit();
            WaitingToStartComboAttack = bitReader.ReadBit();

            ZPosition = binaryReader.ReadSingle();
            AITimer = binaryReader.ReadInt32();
            CurrentState = (AresAIState)binaryReader.ReadInt32();
            PreviousState = (AresAIState)binaryReader.ReadInt32();

            StateQueue.Clear();
            int stateQueueCount = binaryReader.ReadInt32();
            for (int i = 0; i < stateQueueCount; i++)
                StateQueue.Enqueue((AresAIState)binaryReader.ReadInt32());

            AimedLaserBursts_AimOffset = binaryReader.ReadVector2();
        }

        public override bool PreAI()
        {
            InstructionsForHands ??= new HandInstructions[ArmCount];
            if (Main.netMode != NetmodeID.MultiplayerClient && !HasCreatedArms)
                CreateArms();

            if (Inactive && CurrentState != AresAIState.Inactive && CurrentState != AresAIState.Leave)
            {
                CurrentState = AresAIState.Inactive;
                AITimer = 0;
                NPC.netUpdate = true;
            }

            // Leave if the player is dead.
            if ((Target.dead || !Target.active) && CurrentState != AresAIState.Leave)
            {
                CurrentState = AresAIState.Leave;
                AITimer = 0;
                NPC.netUpdate = true;
            }

            PerformPreUpdateResets();
            ExecuteCurrentState();

            HumLoopedSound ??= LoopedSoundManager.CreateNew(HumSound, () => !NPC.active);
            HumLoopedSound?.Update(NPC.Center, sound =>
            {
                sound.Pitch = LumUtils.InverseLerp(20f, 60f, NPC.velocity.Length()) * 0.4f;
                sound.Volume = NPC.scale * 0.125f;
            });

            // Reset the state queue during combo attacks, to ensure that a fresh set is chosen if he's fought alone.
            if (PerformingComboAttack && StateQueue.Count >= 1)
                StateQueue.Clear();

            if (UseStandardRotation)
                NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.X * 0.015f, 0.2f);
            NPC.scale = 1f / (ZPosition + 1f);
            NPC.Opacity = MathHelper.Lerp(NPC.Opacity, Utils.Remap(ZPosition, 0.6f, 2f, 1f, 0.67f), 0.04f);
            NPC.Calamity().ShouldCloseHPBar = Inactive || CurrentState == AresAIState.DeathAnimation;

            AITimer++;

            return false;
        }

        /// <summary>
        /// Selects a new state for Ares.
        /// </summary>
        public void SelectNewState()
        {
            if (CurrentState == AresAIState.Leave)
                return;

            CurrentState = AresAIState.DetachHands;
            AnimationState = AresFrameAnimationState.Default;
            ZPosition = 0f;
            AITimer = 0;

            for (int i = 0; i < NPC.maxAI; i++)
                NPC.ai[i] = 0f;

            NPC.netUpdate = true;
        }

        /// <summary>
        /// Performs Ares' current state.
        /// </summary>
        public void ExecuteCurrentState()
        {
            switch (CurrentState)
            {
                case AresAIState.SpawnAnimation:
                    DoBehavior_SpawnAnimation();
                    break;
                case AresAIState.LargeTeslaOrbBlast:
                    DoBehavior_LargeTeslaOrbBlast();
                    break;
                case AresAIState.DetachHands:
                    DoBehavior_DetachHands();
                    break;
                case AresAIState.NukeAoEAndPlasmaBlasts:
                    DoBehavior_NukeAoEAndPlasmaBlasts();
                    break;
                case AresAIState.KatanaSlashes:
                    DoBehavior_KatanaSlashes();
                    break;
                case AresAIState.KatanaCycloneDashes:
                    DoBehavior_KatanaCycloneDashes();
                    break;
                case AresAIState.AimedLaserBursts:
                    DoBehavior_AimedLaserBursts();
                    break;
                case AresAIState.BackgroundCoreLaserBeams:
                    DoBehavior_BackgroundCoreLaserBeams();
                    break;
                case AresAIState.Leave:
                    DoBehavior_Leave();
                    break;
                case AresAIState.Inactive:
                    DoBehavior_Inactive();
                    break;
                case AresAIState.ReturnToBeingActive:
                    DoBehavior_ReturnToBeingActive();
                    break;
                case AresAIState.DeathAnimation:
                    DoBehavior_DeathAnimation();
                    break;
                case AresAIState.PerformComboAttack:
                    AITimer = ExoMechComboAttackManager.ComboAttackTimer;
                    break;
            }
        }

        /// <summary>
        /// Creates Ares' arms.
        /// </summary>
        public void CreateArms()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int? realLife = null;
            for (int i = 0; i < ArmCount; i++)
            {
                int nextHandIndex = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<AresHand>(), NPC.whoAmI, i);
                if (realLife is not null)
                    Main.npc[nextHandIndex].realLife = realLife.Value;

                realLife = nextHandIndex;
            }

            HasCreatedArms = true;
            NPC.netUpdate = true;
        }

        public void StandardHandUpdate(AresHand hand, Vector2 hoverOffset, int armIndex)
        {
            hand.NPC.SmoothFlyNear(NPC.Center + hoverOffset * NPC.scale, 0.2f, 0.84f);
            hand.NPC.Center = NPC.Center + hoverOffset * NPC.scale;
            hand.RotateToLookAt(Target.Center);
            hand.NPC.Opacity = Utilities.Saturate(hand.NPC.Opacity + 0.2f);
            hand.UsesBackArm = armIndex == 0 || armIndex == ArmCount - 1;
            hand.ArmSide = (armIndex >= ArmCount / 2).ToDirectionInt();

            int animateRate = 3;
            hand.Frame = AITimer / animateRate % 11;
        }

        /// <summary>
        /// A basic method that makes Ares fly towards a given destination.
        /// </summary>
        /// <param name="hoverDestination">The destination to make Ares fly towards.</param>
        public void StandardFlyTowards(Vector2 hoverDestination)
        {
            if (NPC.WithinRange(hoverDestination, 85f))
                NPC.velocity *= 0.93f;
            else
                NPC.SimpleFlyMovement(NPC.SafeDirectionTo(hoverDestination) * 14f, 0.3f);
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
            NPC.damage = NPC.defDamage;
            NPC.defense = NPC.defDefense;
            NPC.dontTakeDamage = false;
            NPC.ShowNameOnHover = true;
            NPC.As<AresBody>().SecondaryAIState = (int)AresBody.SecondaryPhase.Nothing;
            SilhouetteOpacity = 0f;
            SilhouetteDissolveInterpolant = 0f;
            OptionalDrawAction = null;
            UseStandardRotation = true;

            CalamityGlobalNPC.draedonExoMechPrime = NPC.whoAmI;
        }

        /// <summary>
        /// Resets the <see cref="StateQueue"/>, effectively rerolling the set of attacks Ares will perform.
        /// </summary>
        public void ResetStateQueue()
        {
            // Generate the list of states the perform, and shuffle them randomly in such a manner that the first state in the list is never the previous state.
            List<AresAIState> states = [AresAIState.AimedLaserBursts, AresAIState.LargeTeslaOrbBlast, AresAIState.NukeAoEAndPlasmaBlasts];
            if (ExoMechFightStateManager.CurrentPhase >= ExoMechFightDefinitions.BerserkSoloPhaseDefinition)
                states.Add(AresAIState.BackgroundCoreLaserBeams);
            if (NPC.life <= NPC.lifeMax * ExoMechFightDefinitions.FightAloneLifeRatio || ExoMechFightStateManager.CurrentPhase >= ExoMechFightDefinitions.BerserkSoloPhaseDefinition)
                states.Add(AresAIState.KatanaCycloneDashes);

            IOrderedEnumerable<AresAIState> shuffledStates;
            do
            {
                shuffledStates = states.OrderBy(s => Main.rand.NextFloat());
            }
            while (shuffledStates.First() == PreviousState);

            StateQueue.Clear();
            foreach (AresAIState state in shuffledStates)
                StateQueue.Enqueue(state);
        }

        public override Color? GetAlpha(Color drawColor) => Color.Lerp(drawColor, Main.ColorOfTheSkies, LumUtils.InverseLerp(0.4f, 0f, NPC.Opacity)) * NPC.Opacity;

        public override void FindFrame(int frameHeight)
        {
            int startingFrame = 0;
            int endingFrame = 12;

            if (AnimationState == AresFrameAnimationState.Laugh)
            {
                startingFrame = 36;
                endingFrame = 48;
            }

            if (NPC.frameCounter >= 5)
            {
                CurrentFrame++;
                NPC.frameCounter = 0;
            }
            if (CurrentFrame >= endingFrame || CurrentFrame < startingFrame)
                CurrentFrame = startingFrame;

            NPC.frame.Width = 220;
            NPC.frame.Height = 252;
            NPC.frame.X = NPC.frame.Width * (CurrentFrame / 8);
            NPC.frame.Y = NPC.frame.Height * (CurrentFrame % 8);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            int handID = ModContent.NPCType<AresHand>();
            List<AresHand> handsToDraw = new(ArmCount);
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.type != handID)
                    continue;

                handsToDraw.Add(n.As<AresHand>());
            }

            Main.spriteBatch.PrepareForShaders();

            foreach (AresHand hand in handsToDraw.OrderBy(h => h.LocalIndex - h.UsesBackArm.ToInt() * 10))
                hand.DrawArm(spriteBatch, screenPos);

            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBody").Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBodyGlow").Value;
            ApplyNormalMapShader(texture, NPC.frame, true, false);

            Vector2 drawPosition = NPC.Center - screenPos;
            Main.spriteBatch.Draw(texture, drawPosition, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection(), 0f);
            Main.spriteBatch.Draw(glowmask, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection(), 0f);

            Main.spriteBatch.ResetToDefault();

            OptionalDrawAction?.Invoke();

            return false;
        }

        public static void ApplyNormalMapShader(Texture2D texture, Rectangle frame, bool cutOffAtTop, bool invertCutoff)
        {
            var teslaSpheres = LumUtils.AllProjectilesByID(ModContent.ProjectileType<LargeTeslaSphere>());
            Projectile teslaSphere = teslaSpheres.FirstOrDefault();
            if (teslaSphere is null)
                return;

            Main.spriteBatch.PrepareForShaders();

            float glowIntensity = LumUtils.Saturate(teslaSphere.width / 560f);
            ManagedShader normalMapShader = ShaderManager.GetShader("FargowiltasCrossmod.NormalMapGlowShader");
            normalMapShader.TrySetParameter("textureSize0", texture.Size());
            normalMapShader.TrySetParameter("lightColor", new Vector4(0f, 0.8f, 1.8f, 0f) * glowIntensity);
            normalMapShader.TrySetParameter("frame", new Vector4(frame.X, frame.Y, frame.Width, frame.Height));
            normalMapShader.TrySetParameter("cutOffAtTop", cutOffAtTop);
            normalMapShader.TrySetParameter("invertCutoff", invertCutoff);
            normalMapShader.TrySetParameter("lightPosition", teslaSphere.Center - Main.screenPosition);
            normalMapShader.Apply();
        }

        public override bool PreKill()
        {
            DropHelper.BlockDrops(ModContent.ItemType<AresExoskeleton>(), ModContent.ItemType<PhotonRipper>(), ModContent.ItemType<TheJailor>(), ModContent.ItemType<DraedonBag>());
            return true;
        }

        public override bool CheckDead()
        {
            if (CurrentState == AresAIState.DeathAnimation && AITimer >= 10)
                return true;

            NPC.life = 1;
            NPC.dontTakeDamage = true;
            AITimer = 0;
            CurrentState = AresAIState.DeathAnimation;
            ExoMechFightStateManager.ClearExoMechProjectiles();

            NPC.netUpdate = true;

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
            {
                Mod calamity = ModCompatibility.Calamity.Mod;

                // Left body shell.
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, -Vector2.UnitX.RotatedByRandom(0.7f) * 5f, calamity.Find<ModGore>("AresBody1").Type, NPC.scale);

                // Helmet.
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, -Vector2.UnitY.RotatedByRandom(0.12f) * 6f, calamity.Find<ModGore>("AresBody2").Type, NPC.scale);

                // Skull.
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, -Vector2.UnitY.RotatedByRandom(0.12f) * 6f, calamity.Find<ModGore>("AresBody3").Type, NPC.scale);

                // Dismantled, upper ribcage.
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Vector2.UnitY.RotatedByRandom(0.12f) * 4f, calamity.Find<ModGore>("AresBody4").Type, NPC.scale);

                // Core.
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2CircularEdge(4f, 4f), calamity.Find<ModGore>("AresBody5").Type, NPC.scale);

                // Lower body shell.
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Vector2.UnitY.RotatedByRandom(0.12f) * 4f, calamity.Find<ModGore>("AresBody6").Type, NPC.scale);

                // Dismantled, lower ribcage.
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Vector2.UnitY.RotatedByRandom(0.12f) * 4f, calamity.Find<ModGore>("AresBody7").Type, NPC.scale);
            }
        }
    }
}
