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
    public class EDeathSPrime : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.SkeletronPrime;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.WriteVector2(rotationPoint);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            rotationPoint = binaryReader.ReadVector2();
        }
        public Vector2 rotationPoint = Vector2.Zero;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget)
            {
                return true;
            }
            NPC.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.SkeletronPrime>().RunEmodeAI = true;

            /*
            if (NPC.ai[0] == 1 && NPC.ai[1] == 1 && NPC.ai[2] % 120 == 0 && NPC.ai[2] > 0)
            {
                for (int i = 0; i < 12; i++)
                {
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(360 / 12f * i)), ProjectileID.DeathLaser, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                }
            }
            NPC arm = null;
            foreach (NPC n in Main.npc)
            {
                if (n != null && n.active && n.type == NPCID.PrimeLaser && n.ai[1] == NPC.whoAmI && !n.GetGlobalNPC<PrimeLimb>().IsSwipeLimb) arm = n;
            }
            if (arm != null)
            {
                if (NPC.ai[0] == 2 && NPC.ai[1] == 0 && arm.GetGlobalNPC<PrimeLimb>().RangedAttackMode && NPC.ai[2] > 120 && NPC.ai[2] < 300)
                {
                    NPC.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.SkeletronPrime>().RunEmodeAI = false;
                    NPC.ai[2]++;
                    if (NPC.ai[2] == 122)
                    {
                        NPC.velocity *= 0;
                        rotationPoint = NPC.Center + new Vector2(0, 200);
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    }
                    if (NPC.ai[2] % 15 == 0 && DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero), ProjectileID.Skull, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: -1);
                    }

                    NPC.velocity = (rotationPoint - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2 - 0.05f) * 20;
                    return false;
                }
                if (NPC.ai[0] == 2 && NPC.ai[1] == 0 && !arm.GetGlobalNPC<PrimeLimb>().RangedAttackMode && NPC.ai[2] > 120 && NPC.ai[2] < 300)
                {
                    if (NPC.ai[2] % 10 == 0)
                    {

                        if (DLCUtils.HostCheck)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ModContent.ProjectileType<HomingRocket>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);

                    }
                }
            }
            */
            return true;
        }

    }
}
