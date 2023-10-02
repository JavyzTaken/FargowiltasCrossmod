
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using FargowiltasCrossmod.Core;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls;
using FargowiltasCrossmod.Core.Utils;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Golem
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathGolem : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Golem);
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
            //Main.NewText(npc.ai[0] + ", " + npc.ai[1] + ", " + npc.ai[2] + ", " + npc.ai[3]);
            if (npc.ai[0] == 0 && npc.ai[1] == 0 && npc.GetLifePercent() <= 0.75f)
            {
                if (DLCUtils.HostCheck)
                for (int i = 0; i < 3; i++)
                {
                        Projectile fireball = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.BottomRight + new Vector2(0, -10), new Vector2(5, 0).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)), ModContent.ProjectileType<BouncingFireball>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage / 2), 0);
                        Projectile fireball2 = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.BottomLeft + new Vector2(0, -10), new Vector2(-5, 0).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)), ModContent.ProjectileType<BouncingFireball>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage / 2), 0);
                        
                }
            }
            return base.SafePreAI(npc);
        }
    }
}
