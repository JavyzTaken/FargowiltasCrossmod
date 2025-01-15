using CalamityMod.Events;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using CalamityMod;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls;
using ReLogic.Content;
using CalamityMod.Projectiles.Summon;
using Terraria.Audio;
using CalamityMod.Projectiles.Boss;
using Terraria.ID;
using CalamityMod.NPCs.AcidRain;
using FargowiltasSouls.Content.Bosses.VanillaEternity;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ASEternity : CalDLCEmodeBehavior
    {
        public static bool Enabled = false;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;
        public override int NPCOverrideID => ModContent.NPCType<AquaticScourgeHead>();

    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ASBodyAltEternity : ASBodyEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<AquaticScourgeBodyAlt>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ASBodyEternity : CalDLCEmodeBehavior
    {
        public override bool IsLoadingEnabled(Mod mod) => ASEternity.Enabled;
        public override int NPCOverrideID => ModContent.NPCType<AquaticScourgeBody>();
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            DestroyerSegment.PierceResistance(projectile, ref modifiers);
        }
        public override void UpdateLifeRegen(ref int damage)
        {
            if (NPC.lifeRegen < 0)
            {
                NPC.lifeRegen = (int)Math.Round(NPC.lifeRegen / 4f);
            }
        }
        /*

        public bool SpikeAttackSprite = false;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!SpikeAttackSprite)
                return true;

            if (npc.IsABestiaryIconDummy)
                return true;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/AquaticScourge/SpikelessASBody").Value;
            Vector2 vector11 = new Vector2(texture2D15.Width / 2, texture2D15.Height / 2);

            Vector2 vector43 = npc.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            Color color = npc.GetAlpha(drawColor);

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive || Main.zenithWorld)
            {
                if (Main.npc[(int)npc.ai[2]].Calamity().newAI[3] > 300f)
                    color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp((Main.npc[(int)npc.ai[2]].Calamity().newAI[3] - 300f) / 180f, 0f, 1f));
                else if (Main.npc[(int)npc.ai[2]].localAI[3] > 0f)
                    color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp(Main.npc[(int)npc.ai[2]].localAI[3] / 90f, 0f, 1f));
            }

            spriteBatch.Draw(texture2D15, vector43, npc.frame, color, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            return false;
        }
        */
    }
    public class ASPartsEternity : CalDLCEmodeExtraGlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => ASEternity.Enabled;
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            ModContent.NPCType<AquaticScourgeBody>(), 
            ModContent.NPCType<AquaticScourgeHead>(), 
            ModContent.NPCType<AquaticScourgeBodyAlt>(), 
            ModContent.NPCType<AquaticScourgeTail>());

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
            if (npc.type == ModContent.NPCType<AquaticScourgeBody>() && npc.ai[3] >= 1)
            {
                Asset<Texture2D> t = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/AquaticScourge/SpikelessASBody");
                spriteBatch.Draw(t.Value, npc.Center - screenPos, null, drawColor, npc.rotation, t.Size() / 2, npc.scale, SpriteEffects.None, 0);
                return false;
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
            entity.lifeMax = (int)Math.Round(entity.lifeMax * 1.15f);
        }
        public float[] Vars = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int CurAttack = 0;
        public enum Attacks {
            BasicCharge,
            GassyCharge,
            FlamingCharge,
            FloatingGasCharge,
            SuperSpikeCharge,
            Circle,
            Suck
        };
        
        public void AdvanceAttack(Attacks[] cycle, int startAggro = 220)
        {
            Vars[0]++;
            for (int i = 1; i < Vars.Length; i++)
            {
                Vars[i] = 0;
            }
            Vars[9] = startAggro;
            
            if (Vars[0] >= cycle.Length)
            {
                Vars[0] = 0;
            }
            CurAttack = (int)cycle[(int)Vars[0]];
        }
        public void SlowdownPrep(NPC npc, int time, SoundStyle endSound)
        {
            if (Vars[8] > time)
            {
                Vars[1]++;
                Vars[8] = 0;
                SoundEngine.PlaySound(endSound, npc.Center);
            }
        }
        public override bool SafePreAI(NPC npc)
        {

            if (npc.type == ModContent.NPCType<AquaticScourgeHead>() && npc.ai[0] == 6 && npc.Calamity().newAI[0] >= 1)
            {
                npc.TargetClosest();

                //increases when angle is too different. increases turn rate when gets high enough. decreases over time when angle isnt too different

                //Main.NewText(npc.GetLifePercent());

                Attacks[] attackCycle = {
                    Attacks.BasicCharge,
                    Attacks.BasicCharge,
                    Attacks.BasicCharge,
                    Attacks.SuperSpikeCharge
                    
                };
                if (npc.GetLifePercent() < 0.8f)
                {
                    attackCycle = [
                        Attacks.BasicCharge,
                        Attacks.GassyCharge,
                        Attacks.GassyCharge,
                        Attacks.FlamingCharge,
                        Attacks.SuperSpikeCharge,
                        Attacks.FlamingCharge
                    ];
                }
                if (npc.GetLifePercent() < 0.6f)
                {
                    attackCycle = [
                        Attacks.BasicCharge,
                        Attacks.BasicCharge,
                        Attacks.BasicCharge,
                        Attacks.SuperSpikeCharge,
                        Attacks.BasicCharge,
                        Attacks.Circle,
                        Attacks.Suck
                    ];
                }
                if (npc.GetLifePercent() < 0.3f)
                {
                    attackCycle = [
                        Attacks.BasicCharge,
                        Attacks.SuperSpikeCharge,
                        Attacks.GassyCharge,
                        Attacks.Circle,
                        Attacks.Suck,
                        Attacks.GassyCharge,
                        Attacks.FlamingCharge
                    ];
                }



                ref float aggroTimer = ref Vars[9];
                ref float attackTimer = ref Vars[8];
                int att = CurAttack;
                
                

                //Main.NewText(Vars[9]);
                SoundStyle DSroar = new("CalamityMod/Sounds/Custom/DesertScourgeRoar");
                SoundStyle Mroar = Mauler.RoarSound;




                Player target = null;
                if (npc.HasValidTarget)
                {
                    

                    target = Main.player[npc.target];
                    Vector2 targetPos = target.Center;
                    float maxSpeed = 10;
                    bool turnTowardsPlayer = true;
                    float turnAngle = 1;
                    attackTimer++;



                    //move faster if hasnt been able to charge in a while
                    void Follow() {
                        
                        if (Vars[9] > 160 && npc.Distance(target.Center) > 500)
                        {
                            maxSpeed = 15;
                        }
                        //begin charge cycle when close to the player
                        if (npc.Distance(target.Center) < 700 && Vars[9] < 150)
                        {
                            Vars[1]++;
                            Vars[8] = 0;
                        }
                    }
                    //Charge
                    if (att == (int)Attacks.BasicCharge)
                    {
                        
                        if (Vars[1] == 0)
                        {
                            
                            Follow();
                        }
                        //slowdown tele
                        if (Vars[1] == 1)
                        {
                            maxSpeed = 2;
                            turnAngle = 3;
                            SlowdownPrep(npc, 20, DSroar);
                        }
                        //actual charge
                        if (Vars[1] == 2)
                        {
                            turnAngle = 1f;
                            maxSpeed = 30;
                            //stop and barf
                            if (attackTimer >= 70)
                            {
                                AdvanceAttack(attackCycle);
                                SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                                for (int i = 0; i < 12; i++)
                                {
                                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.NextFloat(1, 8), 0).RotatedByRandom(MathHelper.TwoPi),
                                         ModContent.ProjectileType<ToxicGas>(),
                                        FargoSoulsUtil.ScaledProjectileDamage(50), 1, ai0: 1);
                                }
                            }
                        }
                    }
                    if (att == (int)Attacks.GassyCharge)
                    {
                        if (Vars[1] == 0)
                        {
                            Follow();
                        }
                        if (Vars[1] == 1)
                        {
                            maxSpeed = 2;
                            turnAngle = 3;
                            SlowdownPrep(npc, 20, DSroar);
                        }
                        //actual charge
                        if (Vars[1] == 2)
                        {
                            turnAngle = 1f;
                            maxSpeed = 30;
                            if (attackTimer % 5 == 0)
                            {
                                Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, npc.velocity / 10, ModContent.ProjectileType<FlamableGas>(), FargoSoulsUtil.ScaledProjectileDamage(50), 1);
                            }
                            //stop and barf
                            if (attackTimer >= 70)
                            {
                                AdvanceAttack(attackCycle);
                                SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                                for (int i = 0; i < 12; i++)
                                {
                                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.NextFloat(1, 8), 0).RotatedByRandom(MathHelper.TwoPi),
                                         ModContent.ProjectileType<FlamableGas>(),
                                        FargoSoulsUtil.ScaledProjectileDamage(50), 1, ai0: 0);
                                }
                            }
                        }
                    }
                    if (att == (int)Attacks.FlamingCharge)
                    {
                        if (Vars[1] == 0)
                        {
                            Follow();
                        }
                        if (Vars[1] == 1)
                        {
                            maxSpeed = 2;
                            turnAngle = 3;
                            SlowdownPrep(npc, 20, DSroar);
                        }
                        //actual charge
                        if (Vars[1] == 2)
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].type == ModContent.ProjectileType<FlamableGas>() && Main.projectile[i].Distance(npc.Center) < 130 && Main.projectile[i].ai[0] == 0)
                                {
                                    Main.projectile[i].ai[0] = 1;
                                    Main.projectile[i].timeLeft = 30;
                                }
                            }

                            turnAngle = 1f;
                            maxSpeed = 30;
                            Dust d = Dust.NewDustDirect(npc.Center + new Vector2(20, -40).RotatedBy(npc.rotation), 0, 0, DustID.CursedTorch);
                            Dust d2 = Dust.NewDustDirect(npc.Center + new Vector2(-20, -40).RotatedBy(npc.rotation), 0, 0, DustID.CursedTorch);
                            d.scale = 2;
                            d2.scale = 2;
                            //stop and barf
                            if (attackTimer >= 70)
                            {
                                AdvanceAttack(attackCycle);
                                SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                                for (int i = 0; i < 12; i++)
                                {
                                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.NextFloat(1, 8), 0).RotatedByRandom(MathHelper.TwoPi),
                                         ModContent.ProjectileType<ToxicGas>(),
                                        FargoSoulsUtil.ScaledProjectileDamage(50), 1, ai0: 1);
                                }
                            }
                        }
                    }
                    if (att == (int)Attacks.FloatingGasCharge)
                    {
                        if (Vars[1] == 0)
                        {
                            Follow();
                        }
                        if (Vars[1] == 1)
                        {
                            maxSpeed = 2;
                            turnAngle = 3;
                            SlowdownPrep(npc, 20, DSroar);
                            targetPos = target.Center + new Vector2(0, 300);
                        }
                        //actual charge
                        if (Vars[1] == 2)
                        {
                            turnAngle = 1f;
                            maxSpeed = 30;
                            targetPos = target.Center + new Vector2(0, 300);
                            if (attackTimer % 10 == 0)
                            {
                                Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, npc.velocity / 10, ModContent.ProjectileType<ToxicGas>(), FargoSoulsUtil.ScaledProjectileDamage(50), 1, ai0:2);
                            }
                            //stop and barf
                            if (attackTimer >= 70)
                            {
                                AdvanceAttack(attackCycle);
                                SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                                for (int i = 0; i < 12; i++)
                                {
                                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.NextFloat(1, 8), 0).RotatedByRandom(MathHelper.TwoPi),
                                         ModContent.ProjectileType<ToxicGas>(),
                                        FargoSoulsUtil.ScaledProjectileDamage(50), 1, ai0: 1);
                                }
                            }
                        }
                    }
                    //longer charge
                    if (att == (int)Attacks.SuperSpikeCharge)
                    {
                        if (Vars[1] == 0)
                        {
                            Follow();
                        }
                        //slowdown tele
                        if (Vars[1] == 1)
                        {
                            maxSpeed = 2;
                            turnAngle = 3;
                            SlowdownPrep(npc, 40, Mroar);
                        }
                        //speed
                        if (Vars[1] == 2)
                        {
                            aggroTimer = 0;
                            turnAngle = 1f;
                            maxSpeed = 45;
                            if (attackTimer >= 80)
                            {
                                attackTimer = 0;
                                Vars[1]++;
                            }
                        }
                        //stop and shoot spikes
                        if (Vars[1] == 3)
                        {
                            maxSpeed = 2;
                            turnTowardsPlayer = false;
                            int numspieks = 2;
                            if (attackTimer > 10 && attackTimer % 3 == 0)
                            {
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    NPC n = Main.npc[i];
                                    if (n.type == ModContent.NPCType<AquaticScourgeBody>() && n.ai[3] == 0 && Main.rand.NextBool())
                                    {
                                        if (Main.rand.NextBool())
                                        {
                                            Projectile.NewProjectileDirect(n.GetSource_FromAI(), n.Center, npc.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(120) - MathHelper.PiOver2) * 20, ModContent.ProjectileType<Tooth>(), FargoSoulsUtil.ScaledProjectileDamage(50), 1);
                                        }
                                        else
                                        {
                                            Projectile.NewProjectileDirect(n.GetSource_FromAI(), n.Center, npc.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-120) - MathHelper.PiOver2) * 20, ModContent.ProjectileType<Tooth>(), FargoSoulsUtil.ScaledProjectileDamage(50), 1);
                                        }
                                        n.ai[3]++;

                                        SoundEngine.PlaySound(SoundID.Item17 with { MaxInstances = 0, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest }, n.Center);
                                        if (n.ai[3] >= numspieks)
                                            break;
                                    }
                                }
                            }
                            //reset sprites on body parts
                            if (attackTimer > 60)
                            {

                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    NPC n = Main.npc[i];
                                    if (n.type == ModContent.NPCType<AquaticScourgeBody>())
                                    {

                                        n.ai[3] = 0;


                                    }
                                }
                                AdvanceAttack(attackCycle);
                            }
                        }
                    }
                    //circle
                    if (att == (int)Attacks.Circle)
                    {

                        
                        if (Vars[1] == 0)
                        {
                            Follow();
                        }
                        else
                        {
                            maxSpeed = 25;
                            aggroTimer = 0;
                        }
                        //get an angle to start at
                        if (Vars[1] == 1)
                        {
                            
                            float  tangle =  target.Center.AngleTo(npc.Center);
                            Vars[2] = tangle;
                            Vars[1]++;
                        }
                        //move towards position next to player based on found angle
                        if (Vars[1] == 2)
                        {
                            turnAngle = 4;
                            targetPos = target.Center + new Vector2(900, 0).RotatedBy(Vars[2]);
                            if (npc.Distance(targetPos) < 100 || attackTimer > 100)
                            {
                                Vars[1]++;
                               
                                //store a pos to rotate around so it doesnt end up chasing the player relentlessly
                                Vars[3] = target.Center.X;
                                Vars[4] = target.Center.Y;
                            }
                            turnTowardsPlayer = false;
                            npc.velocity = Vector2.Lerp(npc.velocity, (targetPos - npc.Center).SafeNormalize(Vector2.Zero) * maxSpeed, 0.05f);
                        }
                        //rotate around pos
                        if (Vars[1] == 3)
                        {
                            maxSpeed = 30;
                            turnAngle = 5;
                            
                            targetPos = new Vector2(Vars[3], Vars[4]) + new Vector2(800, 0).RotatedBy(MathHelper.WrapAngle(Vars[2] + MathHelper.ToRadians(attackTimer*2)));
                            //vomit gas
                            if (attackTimer % 15 == 0 && npc.Distance(target.Center) > 200)
                            {
                                SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                                for (int i = 0; i < 10; i++)
                                {
                                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, npc.velocity.RotatedByRandom(MathHelper.ToRadians(60)).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 10), ModContent.ProjectileType<ToxicGas>(), FargoSoulsUtil.ScaledProjectileDamage(50), 1, ai0:1);
                                }
                            }
                            if (attackTimer >= 220)
                            {
                                AdvanceAttack(attackCycle);
                            }
                            turnTowardsPlayer = false;
                            npc.velocity = Vector2.Lerp(npc.velocity, (targetPos - npc.Center).SafeNormalize(Vector2.Zero) * maxSpeed, 0.05f);
                        }
                       
                        
                    }
                    //suck
                    if (att == (int)Attacks.Suck)
                    {
                        maxSpeed = 3;
                        turnAngle = 1;
                        if (npc.Distance(target.Center) > 500)
                        {
                            maxSpeed = 10;
                            turnAngle = 4;
                        }
                        //spawn sucker
                        if (attackTimer == 1)
                        {
                            Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<AquaticSuck>(), FargoSoulsUtil.ScaledProjectileDamage(0), 1, ai0: npc.whoAmI);
                        }
                        //shoot gas to the sides so player cant rotate as fast
                        if (attackTimer < 380 && attackTimer % 30 == 0)
                        {
                            int range = 10;
                            SoundEngine.PlaySound(SoundID.Item73, npc.Center);
                            for (int i = -range; i <= range; i++)
                            {
                               
                                if (i < -range/2 || i > range / 2)
                                {
                                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, npc.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(i * 5)) * 10, ModContent.ProjectileType<ToxicGas>(), FargoSoulsUtil.ScaledProjectileDamage(50), 1, ai0: 0);
                                }
                            }
                        }
                        //vomit some gas at the end
                        if (attackTimer > 390)
                        {
                            turnAngle = 0.1f;
                            maxSpeed = 4;
                            if (attackTimer % 5 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item34, npc.Center);
                                
                                Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, npc.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-20, 20)) - MathHelper.PiOver2) * Main.rand.NextFloat(1, 30), ModContent.ProjectileType<FlamableGas>(), FargoSoulsUtil.ScaledProjectileDamage(0), 1);
                                
                            }
                            if (attackTimer % 10 == 0)
                            {
                                Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, npc.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-20, 20)) - MathHelper.PiOver2) * Main.rand.NextFloat(10, 20), ModContent.ProjectileType<ToxicGas>(), FargoSoulsUtil.ScaledProjectileDamage(50), 1, ai0: 1);
                            }
                        }
                        if (attackTimer >= 460)
                        {
                            AdvanceAttack(attackCycle);
                        }
                    }

                    //speed up if too slow and vice versa
                    if (npc.velocity.Length() < maxSpeed)
                    {
                        npc.velocity *= 1.05f;
                    }
                    else
                    {
                        npc.velocity *= 0.95f;
                    }

                    //difference between angle of npc and the angle to the player
                    float angle = MathHelper.ToDegrees(FargoSoulsUtil.RotationDifference(npc.velocity, npc.AngleTo(targetPos).ToRotationVector2()));

                    //increase aggression if facing the complete wrong direction and vice versa
                    if (Math.Abs(angle) > 80)
                    {
                        aggroTimer++;
                    }
                    else if (aggroTimer > 1)
                    {
                        aggroTimer--;
                    }
                    //turning to face the player
                    if (Math.Abs(angle) > 2f && turnTowardsPlayer)
                    {

                        //if aggro timer is up and facing the wrong way turn faster
                        if (Math.Abs(angle) > 80)
                        {
                            if (aggroTimer > 60)
                            {
                                turnAngle = 2;
                            }
                            if (aggroTimer > 200)
                            {
                                turnAngle = 4;
                            }
                        }
                        npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(angle > 0 ? turnAngle : -turnAngle));

                    }
                    

                }
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
                return false;
            }
            //stops body segments from shooting spikes randomly from base calamity code
            if (npc.type == ModContent.NPCType<AquaticScourgeBody>() || npc.type == ModContent.NPCType<AquaticScourgeBodyAlt>() || npc.type == ModContent.NPCType<AquaticScourgeTail>())
            {

                npc.localAI[0] = 0;
               
            }
            return base.SafePreAI(npc);
        }
        
    }
}
