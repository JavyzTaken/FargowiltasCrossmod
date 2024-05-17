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
    public class EDeathSkeletron : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SkeletronHead);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.WriteVector2(telePos);
            binaryWriter.Write7BitEncodedInt(timer);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            telePos = binaryReader.ReadVector2();
            timer = binaryReader.Read7BitEncodedInt();
        }
        Vector2 telePos = Vector2.Zero;
        int timer = 0;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                return true;
            }
            Player target = Main.player[npc.target];
            if (npc.ai[1] == 1 && npc.ai[2] == 280)
            {
                telePos = target.Center + (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 300 + new Vector2(0, -300);
                timer = 120;
            }
            if (timer > 0)
            {
                timer--;
                for (int i = 0; i < 2; i++)
                    Dust.NewDustDirect(telePos, npc.width, npc.height, DustID.Shadowflame, Scale: 2);
                if (timer == 0)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Shadowflame, Scale: 2);
                    }
                    npc.position = telePos;
                    foreach (NPC n in Main.npc)
                    {
                        if (n != null && n.active && n.type == NPCID.SkeletronHand && n.ai[1] == npc.whoAmI)
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
                        int damage = npc.GetProjectileDamage(type);

                        float boltTargetXDist = Main.player[npc.target].Center.X - npc.Center.X;
                        float boltTargetYDist = Main.player[npc.target].Center.Y - npc.Center.Y;
                        float boltTargetDistance = (float)Math.Sqrt(boltTargetXDist * boltTargetXDist + boltTargetYDist * boltTargetYDist);

                        boltTargetDistance = skullSpeed / boltTargetDistance;
                        boltTargetXDist *= boltTargetDistance;
                        boltTargetYDist *= boltTargetDistance;
                        Vector2 center = npc.Center;
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
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), center.X, center.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, -2f);
                            Main.projectile[proj].timeLeft = 600;
                        }

                        npc.netUpdate = true;

                        // OLD SHOTS 
                        /*
                        for (int i = -2; i < 3; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(15 * i)) * 15, ProjectileID.Skull, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                        }
                        */
                    }
                        
                    SoundEngine.PlaySound(SoundID.Item66, npc.Center);

                }
            }
            if (npc.ai[1] == 2)
            {
                if (timer <= 0)
                {
                    telePos = target.Center + (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 300 + new Vector2(0, -300);
                    timer = 120;
                }
            }
            return base.SafePreAI(npc);
        }
    }
}
