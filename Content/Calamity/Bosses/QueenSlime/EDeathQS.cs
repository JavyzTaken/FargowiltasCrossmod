
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using FargowiltasSouls;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.QueenSlime
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathQS : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.QueenSlimeBoss);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write(slam);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            slam = binaryReader.ReadBoolean();
        }
        public bool slam = false;
        
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            if (npc.ai[0] == 4)
            {
                slam = true;
                
            }
            else if (slam)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    for (int i = 0; i < 11; i++)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(-10, 0).RotatedBy(MathHelper.ToRadians(180 / 10f * i)), ModContent.ProjectileType<HallowedSlimeSpike>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                
                slam = false;
            }
            return base.SafePreAI(npc);
        }
    }
}
