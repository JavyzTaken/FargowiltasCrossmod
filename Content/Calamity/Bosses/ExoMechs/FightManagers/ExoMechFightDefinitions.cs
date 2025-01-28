using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using System;
using Terraria;
using Terraria.ModLoader;
using static FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers.ExoMechFightStateManager;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers
{
    /// <summary>
    /// The central location for all Exo Mech phase definitions.
    /// This governs the entire fight structure (such as life ratio threshold state changes), and is intended to be extraordinarily flexible.
    /// </summary>
    public static class ExoMechFightDefinitions
    {
        // NOTE -- Update XML comments if the life ratios are changed.
        public static float SummonOtherMechsLifeRatio => 0.7f;

        public static float FightAloneLifeRatio => 0.4f;

        /// <summary>
        /// How long it takes for the Exo Mechs to return to combat after a death animation if Draedon is not present.
        /// </summary>
        public static int NoDraedonExoMechReturnDelay => LumUtils.SecondsToFrames(2f);

        /// <summary>
        /// The first phase definition.
        /// </summary>
        /// 
        /// <remarks>
        /// During this phase, the player fights the first chosen Exo Mech until it reaches 70%.
        /// </remarks>
        public static readonly PhaseDefinition StartingSoloPhaseDefinition = CreateNewPhase(1, state =>
        {
            foreach (int exoMechID in ExoMechNPCIDs.ManagingExoMechIDs)
            {
                if (NPC.AnyNPCs(exoMechID))
                    return true;
            }

            return false;
        });

        /// <summary>
        /// The second phase definition.
        /// </summary>
        /// 
        /// <remarks>
        /// During this phase the player fights the two mechs that aren't the initial mech until one of them reaches 70%.
        /// </remarks>
        public static readonly PhaseDefinition StartingTwoAtOncePhaseDefinition = CreateNewPhase(2, state =>
        {
            return state.InitialMechState.LifeRatio <= SummonOtherMechsLifeRatio;
        }, state =>
        {
            SummonNotPresentExoMechs();
            MakeExoMechLeaveOrReappear(true, (npc, exoMech) => npc.life <= npc.lifeMax * SummonOtherMechsLifeRatio);
        });

        /// <summary>
        /// The third phase definition.
        /// </summary>
        /// 
        /// <remarks>
        /// During this phase the player fights all three mechs at once, until one of them reaches 40%.
        /// </remarks>
        public static readonly PhaseDefinition MechaMayhemPhaseDefinition = CreateNewPhase(3, state =>
        {
            for (int i = 0; i < state.OtherMechsStates.Length; i++)
            {
                var otherMechState = state.OtherMechsStates[i];
                if (otherMechState.HasBeenSummoned && otherMechState.LifeRatio <= SummonOtherMechsLifeRatio)
                    return true;
            }

            return false;
        }, state => MakeExoMechLeaveOrReappear(false, (npc, exoMech) => true));

        /// <summary>
        /// The fourth phase definition.
        /// </summary>
        /// 
        /// <remarks>
        /// During this phase the player fights the Exo Mech they brought down to 40% until it is killed.
        /// </remarks>
        public static readonly PhaseDefinition FirstSoloUntilDeadPhaseDefinition = CreateNewPhase(4, state =>
        {
            for (int i = 0; i < state.OtherMechsStates.Length; i++)
            {
                var otherMechState = state.OtherMechsStates[i];
                if (otherMechState.HasBeenSummoned && otherMechState.LifeRatio <= FightAloneLifeRatio)
                    return true;
            }

            return state.InitialMechState.LifeRatio <= FightAloneLifeRatio;
        }, state => MakeExoMechLeaveOrReappear(true, (npc, exoMech) => npc.life > npc.lifeMax * FightAloneLifeRatio));

        /// <summary>
        /// The first Draedon interjection, performed after the player kills the first Exo Mech.
        /// </summary>
        public static readonly PhaseDefinition DraedonFirstInterjectionPhaseDefinition = CreateNewPhase(5, state =>
        {
            return state.TotalKilledMechs >= 1;
        }, state =>
        {
            ExoMechSummonDelayTimer = 0;
            SetDraedonState(DraedonEternity.DraedonAIState.FirstInterjection);
        });

        /// <summary>
        /// The fifth phase definition.
        /// </summary>
        /// 
        /// <remarks>
        /// During this phase the player fights the two remaining Exo Mechs until one of them reaches 40%.
        /// </remarks>
        public static readonly PhaseDefinition SecondTwoAtOncePhaseDefinition = CreateNewPhase(6, state =>
        {
            if (state.DraedonState is null)
                return ExoMechSummonDelayTimer >= NoDraedonExoMechReturnDelay;

            return state.DraedonState != DraedonEternity.DraedonAIState.FirstInterjection;
        }, state => MakeExoMechLeaveOrReappear(false, (npc, exoMech) => true));

        /// <summary>
        /// The sixth phase definition.
        /// </summary>
        /// 
        /// <remarks>
        /// During this phase the player fights the Exo Mech they brought down to 40% until it dies.
        /// </remarks>
        public static readonly PhaseDefinition SecondToLastSoloPhaseDefinition = CreateNewPhase(7, state =>
        {
            for (int i = 0; i < state.OtherMechsStates.Length; i++)
            {
                var otherMechState = state.OtherMechsStates[i];
                if (otherMechState.HasBeenSummoned && !otherMechState.Killed && otherMechState.LifeRatio <= FightAloneLifeRatio)
                    return true;
            }

            return !state.InitialMechState.Killed && state.InitialMechState.LifeRatio <= FightAloneLifeRatio;
        }, state => MakeExoMechLeaveOrReappear(true, (npc, exoMech) => npc.life > npc.lifeMax * FightAloneLifeRatio));

        /// <summary>
        /// The second Draedon interjection, performed after the player kills the second Exo Mech.
        /// </summary>
        public static readonly PhaseDefinition DraedonSecondInterjectionPhaseDefinition = CreateNewPhase(8, state =>
        {
            return state.TotalKilledMechs >= 2;
        }, state =>
        {
            ExoMechSummonDelayTimer = 0;
            SetDraedonState(DraedonEternity.DraedonAIState.SecondInterjection);
        });

        /// <summary>
        /// The seventh and final phase definition.
        /// </summary>
        /// 
        /// <remarks>
        /// During this phase the player fights the final Exo Mech until it dies.
        /// </remarks>
        public static readonly PhaseDefinition BerserkSoloPhaseDefinition = CreateNewPhase(9, state =>
        {
            if (state.DraedonState is null)
                return ExoMechSummonDelayTimer >= NoDraedonExoMechReturnDelay;

            return state.DraedonState != DraedonEternity.DraedonAIState.SecondInterjection;
        }, state => MakeExoMechLeaveOrReappear(false, (npc, exoMech) => true));

        /// <summary>
        /// The third and final Draedon interjection, performed after the player completes the Exo Mech fight.
        /// </summary>
        public static readonly PhaseDefinition DraedonThirdInterjectionPhaseDefinition = CreateNewPhase(10, state =>
        {
            return state.TotalKilledMechs >= 3;
        }, state =>
        {
            SetDraedonState(DraedonEternity.DraedonAIState.PostBattleInterjection);
        });

        public static void MakeExoMechLeaveOrReappear(bool leave, Func<NPC, IExoMech, bool> condition)
        {
            if (leave)
                ClearExoMechProjectiles();

            ApplyToAllExoMechs(npc =>
            {
                if (npc.TryGetDLCBehavior(out CalDLCEmodeBehavior b) && b is IExoMech exoMech && condition(npc, exoMech))
                {
                    exoMech.Inactive = leave;
                    npc.netUpdate = true;
                }
            });
        }

        public static void SetDraedonState(DraedonEternity.DraedonAIState state)
        {
            int draedonIndex = NPC.FindFirstNPC(ModContent.NPCType<CalamityMod.NPCs.ExoMechs.Draedon>());
            if (draedonIndex >= 0 && Main.npc[draedonIndex].TryGetDLCBehavior(out DraedonEternity behavior))
            {
                behavior.ChangeAIState(state);
                Main.npc[draedonIndex].netUpdate = true;
            }
        }
    }
}
