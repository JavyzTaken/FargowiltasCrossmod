using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PulseCannonCage : ExoMechComboHandler
    {
        /// <summary>
        /// How long Ares waits before firing.
        /// </summary>
        public static int AresShootDelay => 90;

        /// <summary>
        /// How long a single pulse cannon shoot cycle lasts.
        /// </summary>
        public static int AresShootCycleTime => 97;

        /// <summary>
        /// The amount of delay between each cannon's firing.
        /// </summary>
        public static int AresShootDelayPerCannon => 12;

        /// <summary>
        /// How many shoot cycles should be performed by Ares before the attack ends.
        /// </summary>
        public static int AresShootCycleCount => 6;

        /// <summary>
        /// Hades' spin radius.
        /// </summary>
        public static float HadesSpinRadius => 728f;

        /// <summary>
        /// The maximum speed that players can be pushed to as a consequence of Ares' pulse cannons.
        /// </summary>
        public static float MaxBlastPushSpeed => 26.5f;

        /// <summary>
        /// The origin point that Hades should spin around.
        /// </summary>
        public static Vector2 HadesSpinOrigin
        {
            get
            {
                if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechWorm))
                    return Vector2.Zero;

                NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];
                return new(hades.ai[0], hades.ai[1]);
            }
            set
            {
                if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechWorm))
                    return;

                NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];
                hades.ai[0] = value.X;
                hades.ai[1] = value.Y;
            }
        }

        /// <summary>
        /// The sound played when Ares charges up a pulse cannon.
        /// </summary>
        public static readonly SoundStyle CannonChargeUpSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/PulseCannonCharge") with { Volume = 1.2f, MaxInstances = 0 };

        /// <summary>
        /// The sound played when Ares fires from one of his pulse cannons.
        /// </summary>
        public static readonly SoundStyle CannonFireSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/PulseCannonFire") with { MaxInstances = 0 };

        public override int[] ExpectedManagingExoMechs => [ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<AresBody>()];

        public override bool Perform(NPC npc)
        {
            if (npc.type == ExoMechNPCIDs.AresBodyID)
                return Perform_Ares(npc);

            Perform_Hades(npc);
            return AITimer >= AresShootDelay + AresShootCycleCount * AresShootCycleTime;
        }

        /// <summary>
        /// Performs Ares' part in the PulseCannonCage attack.
        /// </summary>
        /// <param name="npc">Ares' NPC instance.</param>
        public static bool Perform_Ares(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out AresBodyEternity ares))
            {
                npc.active = false;
                return false;
            }

            if (AITimer <= AresBodyEternity.DetachHands_DetachmentDelay)
            {
                for (int i = 0; i < ares.InstructionsForHands.Length; i++)
                {
                    int copyForDelegate = i;
                    ares.InstructionsForHands[i] = new(h => ares.DetachHandsUpdate(h, copyForDelegate));
                }
            }
            else
            {
                ares.InstructionsForHands[0] = new(h => AresHandUpdate(npc, h, new Vector2(-400f, 40f), 0));
                ares.InstructionsForHands[1] = new(h => AresHandUpdate(npc, h, new Vector2(-280f, 224f), 1));
                ares.InstructionsForHands[2] = new(h => AresHandUpdate(npc, h, new Vector2(280f, 224f), 2));
                ares.InstructionsForHands[3] = new(h => AresHandUpdate(npc, h, new Vector2(400f, 40f), 3));
            }

            float colorShiftInterpolant = LumUtils.InverseLerpBump(0f, 30f, AresShootDelay + AresShootCycleCount * AresShootCycleTime - 30f, AresShootDelay + AresShootCycleCount * AresShootCycleTime, AITimer);
            ares.ShiftLightColors(colorShiftInterpolant, new(81, 10, 220), new Color(156, 67, 220), Color.Wheat);

            npc.SmoothFlyNear(Target.Center - Vector2.UnitY.RotatedBy(MathHelper.TwoPi * AITimer / 1600f) * 350f, 0.063f, 0.945f);
            npc.rotation = npc.velocity.X * 0.007f;

            return false;
        }

        /// <summary>
        /// Handles updating for Ares' hands during the PulseCannonCage attack.
        /// </summary>
        /// <param name="aresBody">Ares' NPC instance.</param>
        /// <param name="hand">The hand ModNPC instance.</param>
        /// <param name="hoverOffset">The offset for the hand relative to the body's position.</param>
        /// <param name="aimDestination">Where the hand should aim.</param>
        /// <param name="armIndex">The arm's index in the overall set.</param>
        public static void AresHandUpdate(NPC aresBody, AresHand hand, Vector2 hoverOffset, int armIndex)
        {
            NPC handNPC = hand.NPC;
            Vector2 hoverDestination = aresBody.Center + hoverOffset.RotatedBy(aresBody.rotation) * aresBody.scale;

            hand.UsesBackArm = armIndex == 0 || armIndex == AresBodyEternity.ArmCount - 1;
            hand.ArmSide = (armIndex >= AresBodyEternity.ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.PulseCannon;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.GlowmaskDisabilityInterpolant = 0f;
            handNPC.spriteDirection = -hand.ArmSide;
            handNPC.Opacity = LumUtils.Saturate(handNPC.Opacity + 0.3f);

            int shootCycleDelay = AresShootDelay;
            int shootCycleDuration = AresShootCycleTime;
            int shootCycleTimer = (AITimer + armIndex * AresShootDelayPerCannon - shootCycleDelay) % shootCycleDuration;
            if (AITimer <= shootCycleDelay)
                shootCycleTimer = 0;

            // Charge up the cannon.
            hand.EnergyDrawer.chargeProgress = LumUtils.InverseLerp(shootCycleDuration * 0.56f, shootCycleDuration * 0.86f, shootCycleTimer + PulseBlast.Lifetime);

            // Predictively aim at the target.
            Vector2 aimDestination = Target.Center + Target.velocity * 20f;
            float idealRotation = handNPC.AngleTo(aimDestination);
            if (handNPC.spriteDirection == -1)
                idealRotation += MathHelper.Pi;

            // Look towards the ideal rotation. This rotational tapers off as Ares' cannon charges up, to ensure that it locks in place before firing.
            float aimInterpolant = MathF.Sqrt(1f - hand.EnergyDrawer.chargeProgress);
            handNPC.SmoothFlyNear(hoverDestination, 0.24f, 0.85f);
            handNPC.rotation = handNPC.rotation.AngleLerp(idealRotation, aimInterpolant * 0.3f);

            // Play a charge-up sound.
            if (shootCycleTimer == 1 && armIndex == 1)
                SoundEngine.PlaySound(CannonChargeUpSound, handNPC.Center);

            // Shoot the pulse blast.
            if (shootCycleTimer == shootCycleDuration - PulseBlast.Lifetime - 1)
            {
                SoundEngine.PlaySound(CannonFireSound, handNPC.Center).WithVolumeBoost(1.5f);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    LumUtils.NewProjectileBetter(handNPC.GetSource_FromAI(), handNPC.Center, Vector2.Zero, ModContent.ProjectileType<PulseBlast>(), 0, 0f, -1, handNPC.whoAmI);
                    handNPC.velocity -= handNPC.rotation.ToRotationVector2() * handNPC.spriteDirection * 30f;
                    handNPC.netUpdate = true;
                }
            }
        }

        /// <summary>
        /// Performs Hades' part in the PulseCannonCage attack.
        /// </summary>
        /// <param name="npc">Hades' NPC instance.</param>
        public static void Perform_Hades(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out HadesHeadEternity hades))
                return;

            // Initialize the spin origin if it's zeroed out or if the player managed to escape/got pushed out of the circle zone by Ares.
            bool targetEscaped = !Target.WithinRange(HadesSpinOrigin, HadesSpinRadius + 50f);
            if (HadesSpinOrigin == Vector2.Zero || targetEscaped)
            {
                HadesSpinOrigin = Target.Center;
                npc.netUpdate = true;
            }

            // Wait a bit before doing damage.
            bool doContactDamage = AITimer >= AresShootDelay;
            npc.damage = 0;
            if (doContactDamage)
                npc.damage = npc.defDamage;

            // Spin in place.
            Vector2 flyDestination = HadesSpinOrigin + (MathHelper.TwoPi * AITimer / 67f).ToRotationVector2() * HadesSpinRadius;
            npc.SmoothFlyNear(flyDestination, 0.6f, 0.4f);
            npc.Center = npc.Center.MoveTowards(flyDestination, 5f);
            npc.rotation = (npc.position - npc.oldPosition).ToRotation() + MathHelper.PiOver2;
            hades.SegmentReorientationStrength = 0.151f;

            // Keep some segments open, so that the player can actually do damage to Hades.
            hades.BodyBehaviorAction = new(HadesHeadEternity.EveryNthSegment(2), new(segment =>
            {
                HadesHeadEternity.OpenSegment()?.Invoke(segment);
                segment.NPC.damage = doContactDamage ? segment.NPC.defDamage : 0;
            }));
        }
    }
}
