using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.DukeFishron
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathDuke : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.DukeFishron;
        //public bool PermaDashes = false;
        public int Timer = 0;
        public int Timer2 = 0;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            if (NPC.ai[0] == 10 && NPC.ai[3] == 8 && NPC.ai[2] >= 20 && NPC.GetLifePercent() <= 0.1f)
            {
                if (Timer2 == 0)
                    NPC.Center = Main.player[NPC.target].Center + Main.player[NPC.target].velocity.SafeNormalize(Vector2.Zero) * 1000;
                if (Timer2 < 60)
                {
                    Timer2++;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.AngleTo(Main.player[NPC.target].Center), 0.08f);

                    NPC.spriteDirection = -1;
                    NPC.velocity *= 0.95f;
                    if (Timer2 == 30 && Main.LocalPlayer.Distance(NPC.Center) < 4000)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie20);
                    }
                    return false;
                }
                if (NPC.ai[2] == 25)
                {
                    NPC.ai[3] = 5;
                    Timer2 = 0;
                }
            }
            if (NPC.ai[3] == 1 && NPC.GetLifePercent() <= 0.1f)
            {
                if (Timer < 100)
                {
                    Timer++;
                    NPC.velocity *= 0.98f;
                    if (Timer == 60 && Main.LocalPlayer.Distance(NPC.Center) < 4000)
                    {
                        SoundEngine.PlaySound(SoundID.Roar);
                    }
                    return false;
                }
                NPC.ai[3] = 5;
            }
            return true;
        }
    }
}
