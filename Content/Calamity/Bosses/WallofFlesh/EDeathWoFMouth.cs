using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using FargowiltasSouls;
using Terraria.Audio;
using CalamityMod;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.WallofFlesh
{
    public class EDeathWoFMouth : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.WallofFlesh);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            FargowiltasSouls.Content.Bosses.VanillaEternity.WallofFlesh wof = npc.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.WallofFlesh>();
            //Main.NewText(wof.WorldEvilAttackCycleTimer);
            if (wof.WorldEvilAttackCycleTimer == 150)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(1, 0) * npc.spriteDirection, ModContent.ProjectileType<HomingSickle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
            }
            return base.SafePreAI(npc);
        }
    }
}
