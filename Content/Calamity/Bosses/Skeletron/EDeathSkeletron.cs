using System;
using System.IO;
using CalamityMod;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
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
    public class EDeathSkeletron : EternityDeathBehaviour
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
                    {
                        int num9 = 7;
                        float num10 = MathHelper.ToRadians(76);

                        float num16 = 6f;
                        float num18 = target.Center.X - npc.Center.X;
                        float num19 = target.Center.Y - npc.Center.Y;
                        float num20 = (float)Math.Sqrt(num18 * num18 + num19 * num19);
                        num20 = num16 / num20;
                        num18 *= num20;
                        num19 *= num20;
                        Vector2 center = ((Entity)npc).Center;
                        center.X += num18 * 5f;
                        center.Y += num19 * 5f;
                        float num21 = (float)Math.Sqrt(num18 * num18 + num19 * num19);
                        double num22 = Math.Atan2(num18, num19) - (double)(num10 / 2f);
                        double num23 = num10 / (float)num9;
                        bool flag7 = num9 % 2 == 0;
                        int num24 = (flag7 ? (num9 / 2) : ((num9 - 1) / 2));
                        int num25 = (flag7 ? (num24 - 1) : (-1));
                        float num26 = (float)Math.Floor((double)num9 / (double)num24) * 0.75f;
                        float num27 = (flag7 ? 0.5f : 0f);
                        float num28 = num21;
                        float num29 = 0.5f;
                        for (int j = 0; j < num9; j++)
                        {
                            float num30 = ((flag7 && (j == num24 || j == num25)) ? num29 : MathHelper.Lerp(num29, num26, Math.Abs((float)j + num27 - (float)num24) / (float)num24));
                            num21 = num28;
                            num21 *= num30;
                            double num31 = num22 + num23 * (double)j;
                            int num32 = Projectile.NewProjectile(npc.GetSource_FromAI(null), center.X, center.Y, num21 * (float)Math.Sin(num31), num21 * (float)Math.Cos(num31), ProjectileID.Skull, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, -2f, 0f, 0f);
                            Main.projectile[num32].timeLeft = 600;
                        }
                        npc.netUpdate = true;

                        // OLD SHOTS 
                        /*
                        for (int i = -2; i < 3; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(15 * i)) * 15, ProjectileID.Skull, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                        }
                        */
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
