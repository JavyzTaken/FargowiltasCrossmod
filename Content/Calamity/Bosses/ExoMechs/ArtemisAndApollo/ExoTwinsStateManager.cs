using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Artemis;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Systems;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoTwinsStateManager : ModSystem
    {
        /// <summary>
        /// A shared state that both Artemis and Apollo may access.
        /// </summary>
        public static SharedExoTwinState SharedState
        {
            get;
            set;
        } = new(ExoTwinsAIState.DashesAndLasers, new float[5]);

        /// <summary>
        /// The set of all passive individual AI states the Exo Twins can perform.
        /// </summary>
        public static ExoTwinsIndividualAIState[] PassiveIndividualStates => [ExoTwinsIndividualAIState.Artemis_SimpleLaserShots, ExoTwinsIndividualAIState.Apollo_SimpleLoopDashes];

        /// <summary>
        /// The set of all active individual AI states the Exo Twins can perform.
        /// </summary>
        public static ExoTwinsIndividualAIState[] ActiveIndividualStates => [ExoTwinsIndividualAIState.Artemis_FocusedLaserBursts, ExoTwinsIndividualAIState.Apollo_LoopDashBombardment];

        /// <summary>
        /// The set of all active individual AI states that Artemis can perform.
        /// </summary>
        public static ExoTwinsIndividualAIState[] IndividualArtemisStates => [ExoTwinsIndividualAIState.Artemis_SimpleLaserShots, ExoTwinsIndividualAIState.Artemis_FocusedLaserBursts];

        /// <summary>
        /// The set of all active individual AI states that Apollo can perform.
        /// </summary>
        public static ExoTwinsIndividualAIState[] IndividualApolloStates => [ExoTwinsIndividualAIState.Apollo_SimpleLoopDashes, ExoTwinsIndividualAIState.Apollo_LoopDashBombardment];

        public override void PostUpdateNPCs()
        {
            if (!CalDLCWorldSavingSystem.E_EternityRev)
                return;

            SharedState.Update();

            if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
            {
                NPC apollo = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen];
                if (apollo.active && apollo.TryGetDLCBehavior(out ApolloEternity apolloAI))
                    PerformUpdateLoop(apollo, apolloAI);
            }

            if (CalamityGlobalNPC.draedonExoMechTwinRed != -1)
            {
                NPC artemis = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed];
                if (artemis.active && artemis.TryGetDLCBehavior(out ArtemisEternity artemisAI))
                    PerformUpdateLoop(artemis, artemisAI);
            }

            if (SharedState.AIState == ExoTwinsAIState.PerformComboAttack)
                SharedState.AITimer = ExoMechComboAttackManager.ComboAttackTimer;
        }

        public override void NetSend(BinaryWriter writer) => SharedState.WriteTo(writer);

        public override void NetReceive(BinaryReader reader) => SharedState.ReadFrom(reader);

        /// <summary>
        /// Performs the central AI state update loop for a given Exo Twin.
        /// </summary>
        /// <param name="twin">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        public static void PerformUpdateLoop(NPC twin, IExoTwin twinAttributes)
        {
            bool shouldEnterSecondPhase = twin.life < twin.lifeMax * ExoMechFightDefinitions.FightAloneLifeRatio || ExoMechFightStateManager.CurrentPhase == ExoMechFightDefinitions.BerserkSoloPhaseDefinition;
            if (shouldEnterSecondPhase && !twinAttributes.InPhase2 && SharedState.AIState != ExoTwinsAIState.EnterSecondPhase)
                TransitionToNextState(ExoTwinsAIState.EnterSecondPhase);

            if (twinAttributes is IExoMech exoMech)
            {
                if (exoMech.Inactive && SharedState.AIState != ExoTwinsAIState.Inactive)
                    TransitionToNextState(ExoTwinsAIState.Inactive);
                if (!exoMech.Inactive && SharedState.AIState == ExoTwinsAIState.Inactive)
                    TransitionToNextState();
            }

            if (SharedState.AIState != ExoTwinsAIState.PerformComboAttack)
                twin.damage = 0;

            twin.defense = twin.defDefense;
            twin.dontTakeDamage = false;

            switch (SharedState.AIState)
            {
                case ExoTwinsAIState.SpawnAnimation:
                    ExoTwinsStates.DoBehavior_SpawnAnimation(twin, twinAttributes);
                    break;
                case ExoTwinsAIState.DashesAndLasers:
                    ExoTwinsStates.DoBehavior_DashesAndLasers(twin, twinAttributes);
                    break;
                case ExoTwinsAIState.CloseShots:
                    ExoTwinsStates.DoBehavior_CloseShots(twin, twinAttributes);
                    break;
                case ExoTwinsAIState.MachineGunLasers:
                    ExoTwinsStates.DoBehavior_MachineGunLasers(twin, twinAttributes);
                    break;
                case ExoTwinsAIState.PerformIndividualAttacks:
                    PerformIndividualizedAttacks(twin, twinAttributes);
                    break;

                case ExoTwinsAIState.Inactive:
                    ExoTwinsStates.DoBehavior_Inactive(twin, twinAttributes);
                    break;

                case ExoTwinsAIState.EnterSecondPhase:
                    ExoTwinsStates.DoBehavior_EnterSecondPhase(twin, twinAttributes);
                    break;
            }
        }

        /// <summary>
        /// Performs the central AI state update loop for a given Exo Twin.
        /// </summary>
        /// 
        /// <remarks>
        /// State transitions will be performed based on whichever Exo Twin is performing an active attack rather than a passive one.
        /// </remarks>
        /// 
        /// <param name="twin">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        public static void PerformIndividualizedAttacks(NPC twin, IExoTwin twinAttributes)
        {
            switch (twinAttributes.IndividualState.AIState)
            {
                case ExoTwinsIndividualAIState.Apollo_SimpleLoopDashes:
                    ExoTwinsStates.DoBehavior_SimpleLoopDashes(twin, twinAttributes, ref twinAttributes.IndividualState.AITimer);
                    break;
                case ExoTwinsIndividualAIState.Apollo_LoopDashBombardment:
                    ExoTwinsStates.DoBehavior_LoopDashBombardment(twin, twinAttributes, ref twinAttributes.IndividualState.AITimer);
                    break;

                case ExoTwinsIndividualAIState.Artemis_SimpleLaserShots:
                    ExoTwinsStates.DoBehavior_SimpleLaserShots(twin, twinAttributes, ref twinAttributes.IndividualState.AITimer);
                    break;
                case ExoTwinsIndividualAIState.Artemis_FocusedLaserBursts:
                    ExoTwinsStates.DoBehavior_FocusedLaserBursts(twin, twinAttributes, ref twinAttributes.IndividualState.AITimer);
                    break;
            }
            twinAttributes.IndividualState.AITimer++;
        }

        /// <summary>
        /// Picks an attack that Artemis and Apollo should adhere to.
        /// </summary>
        public static ExoTwinsAIState MakeAIStateChoice()
        {
            if (SharedState.TotalFinishedAttacks % 2 == 1)
                return ExoTwinsAIState.PerformIndividualAttacks;

            return Main.rand.NextFromList(ExoTwinsAIState.DashesAndLasers, ExoTwinsAIState.CloseShots, ExoTwinsAIState.MachineGunLasers);
        }

        /// <summary>
        /// Picks a new set of two individualized AI states for Artemis and Apollo, at random, with there being one passive attack and one active one.
        /// </summary>
        public static void PickIndividualAIStates()
        {
            if (CalamityGlobalNPC.draedonExoMechTwinRed == -1 || !Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].TryGetDLCBehavior(out ArtemisEternity artemis))
                return;

            if (CalamityGlobalNPC.draedonExoMechTwinGreen == -1 || !Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].TryGetDLCBehavior(out ApolloEternity apollo))
                return;

            bool apolloWillPerformActiveState = Main.rand.NextBool();
            ExoTwinsIndividualAIState previousArtemisState = artemis.IndividualState.AIState;
            ExoTwinsIndividualAIState previousApolloState = apollo.IndividualState.AIState;
            ExoTwinsIndividualAIState apolloState = previousApolloState;
            ExoTwinsIndividualAIState artemisState = previousArtemisState;

            for (int tries = 0; tries < 50; tries++)
            {
                if (apolloWillPerformActiveState)
                {
                    var activeApolloStates = IndividualApolloStates.Where(ActiveIndividualStates.Contains);
                    apolloState = Main.rand.Next(activeApolloStates.ToList());

                    var passiveArtemisStates = IndividualArtemisStates.Where(PassiveIndividualStates.Contains);
                    artemisState = Main.rand.Next(passiveArtemisStates.ToList());
                }
                else
                {
                    var passiveApolloStates = IndividualApolloStates.Where(PassiveIndividualStates.Contains);
                    apolloState = Main.rand.Next(passiveApolloStates.ToList());

                    var activeArtemisStates = IndividualArtemisStates.Where(ActiveIndividualStates.Contains);
                    artemisState = Main.rand.Next(activeArtemisStates.ToList());
                }

                // Toggle the apolloWillPerformActiveState variable, so that if there is no set of new individual states with the selected leading mech it's possible to alternate
                // to a case that is valid.
                apolloWillPerformActiveState = !apolloWillPerformActiveState;

                bool newAttackComboSelected = artemisState != previousArtemisState && apolloState != previousApolloState;
                if (newAttackComboSelected)
                    break;
            }

            artemis.IndividualState.AITimer = 0;
            artemis.IndividualState.AIState = artemisState;
            artemis.NPC.netUpdate = true;

            apollo.IndividualState.AITimer = 0;
            apollo.IndividualState.AIState = apolloState;
            apollo.NPC.netUpdate = true;
        }

        /// <summary>
        /// Selects and uses a new AI state for the Exo Twins, resetting attack-state-specific variables in the process.
        /// </summary>
        public static void TransitionToNextState(ExoTwinsAIState? stateToUse = null)
        {
            SharedState.ResetForNextState();
            SharedState.AIState = stateToUse ?? MakeAIStateChoice();

            SoundEngine.PlaySound(Artemis.AttackSelectionSound with { MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew });

            if (CalamityGlobalNPC.draedonExoMechTwinRed != -1 && Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].TryGetDLCBehavior(out ArtemisEternity artemis))
                artemis.ResetLocalStateData();

            if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1 && Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].TryGetDLCBehavior(out ApolloEternity apollo))
                apollo.ResetLocalStateData();

            if (SharedState.AIState == ExoTwinsAIState.PerformIndividualAttacks)
                PickIndividualAIStates();
        }
    }
}
