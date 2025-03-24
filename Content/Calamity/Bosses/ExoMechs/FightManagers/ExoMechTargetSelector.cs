using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public sealed class ExoMechTargetSelector : ModSystem
    {
        private static int? targetIndex;

        /// <summary>
        /// The current target that all Exo Mechs.
        /// </summary>
        public static Player Target
        {
            get
            {
                if (targetIndex is null)
                    SelectNew();

                return Main.player[targetIndex!.Value];
            }
            private set => targetIndex = value.whoAmI;
        }

        /// <summary>
        /// Whether a new target needs to be automatically selected.
        /// </summary>
        private static bool NeedsNewTarget => targetIndex is null || targetIndex <= -1 || Target.dead || !Target.active;

        /// <summary>
        /// Searches for a new potential target to attack.
        /// </summary>
        /// 
        /// <remarks>
        /// This method is intended to be called server-side in multiplayer, where-in changes to the target index are synced.
        /// </remarks>
        public static void SelectNew()
        {
            int previousTargetIndex = targetIndex ?? -1;
            Vector2 relativeSearchPosition;

            if (targetIndex is not null)
                relativeSearchPosition = Target.Center;
            else
                relativeSearchPosition = new Vector2(Main.maxTilesX * 8f, (float)(Main.worldSurface * 16f));

            targetIndex = Player.FindClosest(relativeSearchPosition, 1, 1);
            if (Main.netMode == NetmodeID.Server && targetIndex != previousTargetIndex)
                NetMessage.SendData(MessageID.WorldData);
        }

        public override void PreUpdateEntities()
        {
            if (NeedsNewTarget)
                SelectNew();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(targetIndex ?? -1);
        }

        public override void NetReceive(BinaryReader reader)
        {
            targetIndex = reader.ReadInt32();
            if (targetIndex <= -1)
                targetIndex = null;
        }
    }
}
