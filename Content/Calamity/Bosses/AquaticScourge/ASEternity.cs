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
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;

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

            if (npc.type == ModContent.NPCType<AquaticScourgeBody>() || npc.type == ModContent.NPCType<AquaticScourgeBodyAlt>())
            {
                if (npc.ai[2] == -1)
                {
                    return true;
                }
                NPC head = Main.npc[(int)npc.ai[2]];
                ASPartsEternity ashead = head.GetGlobalNPC<ASPartsEternity>();

                if ((head.GetLifePercent() > 0.6f && ashead.Hittable1 == npc.whoAmI) ||
                    (head.GetLifePercent() > 0.3f && ashead.Hittable2 == npc.whoAmI && head.GetLifePercent() <= 0.6f) ||
                    ( ashead.Hittable3 == npc.whoAmI && head.GetLifePercent() <= 0.3f))
                {
                    Main.instance.LoadItem(ItemID.FloatingTube);
                    Asset<Texture2D> t = TextureAssets.Item[ItemID.FloatingTube];
                    spriteBatch.Draw(t.Value, npc.Center - screenPos, null, drawColor, npc.rotation, t.Size() / 2, 4, SpriteEffects.None, 1);
                }


            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
            entity.lifeMax = (int)Math.Round(entity.lifeMax * 0.2f);
            entity.defense = 0;
            entity.Calamity().DR = 0;
        }
        public override void SafePostAI(NPC npc)
        {
            if (npc.type == ModContent.NPCType<AquaticScourgeBody>() && npc.whoAmI == 7)
            {
                //Main.NewText(npc.ai[0] + ", " + npc.ai[1] + ", " + npc.ai[2] + ", " + npc.ai[3]);
                //Dust.NewDustPerfect(npc.Center, DustID.Terra).noGravity = true;
            }
            base.SafePostAI(npc);
        }
        
        //segments
        public bool Hittable;

        //head
        public int Hittable1 = 0;
        public int Hittable2 = 0;
        public int Hittable3 = 0;

        public float FollowTurnSpeed = 1;
        public int FollowTimer;


        public int AttackChain = 0;
        public int AttackPart = 0;

        public float DashAttackTimer = 0;
        public int RocksTimer = 0;
        public int SpikeTimer = 0;

        public enum Attacks {
            Follow,
            RockDashDashRock,
            GasGasGasRockDash,
            LittleDashes,

            Transition1,

            LittleDashesHomingSpikes,
            SpikesThenGas,
            GasThenSpikes,

            Transition2,
        };
        
        
        public override bool SafePreAI(NPC npc)
        {
            if(npc.type == ModContent.NPCType<AquaticScourgeTail>())
            {
                npc.dontTakeDamage = true;
            }
            //npc.ai[0] = 3;
            if (npc.type == ModContent.NPCType<AquaticScourgeBody>() || npc.type == ModContent.NPCType<AquaticScourgeBodyAlt>())
            {
                if (npc.ai[2] == -1)
                {
                    npc.StrikeInstantKill();
                    return false;
                }
                NPC head = Main.npc[(int)npc.ai[2]];
                ASPartsEternity ashead = head.GetGlobalNPC<ASPartsEternity>();
                npc.dontTakeDamage = true;

                if ((head.GetLifePercent() > 0.6f && ashead.Hittable1 == npc.whoAmI) ||
                    (head.GetLifePercent() > 0.3f && ashead.Hittable2 == npc.whoAmI && head.GetLifePercent() <= 0.6f) ||
                    (ashead.Hittable3 == npc.whoAmI && head.GetLifePercent() <= 0.3f))
                {
                    npc.dontTakeDamage = false;
                }

                //Main.NewText(head.GetLifePercent());
                if ((ashead.Hittable1 == npc.whoAmI && head.GetLifePercent() < 0.65f) ||
                    (ashead.Hittable2 == npc.whoAmI && head.GetLifePercent() < 0.35f))
                {
                    DestroyAllSegmentsBelowMe();
                    NPC.HitInfo info = new NPC.HitInfo();
                    info.Damage = (int)(head.lifeMax * 0.05f);
                    head.StrikeNPC(info, false, true);
                }

                void DestroyAllSegmentsBelowMe()
                {
                    bool reachedTail = false;
                    int thing = npc.whoAmI;
                    while (!reachedTail)
                    {
                        if (Main.npc[thing].type != ModContent.NPCType<AquaticScourgeTail>())
                        {
                            Main.npc[thing].realLife = -1;
                            Main.npc[thing].ai[2] = -1;
                            thing = (int)Main.npc[thing].ai[0];

                        }
                        else
                        {
                            reachedTail = true;
                            Main.npc[thing].ai[1] = npc.ai[1];
                        }
                    }
                }
            }
            if (npc.type == ModContent.NPCType<AquaticScourgeHead>() && npc.Calamity().newAI[0] >= 1)
            {
                npc.TargetClosest();
                npc.dontTakeDamage = true;
                //increases when angle is too different. increases turn rate when gets high enough. decreases over time when angle isnt too different

                //Main.NewText(npc.GetLifePercent());

                //max length is 40 in revengeance and 80 in death. i dont feel like changing the max length.
                int maxLength = CalamityWorld.death ? 80 : 40;

                //get segments that will be hittable throughout the fight
                if (Hittable1 == 0)
                {
                    Hittable1 = FindSegment(2 * maxLength / 3);
                }
                if (Hittable2 == 0)
                {
                    Hittable2 = FindSegment(maxLength / 3);
                }
                if (Hittable3 == 0)
                {
                    Hittable3 = FindSegment(maxLength / 5);
                }

                if (npc.GetLifePercent() > 0.6f)
                {
                    Main.npc[Hittable1].GetGlobalNPC<ASPartsEternity>().Hittable = true;
                }
                else if (npc.GetLifePercent() > 0.3f)
                {
                    Main.npc[Hittable2].GetGlobalNPC<ASPartsEternity>().Hittable = true;
                }
                else
                {
                    Main.npc[Hittable3].GetGlobalNPC<ASPartsEternity>().Hittable = true;
                }

                int FindSegment(int howFarDown)
                {
                    NPC segment = npc;
                    for (int i = 0; i < howFarDown; i++)
                    {
                        NPC thing = Main.npc[(int)segment.ai[0]];
                        if (thing != null && thing.active && (thing.type == ModContent.NPCType<AquaticScourgeBody>() || thing.type == ModContent.NPCType<AquaticScourgeBodyAlt>()))
                        {
                            segment = thing;
                        }
                    }
                    return segment.whoAmI;
                }

                //Main.NewText(Vars[9]);
                SoundStyle DSroar = new("CalamityMod/Sounds/Custom/DesertScourgeRoar");
                SoundStyle Mroar = Mauler.RoarSound;




                Player target = null;
                if (npc.HasValidTarget)
                {
                    

                    target = Main.player[npc.target];
                    Vector2 targetPos = target.Center;


                    float angleDiff = MathHelper.ToDegrees(FargoSoulsUtil.RotationDifference(npc.velocity, npc.AngleTo(targetPos).ToRotationVector2()));
                    if (Math.Abs(angleDiff) > 10)
                    {
                        FollowTurnSpeed += 0.01f;
                    }else if (FollowTurnSpeed > 1)
                    {
                        FollowTurnSpeed -= 0.01f;
                    }
                    //Follow(10, aggroTimer);

                    switch ((Attacks)AttackChain)
                    {
                        case Attacks.RockDashDashRock:
                            if (AttackPart == 0)
                            {
                                RandomRocks(10, npc.AngleTo(target.Center), 40);
                            }
                            else if (AttackPart == 1 || AttackPart == 2)
                            {
                                Dash(25, 100);
                            }
                            else if (AttackPart == 3)
                            {
                                RandomRocks(40, MathHelper.ToRadians(-90), 50);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.GasGasGasRockDash:
                            if (AttackPart < 3)
                            {
                                Dash(25, 100, 1);
                            }
                            else if (AttackPart == 3)
                            {
                                RandomRocks(40, MathHelper.ToRadians(-90), 50);
                            }
                            else if (AttackPart == 4)
                            {
                                Dash(30, 140);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.LittleDashes:
                            if (AttackPart < 3)
                            {
                                Dash(30, 50);
                            }
                            else if (AttackPart == 3)
                            {
                                Dash(25, 100, 1);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.LittleDashesHomingSpikes:
                            if (AttackPart < 8 && AttackPart % 2 == 0)
                            {
                                Dash(30, 80);
                            }
                            else if (AttackPart < 8)
                            {
                                Spikes(5, 30, true, 25);
                            }
                            else if (AttackPart == 8)
                            {
                                Dash(25, 100);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.SpikesThenGas:
                            if (AttackPart == 0)
                            {
                                Dash(25, 80);
                            }
                            else if (AttackPart == 1)
                            {
                                Spikes(5, 40, false);
                            }
                            else if (AttackPart < 4)
                            {
                                Dash(25, 100, 1);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.GasThenSpikes:
                            if (AttackPart < 3)
                            {
                                Dash(25, 100, 1);
                            }
                            else if (AttackPart == 3)
                            {
                                Dash(25, 80);

                            }
                            else if (AttackPart == 4)
                            {
                                Spikes(5, 40, false);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.Follow:
                            FollowTimer++;
                            if (FollowTimer > 300 && npc.Distance(targetPos) < 800)
                            {
                                FollowTimer = 0;
                                AttackChain = Main.rand.Next(1, 4);
                                if (npc.GetLifePercent() <= 0.6f) AttackChain = Main.rand.Next(5, 8);
                                //AttackChain = (int)Attacks.LittleDashesHomingSpikes;
                            }
                            //targetPos = npc.Center - npc.velocity.RotatedBy(MathHelper.ToRadians(10));
                            Follow(10, FollowTurnSpeed);
                            break;
                    }

                    void RandomRocks(float amount, float angle, float spread)
                    {
                        RocksTimer++;
                        if (RocksTimer < 40)
                        {
                            targetPos = npc.Center + angle.ToRotationVector2();
                            Follow(5, 5, 1.05f);
                        }
                        else if (RocksTimer == 40)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                            SoundEngine.PlaySound(Mroar, npc.Center);

                            for (int i = 0; i < amount; i++)
                            {
                                Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, npc.rotation.ToRotationVector2().RotatedBy(-MathHelper.Pi/2).RotatedByRandom(MathHelper.ToRadians(spread)) * Main.rand.NextFloat(10, 15), ModContent.ProjectileType<CalamityMod.Projectiles.Enemy.CrabBoulder>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1);
                            }
                        }
                        if (RocksTimer == 60)
                        {
                            RocksTimer = 0;
                            AttackPart++;
                        }
                    }
                    void Dash(float speed, float time, int epicExtraFlag = 0)
                    {
                        DashAttackTimer++;
                        
                        if (DashAttackTimer == (int)(time * 0.8f) && epicExtraFlag == 1)
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                                Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.NextFloat(2, 7), 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ToxicGas>(), FargoSoulsUtil.ScaledProjectileDamage(40), 1);
                            }
                        }

                        if (DashAttackTimer < time * 0.3f)
                        {
                            
                            Follow(3, 9, 1.05f);
                        }
                        else if (DashAttackTimer == (int)(time * 0.3f)+1)
                        {
                            
                            SoundEngine.PlaySound(Mroar, npc.Center);
                        }
                        else 
                        {
                            Follow(speed, 1f, 1.06f);
                        }

                        if (DashAttackTimer > time && npc.Distance(targetPos) > 500)
                        {
                            DashAttackTimer = 0;
                            //DashAttackState = 0;
                            AttackPart++;
                        }
                    }
                    void Spikes(float speed, float time, bool homing, int howFar = -1)
                    {
                        targetPos = npc.Center + npc.velocity * 10;
                        Follow(speed, 1);
                        int final = howFar == -1 ? maxLength : howFar;

                        SpikeTimer++;
                        if (SpikeTimer == (int)(time * 0.8f))
                        {
                            NPC n = npc;
                            for (int i = 0; i < final; i++)
                            {
                                n = Main.npc[(int)n.ai[0]];
                                if (n == null || !n.active || (n.type != ModContent.NPCType<AquaticScourgeBody>() && n.type != ModContent.NPCType<AquaticScourgeBodyAlt>()))
                                {
                                    break;
                                }
                                else if (n.type == ModContent.NPCType<AquaticScourgeBody>())
                                {
                                    int type = ModContent.ProjectileType<Tooth>();
                                    Projectile.NewProjectileDirect(n.GetSource_FromAI(), n.Center, n.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(90 + 50)) * 20, type, npc.damage, 1, ai0:25, ai1: homing ? 1 : 0, ai2: target.whoAmI);
                                    Projectile.NewProjectileDirect(n.GetSource_FromAI(), n.Center, n.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(90 - 50)) * 20, type, npc.damage, 1, ai0: 25, ai1: homing ? 1 : 0, ai2: target.whoAmI);
                                    SoundEngine.PlaySound(SoundID.Item17, npc.Center);
                                }
                                
                            }
                        }
                        if (SpikeTimer >= time)
                        {
                            SpikeTimer = 0;
                            AttackPart++;
                        }
                    }
                    //Follow the targetPos variable
                    void Follow(float maxSpeed, float turnSpeed, float acceleration = 1.05f) {
                        

                        //speed up if too slow and vice versa
                        if (npc.velocity.Length() < maxSpeed)
                        {
                            npc.velocity *= acceleration;
                        }
                        else
                        {
                            npc.velocity /= acceleration;
                        }
                        angleDiff = MathHelper.ToDegrees(FargoSoulsUtil.RotationDifference(npc.velocity, npc.AngleTo(targetPos).ToRotationVector2()));
                        //turning to face the player
                        if (Math.Abs(angleDiff) > 0.5f )
                        {

                            
                            npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(angleDiff > 0 ? turnSpeed : -turnSpeed));

                        }
                    }
                    //Charge

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
