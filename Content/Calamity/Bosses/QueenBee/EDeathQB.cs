using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls.Core.Systems;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.QueenBee
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathQB : CalDLCEDeathBehavior
    {

        public override int NPCOverrideID => NPCID.QueenBee;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;

            Player player = Main.player[NPC.target];
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();
            var qbEternity = NPC.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.QueenBee>();

            float maxEnrageScale = 2f;
            float enrageScale = 0.5f; // death default
            float lifeRatio = NPC.GetLifePercent();

            // Calamity phases
            bool phase2 = lifeRatio < 0.85f;
            bool phase3 = lifeRatio < 0.7f;
            bool phase4 = lifeRatio < 0.5f;
            bool phase5 = lifeRatio < 0.3f;
            bool phase6 = lifeRatio < 0.1f;

            if (NPC.ai[0] == 0) // charge phase
            {
                // Calamity charges


                // Charging distance from player
                int chargeDistanceX = (int)((phase6 ? 750f : phase5 ? 350f : phase4 ? 650f : phase2 ? 550f : 450f) - 50f * enrageScale);

                // Number of charges
                int chargeAmt = phase6 ? 1 : phase5 ? 3 : phase4 ? 2 : 1;

                // Switch to a random phase if chargeAmt has been exceeded
                if (NPC.ai[1] > (2 * chargeAmt) && NPC.ai[1] % 2f == 0f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                    return false;
                }

                // Charge velocity
                float velocity = ((phase6 ? 25f : phase5 ? 14f : phase4 ? 25f : phase2 ? 20f : 15f) + 3f * enrageScale);

                // Line up and initiate charge
                if (NPC.ai[1] % 2f == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    // Initiate charge
                    float chargeDistanceY = phase6 ? 100f : phase4 ? 50f : 20f;
                    chargeDistanceY += 100f * enrageScale;
                    chargeDistanceY += MathHelper.Lerp(0f, 100f, 1f - lifeRatio);

                    float distanceFromTargetX = Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X);
                    float distanceFromTargetY = Math.Abs(NPC.Center.Y - Main.player[NPC.target].Center.Y);
                    if (distanceFromTargetY < chargeDistanceY && distanceFromTargetX >= chargeDistanceX)
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

                        // Set AI variables and speed
                        NPC.localAI[0] = 1f;
                        NPC.ai[1] += 1f;
                        NPC.ai[2] = 0f;

                        // Get target location
                        Vector2 beeLocation = NPC.Center;
                        float targetXDist = Main.player[NPC.target].Center.X - beeLocation.X;
                        float targetYDist = Main.player[NPC.target].Center.Y - beeLocation.Y;
                        float targetDistance = (float)Math.Sqrt(targetXDist * targetXDist + targetYDist * targetYDist);
                        targetDistance = velocity / targetDistance;
                        NPC.velocity.X = targetXDist * targetDistance;
                        NPC.velocity.Y = targetYDist * targetDistance;

                        // Face the correct direction and play charge sound
                        float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                        NPC.direction = playerLocation < 0 ? 1 : -1;
                        NPC.spriteDirection = NPC.direction;

                        SoundEngine.PlaySound(SoundID.Zombie125, NPC.Center);

                        return false;
                    }

                    // Velocity variables
                    NPC.localAI[0] = 0f;
                    float chargeVelocityX = (phase4 ? 24f : phase2 ? 20f : 16f) + 8f * enrageScale;
                    float chargeVelocityY = (phase4 ? 18f : phase2 ? 15f : 12f) + 6f * enrageScale;
                    float chargeAccelerationX = (phase4 ? 0.9f : phase2 ? 0.7f : 0.5f) + 0.25f * enrageScale;
                    float chargeAccelerationY = (phase4 ? 0.45f : phase2 ? 0.35f : 0.25f) + 0.125f * enrageScale;

                    // Velocity calculations
                    if (NPC.Center.Y < Main.player[NPC.target].Center.Y - chargeDistanceY)
                        NPC.velocity.Y += chargeAccelerationY;
                    else if (NPC.Center.Y > Main.player[NPC.target].Center.Y + chargeDistanceY)
                        NPC.velocity.Y -= chargeAccelerationY;
                    else
                        NPC.velocity.Y *= 0.7f;

                    if (NPC.velocity.Y < -chargeVelocityY)
                        NPC.velocity.Y = -chargeVelocityY;
                    if (NPC.velocity.Y > chargeVelocityY)
                        NPC.velocity.Y = chargeVelocityY;

                    float distanceXMax = 100f;
                    float distanceXMin = 20f;
                    if (distanceFromTargetX > chargeDistanceX + distanceXMax)
                        NPC.velocity.X += chargeAccelerationX * NPC.direction;
                    else if (distanceFromTargetX < chargeDistanceX + distanceXMin)
                        NPC.velocity.X -= chargeAccelerationX * NPC.direction;
                    else
                        NPC.velocity.X *= 0.7f;

                    // Limit velocity
                    if (NPC.velocity.X < -chargeVelocityX)
                        NPC.velocity.X = -chargeVelocityX;
                    if (NPC.velocity.X > chargeVelocityX)
                        NPC.velocity.X = chargeVelocityX;

                    // Face the correct direction
                    float playerLocation2 = NPC.Center.X - Main.player[NPC.target].Center.X;
                    NPC.direction = playerLocation2 < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;

                    NPC.netUpdate = true;

                    if (NPC.netSpam > 10)
                        NPC.netSpam = 10;
                }
                else
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    // Face the correct direction
                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else
                        NPC.direction = 1;

                    NPC.spriteDirection = NPC.direction;

                    // Get which side of the player the boss is on
                    int chargeDirection = 1;
                    if (NPC.Center.X < Main.player[NPC.target].Center.X)
                        chargeDirection = -1;

                    // If boss is in correct position, slow down, if not, reset
                    bool shouldCharge = false;
                    if (NPC.direction == chargeDirection && Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) > chargeDistanceX)
                    {
                        NPC.ai[2] = 1f;
                        shouldCharge = true;
                    }
                    if (Math.Abs(NPC.Center.Y - Main.player[NPC.target].Center.Y) > chargeDistanceX * 1.5f)
                    {
                        NPC.ai[2] = 1f;
                        shouldCharge = true;
                    }
                    if (enrageScale > 0f && shouldCharge)
                        NPC.velocity *= MathHelper.Lerp(0.5f, 1f, 1f - enrageScale / maxEnrageScale);

                    // Keep moving
                    if (NPC.ai[2] != 1f)
                    {
                        // Velocity fix if Queen Bee is slowed
                        if (NPC.velocity.Length() < velocity)
                            NPC.velocity.X = velocity * NPC.direction;

                        float accelerateGateValue = phase6 ? 30f : phase5 ? 10f : 90f;
                        if (enrageScale > 0f)
                            accelerateGateValue *= 0.75f;

                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > accelerateGateValue)
                        {
                            NPC.SyncExtraAI();
                            float velocityXLimit = velocity * 2f;
                            if (Math.Abs(NPC.velocity.X) < velocityXLimit)
                                NPC.velocity.X *= 1.01f;
                        }
                        NPC.localAI[0] = 1f;
                        return false;
                    }

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                    NPC.direction = playerLocation < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;

                    // Slow down
                    NPC.localAI[0] = 0f;
                    NPC.velocity *= (0.9f);

                    float chargeDeceleration = 0.1f;
                    if (phase2)
                    {
                        NPC.velocity *= 0.9f;
                        chargeDeceleration += 0.05f;
                    }
                    if (phase4)
                    {
                        NPC.velocity *= 0.8f;
                        chargeDeceleration += 0.1f;
                    }

                    if (enrageScale > 0f)
                        NPC.velocity *= MathHelper.Lerp(0.7f, 1f, 1f - enrageScale / maxEnrageScale);

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < chargeDeceleration)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[1] += 1f;
                        calamityGlobalNPC.newAI[0] = 0f;
                        NPC.SyncExtraAI();
                    }

                    NPC.netUpdate = true;

                    if (NPC.netSpam > 10)
                        NPC.netSpam = 10;
                }
                return false;
            }

            bool doingBeeswarm = (NPC.ai[0] == 3f || NPC.ai[0] == 1f) && (qbEternity.InPhase2 && qbEternity.BeeSwarmTimer > 600);
            if (doingBeeswarm && qbEternity.BeeSwarmTimer == 780 && !qbEternity.SubjectDR && NPC.HasPlayerTarget) // stinger spread during bee swarm; only if no subjects
            {
                Vector2 stingerSpawnLocation = new Vector2(NPC.Center.X + (Main.rand.Next(20) * NPC.direction), NPC.position.Y + NPC.height * 0.8f);
                SoundEngine.PlaySound(SoundID.Item17, stingerSpawnLocation);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float stingerSpeed = (phase6 ? 5f : 4f) + enrageScale;
                    if (WorldSavingSystem.MasochistModeReal)
                        stingerSpeed += 1f;

                    Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * stingerSpeed;
                    int type = Main.zenithWorld ? ModContent.ProjectileType<PlagueStingerGoliathV2>() : ProjectileID.QueenBeeStinger;
                    int numProj = 7;
                    int spread = 50;

                    if (WorldSavingSystem.MasochistModeReal)
                    {
                        numProj += 4;
                        spread += 15;
                    }

                    float rotation = MathHelper.ToRadians(spread);
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                        if (i % 2f != 0f)
                            perturbedSpeed *= 0.8f;

                        int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), stingerSpawnLocation + perturbedSpeed.SafeNormalize(Vector2.UnitY) * 10f, perturbedSpeed, type, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0f, Main.player[NPC.target].position.Y);
                        Main.projectile[projectile].timeLeft = 1200;
                        Main.projectile[projectile].extraUpdates = 1;

                        if (!Main.zenithWorld)
                            Main.projectile[projectile].tileCollide = false;
                    }
                }
            }
            return true;
        }
    }
}
