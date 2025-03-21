using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using SPrimeEternity = FargowiltasSouls.Content.Bosses.VanillaEternity.SkeletronPrime;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SkeletronPrime
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class EDeathSPrime : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.SkeletronPrime;
        public bool Spinning;
        public bool SpecialAttack;
        public int AttackTimer = 0;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Spinning);
            binaryWriter.Write(SpecialAttack);
            binaryWriter.Write7BitEncodedInt(AttackTimer);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            Spinning = binaryReader.ReadBoolean();
            SpecialAttack = binaryReader.ReadBoolean();
            AttackTimer = binaryReader.Read7BitEncodedInt();
        }
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget)
            {
                return true;
            }
            var sPrimeEternity = NPC.GetGlobalNPC<SPrimeEternity>();
            sPrimeEternity.RunEmodeAI = true;
            bool spinning = NPC.ai[1] == 1f && NPC.ai[2] > 2;
            bool dgPhase = NPC.ai[1] == 2f;
            if (spinning)
                Spinning = true;
            if (!spinning & !dgPhase && Spinning)
            {
                Spinning = false;
                SpecialAttack = true;
            }
            if (SpecialAttack && !dgPhase)
            {
                sPrimeEternity.RunEmodeAI = false;
                AttackTimer++;
                Player player = Main.player[NPC.target];
                if (AttackTimer == 5)
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                if (AttackTimer < 60)
                {
                    NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, player.Center + player.DirectionTo(NPC.Center) * 450, NPC.velocity, 1f, 1f);
                }
                if (AttackTimer > 60 && AttackTimer < 90)
                {
                    NPC.velocity += NPC.DirectionTo(player.Center) * 0.8f;

                    if (AttackTimer % 4 == 0)
                    {
                        if (DLCUtils.HostCheck)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ModContent.ProjectileType<HomingRocket>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }
                }
                if (AttackTimer > 90)
                {
                    NPC.velocity *= 0.95f;
                }
                if (AttackTimer > 120)
                {
                    AttackTimer = 0;
                    SpecialAttack = false;
                }
                return false;
            }


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
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public abstract class EDeathPrimeLimb : CalDLCEDeathBehavior
    {
        public float dir = 0;
        public override bool PreAI()
        {
            NPC head = FargoSoulsUtil.NPCExists(NPC.ai[1], NPCID.SkeletronPrime);
            if (head == null)
            {
                NPC.active = false;
                return true;
                
            }
            var headDLC = head.GetDLCBehavior<EDeathSPrime>();
            var emode = NPC.GetGlobalNPC<PrimeLimb>();
            emode.RunEmodeAI = true;
            if (headDLC.SpecialAttack && !emode.IsSwipeLimb)
            {
                if (headDLC.AttackTimer < 40)
                {
                    if (head.HasPlayerTarget)
                        dir = head.DirectionTo(Main.player[head.target].Center).ToRotation();
                }
                else if (headDLC.AttackTimer < 90)
                {
                    if (head.HasPlayerTarget)
                        dir = dir.ToRotationVector2().RotateTowards(head.DirectionTo(Main.player[head.target].Center).ToRotation(), 0.05f).ToRotation();
                }
                float offset = NPC.type switch
                {
                    NPCID.PrimeCannon => -2,
                    NPCID.PrimeLaser => -1,
                    NPCID.PrimeSaw => 1,
                    NPCID.PrimeVice => 2,
                    _ => 0
                };
                float rot = dir + offset * MathHelper.PiOver2 * 0.2f;
                Vector2 desiredPos = head.Center - rot.ToRotationVector2() * 340;
                if (headDLC.AttackTimer < 60)
                    NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, desiredPos, NPC.velocity, 1f, 1f);
                else if (head.HasPlayerTarget)
                {
                    Vector2 toPlayer = NPC.DirectionTo(Main.player[head.target].Center);
                    if (headDLC.AttackTimer == 60)
                        NPC.velocity = toPlayer * 3;
                    if (headDLC.AttackTimer < 90)
                        NPC.velocity += toPlayer * 2f;
                    else
                        NPC.velocity *= 0.95f;
                }
                    
                emode.RunEmodeAI = false;
                return false;
            }
            return true;
        }

    }
    public class EDeathPrimeCannon : EDeathPrimeLimb
    {
        public override int NPCOverrideID => NPCID.PrimeCannon;
    }
    public class EDeathPrimeLaser : EDeathPrimeLimb
    {
        public override int NPCOverrideID => NPCID.PrimeLaser;
    }
    public class EDeathPrimeSaw : EDeathPrimeLimb
    {
        public override int NPCOverrideID => NPCID.PrimeSaw;
    }
    public class EDeathPrimeVice : EDeathPrimeLimb
    {
        public override int NPCOverrideID => NPCID.PrimeVice;
    }
}
