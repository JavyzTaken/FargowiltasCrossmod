using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Twins
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class EDeathSpazmatism : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Spazmatism);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write(Fireballs);
            binaryWriter.Write7BitEncodedInt(timer);
            binaryWriter.Write7BitEncodedInt(FireballTime);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            Fireballs = binaryReader.ReadBoolean();
            timer = binaryReader.Read7BitEncodedInt();
            FireballTime = binaryReader.Read7BitEncodedInt(); 
        }
        public bool Fireballs = false;
        public int timer = 0;
        public int FireballTime = 200;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget || npc == null)
            {
                return true;
            }
            npc.GetGlobalNPC<Spazmatism>().RunEmodeAI = true;
            if (Fireballs)
            {
                npc.GetGlobalNPC<Spazmatism>().RunEmodeAI = true;
                timer++;
                Player target = Main.player[npc.target];
                npc.velocity = Vector2.Lerp(npc.velocity, (target.Center + new Vector2(0, 350) - npc.Center).SafeNormalize(Vector2.Zero) * 20, 0.04f);
                if (timer > 60 && timer % 45 == 0 && DLCUtils.HostCheck)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 15, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.ShadowflameFireball>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage((int)(npc.damage * 0.75f)), 0, ai1: 1);
                }
                if (timer >= FireballTime)
                {
                    Fireballs = false;
                    timer = 0;
                }
                npc.rotation = npc.AngleTo(target.Center) - MathHelper.PiOver2;
                return false;
            }
            if (npc.ai[0] == 3 && npc.ai[1] == 0 && npc.ai[2] == 398)
            {
                Fireballs = true;
            }
            
            return base.SafePreAI(npc);
        }

    }
}
