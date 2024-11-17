using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Twins
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class EDeathSpazmatism : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.Spazmatism;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Fireballs);
            binaryWriter.Write7BitEncodedInt(timer);
            binaryWriter.Write7BitEncodedInt(FireballTime);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            Fireballs = binaryReader.ReadBoolean();
            timer = binaryReader.Read7BitEncodedInt();
            FireballTime = binaryReader.Read7BitEncodedInt();
        }
        public bool Fireballs = false;
        public int timer = 0;
        public int FireballTime = 200;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget || NPC == null)
            {
                return true;
            }
            NPC.GetGlobalNPC<Spazmatism>().RunEmodeAI = true;
            if (Fireballs)
            {

                NPC.GetGlobalNPC<Spazmatism>().RunEmodeAI = true;
                timer++;
                Player target = Main.player[NPC.target];
                NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center + new Vector2(0, 350) - NPC.Center).SafeNormalize(Vector2.Zero) * 20, 0.04f);
                if (timer > 60 && timer % 45 == 0 && DLCUtils.HostCheck)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.ShadowflameFireball>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage((int)(NPC.damage * 0.75f)), 0, ai1: 1);
                }
                if (timer >= FireballTime)
                {
                    Fireballs = false;
                    timer = 0;
                    NPC.netUpdate = true;
                }
                NPC.rotation = NPC.AngleTo(target.Center) - MathHelper.PiOver2;
                return false;
            }
            if (NPC.ai[0] == 3 && NPC.ai[1] == 0 && NPC.ai[2] == 398)
            {
                Fireballs = true;
                NPC.netUpdate = true;
            }

            return true;
        }

    }
}
