namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs
{
    public interface IExoMech
    {
        /// <summary>
        /// Whether this Exo Mech should be inactive, leaving the battle to let other mechs attack on their own.
        /// </summary>
        public bool Inactive
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this Exo Mech is performing a combo attack.
        /// </summary>
        public bool PerformingComboAttack
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this Exo Mech is a primary mech or not, a.k.a the one that the player chose when starting the battle.
        /// </summary>
        public bool IsPrimaryMech
        {
            get;
            set;
        }

        /// <summary>
        /// Instructs this Exo Mech to use a new state.
        /// </summary>
        public void SelectNewState();
    }
}
