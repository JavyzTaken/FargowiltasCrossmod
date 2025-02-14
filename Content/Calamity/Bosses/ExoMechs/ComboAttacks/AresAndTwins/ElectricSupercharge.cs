using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ElectricSupercharge : ExoMechComboHandler
    {
        public override int[] ExpectedManagingExoMechs => [ModContent.NPCType<AresBody>(), ModContent.NPCType<Apollo>()];

        /// <summary>
        /// How long Ares spends electrifying Artemis and Apollo.
        /// </summary>
        public static int ElectrifyTime => Variables.GetAIInt("ElectricSupercharge_ElectrifyTime", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How long Ares spends redirecting above the player.
        /// </summary>
        public static int AresRedirectTime => Variables.GetAIInt("ElectricSupercharge_AresRedirectTime", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The rate at which Ares' cannons shoot tesla spheres after electrifying Artemis and Apollo.
        /// </summary>
        public static int TeslaSphereShootRate => Variables.GetAIInt("ElectricSupercharge_TeslaSphereShootRate", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How long the Electric Supercharge attack should go on for.
        /// </summary>
        public static int AttackDuration => Variables.GetAIInt("ElectricSupercharge_AttackDuration", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The rate at which Artemis shoots projectiles.
        /// </summary>
        public static int ArtemisShootRate => Variables.GetAIInt("ElectricSupercharge_ArtemisShootRate", ExoMechAIVariableType.Combo);

        // Don't reduce this too much. It plays a key role in minimizing the potential for circle strats.
        /// <summary>
        /// How many electric projectiles Apollo releases upon doing a burst dash.
        /// </summary>
        public static int DashSpreadProjectileCount => Variables.GetAIInt("ElectricSupercharge_DashSpreadProjectileCount", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How much damage Ares' small tesla spheres do.
        /// </summary>
        public static int SmallTeslaSphereDamage => Variables.GetAIInt("SmallTeslaSphereDamage", ExoMechAIVariableType.Ares);

        /// <summary>
        /// The speed interpolant at which Artemis readjusts her rotation to aim towards the target.
        /// </summary>
        public static float ArtemisTurnSpeedInterpolant => Variables.GetAIFloat("ElectricSupercharge_ArtemisTurnSpeedInterpolant", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The speed of projectiles Apollo releases upon doing a burst dash.
        /// </summary>
        public static float DashSpreadProjectileSpeed => Variables.GetAIFloat("ElectricSupercharge_DashSpreadProjectileSpeed", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The speed of tesla spheres shot by Ares.
        /// </summary>
        public static float TeslaSphereShootSpeed => Variables.GetAIFloat("ElectricSupercharge_TeslaSphereShootSpeed", ExoMechAIVariableType.Combo);

        /// <summary>
        /// Apollo's maximum speed before he begins to decelerate.
        /// </summary>
        public static float ApolloMaxSpeed => Variables.GetAIFloat("ElectricSupercharge_ApolloMaxSpeed", ExoMechAIVariableType.Combo);

        public override bool Perform(NPC npc)
        {
            if (npc.type == ExoMechNPCIDs.AresBodyID)
                Perform_Ares(npc);
            if (npc.type == ExoMechNPCIDs.ArtemisID)
                Perform_Artemis(npc);
            if (npc.type == ExoMechNPCIDs.ApolloID)
                Perform_Apollo(npc);

            return AITimer >= ElectrifyTime + AttackDuration;
        }

        /// <summary>
        /// Performs Ares' part in the ElectricSupercharge attack.
        /// </summary>
        /// <param name="npc">Ares' NPC instance.</param>
        public static void Perform_Ares(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out AresBodyEternity ares) || !Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechTwinRed) || !Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechTwinGreen))
            {
                npc.active = false;
                return;
            }

            if (AITimer == AresBodyEternity.DetachHands_DetachmentDelay)
                SoundEngine.PlaySound(AresLaserCannon.TelSound);

            npc.rotation = npc.velocity.X * 0.0062f;

            Vector2 leftHandAimDestination = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Center;
            Vector2 rightHandAimDestination = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Center;
            if (leftHandAimDestination.X > rightHandAimDestination.X)
                Utils.Swap(ref leftHandAimDestination, ref rightHandAimDestination);

            if (AITimer <= AresRedirectTime)
                Perform_Ares_RedirectAbovePlayer(npc);
            else if (AITimer <= ElectrifyTime)
                npc.velocity *= 0.94f;
            else
            {
                npc.SmoothFlyNear(Target.Center - Vector2.UnitY * 450f, 0.02f, 0.96f);
                leftHandAimDestination = Target.Center;
                rightHandAimDestination = Target.Center;
            }

            ScreenShakeSystem.SetUniversalRumble(LumUtils.InverseLerpBump(0f, 0.8f, 0.9f, 1f, AITimer / (float)ElectrifyTime).Squared() * 3f, MathHelper.TwoPi, null, 0.2f);

            if (AITimer <= AresBodyEternity.DetachHands_DetachmentDelay)
            {
                for (int i = 0; i < ares.InstructionsForHands.Length; i++)
                {
                    int copyForDelegate = i;
                    ares.InstructionsForHands[i] = new(h => ares.DetachHandsUpdate(h, copyForDelegate));
                }
            }
            else
            {
                ares.InstructionsForHands[0] = new(h => AresHandUpdate(npc, h, new Vector2(-430f, 50f), leftHandAimDestination, 0));
                ares.InstructionsForHands[1] = new(h => AresHandUpdate(npc, h, new Vector2(-280f, 224f), leftHandAimDestination, 1));
                ares.InstructionsForHands[2] = new(h => AresHandUpdate(npc, h, new Vector2(280f, 224f), rightHandAimDestination, 2));
                ares.InstructionsForHands[3] = new(h => AresHandUpdate(npc, h, new Vector2(430f, 50f), rightHandAimDestination, 3));
            }
            ares.AnimationState = AresBodyEternity.AresFrameAnimationState.Default;
        }

        /// <summary>
        /// Makes Ares redirect above the player.
        /// </summary>
        /// <param name="npc">Ares' NPC instance.</param>
        public static void Perform_Ares_RedirectAbovePlayer(NPC npc)
        {
            float redirectSpeed = MathHelper.Lerp(0.05f, 0.2f, LumUtils.Convert01To010(LumUtils.InverseLerp(0f, 30f, AITimer).Squared()));
            redirectSpeed *= LumUtils.InverseLerp(AresRedirectTime, AresRedirectTime - 45f, AITimer);

            Vector2 hoverDestination = Target.Center + new Vector2(MathF.Cos(MathHelper.TwoPi * AITimer / 90f) * 300f, -410f);
            npc.SmoothFlyNearWithSlowdownRadius(hoverDestination, redirectSpeed, 1f - redirectSpeed, 50f);
        }

        /// <summary>
        /// Handles updating for Ares' hands during the ElectricSupercharge attack.
        /// </summary>
        /// <param name="aresBody">Ares' NPC instance.</param>
        /// <param name="hand">The hand ModNPC instance.</param>
        /// <param name="hoverOffset">The offset for the hand relative to the body's position.</param>
        /// <param name="aimDestination">Where the hand should aim.</param>
        /// <param name="armIndex">The arm's index in the overall set.</param>
        public static void AresHandUpdate(NPC aresBody, AresHand hand, Vector2 hoverOffset, Vector2 aimDestination, int armIndex)
        {
            NPC handNPC = hand.NPC;
            Vector2 hoverDestination = aresBody.Center + hoverOffset * aresBody.scale;

            hand.UsesBackArm = armIndex == 0 || armIndex == AresBodyEternity.ArmCount - 1;
            hand.ArmSide = (armIndex >= AresBodyEternity.ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.TeslaCannon;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.GlowmaskDisabilityInterpolant = 0f;
            handNPC.spriteDirection = 1;
            handNPC.Opacity = LumUtils.Saturate(handNPC.Opacity + 0.3f);

            handNPC.SmoothFlyNear(hoverDestination, 0.25f, 0.75f);
            handNPC.rotation = handNPC.AngleTo(aimDestination);

            Vector2 cannonEnd = handNPC.Center + new Vector2(handNPC.spriteDirection * 54f, 8f).RotatedBy(handNPC.rotation);
            if (AITimer <= ElectrifyTime)
            {
                float arcCreationChance = MathF.Pow(LumUtils.InverseLerp(0f, ElectrifyTime, AITimer), 0.75f) * 0.6f;
                for (int i = 0; i < 2; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(arcCreationChance))
                    {
                        Vector2 arcLength = (aimDestination - cannonEnd).RotatedByRandom(0.02f) * Main.rand.NextFloat(0.97f, 1.03f);
                        LumUtils.NewProjectileBetter(handNPC.GetSource_FromAI(), cannonEnd, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(6, 9));
                    }
                }
                hand.EnergyDrawer.chargeProgress = MathF.Sqrt(LumUtils.InverseLerpBump(0f, ElectrifyTime * 0.75f, ElectrifyTime * 0.81f, ElectrifyTime, AITimer));
            }
            else
            {
                int shootTimer = (AITimer + armIndex * 11) % TeslaSphereShootRate;
                hand.EnergyDrawer.chargeProgress = LumUtils.InverseLerp(TeslaSphereShootRate * 0.35f, TeslaSphereShootRate * 0.95f, shootTimer);

                if (shootTimer == TeslaSphereShootRate - 1)
                {
                    Vector2 aimDirection = (aimDestination - cannonEnd).SafeNormalize(Vector2.Zero);

                    SoundEngine.PlaySound(AresTeslaCannon.TeslaOrbShootSound, handNPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            Vector2 arcLength = aimDirection.RotatedByRandom(1.85f) * Main.rand.NextFloat(80f, 365f);
                            LumUtils.NewProjectileBetter(handNPC.GetSource_FromAI(), cannonEnd, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(6, 32));
                        }

                        Vector2 sphereVelocity = aimDirection * TeslaSphereShootSpeed;
                        LumUtils.NewProjectileBetter(handNPC.GetSource_FromAI(), cannonEnd, sphereVelocity, ModContent.ProjectileType<SmallTeslaSphere>(), SmallTeslaSphereDamage, 0f);
                    }

                    for (int i = 0; i < 15; i++)
                    {
                        Dust electricity = Dust.NewDustPerfect(cannonEnd + Main.rand.NextVector2Circular(10f, 10f), 226);
                        electricity.velocity = aimDirection * 10.5f + Main.rand.NextVector2Circular(3.6f, 3.6f);
                        electricity.scale = Main.rand.NextFloat(0.8f, 1.25f);
                        electricity.noGravity = true;
                    }

                    handNPC.velocity -= aimDirection * 18f;
                    handNPC.netUpdate = true;
                }
            }
        }

        /// <summary>
        /// Makes one of the Exo Twins stay near Ares so that they can be supercharged.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="side">Which side of Ares the Exo Twin should stay near.</param>
        public static void StayNearAres(NPC npc, int side)
        {
            if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechPrime))
                return;

            NPC ares = Main.npc[CalamityGlobalNPC.draedonExoMechPrime];
            Vector2 hoverDestination = ares.Center + new Vector2(side * 600f, 450f);
            npc.SmoothFlyNear(hoverDestination, 0.08f, 0.93f);
            npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.3f);
        }

        /// <summary>
        /// Performs Artemis' part in the ElectricSupercharge attack.
        /// </summary>
        /// <param name="npc">Artemis' NPC instance.</param>
        public static void Perform_Artemis(NPC npc)
        {
            PerformSharedExoTwinsBehaviors(npc);
            if (AITimer <= ElectrifyTime)
            {
                StayNearAres(npc, -1);
                return;
            }

            float hoverSpeed = Utils.Remap(npc.Distance(Target.Center), 180f, 270f, 9f, 16.75f);
            Vector2 hoverDestination = Target.Center + new Vector2(npc.HorizontalDirectionTo(Target.Center) * -540f, -250f);
            Vector2 idealVelocity = npc.SafeDirectionTo(hoverDestination) * hoverSpeed;
            npc.SimpleFlyMovement(idealVelocity, idealVelocity.Length() * 0.018f);

            float fastRedirectInterpolant = LumUtils.InverseLerp(45f, 0f, AITimer - ElectrifyTime);
            if (fastRedirectInterpolant > 0f)
                npc.SmoothFlyNear(hoverDestination, fastRedirectInterpolant * 0.2f, 1f - fastRedirectInterpolant * 0.09f);

            npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), ArtemisTurnSpeedInterpolant);

            bool doneAttacking = AITimer >= ElectrifyTime + AttackDuration - LumUtils.SecondsToFrames(1.25f);
            if (Main.netMode != NetmodeID.MultiplayerClient && !doneAttacking && AITimer % ArtemisShootRate == 0)
            {
                Vector2 laserSpawnPosition = npc.Center + npc.rotation.ToRotationVector2() * 76f;
                for (int i = 0; i < 15; i++)
                {
                    Vector2 particleVelocity = (MathHelper.TwoPi * i / 15f).ToRotationVector2() * 3f + Main.rand.NextVector2Circular(1.2f, 1.2f) + npc.velocity;
                    BloomPixelParticle electricity = new(laserSpawnPosition, particleVelocity, Color.White, Color.DeepSkyBlue * 0.54f, 25, Vector2.One * Main.rand.NextFloat(1.1f, 1.9f));
                    electricity.Spawn();
                }

                Vector2 laserVelocity = npc.rotation.ToRotationVector2() * 3.4f;
                if (Main.rand.NextBool(3))
                {
                    LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), laserSpawnPosition, laserVelocity * 4.5f / ArtemisLaserImproved.TotalUpdates, ModContent.ProjectileType<ArtemisLaserImproved>(), ExoTwinsStates.BasicShotDamage, 0f);
                    SoundEngine.PlaySound(CommonCalamitySounds.ExoLaserShootSound with { Volume = 0.8f }, laserSpawnPosition);
                }
                else
                    LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), laserSpawnPosition, laserVelocity, ModContent.ProjectileType<HomingTeslaBurst>(), AresBodyEternity.TeslaBurstDamage, 0f, -1, HomingTeslaBurst.HomeInTime);
            }
        }

        /// <summary>
        /// Performs Apollo's part in the ElectricSupercharge attack.
        /// </summary>
        /// <param name="npc">Apollo's NPC instance.</param>
        public static void Perform_Apollo(NPC npc)
        {
            PerformSharedExoTwinsBehaviors(npc);
            if (AITimer <= ElectrifyTime)
            {
                StayNearAres(npc, 1);
                return;
            }

            if (npc.WithinRange(Target.Center, 150f))
                npc.velocity *= 1.02f;
            else
            {
                Vector2 idealVelocity = npc.SafeDirectionTo(Target.Center) * 22f;
                if (AITimer % 150 >= 124)
                    idealVelocity *= 0.4f;

                npc.velocity += idealVelocity.SafeNormalize(Vector2.Zero) * 0.8f;
                npc.velocity = Vector2.Lerp(npc.velocity, idealVelocity, 0.012f);

                if (npc.velocity.Length() >= ApolloMaxSpeed)
                    npc.velocity *= 0.985f;
            }

            npc.rotation = npc.velocity.ToRotation();

            npc.damage = npc.defDamage;

            if (AITimer % 150 == 124)
                SoundEngine.PlaySound(Artemis.ChargeTelegraphSound);

            bool dashWouldCauseTelefrag = npc.velocity.AngleBetween(npc.SafeDirectionTo(Target.Center)) <= 0.37f;
            if (AITimer % 150 == 149 && !npc.WithinRange(Target.Center, 360f) && !dashWouldCauseTelefrag)
            {
                SoundEngine.PlaySound(AresTeslaCannon.TeslaOrbShootSound);
                SoundEngine.PlaySound(Artemis.ChargeSound);
                ScreenShakeSystem.StartShakeAtPoint(npc.Center, 5f);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < DashSpreadProjectileCount; i++)
                    {
                        Vector2 teslaBurstVelocity = npc.SafeDirectionTo(Target.Center).RotatedBy(MathHelper.TwoPi * i / DashSpreadProjectileCount) * DashSpreadProjectileSpeed;
                        LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), npc.Center, teslaBurstVelocity, ModContent.ProjectileType<HomingTeslaBurst>(), AresBodyEternity.TeslaBurstDamage, 0f, -1, HomingTeslaBurst.HomeInTime);
                    }

                    npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * 85f;
                    npc.netUpdate = true;
                }
            }
        }

        /// <summary>
        /// Performs behaviors in ElectricSupercharge attack that are shared between both Artemis and Apollo.
        /// </summary>
        /// <param name="npc"></param>
        public static void PerformSharedExoTwinsBehaviors(NPC npc)
        {
            IExoTwin twinInstance = null;
            if (npc.TryGetDLCBehavior(out ArtemisEternity artemis))
                twinInstance = artemis;
            else if (npc.TryGetDLCBehavior(out ApolloEternity apollo))
                twinInstance = apollo;
            if (twinInstance is null)
                return;

            float electricityChargeUpInterpolant = LumUtils.InverseLerp(0f, ElectrifyTime, AITimer).Squared() * LumUtils.InverseLerp(AttackDuration, AttackDuration - 30f, AITimer - ElectrifyTime);
            float gleamInterpolant = LumUtils.InverseLerp(0.42f, 1f, electricityChargeUpInterpolant);
            twinInstance.SpecialShaderAction = (texture, n) =>
            {
                Vector4 electricityColor = new(1f, 1.5f, 2f, 1f);

                // Bias the electricity based on Exo Twin to differentiate the two slightly.
                // Artemis is biased towards yellow, since she generally has an orange look to her, whereas Apollo is biased towards green, due to his green aesthetic.
                if (n.type == ExoMechNPCIDs.ArtemisID)
                    electricityColor += new Vector4(0.75f, 0.75f, -0.49f, 0f);
                else if (n.type == ExoMechNPCIDs.ApolloID)
                    electricityColor += new Vector4(0f, 0.97f, 0f, 0f);

                ManagedShader superchargeShader = ShaderManager.GetShader("FargowiltasCrossmod.ElectricSuperchargeShader");
                superchargeShader.TrySetParameter("textureSize", texture.Size());
                superchargeShader.TrySetParameter("frame", new Vector4(n.frame.X, n.frame.Y, n.frame.Width, n.frame.Height));
                superchargeShader.TrySetParameter("electricColor", electricityColor);
                superchargeShader.TrySetParameter("glowInterpolant", electricityChargeUpInterpolant);
                superchargeShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 1, SamplerState.PointWrap);
                superchargeShader.Apply();
                return true;
            };
            twinInstance.SpecificDrawAction = () =>
            {
                RenderElectricGleam(npc.Center + npc.rotation.ToRotationVector2() * 76f - Main.screenPosition, gleamInterpolant);
            };
            twinInstance.Frame = twinInstance.Animation.CalculateFrame(AITimer / 35f % 1f, twinInstance.InPhase2);
            twinInstance.WingtipVorticesOpacity = LumUtils.InverseLerp(25f, 36f, npc.velocity.Length());

            if (AITimer == ElectrifyTime - 1)
            {
                SoundEngine.PlaySound(TeslaCannon.FireSound);
                ScreenShakeSystem.StartShakeAtPoint(npc.Center, 9.5f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<LargeTeslaSphereExplosion>(), 0, 0f);
                    npc.velocity += npc.SafeDirectionTo(Target.Center) * 32f;
                    npc.netUpdate = true;
                }
            }

            // Jitter as the Exo Twin overflows with electricity.
            float jitterSpeed = LumUtils.InverseLerpBump(0f, 0.75f, 0.85f, 1f, electricityChargeUpInterpolant) * 8.5f;
            npc.Center += Main.rand.NextVector2Unit() * Main.rand.NextFloat(jitterSpeed);

            if (Main.rand.NextBool(electricityChargeUpInterpolant))
            {
                int sparkLifetime = Main.rand.Next(7, 16);
                Vector2 sparkVelocity = Main.rand.NextVector2Circular(7.2f, 7.2f);
                Color sparkColor = Color.Lerp(Color.Wheat, new(0.15f, 0.7f, 1f), Main.rand.NextFloat());
                ElectricSparkParticle spark = new(npc.Center + Main.rand.NextVector2Circular(90f, 90f) - npc.velocity * 1.1f, sparkVelocity, sparkColor, Color.Yellow * 0.3f, sparkLifetime, Vector2.One * 0.19f);
                spark.Spawn();
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(electricityChargeUpInterpolant * 0.67f))
            {
                Vector2 arcStart = npc.Center + Main.rand.NextVector2Circular(30f, 30f);
                Vector2 aimDestination = arcStart + Main.rand.NextVector2Unit() * Main.rand.NextFloat(75f, 150f);
                Vector2 arcLength = (aimDestination - arcStart).RotatedByRandom(0.095f) * Main.rand.NextFloat(0.97f, 1.03f);
                LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), arcStart, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(6, 9));
            }
        }

        /// <summary>
        /// Draws a gleam on an Exo Twin's pupil/front weapon as a telegraph.
        /// </summary>
        /// <param name="drawPosition">The base draw position of the Exo Twin.</param>
        public static void RenderElectricGleam(Vector2 drawPosition, float glimmerInterpolant)
        {
            Texture2D flare = MiscTexturesRegistry.ShineFlareTexture.Value;
            Texture2D bloom = MiscTexturesRegistry.BloomCircleSmall.Value;

            float flareOpacity = LumUtils.InverseLerp(1f, 0.75f, glimmerInterpolant);
            float flareScale = MathF.Pow(LumUtils.Convert01To010(glimmerInterpolant), 1.4f) * 1.9f + 0.1f;
            flareScale *= LumUtils.InverseLerp(0f, 0.09f, glimmerInterpolant);

            float flareRotation = MathHelper.SmoothStep(0f, MathHelper.TwoPi, MathF.Pow(glimmerInterpolant, 0.2f)) + MathHelper.PiOver4;
            Color flareColorA = new(45, 197, 255, 0);
            Color flareColorB = new(174, 255, 255, 0);
            Color flareColorC = new(245, 222, 179, 0);

            Main.spriteBatch.Draw(bloom, drawPosition, null, flareColorA * flareOpacity * 0.32f, 0f, bloom.Size() * 0.5f, flareScale * 1.9f, 0, 0f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, flareColorB * flareOpacity * 0.56f, 0f, bloom.Size() * 0.5f, flareScale, 0, 0f);
            Main.spriteBatch.Draw(flare, drawPosition, null, flareColorC * flareOpacity, flareRotation, flare.Size() * 0.5f, flareScale, 0, 0f);
        }
    }
}
