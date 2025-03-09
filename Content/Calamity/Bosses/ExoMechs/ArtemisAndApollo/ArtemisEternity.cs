using CalamityMod;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Assets.Particles.Metaballs;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
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
    public sealed partial class ArtemisEternity : CalDLCEmodeBehavior, IExoMech, IExoTwin
    {
        private static ILHook? hitEffectHook;

        /// <summary>
        /// Whether Artemis and Apollo are currently performing a combo attack.
        /// </summary>
        public bool PerformingComboAttack
        {
            get => ExoTwinsStateManager.SharedState.AIState == ExoTwinsAIState.PerformComboAttack;
            set
            {
                if (ExoTwinsStateManager.SharedState.AIState == ExoTwinsAIState.Leave)
                    return;

                ExoTwinsStateManager.TransitionToNextState(value ? ExoTwinsAIState.PerformComboAttack : null);
            }
        }

        /// <summary>
        /// Whether Artemis should be inactive, leaving the battle to let other mechs attack on their own.
        /// </summary>
        public bool Inactive
        {
            get;
            set;
        }

        /// <summary>
        /// Whether Artemis is a primary mech or not, a.k.a the one that the player chose when starting the battle.
        /// </summary>
        public bool IsPrimaryMech
        {
            get;
            set;
        }

        /// <summary>
        /// Artemis' current frame.
        /// </summary>
        public int Frame
        {
            get;
            set;
        }

        /// <summary>
        /// Artemis' current AI timer.
        /// </summary>
        public int AITimer
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        /// <summary>
        /// Whether Artemis has fully entered her second phase yet or not.
        /// </summary>
        public bool InPhase2
        {
            get => NPC.ai[0] == 1f;
            set => NPC.ai[0] = value.ToInt();
        }

        /// <summary>
        /// Whether Artemis has been destroyed due to impact during her death animation.
        /// </summary>
        public bool HasBeenDestroyed
        {
            get;
            set;
        }

        /// <summary>
        /// Whether Artemis has verified that Apollo is alive or not.
        /// </summary>
        public bool ApolloSummonCheckPerformed
        {
            get;
            set;
        }

        /// <summary>
        /// The opacity of wingtip vortices on Artemis.
        /// </summary>
        public float WingtipVorticesOpacity
        {
            get;
            set;
        }

        /// <summary>
        /// The interpolant of motion blur for Artemis.
        /// </summary>
        public float MotionBlurInterpolant
        {
            get;
            set;
        }

        /// <summary>
        /// The sensitivity of the optic nerve's angular reach.
        /// </summary>
        public float OpticNerveAngleSensitivity
        {
            get;
            set;
        }

        /// <summary>
        /// The intensity boost of thrusters for Artemis.
        /// </summary>
        public float ThrusterBoost
        {
            get;
            set;
        }

        /// <summary>
        /// Artemis' Z position.
        /// </summary>
        public float ZPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Artemis's current animation.
        /// </summary>
        public ExoTwinAnimation Animation
        {
            get;
            set;
        } = ExoTwinAnimation.Idle;

        /// <summary>
        /// The engine sound Artemis plays.
        /// </summary>
        public LoopedSoundInstance EngineLoopSound
        {
            get;
            set;
        }

        /// <summary>
        /// The individual AI state of Artemis. Only used if the shared AI state is <see cref="ExoTwinsAIState.PerformIndividualAttacks"/>.
        /// </summary>
        public IndividualExoTwinStateHandler IndividualState
        {
            get;
            set;
        } = new(0);

        /// <summary>
        /// Artemis' specific draw action.
        /// </summary>
        public Action? SpecificDrawAction
        {
            get;
            set;
        }

        /// <summary>
        /// Apollo's specific shader action.
        /// </summary>
        public Func<Texture2D, NPC, bool> SpecialShaderAction
        {
            get;
            set;
        }

        /// <summary>
        /// Artemis' optic nerve colors.
        /// </summary>
        public Color[] OpticNervePalette => [new(75, 14, 6), new(145, 35, 4), new(204, 101, 24), new(254, 172, 84), new(224, 147, 40)];

        /// <summary>
        /// Artemis' base texture.
        /// </summary>
        internal static LazyAsset<Texture2D> BaseTexture;

        /// <summary>
        /// Artemis' glowmask texture.
        /// </summary>
        internal static LazyAsset<Texture2D> Glowmask;

        /// <summary>
        /// Artemis' destroyed texture.
        /// </summary>
        internal static LazyAsset<Texture2D> DestroyedTexture;

        /// <summary>
        /// The engine loop sound Artemis and Apollo idly play.
        /// </summary>
        public static readonly SoundStyle EngineSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/ExoTwins/EngineLoop");

        public override int NPCOverrideID => ExoMechNPCIDs.ArtemisID;

        public void ResetLocalStateData()
        {
            AITimer = 0;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.netUpdate = true;
        }

        public override void SetStaticDefaults()
        {
            MethodInfo? hitEffectMethod = typeof(Artemis).GetMethod("HitEffect");
            if (hitEffectMethod is not null)
            {
                new ManagedILEdit("Change Artemis' on hit visuals", Mod, edit =>
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

            BaseTexture = LazyAsset<Texture2D>.Request("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/ArtemisAndApollo/Textures/Artemis");
            Glowmask = LazyAsset<Texture2D>.Request("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/ArtemisAndApollo/Textures/ArtemisGlow");
            DestroyedTexture = LazyAsset<Texture2D>.Request("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/ArtemisAndApollo/Textures/ArtemisDestroyed");
        }

        public override void Unload()
        {
            hitEffectHook?.Undo();
            hitEffectHook?.Dispose();
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 5f;
            NPC.width = 204;
            NPC.height = 226;

            NPC.damage = CommonExoTwinFunctionalities.ContactDamage;
            NPC.Calamity().canBreakPlayerDefense = true;
            if (Main.masterMode)
                NPC.damage /= 3;
            else if (Main.expertMode)
                NPC.damage /= 2;

            NPC.defense = CommonExoTwinFunctionalities.Defense;
            NPC.DR_NERD(CommonExoTwinFunctionalities.DamageReductionFactor);

            float healthBoostFactor = CalamityConfig.Instance.BossHealthBoost * 0.01f + 1f;
            NPC.LifeMaxNERB(1250000, 1495000, 650000);
            NPC.lifeMax = (int)MathF.Round(NPC.lifeMax * healthBoostFactor);

            NPC.aiStyle = -1;
            NPC.Opacity = 0f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(15, 0, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            // The hit sound is played via the HitEffectOverride method, rather than vanilla code.
            NPC.DeathSound = CommonCalamitySounds.ExoDeathSound;

            NPC.netAlways = true;
            NPC.boss = true;
            NPC.BossBar = ModContent.GetInstance<ExoMechBossBar>();

            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(Inactive);
            bitWriter.WriteBit(IsPrimaryMech);

            binaryWriter.Write(ZPosition);
        }

        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            Inactive = bitReader.ReadBit();
            IsPrimaryMech = bitReader.ReadBit();

            ZPosition = binaryReader.ReadSingle();
        }

        public override bool PreAI()
        {
            if (!ApolloSummonCheckPerformed)
            {
                if (!NPC.AnyNPCs(ExoMechNPCIDs.ApolloID))
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ExoMechNPCIDs.ApolloID, NPC.whoAmI);

                ApolloSummonCheckPerformed = true;
                NPC.netUpdate = true;
            }
            else if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1 && Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].type == ExoMechNPCIDs.ApolloID)
            {
                NPC.realLife = CalamityGlobalNPC.draedonExoMechTwinGreen;
                NPC.life = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life;
                if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].TryGetDLCBehavior(out ApolloEternity apollo))
                    Inactive = apollo.Inactive;
            }
            else if (ExoTwinsStateManager.SharedState.AIState != ExoTwinsAIState.Leave)
                NPC.active = false;

            // Use base Calamity's Charge AIState at all times, since Artemis needs that to be enabled for her CanHitPlayer hook to return true.
            NPC.As<Artemis>().AIState = (int)Artemis.Phase.Charge;

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
            CalamityGlobalNPC.draedonExoMechTwinRed = NPC.whoAmI;
            NPC.chaseable = true;
            ThrusterBoost = MathHelper.Clamp(ThrusterBoost - 0.035f, 0f, 10f);
            MotionBlurInterpolant = Utilities.Saturate(MotionBlurInterpolant - 0.05f);
            SpecificDrawAction = null;
            SpecialShaderAction = (_, _2) => false;

            if (!Inactive)
                NPC.Opacity = 1f;
            OpticNerveAngleSensitivity = 1f;
            NPC.Calamity().ShouldCloseHPBar = Inactive;
            NPC.As<Artemis>().SecondaryAIState = (int)Artemis.SecondaryPhase.Nothing;

            NPC.timeLeft = 7200;

            NPC.damage = 0;
        }

        public void UpdateEngineSound()
        {
            EngineLoopSound ??= LoopedSoundManager.CreateNew(EngineSound, () =>
            {
                return !NPC.active;
            });
            EngineLoopSound.Update(NPC.Center, s =>
            {
                if (s.Sound is null)
                    return;

                s.Volume = (Utilities.InverseLerp(12f, 60f, NPC.velocity.Length()) * 1.5f + 0.45f) * NPC.Opacity;
                s.Pitch = Utilities.InverseLerp(9f, 50f, NPC.velocity.Length()) * 0.5f;
                if (Inactive)
                    s.Volume *= 0.01f;
            });
        }

        public static void HitEffectOverride(ModNPC artemis)
        {
            NPC npc = artemis.NPC;

            if (Main.rand.NextBool())
            {
                int pixelLifetime = Main.rand.Next(12, 19);
                Color pixelColor = Color.Lerp(Color.Yellow, Color.White, Main.rand.NextFloat(0.5f, 1f));
                Color pixelBloom = Color.Lerp(Color.Yellow, Color.OrangeRed, Main.rand.NextFloat()) * 0.45f;
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

            if (Main.netMode != NetmodeID.Server && npc.life <= 0 && ExoTwinsStateManager.SharedState.AIState == ExoTwinsAIState.DeathAnimation && ExoTwinsStateManager.SharedState.AITimer >= 10)
            {
                IEntitySource deathSource = npc.GetSource_Death();
                Mod calamity = ModCompatibility.Calamity.Mod;

                for (int i = 1; i <= 5; i++)
                    Gore.NewGore(deathSource, npc.position, npc.velocity + Main.rand.NextVector2Circular(24f, 24f), calamity.Find<ModGore>($"Artemis{i}").Type, npc.scale);
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            float backgroundVfxInterpolant = Utilities.InverseLerp(0.9f, 3.7f, ZPosition);
            float backgroundOpacity = MathHelper.Lerp(1f, 0.65f, backgroundVfxInterpolant);
            drawColor = Color.Lerp(drawColor, Color.DarkGray, backgroundVfxInterpolant * 0.56f) * backgroundOpacity;
            return drawColor * NPC.Opacity;
        }

        public static void HitEffectILEdit(ILContext context, ManagedILEdit edit)
        {
            ILCursor cursor = new(context);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate(HitEffectOverride);
            cursor.Emit(OpCodes.Ret);
        }

        public override bool PreKill()
        {
            DropHelper.BlockDrops(ModContent.ItemType<TheAtomSplitter>(), ModContent.ItemType<SurgeDriver>(), ModContent.ItemType<DraedonBag>());
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            CommonExoTwinFunctionalities.DrawBase(NPC, this, BaseTexture.Value, Glowmask.Value, DestroyedTexture.Value, lightColor, screenPos, Frame);
            return false;
        }

        public override bool CheckDead() => CommonExoTwinFunctionalities.HandleDeath(NPC);
    }
}
