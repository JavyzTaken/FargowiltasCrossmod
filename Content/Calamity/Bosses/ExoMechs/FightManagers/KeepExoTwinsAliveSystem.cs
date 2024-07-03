using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers
{
    // This is probably the most stupid thing I've ever written.
    // I don't know why it's necessary, but it is. Without it Artemis and Apollo despawn for reasons unknown sometimes upon being called to return to the Mecha Mayhem phase.
    // I've been debugging this shit for hours now with zero useful leads.
    // My guess is that somewhere in the forest of vanilla or Calamity code they're being forcibly switched to inactive. I wouldn't know where, though.
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public sealed class KeepExoTwinsAliveSystem : ModSystem
    {
        /// <summary>
        /// How much longer the Exo Twins should be forcibly kept alive, in frames.
        /// </summary>
        public static int KeepAliveCountdown
        {
            get;
            set;
        }

        public override void PostUpdateNPCs()
        {
            if (KeepAliveCountdown <= 0)
                return;

            KeepAliveCountdown--;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.type == ExoMechNPCIDs.ArtemisID || npc.type == ExoMechNPCIDs.ApolloID)
                    npc.active = true;
            }
        }
    }
}
