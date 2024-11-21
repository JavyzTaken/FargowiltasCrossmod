using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SuperchargedTurbodashes : ExoMechComboHandler
    {
        public static int ElectrifyTime => LumUtils.SecondsToFrames(4f);

        public static float AresMaxFlySpeed_Electrify => 23f;

        public static Vector2 AresAcceleration_Electrify => Vector2.One * 0.4f;

        public override int[] ExpectedManagingExoMechs => [ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<AresBody>()];

        public override bool Perform(NPC npc)
        {
            if (AITimer >= ElectrifyTime - 1)
                AITimer = 0;

            if (npc.type == ExoMechNPCIDs.AresBodyID)
                return Perform_Ares(npc);

            Perform_Hades(npc);
            return false;
        }

        /// <summary>
        /// Performs Ares' part in the SuperchargedTurbodashes attack.
        /// </summary>
        /// <param name="npc">Ares' NPC instance.</param>
        public static bool Perform_Ares(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out AresBodyEternity ares))
            {
                npc.active = false;
                return false;
            }

            ares.ShiftLightColors(LumUtils.InverseLerp(0f, 30f, AITimer), new(239, 62, 62), new(242, 112, 72));

            ares.InstructionsForHands[0] = new(h => AresHandUpdate(npc, h, new Vector2(-400f, 40f), 0));
            ares.InstructionsForHands[1] = new(h => AresHandUpdate(npc, h, new Vector2(-280f, 224f), 1));
            ares.InstructionsForHands[2] = new(h => AresHandUpdate(npc, h, new Vector2(280f, 224f), 2));
            ares.InstructionsForHands[3] = new(h => AresHandUpdate(npc, h, new Vector2(400f, 40f), 3));

            if (AITimer <= ElectrifyTime)
            {
                float movementWindUpInterpolant = LumUtils.InverseLerp(0f, 40f, AITimer).Squared();
                Vector2 hoverDestination = Target.Center - Vector2.UnitY * 250f;
                Vector2 idealDirection = npc.SafeDirectionTo(hoverDestination);
                Vector2 acceleration = AresAcceleration_Electrify * movementWindUpInterpolant;

                npc.velocity = (npc.velocity + idealDirection * acceleration).ClampLength(0f, AresMaxFlySpeed_Electrify);
                if (npc.velocity.AngleBetween(idealDirection) >= 1.37f)
                    npc.velocity *= 0.94f;
            }

            return false;
        }

        /// <summary>
        /// Handles updating for Ares' hands during the SuperchargedTurbodashes attack.
        /// </summary>
        /// <param name="aresBody">Ares' NPC instance.</param>
        /// <param name="hand">The hand ModNPC instance.</param>
        /// <param name="hoverOffset">The offset for the hand relative to the body's position.</param>
        /// <param name="aimDestination">Where the hand should aim.</param>
        /// <param name="armIndex">The arm's index in the overall set.</param>
        public static void AresHandUpdate(NPC aresBody, AresHand hand, Vector2 hoverOffset, int armIndex)
        {
            NPC handNPC = hand.NPC;
            Vector2 hoverDestination = aresBody.Center + hoverOffset * aresBody.scale;

            hand.UsesBackArm = armIndex == 0 || armIndex == AresBodyEternity.ArmCount - 1;
            hand.ArmSide = (armIndex >= AresBodyEternity.ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.LaserCannon;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.GlowmaskDisabilityInterpolant = 0f;
            handNPC.spriteDirection = 1;
            handNPC.Opacity = LumUtils.Saturate(handNPC.Opacity + 0.3f);

            // Pick the nearest Hades segment to electrify.
            float minDistance = 999999f;
            Vector2 aimDestination = handNPC.Center + Vector2.UnitY * 300f;
            foreach (NPC hadesSegment in Main.ActiveNPCs)
            {
                if (hadesSegment.type != ExoMechNPCIDs.HadesHeadID && hadesSegment.realLife != CalamityGlobalNPC.draedonExoMechWorm)
                    continue;

                if (minDistance > handNPC.Distance(hadesSegment.Center))
                {
                    minDistance = handNPC.Distance(hadesSegment.Center);
                    aimDestination = hadesSegment.Center;
                }
            }

            handNPC.SmoothFlyNear(hoverDestination, 0.25f, 0.75f);
            handNPC.rotation = handNPC.rotation.AngleLerp(handNPC.AngleTo(aimDestination), 0.15f);

            Vector2 cannonEnd = handNPC.Center + new Vector2(handNPC.spriteDirection * 76f, 8f).RotatedBy(handNPC.rotation);
            if (AITimer <= ElectrifyTime)
            {
                float arcCreationChance = MathF.Pow(LumUtils.InverseLerp(0f, ElectrifyTime, AITimer), 0.75f) * 0.6f;
                for (int i = 0; i < 2; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(arcCreationChance))
                    {
                        Vector2 arcLength = (aimDestination - cannonEnd).RotatedByRandom(0.02f) * Main.rand.NextFloat(0.97f, 1.03f);
                        LumUtils.NewProjectileBetter(handNPC.GetSource_FromAI(), cannonEnd, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(6, 9), 1f);
                    }
                }

                Dust energy = Dust.NewDustPerfect(cannonEnd, 182);
                energy.velocity = handNPC.SafeDirectionTo(cannonEnd).RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloatDirection() * 6f + handNPC.velocity;
                energy.noGravity = true;

                hand.EnergyDrawer.chargeProgress = MathF.Sqrt(LumUtils.InverseLerpBump(0f, ElectrifyTime * 0.75f, ElectrifyTime * 0.81f, ElectrifyTime, AITimer));
            }
        }

        /// <summary>
        /// Performs Hades' part in the SuperchargedTurbodashes attack.
        /// </summary>
        /// <param name="npc">Hades' NPC instance.</param>
        public static void Perform_Hades(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out HadesHeadEternity hades))
            {
                npc.active = false;
                return;
            }

            if (AITimer <= 120)
                npc.damage = 0;

            Vector2 hoverDestination = Target.Center;
            npc.velocity = Vector2.Lerp(npc.velocity, npc.SafeDirectionTo(hoverDestination) * 17f, 0.04f);
            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;

            hades.SegmentReorientationStrength = 0f;
            hades.BodyBehaviorAction = new(HadesHeadEternity.EveryNthSegment(3), HadesHeadEternity.OpenSegment());
        }
    }
}
