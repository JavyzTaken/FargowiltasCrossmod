using System;
using System.IO;
using CalamityMod;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Skeletron
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathSkeletron : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.SkeletronHead;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.WriteVector2(telePos);
            binaryWriter.Write7BitEncodedInt(timer);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            telePos = binaryReader.ReadVector2();
            timer = binaryReader.Read7BitEncodedInt();
        }
        Vector2 telePos = Vector2.Zero;
        int timer = 0;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget)
            {
                return true;
            }
            Player target = Main.player[NPC.target];
            if (NPC.ai[1] == 1 && NPC.ai[2] == 280)
            {
                telePos = target.Center + (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 300 + new Vector2(0, -300);
                timer = 120;
            }
            timer--;
            if (timer > 0)
            {
                for (int i = 0; i < 2; i++)
                    Dust.NewDustDirect(telePos, NPC.width, NPC.height, DustID.Shadowflame, Scale: 2);
                if (timer == 1)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, Scale: 2);
                    }
                    NPC.position = telePos;
                    foreach (NPC n in Main.npc)
                    {
                        if (n != null && n.active && n.type == NPCID.SkeletronHand && n.ai[1] == NPC.whoAmI)
                        {
                            n.position = telePos;
                        }
                    }
                    if (DLCUtils.HostCheck)
                    {
                        int numProj = 7;
                        float spread = MathHelper.ToRadians(76);

                        float skullSpeed = 6f;
                        int type = ProjectileID.Skull;
                        int damage = NPC.GetProjectileDamage(type);

                        float boltTargetXDist = Main.player[NPC.target].Center.X - NPC.Center.X;
                        float boltTargetYDist = Main.player[NPC.target].Center.Y - NPC.Center.Y;
                        float boltTargetDistance = (float)Math.Sqrt(boltTargetXDist * boltTargetXDist + boltTargetYDist * boltTargetYDist);

                        boltTargetDistance = skullSpeed / boltTargetDistance;
                        boltTargetXDist *= boltTargetDistance;
                        boltTargetYDist *= boltTargetDistance;
                        Vector2 center = NPC.Center;
                        center.X += boltTargetXDist * 5f;
                        center.Y += boltTargetYDist * 5f;

                        float baseSpeed = (float)Math.Sqrt(boltTargetXDist * boltTargetXDist + boltTargetYDist * boltTargetYDist);
                        double startAngle = Math.Atan2(boltTargetXDist, boltTargetYDist) - spread / 2;
                        double deltaAngle = spread / numProj;
                        double offsetAngle;

                        // Inverse parabolic projectile spreads
                        bool evenNumberOfProjectiles = numProj % 2 == 0;
                        int centralProjectile = evenNumberOfProjectiles ? numProj / 2 : (numProj - 1) / 2;
                        int otherCentralProjectile = evenNumberOfProjectiles ? centralProjectile - 1 : -1;
                        float centralScalingAmount = (float)Math.Floor(numProj / (double)centralProjectile) * 0.75f;
                        float amountToAdd = evenNumberOfProjectiles ? 0.5f : 0f;
                        float originalBaseSpeed = baseSpeed;
                        float minVelocityMultiplier = 0.5f;
                        for (int i = 0; i < numProj; i++)
                        {
                            float velocityScalar = (evenNumberOfProjectiles && (i == centralProjectile || i == otherCentralProjectile)) ? minVelocityMultiplier : MathHelper.Lerp(minVelocityMultiplier, centralScalingAmount, Math.Abs((i + amountToAdd) - centralProjectile) / (float)centralProjectile);
                            baseSpeed = originalBaseSpeed;
                            baseSpeed *= velocityScalar;
                            offsetAngle = startAngle + deltaAngle * i;
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), center.X, center.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, -2f);
                            Main.projectile[proj].timeLeft = 600;
                        }

                        NPC.netUpdate = true;

                        // OLD SHOTS 
                        /*
                        for (int i = -2; i < 3; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(15 * i)) * 15, ProjectileID.Skull, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        }
                        */
                    }
                        
                    SoundEngine.PlaySound(SoundID.Item66, NPC.Center);

                }
            }
            if (NPC.ai[1] == 2)
            {
                if (timer <= -120 && NPC.GetLifePercent() > 0.01f)
                {
                    telePos = target.Center + (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 300 + new Vector2(0, -300);
                    timer = 120;
                }else if (NPC.GetLifePercent() <= 0.01f && timer <= -300)
                {
                    telePos = target.Center + (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 300 + new Vector2(0, -300);
                    timer = 120;
                }
            }
            return true;
        }
    }
}
