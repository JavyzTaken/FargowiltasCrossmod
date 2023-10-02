
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using FargowiltasCrossmod.Core;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using Microsoft.Xna.Framework;
using FargowiltasSouls;
using FargowiltasCrossmod.Core.Utils;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Golem
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathGolemHead : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.GolemHeadFree);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            
            GolemHead gol = npc.GetGlobalNPC<GolemHead>();
            //Main.NewText(gol.AttackTimer);
            Player target = Main.player[npc.target];
            if (gol != null && gol.AttackTimer == 260)
            {
                if (DLCUtils.HostCheck)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 8, ProjectileID.InfernoHostileBolt, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0:target.Center.X, ai1:target.Center.Y);
                }
            }
            return base.SafePreAI(npc);
        }
    }
}
