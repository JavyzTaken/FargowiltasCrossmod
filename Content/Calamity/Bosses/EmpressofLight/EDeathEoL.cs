using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.EmpressofLight
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathEoL : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.HallowBoss);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(wallAttack);
            binaryWriter.Write7BitEncodedInt(timer);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            wallAttack = binaryReader.ReadBoolean();
            timer = binaryReader.Read7BitEncodedInt();
        }
        public bool wallAttack = false;
        public int timer = 0;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            Player target = Main.player[npc.target];
            if (wallAttack && npc.ai[2] % 10 != 0)
            {
                npc.velocity = (target.Center + new Vector2(0, -300) - npc.Center).SafeNormalize(Vector2.Zero) * 3;
                npc.localAI[0]++;
                Lighting.AddLight(npc.Center, TorchID.White);

                timer++;
                if (npc.localAI[0] >= 45)
                {
                    npc.localAI[0] = 0;

                }
                if (timer == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item161, target.Center);
                }
                if (timer > 50 && timer % 50 == 0 && timer <= 450)
                {
                    SoundEngine.PlaySound(SoundID.Item163, target.Center);
                    float angle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    if (DLCUtils.HostCheck)
                        for (int i = -10; i < 11; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), target.Center + new Vector2(i * 150, -800).RotatedBy(angle), Vector2.Zero, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: MathHelper.PiOver2 + angle, ai1: 0.1f * i);
                        }
                }
                if (timer >= 550)
                {
                    timer = 0;
                    wallAttack = false;
                }
                return false;

            }
            if (npc.ai[3] == 1 && npc.ai[2] % 10 == 0)
            {
                wallAttack = true;
            }
            return base.SafePreAI(npc);
        }
    }
}
