using FargowiltasCrossmod.Core.Calamity.Globals;
using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior
    {
        public void DoBehavior_ReturnToBeingActive()
        {
            ZPosition = MathHelper.Clamp(ZPosition - 0.07f, 0f, 10f);
            NPC.velocity *= 0.85f;
            NPC.dontTakeDamage = true;
            NPC.damage = 0;

            BasicHandUpdateWrapper();

            if (!Inactive && ZPosition <= 0f)
                SelectNewState();
        }
    }
}
