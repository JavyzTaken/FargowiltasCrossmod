using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Core.NPCMatching;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls;
using FargowiltasSouls.Core.Systems;
using System.IO;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using CalamityMod;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using System.Data;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrainofCthulhu
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathBoC : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BrainofCthulhu);
        public int ChargeCounter = 0;
        public int ChargePhase = 0;
        public float oldKnockback = 0f;
        public float[] oldAI = new float[4];
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(ChargePhase);
            binaryWriter.Write(ChargeCounter);
            binaryWriter.Write(oldKnockback);
            for (int i = 0; i <= 3; i++)
            {
                binaryWriter.Write(oldAI[i]);
            }
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            ChargePhase = binaryReader.ReadInt32();
            ChargeCounter = binaryReader.ReadInt32();
            oldKnockback = binaryReader.ReadSingle();
            for (int i = 0; i <= 3; i++)
            {
                oldAI[i] = binaryReader.ReadSingle();
            }
        }
        public override bool SafePreAI(NPC npc)
        {
            FargoSoulsUtil.PrintAI(npc);

            FargowiltasSouls.Content.Bosses.VanillaEternity.BrainofCthulhu emodeBoC = npc.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.BrainofCthulhu>();

            // Calamity variables
            float lifeRatio = npc.GetLifePercent();
            float enrageScale = 0f;

            float spinVelocity = 12f;
            int spinRadius = 45;
            float chargeVelocity = 18f + 3f * enrageScale;

            if (emodeBoC.EnteredPhase2)
            {

                emodeBoC.RunEmodeAI = true;
                if (emodeBoC.ConfusionTimer == 70)
                {
                    ChargeCounter++;
                    if (ChargeCounter > 2)
                        ChargeCounter = 0;
                }
                if (emodeBoC.ConfusionTimer < 70 && emodeBoC.ConfusionTimer > 60 && ChargeCounter == 1)
                {
                    if (ChargePhase < 2)
                    {
                        emodeBoC.RunEmodeAI = false;
                        if (ChargePhase == 0)
                        {
                            float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                            // start spinning
                            npc.velocity = (Main.player[npc.target].Center - npc.Center).SafeNormalize(Vector2.UnitY) * spinVelocity;
                            for (int i = 0; i <= 3; i++)
                            {
                                oldAI[i] = npc.ai[i];
                            }
                            npc.ai[0] = -4f;
                            npc.ai[1] = playerLocation < 0 ? 1f : -1f;
                            npc.ai[2] = 0f;

                            int maxRandomTime = 30;
                            npc.ai[3] = Main.rand.Next(maxRandomTime) + 31;
                            npc.localAI[1] = 0f;
                            npc.alpha = 0;
                            npc.netUpdate = true;
                            ChargePhase = 1;
                            oldKnockback = npc.knockBackResist; // Save kb resist to be restored when attack ends
                            npc.knockBackResist = 0;
                        }


                        bool spinning = npc.ai[0] == -4f;
                        // calamity charge
                        // Charge
                        if (!spinning)
                        {
                            // Not charging
                            if (npc.ai[0] != -5f)
                            {

                            }
                            // Charge, -5
                            else
                            {
                                npc.ai[1] += 1f;

                                float chargeDistance = 960f; // 60 tile charge distance
                                float chargeDuration = chargeDistance / chargeVelocity;
                                float chargeGateValue = 10f;

                                if (npc.ai[1] < chargeGateValue)
                                {
                                    // Avoid cheap bullshit
                                    npc.damage = 0;
                                }
                                else
                                {
                                    // Set damage
                                    npc.damage = npc.defDamage;
                                }

                                // Teleport
                                float timeGateValue = chargeDuration + chargeGateValue;
                                if (npc.ai[1] >= timeGateValue)
                                {
                                    
                                    npc.netUpdate = true;
                                    for (int i = 0; i <= 3; i++)
                                    {
                                        npc.ai[i] = oldAI[i];
                                    }
                                    ChargePhase = 2;
                                    npc.knockBackResist = oldKnockback; // Restore kb resist
                                }

                                // Charge sound and velocity
                                else if (npc.ai[1] == chargeGateValue)
                                {
                                    // Sound
                                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);

                                    // Velocity
                                    npc.velocity = (Main.player[npc.target].Center + (false ? Main.player[npc.target].velocity * 20f * enrageScale : Vector2.Zero) - npc.Center).SafeNormalize(Vector2.UnitY) * chargeVelocity;
                                }
                            }
                        }

                        // Circle around, -4
                        if (spinning)
                        {
                            // Avoid cheap bullshit
                            npc.damage = 0;

                            // Charge sound
                            if (npc.ai[2] == 0f)
                            {
                                SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center);
                            }

                            // Velocity
                            float velocity = MathHelper.TwoPi / spinRadius;
                            npc.velocity = npc.velocity.RotatedBy(-(double)velocity * npc.ai[1]);

                            npc.ai[2] += 1f;

                            float timer = 0f + npc.ai[3];

                            // Move the brain away from the target in order to ensure fairness
                            if (npc.ai[2] >= timer - 5f)
                            {
                                float minChargeDistance = 640f; // 40 tile distance
                                if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) < minChargeDistance)
                                {
                                    npc.ai[2] -= 1f;
                                    npc.velocity = (Main.player[npc.target].Center - npc.Center).SafeNormalize(Vector2.UnitY) * (-chargeVelocity - 2f * enrageScale);
                                    if (Main.getGoodWorld)
                                        npc.velocity *= 1.15f;
                                }
                            }

                            // Charge at target
                            if (npc.ai[2] >= timer)
                            {
                                // Shoot projectiles from 4 directions, alternating between diagonal and cardinal
                                float bloodShotVelocity = 7.5f + enrageScale;


                                Vector2 projectileVelocity2 = (Main.player[npc.target].Center - npc.Center).SafeNormalize(Vector2.UnitY) * bloodShotVelocity;
                                bool canHit2 = Collision.CanHitLine(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<GoldenShowerShot>();
                                    int damage = FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage);
                                    int numProj = 9;
                                    int spread = 45;


                                    float rotation = MathHelper.ToRadians(spread);
                                    for (int i = 0; i < numProj; i++)
                                    {
                                        Vector2 perturbedSpeed = projectileVelocity2.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * 10f, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                                        Main.projectile[proj].timeLeft = 600;
                                        if (!canHit2)
                                            Main.projectile[proj].tileCollide = false;
                                    }
                                }

                                // Complete stop
                                npc.velocity *= 0f;

                                npc.ai[0] = -5f;
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.ai[3] = 0f;
                                npc.TargetClosest();
                                npc.netUpdate = true;
                            }
                        }
                        return false;
                    }
                }
                else
                {
                    ChargePhase = 0;
                }
            }
            return base.SafePreAI(npc);
        }
    }
}
