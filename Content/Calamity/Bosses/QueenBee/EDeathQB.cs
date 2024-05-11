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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.QueenBee
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathQB : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.QueenBee);
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;

            Player player = Main.player[npc.target];
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            float maxEnrageScale = 2f;
            float enrageScale = 0.5f; // death default
            float lifeRatio = npc.GetLifePercent();

            // Calamity phases
            bool phase2 = lifeRatio < 0.85f;
            bool phase3 = lifeRatio < 0.7f;
            bool phase4 = lifeRatio < 0.5f;
            bool phase5 = lifeRatio < 0.3f;
            bool phase6 = lifeRatio < 0.1f;

            if (npc.ai[0] == 0) // charge phase
            {
                // Calamity charges


                // Charging distance from player
                int chargeDistanceX = (int)((phase6 ? 750f : phase5 ? 350f : phase4 ? 650f : phase2 ? 550f : 450f) - 50f * enrageScale);

                // Number of charges
                int chargeAmt = phase6 ? 1 : phase5 ? 3 : phase4 ? 2 : 1;

                // Switch to a random phase if chargeAmt has been exceeded
                if (npc.ai[1] > (2 * chargeAmt) && npc.ai[1] % 2f == 0f)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                    return false;
                }

                // Charge velocity
                float velocity = ((phase6 ? 25f : phase5 ? 14f : phase4 ? 25f : phase2 ? 20f : 15f) + 3f * enrageScale);

                // Line up and initiate charge
                if (npc.ai[1] % 2f == 0f)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    // Initiate charge
                    float chargeDistanceY = phase6 ? 100f : phase4 ? 50f : 20f;
                    chargeDistanceY += 100f * enrageScale;
                    chargeDistanceY += MathHelper.Lerp(0f, 100f, 1f - lifeRatio);

                    float distanceFromTargetX = Math.Abs(npc.Center.X - Main.player[npc.target].Center.X);
                    float distanceFromTargetY = Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y);
                    if (distanceFromTargetY < chargeDistanceY && distanceFromTargetX >= chargeDistanceX)
                    {
                        // Set damage
                        npc.damage = npc.defDamage;

                        // Set AI variables and speed
                        npc.localAI[0] = 1f;
                        npc.ai[1] += 1f;
                        npc.ai[2] = 0f;

                        // Get target location
                        Vector2 beeLocation = npc.Center;
                        float targetXDist = Main.player[npc.target].Center.X - beeLocation.X;
                        float targetYDist = Main.player[npc.target].Center.Y - beeLocation.Y;
                        float targetDistance = (float)Math.Sqrt(targetXDist * targetXDist + targetYDist * targetYDist);
                        targetDistance = velocity / targetDistance;
                        npc.velocity.X = targetXDist * targetDistance;
                        npc.velocity.Y = targetYDist * targetDistance;

                        // Face the correct direction and play charge sound
                        float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                        npc.direction = playerLocation < 0 ? 1 : -1;
                        npc.spriteDirection = npc.direction;

                        SoundEngine.PlaySound(SoundID.Zombie125, npc.Center);

                        return false;
                    }

                    // Velocity variables
                    npc.localAI[0] = 0f;
                    float chargeVelocityX = (phase4 ? 24f : phase2 ? 20f : 16f) + 8f * enrageScale;
                    float chargeVelocityY = (phase4 ? 18f : phase2 ? 15f : 12f) + 6f * enrageScale;
                    float chargeAccelerationX = (phase4 ? 0.9f : phase2 ? 0.7f : 0.5f) + 0.25f * enrageScale;
                    float chargeAccelerationY = (phase4 ? 0.45f : phase2 ? 0.35f : 0.25f) + 0.125f * enrageScale;

                    // Velocity calculations
                    if (npc.Center.Y < Main.player[npc.target].Center.Y - chargeDistanceY)
                        npc.velocity.Y += chargeAccelerationY;
                    else if (npc.Center.Y > Main.player[npc.target].Center.Y + chargeDistanceY)
                        npc.velocity.Y -= chargeAccelerationY;
                    else
                        npc.velocity.Y *= 0.7f;

                    if (npc.velocity.Y < -chargeVelocityY)
                        npc.velocity.Y = -chargeVelocityY;
                    if (npc.velocity.Y > chargeVelocityY)
                        npc.velocity.Y = chargeVelocityY;

                    float distanceXMax = 100f;
                    float distanceXMin = 20f;
                    if (distanceFromTargetX > chargeDistanceX + distanceXMax)
                        npc.velocity.X += chargeAccelerationX * npc.direction;
                    else if (distanceFromTargetX < chargeDistanceX + distanceXMin)
                        npc.velocity.X -= chargeAccelerationX * npc.direction;
                    else
                        npc.velocity.X *= 0.7f;

                    // Limit velocity
                    if (npc.velocity.X < -chargeVelocityX)
                        npc.velocity.X = -chargeVelocityX;
                    if (npc.velocity.X > chargeVelocityX)
                        npc.velocity.X = chargeVelocityX;

                    // Face the correct direction
                    float playerLocation2 = npc.Center.X - Main.player[npc.target].Center.X;
                    npc.direction = playerLocation2 < 0 ? 1 : -1;
                    npc.spriteDirection = npc.direction;

                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }
                else
                {
                    // Set damage
                    npc.damage = npc.defDamage;

                    // Face the correct direction
                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;

                    // Get which side of the player the boss is on
                    int chargeDirection = 1;
                    if (npc.Center.X < Main.player[npc.target].Center.X)
                        chargeDirection = -1;

                    // If boss is in correct position, slow down, if not, reset
                    bool shouldCharge = false;
                    if (npc.direction == chargeDirection && Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > chargeDistanceX)
                    {
                        npc.ai[2] = 1f;
                        shouldCharge = true;
                    }
                    if (Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > chargeDistanceX * 1.5f)
                    {
                        npc.ai[2] = 1f;
                        shouldCharge = true;
                    }
                    if (enrageScale > 0f && shouldCharge)
                        npc.velocity *= MathHelper.Lerp(0.5f, 1f, 1f - enrageScale / maxEnrageScale);

                    // Keep moving
                    if (npc.ai[2] != 1f)
                    {
                        // Velocity fix if Queen Bee is slowed
                        if (npc.velocity.Length() < velocity)
                            npc.velocity.X = velocity * npc.direction;

                        float accelerateGateValue = phase6 ? 30f : phase5 ? 10f : 90f;
                        if (enrageScale > 0f)
                            accelerateGateValue *= 0.75f;

                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > accelerateGateValue)
                        {
                            npc.SyncExtraAI();
                            float velocityXLimit = velocity * 2f;
                            if (Math.Abs(npc.velocity.X) < velocityXLimit)
                                npc.velocity.X *= 1.01f;
                        }
                        npc.localAI[0] = 1f;
                        return false;
                    }

                    // Avoid cheap bullshit
                    npc.damage = 0;

                    float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                    npc.direction = playerLocation < 0 ? 1 : -1;
                    npc.spriteDirection = npc.direction;

                    // Slow down
                    npc.localAI[0] = 0f;
                    npc.velocity *= (0.9f);

                    float chargeDeceleration = 0.1f;
                    if (phase2)
                    {
                        npc.velocity *= 0.9f;
                        chargeDeceleration += 0.05f;
                    }
                    if (phase4)
                    {
                        npc.velocity *= 0.8f;
                        chargeDeceleration += 0.1f;
                    }

                    if (enrageScale > 0f)
                        npc.velocity *= MathHelper.Lerp(0.7f, 1f, 1f - enrageScale / maxEnrageScale);

                    if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < chargeDeceleration)
                    {
                        npc.ai[2] = 0f;
                        npc.ai[1] += 1f;
                        calamityGlobalNPC.newAI[0] = 0f;
                        npc.SyncExtraAI();
                    }

                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }
                return false;
            }
            return base.SafePreAI(npc);
        }
    }
}
