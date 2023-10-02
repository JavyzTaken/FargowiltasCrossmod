
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Core;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Destroyer
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathDestroyerHead : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.TheDestroyer);
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
            FargowiltasSouls.Content.Bosses.VanillaEternity.Destroyer destroyer = npc.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.Destroyer>();
            //Main.NewText(destroyer.AttackModeTimer + ", " + destroyer.SecondaryAttackTimer + ", " + npc.ai[2]);
            if (destroyer.InPhase2 && destroyer.SecondaryAttackTimer == 5 && destroyer.AttackModeTimer > 0 && destroyer.AttackModeTimer % 100 == 0 && !destroyer.IsCoiling)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    for (int i = -2; i < 3; i++)
                    {
                        if (i != 0)
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(7 * i) - MathHelper.PiOver2) * 7, ProjectileID.DeathLaser, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage / 2), 0);
                    }
            }
            return base.SafePreAI(npc);
        }
    }
}
