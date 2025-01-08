using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public sealed partial class DraedonEternity : CalDLCEmodeBehavior
    {
        public enum DraedonAIState
        {
            AppearAsHologram,
            StartingMonologue,
            ExoMechSpawnAnimation,
            MoveAroundDuringBattle,
            TemporarilyLeave,

            StandardPlayerDeathMonologue,
            FunnyPlayerDeathMonologue,

            FirstInterjection,
            SecondInterjection,
            PostBattleInterjection,

            ReconBodyKilledInterruption
        }

        internal static LazyAsset<Texture2D> HologramTexture;

        public Player PlayerToFollow => Main.player[NPC.target];

        /// <summary>
        /// The AI timer that Draedon had during her post battle interjection before being interruted.
        /// </summary>
        public int PostBattleInterjectionTimer
        {
            get;
            set;
        }

        /// <summary>
        /// Whether Draedon was "killed" during his post-battle interjection.
        /// </summary>
        public bool WasKilled
        {
            get;
            set;
        }

        /// <summary>
        /// Draedon's current AI state.
        /// </summary>
        public DraedonAIState AIState
        {
            get => (DraedonAIState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        /// <summary>
        /// Draedon's AI timer.
        /// </summary>
        public ref float AITimer => ref NPC.ai[1];

        /// <summary>
        /// The maximum intensity that the Exo Mechs sky can be drawn at.
        /// </summary>
        public ref float MaxSkyOpacity => ref NPC.ai[2];

        /// <summary>
        /// Draedon's current frame.
        /// </summary>
        public ref float Frame => ref NPC.localAI[0];

        /// <summary>
        /// The 0-1 interpolant which dictates how much Draedon looks like a hologram.
        /// </summary>
        public ref float HologramOverlayInterpolant => ref NPC.localAI[1];

        /// <summary>
        /// Draedon's local frame timer.
        /// </summary>
        public ref float FrameTimer => ref NPC.localAI[2];

        /// <summary>
        /// How far forward the summon animation plane is.
        /// </summary>
        public ref float PlaneFlyForwardInterpolant => ref NPC.localAI[3];

        /// <summary>
        /// How long Draedon typically waits between spoken dialogue.
        /// </summary>
        public static readonly int StandardSpeakTime = LumUtils.SecondsToFrames(2.15f);

        /// <summary>
        /// Draedon's starting monologue. This is spoken the first time the player interacts with him.
        /// </summary>
        public static readonly DraedonDialogueChain StartingMonologue = new DraedonDialogueChain().
            Add("IntroductionMonologue1").
            Add("IntroductionMonologue2").
            Add("IntroductionMonologue3").
            Add("IntroductionMonologue4").
            Add("IntroductionMonologue5");

        /// <summary>
        /// Draedon's starting monologue. This is spoken in successive battles.
        /// </summary>
        public static readonly DraedonDialogueChain StartingMonologueBrief = new DraedonDialogueChain().
            Add("IntroductionMonologueBrief");

        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.ExoMechs.Draedon>();

        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            HologramTexture = LazyAsset<Texture2D>.Request("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Draedon/Hologram");
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest(false);
        }

        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(WasKilled);
            binaryWriter.Write(PostBattleInterjectionTimer);
        }

        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            WasKilled = bitReader.ReadBit();
            PostBattleInterjectionTimer = binaryReader.ReadInt32();
        }

        public override bool PreAI()
        {
            // Be immune to every debuff.
            for (int k = 0; k < NPC.buffImmune.Length; k++)
                NPC.buffImmune[k] = true;

            // Set the whoAmI variable.
            CalamityGlobalNPC.draedon = NPC.whoAmI;
            CalamityGlobalNPC.draedonAmbience = -1;

            // Emit music. If the battle is ongoing, Draedon emits the battle theme.
            // Otherwise, he emits his trademark ambience.
            // This takes priority over everything.
            if (ExoMechFightStateManager.FightState.TotalAliveMechs <= 0 && AIState != DraedonAIState.ExoMechSpawnAnimation)
                CalamityGlobalNPC.draedonAmbience = NPC.whoAmI;

            // Pick someone else to pay attention to if the old target is gone.
            if (PlayerToFollow.dead || !PlayerToFollow.active)
            {
                NPC.TargetClosest(false);

                // Fuck off if no living target exists.
                bool deathAnimation = AIState == DraedonAIState.StandardPlayerDeathMonologue || AIState == DraedonAIState.FunnyPlayerDeathMonologue;
                if ((PlayerToFollow.dead || !PlayerToFollow.active) && !deathAnimation)
                {
                    if (AIState == DraedonAIState.MoveAroundDuringBattle)
                        ChangeAIState(DraedonAIState.StandardPlayerDeathMonologue);
                    else if (AIState == DraedonAIState.FirstInterjection || AIState == DraedonAIState.SecondInterjection || AIState == DraedonAIState.PostBattleInterjection)
                        ChangeAIState(DraedonAIState.FunnyPlayerDeathMonologue);
                    else
                        NPC.active = false;
                }
            }

            NPC.spriteDirection = (PlayerToFollow.Center.X - NPC.Center.X).NonZeroSign();
            NPC.dontTakeDamage = true;

            switch (AIState)
            {
                case DraedonAIState.AppearAsHologram:
                    DoBehavior_AppearAsHologram();
                    break;
                case DraedonAIState.StartingMonologue:
                    DoBehavior_StartingMonologue();
                    break;
                case DraedonAIState.ExoMechSpawnAnimation:
                    DoBehavior_ExoMechSpawnAnimation();
                    break;
                case DraedonAIState.MoveAroundDuringBattle:
                    DoBehavior_MoveAroundDuringBattle();
                    break;
                case DraedonAIState.TemporarilyLeave:
                    DoBehavior_TemporarilyLeave();
                    break;
                case DraedonAIState.FirstInterjection:
                    DoBehavior_FirstInterjection();
                    break;
                case DraedonAIState.SecondInterjection:
                    DoBehavior_SecondInterjection();
                    break;
                case DraedonAIState.PostBattleInterjection:
                    DoBehavior_PostBattleInterjection();
                    break;
                case DraedonAIState.ReconBodyKilledInterruption:
                    DoBehavior_ReconBodyKilledInterruption();
                    break;
                case DraedonAIState.StandardPlayerDeathMonologue:
                case DraedonAIState.FunnyPlayerDeathMonologue:
                    DoBehavior_PlayerDeathMonologue();
                    break;
            }

            Lighting.AddLight(NPC.Center, Vector3.One * 0.76f);

            // Stay within the world.
            NPC.position.Y = MathHelper.Clamp(NPC.position.Y, 150f, Main.maxTilesY * 16f - 150f);

            NPC.ShowNameOnHover = HologramOverlayInterpolant <= 0.75f && NPC.Opacity >= 0.25f;

            AITimer++;
            FrameTimer++;
            return false;
        }

        /// <summary>
        /// Performs Draedon's standard framing, with him sitting down in a thinking pose.
        /// </summary>
        public void PerformStandardFraming()
        {
            if (FrameTimer % 7f == 6f)
            {
                Frame++;
                if (Frame >= 48f)
                    Frame = 23f;
            }
        }

        /// <summary>
        /// Naturally changes Draedon's AI state, resetting state-specific variables in the process.
        /// </summary>
        /// <param name="nextAIState">The AI state to transition to.</param>
        public void ChangeAIState(DraedonAIState nextAIState)
        {
            if (AIState == DraedonAIState.StandardPlayerDeathMonologue || AIState == DraedonAIState.FunnyPlayerDeathMonologue)
                return;

            AITimer = 0f;
            AIState = nextAIState;
            NPC.netUpdate = true;
        }

        /// <summary>
        /// Renders Draedon's hologram projection.
        /// </summary>
        /// <param name="screenPos">The screen position.</param>
        /// <param name="lightColor">The color of light at Draedon's center.</param>
        public void RenderHologramProjection(Vector2 screenPos, Color lightColor)
        {
            Texture2D projector = CalamityMod.NPCs.ExoMechs.Draedon.ProjectorTexture.Value;
            Texture2D projectorGlowmask = CalamityMod.NPCs.ExoMechs.Draedon.ProjectorTexture_Glow.Value;
            Texture2D hologram = HologramTexture.Value;

            Color drawColor = lightColor * NPC.Opacity * MathF.Sqrt(1f - HologramOverlayInterpolant);
            Color glowmaskColor = Color.White * NPC.Opacity * MathF.Sqrt(1f - HologramOverlayInterpolant);

            float baseVerticalOffset = MathF.Cos(Main.GlobalTimeWrappedHourly * 3f + NPC.whoAmI) * 12f - MathF.Pow(HologramOverlayInterpolant, 1.4f) * 400f;
            Vector2 hologramDrawPosition = NPC.Center - screenPos + Vector2.UnitY * baseVerticalOffset;
            Vector2 projectorDrawPosition = hologramDrawPosition + Vector2.UnitY * NPC.scale * (ProjectorVerticalOffset + 85f);
            Rectangle projectorFrame = projector.Frame(1, 4, 0, (int)AITimer / 5 % 4);

            // Render the projector.
            Main.spriteBatch.Draw(projector, projectorDrawPosition, projectorFrame, drawColor, 0f, projectorFrame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(projectorGlowmask, projectorDrawPosition, projectorFrame, glowmaskColor, 0f, projectorFrame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally, 0f);

            Main.spriteBatch.PrepareForShaders();

            // Render the projector's light area.
            Texture2D pixel = MiscTexturesRegistry.Pixel.Value;
            Vector2 projectionArea = Vector2.One * 200f / pixel.Size();
            Color projectionColor = glowmaskColor.MultiplyRGB(new(0.75f, 1f, 1f)) * HologramOpacity;
            ManagedShader projectionShader = ShaderManager.GetShader("FargowiltasCrossmod.HologramProjectorAreaShader");
            projectionShader.TrySetParameter("textureSize", projectionArea);
            projectionShader.TrySetParameter("spread", HologramOpacity * (1f - HologramOverlayInterpolant) * 0.5f);
            projectionShader.Apply();
            Main.spriteBatch.Draw(pixel, projectorDrawPosition, null, projectionColor, 0f, pixel.Size() * new Vector2(0.5f, 1f), projectionArea, 0, 0f);

            ManagedShader glitchShader = ShaderManager.GetShader("FargowiltasCrossmod.GlitchShader");
            glitchShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly * 0.89f + NPC.whoAmI * 0.517f);
            glitchShader.TrySetParameter("textureSize", hologram.Size());
            glitchShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 1, SamplerState.LinearWrap);
            glitchShader.Apply();

            Main.spriteBatch.Draw(hologram, hologramDrawPosition, null, glowmaskColor * HologramOpacity, NPC.rotation, hologram.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.ResetToDefault();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            if (WasKilled)
            {
                RenderHologramProjection(screenPos, lightColor);
                return false;
            }

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowmask = CalamityMod.NPCs.ExoMechs.Draedon.Texture_Glow.Value;
            Rectangle frame = texture.Frame(4, Main.npcFrameCount[NPC.type], (int)Frame / Main.npcFrameCount[NPC.type], (int)Frame % Main.npcFrameCount[NPC.type]);
            Vector2 drawPosition = NPC.Center - screenPos;
            Color drawColor = lightColor * NPC.Opacity * MathF.Sqrt(1f - HologramOverlayInterpolant);
            Color glowmaskColor = Color.White * NPC.Opacity * MathF.Sqrt(1f - HologramOverlayInterpolant);

            bool drawHologramShader = HologramOverlayInterpolant > 0f;
            if (drawHologramShader)
            {
                Main.spriteBatch.PrepareForShaders();

                Vector4 frameArea = new(frame.Left / (float)texture.Width, frame.Top / (float)texture.Height, frame.Right / (float)texture.Width, frame.Bottom / (float)texture.Height);
                ManagedShader hologramShader = ShaderManager.GetShader("FargowiltasCrossmod.HologramShader");
                hologramShader.TrySetParameter("hologramInterpolant", HologramOverlayInterpolant);
                hologramShader.TrySetParameter("hologramSinusoidalOffset", MathF.Pow(HologramOverlayInterpolant, 7f) * 0.02f + LumUtils.InverseLerp(0.4f, 1f, HologramOverlayInterpolant) * 0.04f);
                hologramShader.TrySetParameter("textureSize0", texture.Size());
                hologramShader.TrySetParameter("frameArea", frameArea);
                hologramShader.Apply();
            }

            Main.spriteBatch.Draw(texture, drawPosition, frame, drawColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(glowmask, drawPosition, frame, glowmaskColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection() ^ SpriteEffects.FlipHorizontally, 0f);

            if (drawHologramShader)
                Main.spriteBatch.ResetToDefault();

            return false;
        }

        public override bool CheckDead()
        {
            SoundEngine.PlaySound(CommonCalamitySounds.ExoHitSound, NPC.Center);

            DraedonDialogueChain dialogue = DownedBossSystem.downedExoMechs ? PostBattleAnalysisInterjection : PostBattleInterjection;

            WasKilled = true;
            PostBattleInterjectionTimer = (int)AITimer;

            // Skip to the next line.
            dialogue.Process(PostBattleInterjectionTimer, out DraedonDialogue? currentDialogue, out int relativeTime);
            if (currentDialogue is not null)
                PostBattleInterjectionTimer += currentDialogue.Duration - relativeTime - 1;
            DraedonSubtitleManager.Stop();

            ChangeAIState(DraedonAIState.ReconBodyKilledInterruption);

            NPC.netUpdate = true;
            return true;
        }
    }
}
