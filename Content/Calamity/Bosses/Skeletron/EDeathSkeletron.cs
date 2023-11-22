using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Skeletron
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathSkeletron : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SkeletronHead);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.WriteVector2(telePos);
            binaryWriter.Write7BitEncodedInt(timer);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            telePos = binaryReader.ReadVector2();
            timer = binaryReader.Read7BitEncodedInt();
        }
        Vector2 telePos = Vector2.Zero;
        int timer = 0;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                return true;
            }
            Player target = Main.player[npc.target];
            if (npc.ai[1] == 1 && npc.ai[2] == 280)
            {
                telePos = target.Center + (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 300 + new Vector2(0, -300);
                timer = 120;
            }
            if (timer > 0)
            {
                timer--;
                for (int i = 0; i < 2; i++)
                    Dust.NewDustDirect(telePos, npc.width, npc.height, DustID.Shadowflame, Scale: 2);
                if (timer == 0)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Shadowflame, Scale: 2);
                    }
                    npc.position = telePos;
                    foreach (NPC n in Main.npc)
                    {
                        if (n != null && n.active && n.type == NPCID.SkeletronHand && n.ai[1] == npc.whoAmI)
                        {
                            n.position = telePos;
                        }
                    }
                    if (DLCUtils.HostCheck)
                        for (int i = -2; i < 3; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(15 * i)) * 15, ProjectileID.Skull, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                        }
                    SoundEngine.PlaySound(SoundID.Item66, npc.Center);

                }
            }
            if (npc.ai[1] == 2)
            {
                if (timer <= 0)
                {
                    telePos = target.Center + (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 300 + new Vector2(0, -300);
                    timer = 120;
                }
            }
            return base.SafePreAI(npc);
        }
    }
}
