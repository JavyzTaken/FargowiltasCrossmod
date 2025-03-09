using System.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public class IndividualExoTwinStateHandler
    {
        /// <summary>
        /// The AI timer used by the Exo Twin.
        /// </summary>
        public int AITimer;

        /// <summary>
        /// The current state that the Exo Twin is performing. This only applies if the Exo Twins are free to perform their own states.
        /// </summary>
        public ExoTwinsIndividualAIState AIState
        {
            get;
            set;
        }

        public IndividualExoTwinStateHandler(ExoTwinsIndividualAIState state)
        {
            AIState = state;
        }

        /// <summary>
        /// Updates this state.
        /// </summary>
        public void Update() => AITimer++;

        /// <summary>
        /// Resets all mutable data for this state in anticipation of a different one.
        /// </summary>
        public void Reset()
        {
            AITimer = 0;
        }

        /// <summary>
        /// Writes this state to a <see cref="BinaryWriter"/> for the purposes of being sent across the network.
        /// </summary>
        /// <param name="writer">The binary writer.</param>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((int)AIState);
            writer.Write(AITimer);
        }

        /// <summary>
        /// Reads a from a <see cref="BinaryReader"/> for the purposes of being received from across the network.
        /// </summary>
        /// <param name="reader">The binary reader.</param>
        public void ReadFrom(BinaryReader reader)
        {
            AIState = (ExoTwinsIndividualAIState)reader.ReadInt32();
            AITimer = reader.ReadInt32();
        }
    }
}
