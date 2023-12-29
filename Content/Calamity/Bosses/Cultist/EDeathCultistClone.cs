using System.IO;
using System.Linq;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Projectiles.Masomode;
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
            binaryWriter.Write7BitEncodedInt(NoAttackTimer);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            fireball = binaryReader.ReadBoolean();
            NoAttackTimer = binaryReader.Read7BitEncodedInt();
        }
        public bool fireball = false;
        public int NoAttackTimer = 0;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            NPC owner = Main.npc[(int)npc.ai[3]];
            Player target = Main.player[npc.target];


            if (Main.projectile.Any(p => p.active && p.hostile && p.type == ModContent.ProjectileType<CelestialPillar>()))
                NoAttackTimer = 60 * 10;

            if (NoAttackTimer > 0)
            {
                npc.ai[1] = 0; //disable normal attack AI
                NoAttackTimer--;
            }
            else
                NoAttackTimer = 0;

            if (fireball && owner.ai[0] != 0 && owner.ai[1] == 30 && owner.ai[0] != 5 && NoAttackTimer <= 0)
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
