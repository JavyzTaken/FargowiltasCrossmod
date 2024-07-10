using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public enum ExoTwinsAIState
    {
        SpawnAnimation,
        DashesAndLasers,
        CloseShots,
        MachineGunLasers,
        ExothermalOverload,

        Inactive,
        Leave,

        EnterSecondPhase,

        DeathAnimation,

        PerformIndividualAttacks,

        PerformComboAttack = ExoMechComboAttackManager.ComboAttackValue
    }
}
