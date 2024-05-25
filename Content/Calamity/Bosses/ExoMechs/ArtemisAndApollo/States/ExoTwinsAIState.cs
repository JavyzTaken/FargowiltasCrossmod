using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public enum ExoTwinsAIState
    {
        SpawnAnimation,
        DashesAndLasers,
        CloseShots,
        MachineGunLasers,

        Inactive,

        EnterSecondPhase,

        UltimateAttack,

        PerformIndividualAttacks,

        PerformComboAttack = ExoMechComboAttackManager.ComboAttackValue
    }
}
