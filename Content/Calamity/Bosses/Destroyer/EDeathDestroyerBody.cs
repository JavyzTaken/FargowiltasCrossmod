using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Destroyer
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathDestroyerBody : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.TheDestroyerBody);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(timer);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            timer = binaryReader.Read7BitEncodedInt();
        }
        public int timer = 0;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;

            NPC destroyer = FargoSoulsUtil.NPCExists(npc.realLife, NPCID.TheDestroyer);
            Player target = Main.player[npc.target];
            if (destroyer != null && !destroyer.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.Destroyer>().InPhase2)
            {
                npc.GetGlobalNPC<DestroyerSegment>().AttackTimer = 0;
                NPC prevSegment = FargoSoulsUtil.NPCExists(npc.ai[1], NPCID.TheDestroyerBody);
                if (prevSegment != null)
                {
                    if (prevSegment.GetGlobalNPC<EDeathDestroyerBody>().timer == 10)
                    {
                        timer++;
                    }
                }
                if (npc.ai[1] == npc.realLife || timer > 0)
                {
                    timer++;
                    if (timer == 800)
                    {
                        timer = 0;
                        if (DLCUtils.HostCheck && Collision.CanHitLine(npc.Center, 1, 1, target.Center, 1, 1))
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 7, ProjectileID.DeathLaser, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                }
            }
            //if (npc.whoAmI == 7)
            //{
            //    Main.NewText("ai: " + npc.ai[0] + ", " + npc.ai[1] + ", " + npc.ai[2] + ", " + npc.ai[3]);
            //    Main.NewText("localai: " + npc.localAI[0] + ", " + npc.localAI[1] + ", " + npc.localAI[2] + ", " + npc.localAI[3]);
            //}
            return base.SafePreAI(npc);
        }
    }
}
