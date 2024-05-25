using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.EaterofWorlds
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathEoWHead : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.EaterofWorldsHead;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(timer);
            binaryWriter.Write7BitEncodedInt(time);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            timer = binaryReader.Read7BitEncodedInt();
            time = binaryReader.Read7BitEncodedInt();
        }
        int timer = 800;
        int time = 0;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            Player target = Main.player[NPC.target];
            if (time == 0) time = Main.rand.Next(3000, 4000);
            if (NPC.CountNPCS(NPCID.EaterofWorldsBody) + NPC.CountNPCS(NPCID.EaterofWorldsHead) + NPC.CountNPCS(NPCID.EaterofWorldsTail) <= 60)
            {
                timer++;
                if (timer >= time)
                {

                    timer = 0;
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10, ProjectileID.CursedFlameHostile, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                }
            }
            return true;
        }
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathEoWBody : EDeathEoWHead
    {
        public override int NPCOverrideID => NPCID.EaterofWorldsBody;
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathEoWTail : EDeathEoWHead
    {
        public override int NPCOverrideID => NPCID.EaterofWorldsTail;
    }
}
