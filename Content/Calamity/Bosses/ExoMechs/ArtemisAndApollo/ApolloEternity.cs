using CalamityMod;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using CalamityMod.UI;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Assets.Particles.Metaballs;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
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
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public sealed partial class ApolloEternity : CalDLCEmodeBehavior, IExoMech, IExoTwin
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
        /// Whether Apollo has been destroyed due to impact during his death animation.
        /// </summary>
        public bool HasBeenDestroyed
        {
            get;
            set;
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
        /// The sensitivity of the optic nerve's angular reach.
        /// </summary>
        public float OpticNerveAngleSensitivity
        {
            get;
            set;
        }

        /// <summary>
        /// How much Apollo's form should be engulfed in frames, as a 0-1 interpolant.
        /// </summary>
        public float FlameEngulfInterpolant
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
        /// Apollo's specific shader action.
        /// </summary>
        public Func<Texture2D, NPC, bool> SpecialShaderAction
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

        /// <summary>
        /// Apollo's destroyed texture.
        /// </summary>
        internal static LazyAsset<Texture2D> DestroyedTexture;

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

            MethodInfo? addBarMethod = typeof(BossHealthBarManager).GetMethod("AttemptToAddBar");
            if (addBarMethod is not null)
            {
                new ManagedILEdit("Change Apollo's name on Calamity's boss bar", Mod, edit =>
                {
                    hitEffectHook = new(addBarMethod, new(c => edit.EditingFunction(c, edit)));
                }, edit =>
                {
                    hitEffectHook?.Undo();
                    hitEffectHook?.Dispose();
                }, RenameILEdit).Apply();
            }

            if (Main.netMode == NetmodeID.Server)
                return;

            BaseTexture = LazyAsset<Texture2D>.Request("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/ArtemisAndApollo/Textures/Apollo");
            Glowmask = LazyAsset<Texture2D>.Request("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/ArtemisAndApollo/Textures/ApolloGlow");
            DestroyedTexture = LazyAsset<Texture2D>.Request("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/ArtemisAndApollo/Textures/ApolloDestroyed");
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
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;

            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
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
            else if (CalamityGlobalNPC.draedonExoMechTwinRed == -1 && ExoTwinsStateManager.SharedState.AIState != ExoTwinsAIState.Leave)
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
            FlameEngulfInterpolant = Utilities.Saturate(FlameEngulfInterpolant - 0.06f);
            SpecificDrawAction = null;
            SpecialShaderAction = (_, _2) => false;

            if (!Inactive)
                NPC.Opacity = 1f;
            OpticNerveAngleSensitivity = 1f;
            NPC.Calamity().ShouldCloseHPBar = Inactive;
            NPC.As<Apollo>().SecondaryAIState = (int)Apollo.SecondaryPhase.Nothing;

            // Use base Calamity's ChargeCombo AIState at all times, since Apollo needs that to be enabled for his CanHitPlayer hook to return true.
            NPC.As<Apollo>().AIState = (int)Apollo.Phase.ChargeCombo;

            NPC.timeLeft = 7200;

            NPC.damage = 0;
        }

        public void UpdateEngineSound()
        {
            EngineLoopSound ??= LoopedSoundManager.CreateNew(ArtemisEternity.EngineSound, () =>
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

            if (Main.netMode != NetmodeID.Server && npc.life <= 0 && ExoTwinsStateManager.SharedState.AIState == ExoTwinsAIState.DeathAnimation && ExoTwinsStateManager.SharedState.AITimer >= 10)
            {
                IEntitySource deathSource = npc.GetSource_Death();
                Mod calamity = ModCompatibility.Calamity.Mod;

                for (int i = 1; i <= 5; i++)
                    Gore.NewGore(deathSource, npc.position, npc.velocity + Main.rand.NextVector2Circular(24f, 24f), calamity.Find<ModGore>($"Apollo{i}").Type, npc.scale);
            }
        }

        private static void HitEffectILEdit(ILContext context, ManagedILEdit edit)
        {
            ILCursor cursor = new(context);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate(HitEffectOverride);
            cursor.Emit(OpCodes.Ret);
        }

        private static void RenameILEdit(ILContext context, ManagedILEdit edit)
        {
            ILCursor cursor = new(context);

            if (!cursor.TryGotoNext(i => i.MatchLdstr("UI.ExoTwinsName")))
            {
                edit.LogFailure("Could not locate the 'UI.ExoTwinsName' base string.");
                return;
            }

            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall(typeof(CalamityUtils).GetMethod("GetTextValue"))))
            {
                edit.LogFailure("Could not locate the GetTextValue call.");
                return;
            }

            cursor.EmitDelegate((string originalText) =>
            {
                if (CalDLCWorldSavingSystem.E_EternityRev)
                    return Language.GetTextValue("Mods.FargowiltasCrossmod.NPCs.ExoTwinsRenameNormal");

                return originalText;
            });
        }

        public override bool PreKill()
        {
            DropHelper.BlockDrops(ModContent.ItemType<TheAtomSplitter>(), ModContent.ItemType<SurgeDriver>(), ModContent.ItemType<DraedonBag>());
            return true;
        }

        public override void ModifyTypeName(ref string typeName)
        {
            typeName = Language.GetTextValue("Mods.FargowiltasCrossmod.NPCs.ApolloRename");
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            CommonExoTwinFunctionalities.DrawBase(NPC, this, BaseTexture.Value, Glowmask.Value, DestroyedTexture.Value, lightColor, screenPos, Frame);
            DrawPlasmaFlameEngulfEffect();
            return false;
        }

        public float FlameEngulfWidthFunction(float completionRatio)
        {
            float baseWidth = MathHelper.Lerp(114f, 50f, completionRatio);
            float tipSmoothenFactor = MathF.Sqrt(1f - Utilities.InverseLerp(0.3f, 0.015f, completionRatio).Cubed());
            return NPC.scale * baseWidth * tipSmoothenFactor;
        }

        public Color FlameEngulfColorFunction(float completionRatio)
        {
            Color flameTipColor = new(255, 255, 208);
            Color limeFlameColor = new(173, 255, 36);
            Color greenFlameColor = new(52, 156, 17);
            Color trailColor = Utilities.MulticolorLerp(MathF.Pow(completionRatio, 0.75f) * 0.7f, flameTipColor, limeFlameColor, greenFlameColor);
            return NPC.GetAlpha(trailColor) * (1 - completionRatio) * FlameEngulfInterpolant;
        }

        public void DrawPlasmaFlameEngulfEffect()
        {
            if (FlameEngulfInterpolant <= 0f)
                return;

            Vector2[] flamePositions = new Vector2[8];
            for (int i = 0; i < flamePositions.Length; i++)
                flamePositions[i] = NPC.Center - NPC.oldRot[i].ToRotationVector2() * (i * 90f - 96f);

            ManagedShader flameShader = ShaderManager.GetShader("FargowiltasCrossmod.FlameEngulfShader");
            flameShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 1, SamplerState.LinearWrap);
            flameShader.SetTexture(MiscTexturesRegistry.TurbulentNoise.Value, 2, SamplerState.LinearWrap);

            PrimitiveSettings flameSettings = new(FlameEngulfWidthFunction, FlameEngulfColorFunction, Shader: flameShader);
            PrimitiveRenderer.RenderTrail(flamePositions, flameSettings, 60);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (ExoTwinsStates.DoBehavior_EnterSecondPhase_ApolloIsProtectingArtemis(this))
            {
                modifiers.FinalDamage *= ExoTwinsStates.EnterSecondPhase_ApolloDamageProtectionFactor;
                if (!CalamityLists.projectileDestroyExceptionList.Contains(projectile.type))
                    projectile.active = false;
            }
        }

        public override bool CheckDead() => CommonExoTwinFunctionalities.HandleDeath(NPC);
    }
}
