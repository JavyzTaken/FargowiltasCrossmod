using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cultist
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathCultistClone : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.CultistBossClone);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(fireball);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            fireball = binaryReader.ReadBoolean();
        }
        public bool fireball = false;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            NPC owner = Main.npc[(int)npc.ai[3]];
            Player target = Main.player[npc.target];

            if (fireball && owner.ai[0] != 0 && owner.ai[1] == 30 && owner.ai[0] != 5)
            {

                fireball = false;
                if (DLCUtils.HostCheck)
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 7, ProjectileID.CultistBossFireBallClone, FargoSoulsUtil.ScaledProjectileDamage(150), 0);
            }
            if (owner.ai[0] == 0)
            {

                fireball = true;
            }
            return base.SafePreAI(npc);
        }
    }
}
