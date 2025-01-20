using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common.InverseKinematics;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    public class PerforatorLeg
    {
        /// <summary>
        /// The 0-1 interpolant of how far this leg is in its forward step animation.
        /// </summary>
        public float StepAnimationInterpolant;

        /// <summary>
        /// The standard offset for this leg from its owner when not moving.
        /// </summary>
        public Vector2 DefaultOffset;

        /// <summary>
        /// Where the leg started at at the beginning of its step animation.
        /// </summary>
        public Vector2 EndEffectorPositionAtStartOfStep;

        /// <summary>
        /// Where the leg should end up at the end of its step animation.
        /// </summary>
        public Vector2 StepDestination;

        /// <summary>
        /// The effective offset for this leg from its owner when not moving. Unlike <see cref="DefaultOffset"/>, this is subject to safety conditions such as tile-collision checks.
        /// </summary>
        public Vector2 MovingDefaultStepPosition;

        /// <summary>
        /// The kinematic chain that governs the orientation of this leg.
        /// </summary>
        public KinematicChain Leg;

        /// <summary>
        /// The general size factor of this spider's legs.
        /// </summary>
        public readonly float LegSizeFactor;

        public readonly int Index;

        public PerforatorLeg(Vector2 defaultOffset, float legSizeFactor, float legLength, int index)
        {
            LegSizeFactor = legSizeFactor;
            DefaultOffset = defaultOffset;
            StepAnimationInterpolant = 0.02f;
            Leg = new();
            Leg.Add(new(LegSizeFactor * legLength));
            Leg.Add(new(LegSizeFactor * legLength));
            Index = index;
        }
        Vector2 LegCenter(PerfsEternityNew owner) => owner.NPC.Center + owner.LegBraces[Index];
        public void Update(NPC owner)
        {
            var hive = owner.GetDLCBehavior<PerfsEternityNew>();
            if (owner.IsABestiaryIconDummy)
            {
                Leg.Update(LegCenter(hive) + DefaultOffset);
                return;
            }

            float spawnProgress = hive.SpawnProgress;

            // Calculate how many legs are on the ground.
            int legSide = DefaultOffset.X.NonZeroSign();
            var legOnSameSideAsMe = hive.Legs.Where(l => l.DefaultOffset.X.NonZeroSign() == legSide).ToArray();
            int totalRemainingGroundedLegs = legOnSameSideAsMe.Count(l => l.StepAnimationInterpolant <= 0f);
            int legsOnGroundIfISteppedForward = totalRemainingGroundedLegs - 1;

            // Store direction vectors for ease of use.
            Vector2 gravityDirection = hive.GravityDirection;
            Vector2 forwardDirection = new(gravityDirection.Y, gravityDirection.X);
            bool falling = Vector2.Dot(owner.velocity, gravityDirection) >= 8f || spawnProgress < 0.8f;

            // Initialize the step destination if necessary.
            if (StepDestination == Vector2.Zero)
                StepDestination = LegCenter(hive) + DefaultOffset;

            // Keep the leg below the owner if they're falling.
            if (falling)
                StepDestination = Vector2.Lerp(StepDestination, LegCenter(hive) + gravityDirection * LegSizeFactor * 160f, 1f);

            // Prevent the leg from being behind walls.
            Vector2 stepOffset = DefaultOffset.RotatedBy(gravityDirection.AngleBetween(Vector2.UnitY));
            Vector2 idealDefaultStepPosition = LumUtils.FindGround((LegCenter(hive) + stepOffset).ToTileCoordinates(), gravityDirection).ToWorldCoordinates(8f, -16f);
            for (int i = 0; i < 50; i++)
            {
                if (Collision.CanHitLine(LegCenter(hive), 1, 1, idealDefaultStepPosition, 1, 1) && !Collision.SolidCollision(idealDefaultStepPosition, 1, 1))
                    break;

                idealDefaultStepPosition -= gravityDirection * 8f;
            }

            // Make the step position interpolate towards its ideal. This helps allow for the legs to reorient naturally and prevents edge-cases where the leg snaps to a new position.
            MovingDefaultStepPosition = Vector2.Lerp(MovingDefaultStepPosition, idealDefaultStepPosition, 0.11f);

            // Attach to the owner at all times.
            Leg.StartingPoint = LegCenter(hive);

            // Keep the limbs from pointing downward.
            Leg[0].Constraints = [new FixedAngleDifferenceConstraint(0.6f, gravityDirection.ToRotation() + MathF.PI)];

            // Move limbs forward if necessary.
            if (StepAnimationInterpolant > 0f)
                UpdateMovementAnimation(gravityDirection);
            else
                KeepLegInPlace(gravityDirection);

            // Determine if the limb needs to change position. If it does, do so. This is based on the following conditions:
            // 1. A leg is unrealistically close to the body, step back.
            // 2. A leg is too far from the body and thusly lagging behind, step forward.

            // However, as long as the owner is visible, a step cannot happen if any of the following conditions are true:
            // 1. The owner is falling. There would be no point to trying to step forward if there's no nearby ground in the first place.
            // 2. If in stepping forward, no more legs would be on the ground. It obviously makes no logical sense for a spider to move its leg if in doing so it would lose its balance.
            // 3. An animation is already ongoing. Trying to restart it during this process would cause the animations to fail and keyframes to become inaccurate, as
            // they assume that when a leg starts an animation it was on ground to begin with.
            // 4. The owner is barely moving at all (assuming they're visible).
            float perpendicularDistanceFromOwner = Math.Abs(LumUtils.SignedDistanceToLine(Leg.EndEffectorPosition, LegCenter(hive), forwardDirection));

            bool tooCloseToBody = perpendicularDistanceFromOwner <= Math.Abs(DefaultOffset.X) * 0.2f;
            bool tooFarFromOwner = perpendicularDistanceFromOwner >= LegSizeFactor * 130f || !StepDestination.WithinRange(MovingDefaultStepPosition, LegSizeFactor * 130f);
            bool shouldStepForward = tooFarFromOwner || tooCloseToBody;
            bool cannotStepForward = falling || legsOnGroundIfISteppedForward <= 0 || StepAnimationInterpolant > 0f || owner.velocity.Length() <= 0.3f;
            if (owner.Opacity <= 0.1f)
                cannotStepForward = false;

            if (shouldStepForward && !cannotStepForward)
                StartStepAnimation(owner, gravityDirection, forwardDirection);
        }

        public static void ApplySlopeOffsets(ref Vector2 idealStepPosition)
        {
            Tile ground = Framing.GetTileSafely(idealStepPosition.ToTileCoordinates());
            Vector2 groundPositionSnapped = (idealStepPosition / 16f).Floor() * 16f + Vector2.UnitY * 16f;

            // Ignore tiles that aren't interactable.
            if (!ground.HasUnactuatedTile)
                return;

            float tileSlopeInterpolant = LumUtils.InverseLerp(0f, 16f, idealStepPosition.X % 16f);
            if (ground.IsHalfBlock)
                idealStepPosition.Y = groundPositionSnapped.Y - 8f;
            else if (ground.Slope == SlopeType.SlopeDownLeft)
                idealStepPosition.Y = groundPositionSnapped.Y - MathHelper.Lerp(16f, 0f, tileSlopeInterpolant) + 2f;
            else if (ground.Slope == SlopeType.SlopeDownRight)
                idealStepPosition.Y = groundPositionSnapped.Y - MathHelper.Lerp(0f, 16f, tileSlopeInterpolant) + 2f;
        }

        public void UpdateMovementAnimation(Vector2 gravityDirection)
        {
            // Increment the animation interpolant.
            StepAnimationInterpolant += 0.05f; //0.064f;

            // Calculate the current movement destination based on the animation's completion.
            // This gradually goes from the starting position and ends up at the step destination, making a slight upward arc while doing so.
            Vector2 movementDestination = Vector2.Lerp(EndEffectorPositionAtStartOfStep, StepDestination, LumUtils.Saturate(StepAnimationInterpolant));
            movementDestination -= gravityDirection * LumUtils.Convert01To010(StepAnimationInterpolant) * 18f;

            // Move the leg.
            Leg.Update(movementDestination);

            // Stop the animation once it has completed.
            if (StepAnimationInterpolant >= 1f)
            {
                StepAnimationInterpolant = 0f;
                SoundEngine.PlaySound(SoundID.Dig with { Pitch = -0.5f }, StepDestination);
            }
        }

        public void KeepLegInPlace(Vector2 gravityDirection)
        {
            // Stay at the step destination.
            // This will, barring the above exception, be where the leg stopped at the last time a step was performed.
            Leg.Update(StepDestination);
        }

        public void StartStepAnimation(NPC owner, Vector2 gravityDirection, Vector2 forwardDirection)
        {
            // Calculate the position to step towards.
            float ownerDirection = Vector2.Dot(owner.velocity, forwardDirection).NonZeroSign();
            float offsetDirection = DefaultOffset.X.NonZeroSign();
            Vector2 aimAheadOffset = new Vector2(Math.Abs(forwardDirection.X), Math.Abs(forwardDirection.Y)) * owner.velocity.ClampLength(0f, 3.67f) * 12f;
            if (ownerDirection != offsetDirection)
                aimAheadOffset *= 2.2f;
            else
                aimAheadOffset /= LegSizeFactor;
            aimAheadOffset.X += Main.rand.NextFloatDirection() * 20f;

            // Start the animation.
            StepAnimationInterpolant = 0.02f;
            EndEffectorPositionAtStartOfStep = Leg.EndEffectorPosition;
            StepDestination = LumUtils.FindGround((MovingDefaultStepPosition + aimAheadOffset).ToTileCoordinates(), gravityDirection).ToWorldCoordinates(8f, 20f);

            // Apply slope vertical offsets to the step position.
            ApplySlopeOffsets(ref StepDestination);
        }
    }
}
