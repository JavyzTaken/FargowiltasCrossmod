using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        // This uses newAI[2] of all things because that happens to coincide with the immunity timer base Hades has, which makes incoming hits basically not matter if it hasn't exceeded a certain threshold.
        // By using it for the general existence timer, that guarantees that the immunity timer doesn't stay at zero 24/7 and effectively make Hades unable to be damaged.
        /// <summary>
        /// How long this body segment has existed, in frames.
        /// </summary>
        public ref float ExistenceTimer => ref NPC.Calamity().newAI[2];

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

        public override bool PreAI()
        {
            ExistenceTimer++;
            if (ExistenceTimer <= ActivationDelay)
                return false;

            if (!ValidateAheadSegment())
            {
                NPC.active = false;
                return false;
            }

            NPC aheadSegment = Main.npc[AheadSegmentIndex];
            Vector2 directionToNextSegment = aheadSegment.Center - NPC.Center;
            if (aheadSegment.rotation != NPC.rotation && ShouldReorientDirection)
            {
                float angleOffset = MathHelper.WrapAngle(aheadSegment.rotation - NPC.rotation) * 0.04f;
                if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].velocity.Length() <= 5f)
                    angleOffset = 0f;

                directionToNextSegment = directionToNextSegment.RotatedBy(angleOffset);
            }

            // Hack to ensure that segments retain Hades' secondary AI state, and thusly use the correct map icon.
            NPC.Calamity().newAI[1] = aheadSegment.Calamity().newAI[1];
            NPC.damage = NPC.defDamage;
            NPC.Opacity = aheadSegment.Opacity;
            NPC.Center = aheadSegment.Center - directionToNextSegment.SafeNormalize(Vector2.Zero) * NPC.width * NPC.scale * 0.97f;
            NPC.rotation = directionToNextSegment.ToRotation() + MathHelper.PiOver2;
            NPC.spriteDirection = directionToNextSegment.X.NonZeroSign();
            ShouldReorientDirection = true;

            NPC.HitSound = SegmentOpenInterpolant >= 0.75f ? ThanatosHead.ThanatosHitSoundOpen : ThanatosHead.ThanatosHitSoundClosed;

            ListenToHeadInstructions();
            ModifyDRBasedOnOpenInterpolant();

            if (GenericCountdown > 0f)
                GenericCountdown--;
            return false;
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

            // TODO -- Move this.
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
            float damageReduction = MathHelper.SmoothStep(0.9999f, 0f, SegmentOpenInterpolant);
            CalamityGlobalNPC globalNPC = NPC.Calamity();
            globalNPC.unbreakableDR = damageReduction >= 0.999f;
            globalNPC.DR = damageReduction;
            NPC.chaseable = !globalNPC.unbreakableDR;
        }

        /// <summary>
        /// Validates whether the ahead segment is valid.
        /// </summary>
        /// <returns>Whether this segment can still exist due to having a valid ahead segment.</returns>
        public bool ValidateAheadSegment()
        {
            if (!Main.npc.IndexInRange(AheadSegmentIndex))
                return false;

            NPC aheadSegment = Main.npc[AheadSegmentIndex];
            bool connectedToSameWorm = NPC.realLife == aheadSegment.realLife;
            bool aheadSegmentIsHead = aheadSegment.type == ExoMechNPCIDs.HadesHeadID;
            if (!connectedToSameWorm && !aheadSegmentIsHead)
                return false;

            if (!aheadSegment.active)
                return false;

            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
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

        public override void FindFrame(int frameHeight)
        {
            int frame = Utils.Clamp((int)(SegmentOpenInterpolant * Main.npcFrameCount[NPC.type]), 0, Main.npcFrameCount[NPC.type] - 1);
            NPC.frame.Y = frame * frameHeight;
        }

        public override void ModifyTypeName(ref string typeName)
        {
            typeName = Language.GetTextValue("Mods.FargowiltasCrossmod.NPCs.ThanatosRename");
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

            int frame = NPC.frame.Y / NPC.frame.Height;
            Vector2 drawPosition = NPC.Center - screenPos;
            Rectangle rectangleFrame = texture.Frame(1, Main.npcFrameCount[NPC.type], 0, frame);
            Main.spriteBatch.Draw(texture, drawPosition, rectangleFrame, NPC.GetAlpha(lightColor), NPC.rotation, rectangleFrame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection(), 0f);
            Main.spriteBatch.Draw(glowmask, drawPosition, rectangleFrame, NPC.GetAlpha(Color.White), NPC.rotation, rectangleFrame.Size() * 0.5f, NPC.scale, NPC.spriteDirection.ToSpriteDirection(), 0f);

            float bloomOpacity = SegmentOpenInterpolant.Squared() * 0.56f;
            Vector2 bloomDrawPosition = TurretPosition - screenPos;
            Main.spriteBatch.Draw(bloom, bloomDrawPosition, null, NPC.GetAlpha(Color.Red with { A = 0 }) * bloomOpacity, 0f, bloom.Size() * 0.5f, NPC.scale * 0.4f, 0, 0f);
            Main.spriteBatch.Draw(bloom, bloomDrawPosition, null, NPC.GetAlpha(Color.Wheat with { A = 0 }) * bloomOpacity * 0.5f, 0f, bloom.Size() * 0.5f, NPC.scale * 0.2f, 0, 0f);

            RenderInAccordanceWithHeadInstructions();

            return false;
        }
    }
}
