using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// AI update loop method for the inactive state.
        /// </summary>
        public void DoBehavior_Inactive()
        {
            ZPosition = MathHelper.Clamp(ZPosition + 0.1f, -0.99f, 4f);
            NPC.SmoothFlyNear(Target.Center - Vector2.UnitY * (ZPosition * 160f + 100f), ZPosition * 0.03f, 0.87f);
            NPC.dontTakeDamage = true;
            NPC.damage = 0;

            // This is necessary to ensure that the map icon goes away.
            NPC.As<AresBody>().SecondaryAIState = (int)AresBody.SecondaryPhase.PassiveAndImmune;

            BasicHandUpdateWrapper();

            if (!Inactive)
            {
                CurrentState = AresAIState.ReturnToBeingActive;
                AITimer = 0;
                NPC.netUpdate = true;
            }
        }
    }
}
