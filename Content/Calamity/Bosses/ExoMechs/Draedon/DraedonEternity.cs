using CalamityMod.NPCs;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

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

            FirstInterjection,
            SecondInterjection,
            PostBattleInterjection
        }

        public Player PlayerToFollow => Main.player[NPC.target];

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
        public ref float HologramInterpolant => ref NPC.localAI[1];

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
        public static readonly int StandardSpeakTime = Utilities.SecondsToFrames(3f);

        /// <summary>
        /// Draedon's starting monologue. This is spoken the first time the player interacts with him.
        /// </summary>
        public static readonly DraedonDialogueChain StartingMonologue = new DraedonDialogueChain("Mods.FargowiltasCrossmod.NPCs.Draedon.").
            Add("IntroductionMonologue1").
            Add("IntroductionMonologue2").
            Add("IntroductionMonologue3").
            Add("IntroductionMonologue4").
            Add("IntroductionMonologue5", CalamityMod.NPCs.ExoMechs.Draedon.TextColorEdgy, 1);

        /// <summary>
        /// Draedon's starting monologue. This is spoken in successive battles.
        /// </summary>
        public static readonly DraedonDialogueChain StartingMonologueBrief = new DraedonDialogueChain("Mods.FargowiltasCrossmod.NPCs.Draedon.").
            Add("IntroductionMonologueBrief", CalamityMod.NPCs.ExoMechs.Draedon.TextColorEdgy, 1);

        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.ExoMechs.Draedon>();

        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest(false);
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
            if (ExoMechFightStateManager.ActiveExoMechs.Count <= 0 && AIState != DraedonAIState.ExoMechSpawnAnimation)
                CalamityGlobalNPC.draedonAmbience = NPC.whoAmI;

            // Pick someone else to pay attention to if the old target is gone.
            if (PlayerToFollow.dead || !PlayerToFollow.active)
            {
                NPC.TargetClosest(false);

                // Fuck off if no living target exists.
                if (PlayerToFollow.dead || !PlayerToFollow.active)
                {
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }

            NPC.spriteDirection = (PlayerToFollow.Center.X - NPC.Center.X).NonZeroSign();

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
            }

            Lighting.AddLight(NPC.Center, Vector3.One * 0.76f);

            // Stay within the world.
            NPC.position.Y = MathHelper.Clamp(NPC.position.Y, 150f, Main.maxTilesY * 16f - 150f);

            NPC.ShowNameOnHover = HologramInterpolant <= 0.75f && NPC.Opacity >= 0.25f;

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
            AITimer = 0f;
            AIState = nextAIState;
            NPC.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowmask = CalamityMod.NPCs.ExoMechs.Draedon.Texture_Glow.Value;
            Rectangle frame = texture.Frame(4, Main.npcFrameCount[NPC.type], (int)Frame / Main.npcFrameCount[NPC.type], (int)Frame % Main.npcFrameCount[NPC.type]);
            Vector2 drawPosition = NPC.Center - screenPos;
            Color drawColor = lightColor * NPC.Opacity * MathF.Sqrt(1f - HologramInterpolant);
            Color glowmaskColor = Color.White * NPC.Opacity * MathF.Sqrt(1f - HologramInterpolant);

            bool drawHologramShader = HologramInterpolant > 0f;
            if (drawHologramShader)
            {
                Main.spriteBatch.PrepareForShaders();

                Vector4 frameArea = new(frame.Left / (float)texture.Width, frame.Top / (float)texture.Height, frame.Right / (float)texture.Width, frame.Bottom / (float)texture.Height);
                ManagedShader hologramShader = ShaderManager.GetShader("FargowiltasCrossmod.HologramShader");
                hologramShader.TrySetParameter("hologramInterpolant", HologramInterpolant);
                hologramShader.TrySetParameter("hologramSinusoidalOffset", MathF.Pow(HologramInterpolant, 7f) * 0.02f + Utilities.InverseLerp(0.4f, 1f, HologramInterpolant) * 0.04f);
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
    }
}
