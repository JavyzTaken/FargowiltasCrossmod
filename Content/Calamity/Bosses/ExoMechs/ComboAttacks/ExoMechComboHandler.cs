using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    public abstract class ExoMechComboHandler : ModType
    {
        // These are placed here in this class for ease of use when writing behavior code.
        /// <summary>
        /// The timer used during combo attacks. This is shared between all Exo Mechs that participate in the combo attack.
        /// </summary>
        protected static ref int AITimer => ref ExoMechComboAttackManager.ComboAttackTimer;

        /// <summary>
        /// The target that should be attacked during combo attacks.
        /// </summary>
        public static Player Target => ExoMechTargetSelector.Target;

        /// <summary>
        /// The set of all <b>Managing</b> Exo Mechs that are expected to be present for this combo attack.
        /// </summary>
        /// 
        /// <remarks>
        /// If this array is populated with Exo Mechs that aren't managing, such as Artemis, it will fail, since this array will only be compared against the active managing Exo Mechs, expecting an exact match.
        /// </remarks>
        public abstract int[] ExpectedManagingExoMechs
        {
            get;
        }

        protected sealed override void Register() => ExoMechComboAttackManager.RegisteredComboAttacks.Add(new(Perform, ExpectedManagingExoMechs));

        public sealed override void SetupContent() => SetStaticDefaults();

        /// <summary>
        /// Performs the combo attack's behavior.
        /// </summary>
        /// <param name="npc">The Exo Mech that should perform the attack.</param>
        /// <returns>Whether the combo attack has completed and a new one should be selected.</returns>
        public abstract bool Perform(NPC npc);
    }
}
