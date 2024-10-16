using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class WalledSlashes : ExoMechComboHandler
    {
        /// <summary>
        /// How long the walled slashes attack goes on for until a new combo is selected.
        /// </summary>
        public static int AttackDuration => Variables.GetAIInt("WalledSlashes_AttackDuration", ExoMechAIVariableType.Combo);

        /// <summary>
        /// Ares' slash cycle time, which dictates how fast each set of slashes are.
        /// </summary>
        public static int AresSlashCycleTime => Variables.GetAIInt("WalledSlashes_AresSlashCycleTime", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How long Ares waits before slashing.
        /// </summary>
        public static int AresSlashDelay => Variables.GetAIInt("WalledSlashes_AresSlashDelay", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The max speed at which Ares can fly when trying to reach the player.
        /// </summary>
        public static float AresMaxFlySpeed => Variables.GetAIFloat("WalledSlashes_AresMaxFlySpeed", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How far vertically Ares attempts to hover relative to the target's position.
        /// </summary>
        public static float AresVerticalHoverOffset => Variables.GetAIFloat("WalledSlashes_AresVerticalHoverOffset", ExoMechAIVariableType.Combo);

        /// <summary>
        /// Ares' fly acceleration while he attempts to slash the player.
        /// </summary>
        public static Vector2 AresAcceleration => new Vector2(0.3f, 0.14f) * Variables.GetAIFloat("WalledSlashes_AresAccelerationFactor", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The rate at which Hades releases mines.
        /// </summary>
        public static int HadesMineReleaseRate => Variables.GetAIInt("WalledSlashes_HadesMineReleaseRate", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The radius at which Hades spins around his focal point.
        /// </summary>
        public static float HadesSpinRadius => Variables.GetAIFloat("WalledSlashes_HadesSpinRadius", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The speed at which Hades spins around his focal point.
        /// </summary>
        public static float HadesSpinSpeed => Variables.GetAIFloat("WalledSlashes_HadesSpinSpeed", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How slowly Hades rotates around his focal point.
        /// </summary>
        public static float HadesSpinPeriod => Variables.GetAIFloat("WalledSlashes_HadesSpinPeriod", ExoMechAIVariableType.Combo);

        public override int[] ExpectedManagingExoMechs => [ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<AresBody>()];

        public override bool Perform(NPC npc)
        {
            if (npc.type == ModContent.NPCType<AresBody>())
                return Perform_Ares(npc);

            Perform_Hades(npc);
            return false;
        }

        /// <summary>
        /// Performs Ares' part in the WalledSlashes attack.
        /// </summary>
        /// <param name="npc">Ares' NPC instance.</param>
        public static bool Perform_Ares(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out AresBodyEternity ares))
            {
                npc.active = false;
                return false;
            }

            ares.InstructionsForHands[0] = new(h => ares.KatanaSlashesHandUpdate(h, new Vector2(-400f, 40f), AresSlashDelay, AresSlashCycleTime, 0, true));
            ares.InstructionsForHands[1] = new(h => ares.KatanaSlashesHandUpdate(h, new Vector2(-280f, 224f), AresSlashDelay, AresSlashCycleTime, 1, true));
            ares.InstructionsForHands[2] = new(h => ares.KatanaSlashesHandUpdate(h, new Vector2(280f, 224f), AresSlashDelay, AresSlashCycleTime, 2, true));
            ares.InstructionsForHands[3] = new(h => ares.KatanaSlashesHandUpdate(h, new Vector2(400f, 40f), AresSlashDelay, AresSlashCycleTime, 3, true));

            float movementWindUpInterpolant = Utilities.InverseLerp(0f, AresSlashDelay, AITimer).Squared();
            Vector2 hoverDestination = Target.Center + Vector2.UnitY * AresVerticalHoverOffset;
            Vector2 idealDirection = npc.SafeDirectionTo(hoverDestination);
            Vector2 acceleration = AresAcceleration * movementWindUpInterpolant;

            npc.velocity = (npc.velocity + idealDirection * acceleration).ClampLength(0f, AresMaxFlySpeed);
            if (npc.velocity.AngleBetween(idealDirection) >= 1.37f)
                npc.velocity *= 0.93f;

            bool attackHasCompleted = AITimer >= AttackDuration;
            return attackHasCompleted;
        }

        /// <summary>
        /// Performs Hades' part in the WalledSlashes attack.
        /// </summary>
        /// <param name="npc">Hades' NPC instance.</param>
        public static void Perform_Hades(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out HadesHeadEternity hades))
            {
                npc.active = false;
                return;
            }

            if (AITimer == 1)
            {
                npc.ai[0] = Target.Center.X;
                npc.ai[1] = Target.Center.Y;
                npc.netUpdate = true;
            }

            if (AITimer <= 120)
                npc.damage = 0;

            npc.ai[2] += MathHelper.TwoPi * Utilities.InverseLerp(0f, 150f, AITimer).Squared() / HadesSpinPeriod;

            Vector2 spinOffset = npc.ai[2].ToRotationVector2() * HadesSpinRadius;
            Vector2 hoverDestination = new Vector2(npc.ai[0], npc.ai[1]) + spinOffset;
            npc.velocity = Vector2.Lerp(npc.velocity, npc.SafeDirectionTo(hoverDestination) * HadesSpinSpeed, 0.3f);
            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;

            if (Main.netMode != NetmodeID.MultiplayerClient && AITimer % HadesMineReleaseRate == HadesMineReleaseRate - 1)
                Utilities.NewProjectileBetter(npc.GetSource_FromAI(), npc.Center, npc.SafeDirectionTo(Target.Center) * Main.rand.NextFloat(50f, 140f), ModContent.ProjectileType<HadesMine>(), HadesHeadEternity.MineDamage, 0f);

            hades.SegmentReorientationStrength = 0f;
            hades.BodyBehaviorAction = new(HadesHeadEternity.EveryNthSegment(3), HadesHeadEternity.OpenSegment());
        }
    }
}
