using System.IO;
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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SkeletronPrime
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class EDeathSPrime : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SkeletronPrime);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.WriteVector2(rotationPoint);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            rotationPoint = binaryReader.ReadVector2();
        }
        public Vector2 rotationPoint = Vector2.Zero;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                return true;
            }
            npc.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.SkeletronPrime>().RunEmodeAI = true;

            if (npc.ai[0] == 1 && npc.ai[1] == 1 && npc.ai[2] % 120 == 0 && npc.ai[2] > 0)
            {
                for (int i = 0; i < 12; i++)
                {
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(360 / 12f * i)), ProjectileID.DeathLaser, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0);
                }
            }
            NPC arm = null;
            foreach (NPC n in Main.npc)
            {
                if (n != null && n.active && n.type == NPCID.PrimeLaser && n.ai[1] == npc.whoAmI && !n.GetGlobalNPC<PrimeLimb>().IsSwipeLimb) arm = n;
            }
            if (arm != null)
            {
                if (npc.ai[0] == 2 && npc.ai[1] == 0 && arm.GetGlobalNPC<PrimeLimb>().RangedAttackMode && npc.ai[2] > 120 && npc.ai[2] < 300)
                {
                    npc.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.SkeletronPrime>().RunEmodeAI = false;
                    npc.ai[2]++;
                    if (npc.ai[2] == 122)
                    {
                        npc.velocity *= 0;
                        rotationPoint = npc.Center + new Vector2(0, 200);
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                    }
                    if (npc.ai[2] % 15 == 0 && DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (Main.player[npc.target].Center - npc.Center).SafeNormalize(Vector2.Zero), ProjectileID.Skull, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0, ai0: -1);
                    }

                    npc.velocity = (rotationPoint - npc.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2 - 0.05f) * 20;
                    return false;
                }
                if (npc.ai[0] == 2 && npc.ai[1] == 0 && !arm.GetGlobalNPC<PrimeLimb>().RangedAttackMode && npc.ai[2] > 120 && npc.ai[2] < 300)
                {
                    if (npc.ai[2] % 10 == 0)
                    {

                        if (DLCUtils.HostCheck)
                            Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, new Vector2(0, -4).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ModContent.ProjectileType<HomingRocket>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0);

                    }
                }
            }
            return base.SafePreAI(npc);
        }

    }
}
