using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;
using Terraria.ModLoader;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls;
using FargowiltasSouls.Content.Projectiles;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrainofCthulhu
{
    public class EDeathBoCClone : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<BrainClone>());
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(timer);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            timer = binaryReader.Read7BitEncodedInt();
        }
        int timer;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            NPC brain = FargoSoulsUtil.NPCExists(EModeGlobalNPC.brainBoss, NPCID.BrainofCthulhu);
            if (brain == null) return true;
            
            if (brain.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.BrainofCthulhu>().ConfusionTimer == 300 && !Main.player[npc.target].HasBuff(BuffID.Confused) && timer == 0)
            {
                timer++;
            }
            if (timer > 0)
            {
                timer++;
                if (timer == 120)
                {
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 180);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 200);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 220);
                }
                if (timer == 180)
                {
                    npc.velocity = (Main.player[npc.target].Center - npc.Center).SafeNormalize(Vector2.Zero) * 20;
                    timer = 0;
                }
            }
            return base.SafePreAI(npc);
        }
    }
}
