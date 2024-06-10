using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
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
    public class EDeathDestroyerBody : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.TheDestroyerBody;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(timer);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            timer = binaryReader.Read7BitEncodedInt();
        }
        public int timer = 0;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;

            NPC destroyer = FargoSoulsUtil.NPCExists(NPC.realLife, NPCID.TheDestroyer);
            Player target = Main.player[NPC.target];
            if (destroyer != null && !destroyer.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.Destroyer>().InPhase2)
            {
                NPC.GetGlobalNPC<DestroyerSegment>().AttackTimer = 0;
                NPC prevSegment = FargoSoulsUtil.NPCExists(NPC.ai[1], NPCID.TheDestroyerBody);
                if (prevSegment != null)
                {
                    var behavior = prevSegment.GetDLCBehavior<EDeathDestroyerBody>();
                    if (behavior != null && behavior.timer == 10)
                    {
                        timer++;
                    }
                }
                if (NPC.ai[1] == NPC.realLife || timer > 0)
                {
                    timer++;
                    if (timer == 800)
                    {
                        timer = 0;
                        if (DLCUtils.HostCheck && Collision.CanHitLine(NPC.Center, 1, 1, target.Center, 1, 1))
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 7, ProjectileID.DeathLaser, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }
                }
            }
            //if (NPC.whoAmI == 7)
            //{
            //    Main.NewText("ai: " + NPC.ai[0] + ", " + NPC.ai[1] + ", " + NPC.ai[2] + ", " + NPC.ai[3]);
            //    Main.NewText("localai: " + NPC.localAI[0] + ", " + NPC.localAI[1] + ", " + NPC.localAI[2] + ", " + NPC.localAI[3]);
            //}
            return true;
        }
    }
}
