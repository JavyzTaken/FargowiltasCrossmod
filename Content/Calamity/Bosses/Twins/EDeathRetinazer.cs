using System.IO;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
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
    public class EDeathRetinazer : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Retinazer);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(DashAttack);
            binaryWriter.Write7BitEncodedInt(dashTime);
            binaryWriter.Write7BitEncodedInt(timer);
            binaryWriter.Write7BitEncodedInt(dashCounter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
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
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                return true;
            }
            npc.GetGlobalNPC<Retinazer>().RunEmodeAI = true;
            if (DashAttack && npc.GetGlobalNPC<Retinazer>().DeathrayState == 0)
            {
                npc.GetGlobalNPC<Retinazer>().RunEmodeAI = true;
                Player target = Main.player[npc.target];
                timer++;
                if (timer < 60)
                {
                    int side = 1;
                    if (target.Center.X > npc.Center.X) side = -1;
                    npc.velocity = Vector2.Lerp(npc.velocity, (target.Center + new Vector2(400 * side, -100) - npc.Center).SafeNormalize(Vector2.Zero) * 17, 0.03f);
                    npc.rotation = npc.AngleTo(target.Center) - MathHelper.PiOver2;
                }
                if (timer == 60)
                {
                    npc.velocity = (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 22;
                }
                if (timer > 60 && timer % 15 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item33, npc.Center);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<ScavengerLaser>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage((int)(npc.damage * 0.75f)), 0);
                }
                if (timer >= dashTime - 20)
                {
                    npc.velocity = Vector2.Zero;
                    timer = 0;
                    dashCounter++;
                    if (dashCounter > 1)
                    {
                        DashAttack = false;
                        dashCounter = 0;
                    }
                }
                return false;
            }
            //Main.NewText(npc.ai[2]);
            //Main.NewText(npc.GetGlobalNPC<Retinazer>().DeathrayState);
            if (npc.GetGlobalNPC<Retinazer>().DeathrayState == 3)
            {
                DashAttack = true;

            }
            return base.SafePreAI(npc);
        }

    }
}
