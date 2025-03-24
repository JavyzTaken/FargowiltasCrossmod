using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
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
            binaryWriter.Write7BitEncodedInt(numBefore);
            binaryWriter.Write7BitEncodedInt(lastTimeUsed);
            binaryWriter.Write7BitEncodedInt(useInNext);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            wallAttack = binaryReader.ReadBoolean();
            timer = binaryReader.Read7BitEncodedInt();
            numBefore = binaryReader.Read7BitEncodedInt();
            lastTimeUsed = binaryReader.Read7BitEncodedInt();
            useInNext = binaryReader.Read7BitEncodedInt();
        }
        public bool wallAttack = false;
        public int timer = 25;
        public int numBefore = 0;
        public int lastTimeUsed = 0;
        public int useInNext = 8;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            Player target = Main.player[NPC.target];
            //Main.NewText(wallAttack);
            if (wallAttack && NPC.ai[2] % 10 != 0)
            {
                    NPC.velocity = (target.Center + new Vector2(0, -300) - NPC.Center).SafeNormalize(Vector2.Zero) * 3;
                NPC.localAI[0]++;
                Lighting.AddLight(NPC.Center, TorchID.White);
                if (WorldSavingSystem.MasochistModeReal)
                {
                    NPC.ai[0] = 7;
                    NPC.ai[2] = 5;
                }
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
                    timer = 25;
                    wallAttack = false;
                    NPC.ai[2] = numBefore + 1;
                    if (WorldSavingSystem.MasochistModeReal)
                    {
                        NPC.ai[0] = 1;
                    }
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                    }
                }
                return false;

            }
            //wallAttack = true;
            if (NPC.ai[3] == 1  && Main.netMode != NetmodeID.MultiplayerClient && ((WorldSavingSystem.MasochistModeReal && NPC.ai[2] >= lastTimeUsed + useInNext) || (!WorldSavingSystem.MasochistModeReal && NPC.ai[2] % 10 == 0)))
            {
                wallAttack = true;
                numBefore = (int)NPC.ai[2];
                lastTimeUsed = (int)NPC.ai[2];
                useInNext = Main.rand.Next(8, 14);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                }
            }
            return true;
        }
    }
}
