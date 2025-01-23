using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
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
    public class EDeathEoL : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.HallowBoss;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(wallAttack);
            binaryWriter.Write7BitEncodedInt(timer);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            wallAttack = binaryReader.ReadBoolean();
            timer = binaryReader.Read7BitEncodedInt();
        }
        public bool wallAttack = false;
        public int timer = 0;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            Player target = Main.player[NPC.target];
            if (wallAttack && NPC.ai[2] % 10 != 0)
            {
                NPC.velocity = (target.Center + new Vector2(0, -300) - NPC.Center).SafeNormalize(Vector2.Zero) * 3;
                NPC.localAI[0]++;
                Lighting.AddLight(NPC.Center, TorchID.White);

                timer++;
                if (NPC.localAI[0] >= 45)
                {
                    NPC.localAI[0] = 0;

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
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + new Vector2(i * 150, -800).RotatedBy(angle), Vector2.Zero, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: MathHelper.PiOver2 + angle, ai1: 0.1f * i);
                        }
                }
                if (timer >= 550 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    timer = 0;
                    wallAttack = false;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                    }
                }
                return false;

            }
            if (NPC.ai[3] == 1 && NPC.ai[2] % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                wallAttack = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                }
            }
            return true;
        }
    }
}
