using CalamityMod;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Assets.Particles.Metaballs;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Hooking;
using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public sealed partial class ApolloBehaviorOverride : CalDLCEmodeBehavior, IExoMech, IExoTwin
    {
        private static ILHook? hitEffectHook;

        /// <summary>
        /// Whether Artemis and Apollo are currently performing a combo attack.
        /// </summary>
        public bool PerformingComboAttack
        {
            get => ExoTwinsStateManager.SharedState.AIState == ExoTwinsAIState.PerformComboAttack;
            set => ExoTwinsStateManager.TransitionToNextState(value ? ExoTwinsAIState.PerformComboAttack : null);
        }

        /// <summary>
        /// Whether Apollo should be inactive, leaving the battle to let other mechs attack on their own.
        /// </summary>
        public bool Inactive
        {
            get;
            set;
        }

        /// <summary>
        /// Whether Apollo is a primary mech or not, a.k.a the one that the player chose when starting the battle.
        /// </summary>
        public bool IsPrimaryMech
        {
            get;
            set;
        }

        /// <summary>
        /// Apollo's current frame.
        /// </summary>
        public int Frame
        {
            get;
            set;
        }

        /// <summary>
        /// Apollo's current AI timer.
        /// </summary>
        public int AITimer
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        /// <summary>
        /// Whether Apollo has fully entered his second phase yet or not.
        /// </summary>
        public bool InPhase2
        {
            get => NPC.ai[0] == 1f;
            set => NPC.ai[0] = value.ToInt();
        }

        /// <summary>
        /// Whether Apollo has verified that Artemis is alive or not.
        /// </summary>
        public bool ArtemisSummonCheckPerformed
        {
            get;
            set;
        }

        /// <summary>
        /// The opacity of wingtip vortices on Apollo.
        /// </summary>
        public float WingtipVorticesOpacity
        {
            get;
            set;
        }

        /// <summary>
        /// The intensity boost of thrusters for Apollo.
        /// </summary>
        public float ThrusterBoost
        {
            get;
            set;
        }

        /// <summary>
        /// The interpolant of motion blur for Apollo.
        /// </summary>
        public float MotionBlurInterpolant
        {
            get;
            set;
        }

        /// <summary>
        /// Apollo's current animation.
        /// </summary>
        public ExoTwinAnimation Animation
        {
            get;
            set;
        } = ExoTwinAnimation.Idle;

        /// <summary>
        /// The engine sound Apollo plays.
        /// </summary>
        public LoopedSoundInstance EngineLoopSound
        {
            get;
            set;
        }

        /// <summary>
        /// The individual AI state of Apollo. Only used if the shared AI state is <see cref="ExoTwinsAIState.PerformIndividualAttacks"/>.
        /// </summary>
        public IndividualExoTwinStateHandler IndividualState
        {
            get;
            set;
        } = new(0);

        /// <summary>
        /// Apollo's specific draw action.
        /// </summary>
        public Action? SpecificDrawAction
        {
            get;
            set;
        }

        /// <summary>
        /// Apollo's optic nerve colors.
        /// </summary>
        public Color[] OpticNervePalette => [new(28, 58, 60), new(62, 105, 80), new(108, 167, 94), new(144, 246, 100), new(81, 126, 85)];

        /// <summary>
        /// Apollo's base texture.
        /// </summary>
        internal static LazyAsset<Texture2D> BaseTexture;

        /// <summary>
        /// Apollo's glowmask texture.
        /// </summary>
        internal static LazyAsset<Texture2D> Glowmask;

        public override int NPCOverrideID => ExoMechNPCIDs.ApolloID;

        public void ResetLocalStateData()
        {
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
        }

        public override void SetStaticDefaults()
        {
            MethodInfo? hitEffectMethod = typeof(Apollo).GetMethod("HitEffect");
            if (hitEffectMethod is not null)
            {
                new ManagedILEdit("Change Apollo's on hit visuals", Mod, edit =>
                {
                    hitEffectHook = new(hitEffectMethod, new(c => edit.EditingFunction(c, edit)));
                }, edit =>
                {
                    hitEffectHook?.Undo();
                    hitEffectHook?.Dispose();
                }, HitEffectILEdit).Apply();
            }

            if (Main.netMode == NetmodeID.Server)
                return;

            BaseTexture = LazyAsset<Texture2D>.Request("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/ArtemisAndApollo/Textures/Apollo");
            Glowmask = LazyAsset<Texture2D>.Request("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/ArtemisAndApollo/Textures/ApolloGlow");
        }

        public override void Unload()
        {
            hitEffectHook?.Undo();
            hitEffectHook?.Dispose();
        }

        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(Inactive);
            bitWriter.WriteBit(IsPrimaryMech);
        }

        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            Inactive = bitReader.ReadBit();
            IsPrimaryMech = bitReader.ReadBit();
        }

        public override bool PreAI()
        {
            if (!ArtemisSummonCheckPerformed)
            {
                if (!NPC.AnyNPCs(ExoMechNPCIDs.ArtemisID))
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ExoMechNPCIDs.ArtemisID, NPC.whoAmI);

                ArtemisSummonCheckPerformed = true;
                NPC.netUpdate = true;
            }
            else if (CalamityGlobalNPC.draedonExoMechTwinRed == -1)
                NPC.active = false;

            Vector2 actualHitboxSize = new(164f, 164f);
            if (NPC.Size != actualHitboxSize)
                NPC.Size = actualHitboxSize;

            UpdateEngineSound();

            Vector2 thrusterPosition = NPC.Center - NPC.rotation.ToRotationVector2() * NPC.scale * 34f + NPC.velocity;
            ModContent.GetInstance<HeatDistortionMetaball>().CreateParticle(thrusterPosition, Main.rand.NextVector2Circular(8f, 8f), ThrusterBoost * 60f + 108f, 16f);

            PerformPreUpdateResets();

            AITimer++;
            return false;
        }

        /// <summary>
        /// Selects a new state for Artemis and Apollo.
        /// </summary>
        public void SelectNewState() => ExoTwinsStateManager.TransitionToNextState();

        /// <summary>
        /// Resets various things pertaining to the fight state prior to behavior updates.
        /// </summary>
        /// <remarks>
        /// This serves as a means of ensuring that changes to the fight state are gracefully reset if something suddenly changes, while affording the ability to make changes during updates.<br></br>
        /// As a result, this alleviates behaviors AI states from the burden of having to assume that they may terminate at any time and must account for that to ensure that the state is reset.
        /// </remarks>
        public void PerformPreUpdateResets()
        {
            CalamityGlobalNPC.draedonExoMechTwinGreen = NPC.whoAmI;
            NPC.chaseable = true;
            ThrusterBoost = MathHelper.Clamp(ThrusterBoost - 0.035f, 0f, 10f);
            MotionBlurInterpolant = Utilities.Saturate(MotionBlurInterpolant - 0.05f);
            SpecificDrawAction = null;

            if (!Inactive)
                NPC.Opacity = 1f;
            NPC.Calamity().ShouldCloseHPBar = Inactive;
            NPC.As<Apollo>().SecondaryAIState = (int)Apollo.SecondaryPhase.Nothing;

            // Use base Calamity's ChargeCombo AIState at all times, since Apollo needs that to be enabled for his CanHitPlayer hook to return true.
            NPC.As<Apollo>().AIState = (int)Apollo.Phase.ChargeCombo;

            NPC.damage = 0;
        }

        public void UpdateEngineSound()
        {
            EngineLoopSound ??= LoopedSoundManager.CreateNew(ArtemisBehaviorOverride.EngineSound, () =>
            {
                return !NPC.active;
            });
            EngineLoopSound.Update(NPC.Center, s =>
            {
                if (s.Sound is null)
                    return;

                s.Volume = Utilities.InverseLerp(12f, 60f, NPC.velocity.Length()) * 1.5f + 0.45f;
                s.Pitch = Utilities.InverseLerp(9f, 50f, NPC.velocity.Length()) * 0.5f;
                if (Inactive)
                    s.Volume *= 0.01f;
            });
        }

        public static void HitEffectOverride(ModNPC apollo)
        {
            NPC npc = apollo.NPC;

            if (Main.rand.NextBool())
            {
                int pixelLifetime = Main.rand.Next(12, 19);
                Color pixelColor = Color.Lerp(Color.Cyan, Color.White, Main.rand.NextFloat(0.5f, 1f));
                Color pixelBloom = Color.Lerp(Color.ForestGreen, Color.Lime, Main.rand.NextFloat()) * 0.45f;
                Vector2 pixelScale = Vector2.One * Main.rand.NextFloat(0.67f, 1.5f);
                Vector2 pixelVelocity = Main.LocalPlayer.SafeDirectionTo(npc.Center).RotatedByRandom(0.95f) * Main.rand.NextFloat(3f, 35f);
                BloomPixelParticle pixel = new(npc.Center - pixelVelocity * 3.1f, pixelVelocity, pixelColor, pixelBloom, pixelLifetime, pixelScale);
                pixel.Spawn();
            }

            if (Main.rand.NextBool(10))
            {
                Vector2 lineVelocity = Main.LocalPlayer.SafeDirectionTo(npc.Center).RotatedByRandom(0.7f) * Main.rand.NextFloat(16f, 35f);
                LineParticle line = new(npc.Center + Main.rand.NextVector2Circular(30f, 30f), lineVelocity, Main.rand.NextBool(4), 20, 0.8f, Color.Orange);
                GeneralParticleHandler.SpawnParticle(line);
            }

            if (npc.soundDelay <= 0)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.ExoHitSound, npc.Center);
                npc.soundDelay = 3;
            }

            if (Main.netMode != NetmodeID.Server && npc.life <= 0)
            {
                IEntitySource deathSource = npc.GetSource_Death();
                Mod calamity = ModContent.GetInstance<CalamityMod.CalamityMod>();

                for (int i = 1; i <= 5; i++)
                    Gore.NewGore(deathSource, npc.position, npc.velocity, calamity.Find<ModGore>($"Apollo{i}").Type, npc.scale);
            }
        }

        public static void HitEffectILEdit(ILContext context, ManagedILEdit edit)
        {
            ILCursor cursor = new(context);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate(HitEffectOverride);
            cursor.Emit(OpCodes.Ret);
        }

        public override void OnKill()
        {
            DropHelper.BlockDrops(ModContent.ItemType<TheAtomSplitter>(), ModContent.ItemType<SurgeDriver>(), ModContent.ItemType<DraedonBag>());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            CommonExoTwinFunctionalities.DrawBase(NPC, this, BaseTexture.Value, Glowmask.Value, lightColor, screenPos, Frame);
            return false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (ExoTwinsStates.DoBehavior_EnterSecondPhase_ApolloIsProtectingArtemis(this))
            {
                modifiers.FinalDamage *= ExoTwinsStates.EnterSecondPhase_ApolloDamageProtectionFactor;
                if (!CalamityLists.projectileDestroyExceptionList.Contains(projectile.type))
                    projectile.Kill();
            }
        }
    }
}
