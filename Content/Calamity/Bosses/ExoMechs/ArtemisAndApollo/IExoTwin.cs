using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public interface IExoTwin
    {
        /// <summary>
        /// The current frame for this Exo Twin.
        /// </summary>
        public int Frame
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this Exo Twin has fully entered its second phase yet or not.
        /// </summary>
        /// 
        /// <remarks>
        /// In this context, "second phase" does not refer to the phases of the overall battle, instead referring to whether the Exo Twin has removed its lens and revealed its full mechanical form.
        /// </remarks>
        public bool InPhase2
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this Exo Twin has been destroyed due to impact during its death animation.
        /// </summary>
        public bool HasBeenDestroyed
        {
            get;
            set;
        }

        /// <summary>
        /// The opacity of wingtip vortices on this Exo Twin.
        /// </summary>
        public float WingtipVorticesOpacity
        {
            get;
            set;
        }

        /// <summary>
        /// The intensity boost of thrusters for this Exo Twin.
        /// </summary>
        public float ThrusterBoost
        {
            get;
            set;
        }

        /// <summary>
        /// The interpolant of motion blur for this Exo Twin.
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
        /// The current animation of this Exo Twin.
        /// </summary>
        public ExoTwinAnimation Animation
        {
            get;
            set;
        }

        /// <summary>
        /// The engine sound this Exo Twin plays.
        /// </summary>
        public LoopedSoundInstance EngineLoopSound
        {
            get;
            set;
        }

        /// <summary>
        /// The individual AI state of this Exo Twin. Only used if the shared AI state is <see cref="ExoTwinsAIState.PerformIndividualAttacks"/>.
        /// </summary>
        public IndividualExoTwinStateHandler IndividualState
        {
            get;
            set;
        }

        /// <summary>
        /// The palette of the optic nerve of this Exo Twin.
        /// </summary>
        public Color[] OpticNervePalette
        {
            get;
        }

        /// <summary>
        /// A specific, optionally definable draw action that may be used for specific, circumstantial effects.
        /// </summary>
        public Action? SpecificDrawAction
        {
            get;
            set;
        }

        /// <summary>
        /// A specific, optionally defineable shader action that may be used to prepare special effects in place of the standard afterimage blur shader.
        /// </summary>
        /// 
        /// <remarks>
        /// Returning <see langword="true"/> in this function indicates that an override has happened.
        /// </remarks>
        public Func<Texture2D, NPC, bool> SpecialShaderAction
        {
            get;
            set;
        }

        /// <summary>
        /// Resets local data for this Exo Twin due to an AI state transition.
        /// </summary>
        public void ResetLocalStateData();
    }
}
