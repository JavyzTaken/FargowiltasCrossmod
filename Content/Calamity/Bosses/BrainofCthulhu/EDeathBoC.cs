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
using FargowiltasCrossmod.Core.Calamity.Systems;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrainofCthulhu
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathBoC : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.BrainofCthulhu;

        public int ChargeCounter = 0;
        public int ChargePhase = 0;
        public float oldKnockback = 0f;
        public float[] oldAI = new float[4];
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(ChargePhase);
            binaryWriter.Write(ChargeCounter);
            binaryWriter.Write(oldKnockback);
            for (int i = 0; i <= 3; i++)
            {
                binaryWriter.Write(oldAI[i]);
            }
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            ChargePhase = binaryReader.ReadInt32();
            ChargeCounter = binaryReader.ReadInt32();
            oldKnockback = binaryReader.ReadSingle();
            for (int i = 0; i <= 3; i++)
            {
                oldAI[i] = binaryReader.ReadSingle();
            }
        }
        public override bool PreAI()
        {
            FargowiltasSouls.Content.Bosses.VanillaEternity.BrainofCthulhu emodeBoC = NPC.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.BrainofCthulhu>();

            // Calamity variables
            float lifeRatio = NPC.GetLifePercent();
            float enrageScale = 0f;

            float spinVelocity = 12f;
            int spinRadius = 45;
            float chargeVelocity = 18f + 3f * enrageScale;

            if (emodeBoC.EnteredPhase2)
            {
                emodeBoC.RunEmodeAI = true;
                if (emodeBoC.ConfusionTimer == 70) // Periodically make the attack available, first cycle has attack since counter will increment to 1
                {
                    ChargeCounter++;
                    if (ChargeCounter > 2)
                        ChargeCounter = 0;
                }
                if (emodeBoC.ConfusionTimer < 70 && emodeBoC.ConfusionTimer > 60 && ChargeCounter == 1) // approx 1 second before confusion flip, if attack is in cycle, do it
                {
                    if (ChargePhase < 2)
                    {
                        emodeBoC.RunEmodeAI = false;
                        if (ChargePhase == 0) // Start of attack, initialize attack variables and save old variables to be restored when it ends
                        {
                            float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                            // start spinning
                            NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * spinVelocity;
                            for (int i = 0; i <= 3; i++) //Save AI values to be restored when attack ends
                            {
                                oldAI[i] = NPC.ai[i];
                            }
                            NPC.ai[0] = -4f;
                            NPC.ai[1] = playerLocation < 0 ? 1f : -1f;
                            NPC.ai[2] = 0f;

                            int maxRandomTime = 30;
                            NPC.ai[3] = Main.rand.Next(maxRandomTime) + 31;
                            NPC.localAI[1] = 0f;
                            NPC.alpha = 0;
                            NPC.netUpdate = true;
                            ChargePhase = 1; // Mark attack as started
                            oldKnockback = NPC.knockBackResist; // Save kb resist to be restored when attack ends
                            NPC.knockBackResist = 0;
                        }
                        // Calamity spinny charge, slightly modified code from Calamity

                        bool spinning = NPC.ai[0] == -4f;
                        
                        // Charge
                        if (!spinning)
                        {
                            // Not charging
                            if (NPC.ai[0] != -5f)
                            {

                            }
                            // Charge, -5
                            else
                            {
                                NPC.ai[1] += 1f;

                                float chargeDistance = 960f; // 60 tile charge distance
                                float chargeDuration = chargeDistance / chargeVelocity;
                                float chargeGateValue = 10f;

                                if (NPC.ai[1] < chargeGateValue)
                                {
                                    // Avoid cheap bullshit
                                    NPC.damage = 0;
                                }
                                else
                                {
                                    // Set damage
                                    NPC.damage = NPC.defDamage;
                                }

                                // Teleport
                                float timeGateValue = chargeDuration + chargeGateValue;
                                if (NPC.ai[1] >= timeGateValue)
                                {
                                    // End the attack, go back to emode AI
                                    NPC.netUpdate = true;
                                    for (int i = 0; i <= 3; i++) // Restore AI values
                                    {
                                        NPC.ai[i] = oldAI[i];
                                    }
                                    ChargePhase = 2;
                                    NPC.knockBackResist = oldKnockback; // Restore kb resist
                                }

                                // Charge sound and velocity
                                else if (NPC.ai[1] == chargeGateValue)
                                {
                                    // Sound
                                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);

                                    // Velocity
                                    NPC.velocity = (Main.player[NPC.target].Center + (false ? Main.player[NPC.target].velocity * 20f * enrageScale : Vector2.Zero) - NPC.Center).SafeNormalize(Vector2.UnitY) * chargeVelocity;
                                }
                            }
                        }

                        // Circle around, -4
                        if (spinning)
                        {
                            // Avoid cheap bullshit
                            NPC.damage = 0;

                            // Charge sound
                            if (NPC.ai[2] == 0f)
                            {
                                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                            }

                            // Velocity
                            float velocity = MathHelper.TwoPi / spinRadius;
                            NPC.velocity = NPC.velocity.RotatedBy(-(double)velocity * NPC.ai[1]);

                            NPC.ai[2] += 1f;

                            float timer = 0f + NPC.ai[3];

                            // Move the brain away from the target in order to ensure fairness
                            if (NPC.ai[2] >= timer - 5f)
                            {
                                float minChargeDistance = 640f; // 40 tile distance
                                if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < minChargeDistance)
                                {
                                    NPC.ai[2] -= 1f;
                                    NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * (-chargeVelocity - 2f * enrageScale);
                                    if (Main.getGoodWorld)
                                        NPC.velocity *= 1.15f;
                                }
                            }

                            // Charge at target
                            if (NPC.ai[2] >= timer)
                            {
                                // Shoot projectiles from 4 directions, alternating between diagonal and cardinal
                                float bloodShotVelocity = 7.5f + enrageScale;


                                Vector2 projectileVelocity2 = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * bloodShotVelocity;
                                bool canHit2 = Collision.CanHitLine(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<GoldenShowerShot>();
                                    int damage = FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);
                                    int numProj = 9;
                                    int spread = 45;


                                    float rotation = MathHelper.ToRadians(spread);
                                    for (int i = 0; i < numProj; i++)
                                    {
                                        Vector2 perturbedSpeed = projectileVelocity2.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * 10f, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                                        Main.projectile[proj].timeLeft = 600;
                                        if (!canHit2)
                                            Main.projectile[proj].tileCollide = false;
                                    }
                                }

                                // Complete stop
                                NPC.velocity *= 0f;

                                NPC.ai[0] = -5f;
                                NPC.ai[1] = 0f;
                                NPC.ai[2] = 0f;
                                NPC.ai[3] = 0f;
                                NPC.TargetClosest();
                                NPC.netUpdate = true;
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
            return true;
        }
    }
}
