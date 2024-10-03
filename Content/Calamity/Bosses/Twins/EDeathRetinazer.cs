using System.IO;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Twins
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class EDeathRetinazer : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.Retinazer;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(DashAttack);
            binaryWriter.Write7BitEncodedInt(dashTime);
            binaryWriter.Write7BitEncodedInt(timer);
            binaryWriter.Write7BitEncodedInt(dashCounter);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            DashAttack = binaryReader.ReadBoolean();
            dashTime = binaryReader.Read7BitEncodedInt();
            timer = binaryReader.Read7BitEncodedInt();
            dashCounter = binaryReader.Read7BitEncodedInt();
        }
        public bool DashAttack = false;
        public int dashTime = 150;
        public int timer = 0;
        public int dashCounter = 0;
        public override bool PreAI()
        {
            NPC.GetGlobalNPC<Retinazer>().RunEmodeAI = true;
            if (!NPC.HasValidTarget)
            {
                return true;
            }
            if (DashAttack && NPC.GetGlobalNPC<Retinazer>().DeathrayState == 0)
            {
                NPC.GetGlobalNPC<Retinazer>().RunEmodeAI = false;
                Player target = Main.player[NPC.target];
                timer++;
                if (timer < 60)
                {
                    int side = 1;
                    if (target.Center.X > NPC.Center.X) side = -1;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center + new Vector2(400 * side, -100) - NPC.Center).SafeNormalize(Vector2.Zero) * 17, 0.03f);
                    NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                }
                if (timer == 60)
                {
                    NPC.velocity = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 16;
                    NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                    NPC.netUpdate = true;
                }
                if (timer > 60 && timer % 15 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ScavengerLaser>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage((int)(NPC.damage * 0.75f)), 0);
                }
                if (timer >= dashTime - 20)
                {
                    NPC.velocity = Vector2.Zero;
                    timer = 0;
                    dashCounter++;
                    if (dashCounter > 1)
                    {
                        DashAttack = false;
                        dashCounter = 0;
                    }
                    NPC.netUpdate = true;
                }
                return false;
            }
            //Main.NewText(NPC.ai[2]);
            //Main.NewText(NPC.GetGlobalNPC<Retinazer>().DeathrayState);
            if (NPC.GetGlobalNPC<Retinazer>().DeathrayState == 3)
            {
                if (DashAttack != true)
                {
                    DashAttack = true;
                    NPC.netUpdate = true;
                }
                
            }
            return true;
        }

    }
}
