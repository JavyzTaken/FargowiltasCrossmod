namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers
{
    /// <summary>
    /// A representation of the state of a single Exo Mech.
    /// </summary>
    /// <param name="LifeRatio">The life ratio of the Exo Mech.</param>
    /// <param name="HasBeenSummoned">Whether the Exo Mech has been summoned yet or not.</param>
    /// <param name="Killed">Whether the Exo Mech has been killed yet or not.</param>
    public record ExoMechState(float LifeRatio, bool HasBeenSummoned, bool Killed)
    {
        /// <summary>
        /// A representation of an undefined Exo Mech state, where it has neither been summoned nor killed.
        /// </summary>
        public static readonly ExoMechState UndefinedExoMechState = new(1f, false, false);
    }
}
