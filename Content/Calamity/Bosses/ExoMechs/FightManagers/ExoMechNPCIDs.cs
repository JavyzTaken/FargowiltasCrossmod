using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Core;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoMechNPCIDs : ModSystem
    {
        /// <summary>
        /// The NPC ID of Artemis.
        /// </summary>
        public static int ArtemisID
        {
            get;
            private set;
        }

        /// <summary>
        /// The NPC ID of Apollo.
        /// </summary>
        public static int ApolloID
        {
            get;
            private set;
        }

        /// <summary>
        /// The NPC ID of Ares' body.
        /// </summary>
        public static int AresBodyID
        {
            get;
            private set;
        }

        /// <summary>
        /// The NPC ID of Hades' head.
        /// </summary>
        public static int HadesHeadID
        {
            get;
            private set;
        }

        /// <summary>
        /// The set of all Exo Mech types.
        /// </summary>
        public static ReadOnlyCollection<int> ExoMechIDs
        {
            get;
            private set;
        }

        /// <summary>
        /// The set of all managing Exo Mech types.
        /// </summary>
        public static ReadOnlyCollection<int> ManagingExoMechIDs
        {
            get;
            private set;
        }

        public override void OnModLoad()
        {
            ArtemisID = ModContent.NPCType<Artemis>();
            ApolloID = ModContent.NPCType<Apollo>();
            AresBodyID = ModContent.NPCType<AresBody>();
            HadesHeadID = ModContent.NPCType<ThanatosHead>();

            ExoMechIDs = new([AresBodyID, HadesHeadID, ArtemisID, ApolloID]);
            ManagingExoMechIDs = new([AresBodyID, HadesHeadID, ApolloID]);
        }

        /// <summary>
        /// Evaluates whether an NPC is an Exo Mech or not.
        /// </summary>
        /// 
        /// <remarks>
        /// In this context, "Exo Mech" refers to an NPC type that serves as an AI manager. So as an example, Ares' body would count as an Exo Mech by this method, but his individual cannons would not, since they're simply battle actors.<br></br>
        /// </remarks>
        /// <param name="npc">The NPC to evaluate.</param>
        public static bool IsExoMech(NPC npc) => ExoMechIDs.Contains(npc.type);

        /// <summary>
        /// Evaluates whether an NPC is a managing Exo Mech or not.
        /// </summary>
        /// 
        /// <remarks>
        /// In this context, "Managing Exo Mech" refers to an NPC type that's responsible for AI management. So as an example, Ares' body would count as an Exo Mech by this method, but his individual cannons would not, since they're simply battle actors.<br></br>
        /// Furthermore, Artemis does not count as a managing Exo Mech, as Apollo is expected to be responsible for phase state management and the like.
        /// </remarks>
        /// <param name="npc">The NPC to evaluate.</param>
        public static bool IsManagingExoMech(NPC npc) => ManagingExoMechIDs.Contains(npc.type);
    }
}
