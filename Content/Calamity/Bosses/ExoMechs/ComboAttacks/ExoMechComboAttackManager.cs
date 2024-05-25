using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoMechComboAttackManager : ModSystem
    {
        // TODO -- This is definitely gonna need to be synced somewhere.
        /// <summary>
        /// The current attack timer for the combo attack.
        /// </summary>
        internal static int ComboAttackTimer;

        /// <summary>
        /// The set of all registered combo attacks that can be performed.
        /// </summary>
        internal static readonly List<ExoMechComboAttack> RegisteredComboAttacks = new(16);

        /// <summary>
        /// The current combo attack state that all Exo Mechs should perform.
        /// </summary>
        public static ExoMechComboAttack CurrentState
        {
            get;
            private set;
        }

        /// <summary>
        /// A representation of a null combo attack state. Used as a fallback when Exo Mechs are not currently using a combo attack.
        /// </summary>
        public static readonly ExoMechComboAttack NullComboState = new(n => false);

        /// <summary>
        /// The underlying value that should be used for all AI state enumerations across the Exo Mechs.
        /// </summary>
        public const int ComboAttackValue = 1000;

        /// <summary>
        /// Represents an Exo Mech combo attack action.
        /// </summary>
        /// <param name="npc">The Exo Mech that should perform the attack.</param>
        /// <returns>Whether the combo attack has completed and a new one should be selected.</returns>
        public delegate bool ExoMechComboAttackAction(NPC npc);

        /// <summary>
        /// Represents an Exo Mech combo attack.
        /// </summary>
        /// <param name="AttackAction">The delegate responsible for updating the Exo Mechs during the combo attack.</param>
        /// <param name="ExpectedManagingExoMechIDs">The set of all managing Exo Mech NPC IDs that are expected to be present during the combo attack. If the exo mech set does not match this exactly, this attack will not execute.</param>
        public record ExoMechComboAttack(ExoMechComboAttackAction AttackAction, params int[] ExpectedManagingExoMechIDs)
        {
            /// <summary>
            /// Whether this combo attack is undefined.
            /// </summary>
            public bool Undefined => ExpectedManagingExoMechIDs.Length <= 0;
        }

        public override void PostUpdateNPCs()
        {
            if (!ExoMechFightStateManager.FightOngoing)
            {
                Reset();
                return;
            }

            // This is indirectly responsible for ensuring that Exo Mechs return to solo attacks once the combo attacks need to end.
            if (ExoMechFightStateManager.FightState.TotalActiveMechs <= 1)
            {
                Reset();
                SetComboStateForAllActiveExoMechs(false);
                return;
            }

            EnsureAllExoMechsArePerformingCombo();
            PuppeteerActiveExoMechs();
            ComboAttackTimer++;
        }

        /// <summary>
        /// Resets generic data pertaining to the combo attacks.
        /// </summary>
        private static void Reset()
        {
            CurrentState = NullComboState;
            ComboAttackTimer = 0;
        }

        /// <summary>
        /// Performs checks and corrections that ensure that all Exo Mechs are performing the same combo attack.
        /// </summary>
        private static void EnsureAllExoMechsArePerformingCombo()
        {
            bool everyoneIsPerformingCombo = ExoMechFightStateManager.ActiveManagingExoMechs.All(m => ((IExoMech)m).PerformingComboAttack);

            if (!everyoneIsPerformingCombo)
            {
                if (CurrentState?.Undefined ?? true)
                    SelectNewComboAttackState();

                SetComboStateForAllActiveExoMechs(true);
            }

            if (!VerifyComboStateIsValid(CurrentState))
                SelectNewComboAttackState();
        }

        /// <summary>
        /// Determines whether a combo attack state is valid given the current fight state.
        /// </summary>
        /// <param name="attack">The attack to evaluate.</param>
        private static bool VerifyComboStateIsValid(ExoMechComboAttack? attack)
        {
            if (attack is null)
                return false;

            bool isValid = true;
            HashSet<int> expectedSet = new(attack.ExpectedManagingExoMechIDs);
            HashSet<int> activeSet = new(ExoMechFightStateManager.ActiveManagingExoMechs.Select(m => m.NPC.type));
            if (!expectedSet.SetEquals(activeSet))
                isValid = false;

            return isValid;
        }

        /// <summary>
        /// Makes all active Exo Mechs either start or stop performing combo attacks.
        /// </summary>
        /// <param name="everyoneShouldPerformCombos">Whether the combo attacks should be performed or not.</param>
        private static void SetComboStateForAllActiveExoMechs(bool everyoneShouldPerformCombos)
        {
            bool anyStatesWereChanged = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!ExoMechNPCIDs.ManagingExoMechIDs.Contains(npc.type) || !npc.TryGetDLCBehavior(out CalDLCEmodeBehavior behavior))
                    continue;

                if (behavior is IExoMech exoMech && !exoMech.Inactive && exoMech.PerformingComboAttack != everyoneShouldPerformCombos)
                {
                    exoMech.PerformingComboAttack = everyoneShouldPerformCombos;
                    anyStatesWereChanged = true;
                }
            }

            if (anyStatesWereChanged)
                ExoMechFightStateManager.ClearExoMechProjectiles();
        }

        /// <summary>
        /// Selects a new combo attack that the Exo Mechs can perform.
        /// </summary>
        private static void SelectNewComboAttackState()
        {
            ComboAttackTimer = 0;

            var potentialCandidates = RegisteredComboAttacks.Where(VerifyComboStateIsValid).OrderBy(_ => Main.rand.NextFloat());
            var previousState = CurrentState;

            for (int i = 0; i < 100; i++)
            {
                CurrentState = potentialCandidates.FirstOrDefault() ?? NullComboState;
                if (CurrentState != previousState)
                    break;
            }
        }

        /// <summary>
        /// Pupeteers all active Exo Mechs, ensuring that they adhere to the strict organization of the current combo attack and execute the behavior delegate.
        /// </summary>
        private static void PuppeteerActiveExoMechs()
        {
            bool hadesIsPresent = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!ExoMechNPCIDs.ExoMechIDs.Contains(npc.type) || !npc.TryGetDLCBehavior(out CalDLCEmodeBehavior behavior))
                    continue;

                if (behavior is IExoMech exoMech && !exoMech.Inactive)
                {
                    // To clarify, this if statement's execution encompasses the behavior state update.
                    if (CurrentState?.AttackAction?.Invoke(npc) ?? true)
                        SelectNewComboAttackState();

                    if (behavior is HadesHeadEternity)
                        hadesIsPresent = true;
                }
            }

            // Special case: Since puppeteering happens after all NPCs are active it's necessary that Hades' segments be told what to do manually, since the
            // instructions will otherwise be reset on the next frame, before the segments are updated.
            if (hadesIsPresent)
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.TryGetDLCBehavior(out HadesBodyEternity body))
                    {
                        body.ListenToHeadInstructions();
                        body.ModifyDRBasedOnOpenInterpolant();
                    }
                }
            }
        }
    }
}
