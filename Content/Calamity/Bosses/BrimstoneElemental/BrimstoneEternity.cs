using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common.Systems;
using FargowiltasSouls;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimstoneEternity : EModeCalBehaviour
    {
        public const bool Enabled = false;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>());
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!npc.HasValidTarget || !DLCCalamityConfig.Instance.EternityPriorityOverRev || !DLCWorldSavingSystem.EternityRev)
                return base.PreDraw(npc, spriteBatch, screenPos, drawColor);

            //drawing the aura
            Asset<Texture2D> aura = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/SoftEdgeRing");
            Color auraColor = Color.Red;
            auraColor.A = 0;
            spriteBatch.Draw(aura.Value, auraPos - Main.screenPosition, null, auraColor, 0, aura.Size() / 2, 4, SpriteEffects.None, 1);

            
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        //offset is for movement targeting
        //auraPos is where the aura is drawn (center of the circle on the pos)
        public Vector2 offset = new Vector2(0, 0);
        public Vector2 auraPos = new Vector2(0, 0);
        public int phase = 0;
        //ai[0] is animation (done by base cal, cant be changed)
        //ai[1] is attack/phase or whatever
        //ai[2] is a timer
        //ai[3] is whatever else
        
        public override bool SafePreAI(NPC npc)
        {

            if (!npc.HasValidTarget || !DLCCalamityConfig.Instance.EternityPriorityOverRev || !DLCWorldSavingSystem.EternityRev) 
                return true;
            //return true;
            

            //useful values
            Player target = Main.player[npc.target];
            Vector2 totarget = (target.Center - npc.Center).SafeNormalize(Vector2.Zero);
            Vector2 eyePos = npc.Center + new Vector2(10 * npc.spriteDirection, -60);
            Vector2 toTargetfromEye = (target.Center - eyePos).SafeNormalize(Vector2.Zero);
            //main attack is the int of the value, decimals on ai[1] can be used for extra data
            int mainAttack = (int)npc.ai[1];
            ref float attack = ref npc.ai[1];
            ref float timer = ref npc.ai[2];
            ref float data = ref npc.ai[3];
            int predeterminedSmolAttack = -1;
            //animation of 1 to be converted to angry moving later in code
            //animation of 2 to be converted to cocoon later in code
            //anything else converted to normal moving
            int animation = 0;
            //same logic as base cals' brimstone elemental frame logic, allowing to easily check which animation its on later in code
            if (npc.ai[0] == 3 || npc.ai[0] == 5) animation = 1;
            else if (npc.ai[0] > 2) animation = 2;

            int laserProj = -1;
            //set this to prevent the once per second simple attacks from happening this frame
            bool dontBasicAttack = false;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BrimBeam>() && Main.projectile[i].ai[0] == npc.whoAmI)
                {
                    dontBasicAttack = true;
                    laserProj = i;
                }
            }
            //timer
            //remember that because this is at the start timer will never be 0, lowest is 1
            timer++;

            //make sure the aura doesnt come from space on spawn (default position is 0, 0)
            if (auraPos == Vector2.Zero)
                auraPos = npc.Center;
            //move aura towards brimstone ele
            auraPos = Vector2.Lerp(auraPos, npc.Center, 0.03f);

            //debuff if too far away
            if (target.Distance(auraPos) > 220 * 4)
            {
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 2);
                target.AddBuff(BuffID.Obstructed, 2);
            }



            //face the player
            if (laserProj < 0)
            {
                if (npc.Center.X > target.Center.X)
                    npc.spriteDirection = -1;
                else
                    npc.spriteDirection = 1;
            }

            //move towards the target position (close to edge of screen), occasionally change y offset, occasionally teleport
            if (mainAttack == 0)
            {
                animation = 0;
                if (attack > 1.4f) animation = 1;
                //change random Y offset every 3 seconds
                if (timer % 180 == 0 && laserProj < 0)
                {

                    offset = new Vector2(Main.rand.Next(900, 900), Main.rand.Next(-200, 200));


                    NetSync(npc);
                    
                }

                int lasertime = phase == 2 ? 1160 : 800; 
                if (timer == lasertime && npc.GetLifePercent() < 0.67)
                {
                    predeterminedSmolAttack = 5;
                }
                offset.X = Math.Abs(offset.X) * -npc.spriteDirection;

                //summon the teleport telegraph every 3 moves
                if (timer % 540 == 0)
                {
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneTeleport>(), 0, 0, ai0: npc.whoAmI, ai1: target.whoAmI, ai2: npc.spriteDirection);
                    
                }
                
                

                //dust, sound, and reverse target direction right before the telegraph teleports the boss (actual teleport is done by the telegraph projectile)
                if ((timer - 199) % 540 == 0 && (timer - 199) > 0)
                {
                    if (laserProj >= 0)
                    {
                        Main.projectile[laserProj].rotation *= -1;
                        offset.Y *= -1;
                    }
                    for (int i = 0; i < 150; i++)
                    {
                        Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.LifeDrain, Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, 7), Scale: 2).noGravity = true;
                    }
                    offset.X *= -1;
                    SoundEngine.PlaySound(SoundID.Item109, target.Center);
                    //increment attack by a bit every teleport until moved into next attack, reset timer when that happens
                    attack += 0.4f;
                    if (attack >= 1)
                    {
                        attack = 1;
                        timer = 0;
                    }
                }
                //moving to target position
                if (npc.ai[0] != 4 && npc.ai[3] == 0)
                {
                    Vector2 topos = (target.Center + offset - npc.Center).SafeNormalize(Vector2.Zero);
                    Vector2 targVel = topos * npc.Distance(target.Center + offset) / 30;
                    float maxVel = 20;
                    if (laserProj >= 0)
                    {
                        maxVel = 3;
                    }
                    if (targVel.Length() > maxVel)
                        targVel = targVel.SafeNormalize(Vector2.Zero) * maxVel;
                    npc.velocity = Vector2.Lerp(npc.velocity, targVel, 0.05f);

                    
                }
            }
            //cocoon phase
            if (mainAttack == 1)
            {
                //summon brimlings at the start
                if (timer == 1)
                {
                    npc.defense = 50;

                    NPC.NewNPCDirect(npc.GetSource_FromAI(), npc.Center, ModContent.NPCType<Brimling>(), 0, npc.whoAmI, 0, Main.rand.Next(0, 360), 2);
                    NPC.NewNPCDirect(npc.GetSource_FromAI(), npc.Center, ModContent.NPCType<Brimling>(), 0, npc.whoAmI, 0, Main.rand.Next(0, 360) + 0.4f, 3.5f);

                    if (npc.GetLifePercent() < 0.67f)
                    {
                        NPC.NewNPCDirect(npc.GetSource_FromAI(), npc.Center, ModContent.NPCType<Brimling>(), 0, npc.whoAmI, 1, 0);
                        NPC.NewNPCDirect(npc.GetSource_FromAI(), npc.Center, ModContent.NPCType<Brimling>(), 0, npc.whoAmI, 1, 50);
                    }
                    else
                    {
                        NPC.NewNPCDirect(npc.GetSource_FromAI(), npc.Center, ModContent.NPCType<Brimling>(), 0, npc.whoAmI, 0, Main.rand.Next(0, 360) + 0.2f, 3);
                    }
                }
                animation = 2;
                npc.velocity *= 0.1f;

                //timer gone enough and hasnt gone into rock cover phase, up stuff for ending of cocoon phase, and spawn rocks
                if (timer >= 1000 && attack == 1)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.Next(7, 10), 0).RotatedBy(Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<BrimstoneDebris>(), npc.damage, 0);
                    }
                    attack += 0.5f;
                    timer = 1;
                    npc.defense = 15;

                }
                //if in ending of cocoon phase, shoot a tight ring of darts everywhere after a couple seconds, then 20 ticks later end cocoon phase (return to moving phase)
                if (attack > 1)
                {
                    dontBasicAttack = true;
                    animation = 1;
                    if (timer == 140)
                    {
                        SoundEngine.PlaySound(SoundID.Item20, eyePos);
                        for (int i = 0; i < 60; i++)
                        {
                            Projectile.NewProjectileDirect(npc.GetSource_FromAI(), eyePos, toTargetfromEye.RotatedBy(MathHelper.ToRadians(360 / 60f * i)) * 6, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                        }
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai1: 10);
                    }
                    if (timer == 180)
                    {
                        timer = 0;
                        attack = 0;
                    }
                }
            }

            //phase transition 1
            if (npc.GetLifePercent() < 0.67f && phase == 0 && mainAttack == 0 && timer > 100)
            {
                attack = 2;
                timer = 0;
                data = 0;
                phase = 1;
            }
            if (mainAttack == 2)
            {
                animation = 0;
                npc.velocity *= 0.97f;
                if (timer == 100)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath39, npc.Center);
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai1: 5);
                }
                if (timer >= 140)
                {
                    timer = 0;
                    attack = 0;
                }
                return false;
            }

            //phase transition 2
            if (npc.GetLifePercent() < 0.33f && phase == 1 && mainAttack == 0 && timer > 100)
            {
                attack = 3;
                timer = 0;
                data = 0;
                phase = 2;
            }
            if (mainAttack == 3)
            {
                animation = 0;
                npc.velocity *= 0.97f;
                if (timer == 100)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath39, npc.Center);
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai1: 5);
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 chainPos = npc.Center + new Vector2(800, 0).RotatedBy(MathHelper.ToRadians(360f / 6 * i + Main.rand.Next(-10, 10)));
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneChainExplosion>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: chainPos.X, ai1: chainPos.Y);
                    }
                }
                if (timer >= 140)
                {
                    timer = 0;
                    attack = 0;
                }
                return false;
            }


            //choose an attack every 80 ticks, or 60 ticks if angry animation
            int smolAttack = 0;
            if ((animation == 2 && timer % 60 == 0) || (animation != 2 && timer % 80 == 0) && !dontBasicAttack)
            {
                
                smolAttack = Main.rand.Next(1, 4);
                //switch from 1 to 4 if in phase 3
                if (phase == 2 && smolAttack == 1) smolAttack = 4;
                //reroll if attack 1 is chosen and in phase 2 or greater
                if (phase > 0 && smolAttack == 1) smolAttack = Main.rand.Next(1, 4);

            }
            if (predeterminedSmolAttack > 0 && !dontBasicAttack) smolAttack = predeterminedSmolAttack;

            //shoot darts in a line
            //cocoon : shoot darts in even circle
            if (smolAttack == 1)
            {
                if (animation == 2)
                {
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    for (int i = 0; i < 16; i++)
                    {
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, totarget.RotatedBy(MathHelper.ToRadians(360/16 * i)) * 4, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai1: 10);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), eyePos, toTargetfromEye * (i / 2f + 3), ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                }
            }
            //shoot a spread of darts
            //cocoon : shoot darts in random directions
            if (smolAttack == 2)
            {
                if (animation == 2)
                {
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    for (int i = 0; i < 14; i++)
                    {
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, totarget.RotatedBy(Main.rand.NextFloat(0, MathHelper.TwoPi)) * 4, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai1: 10);
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), eyePos, toTargetfromEye.RotatedBy(MathHelper.ToRadians((i - 1) * 10 - 5f)) * 4, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                }
            }
            //shoot 2 inferno fork things that explode shortly
            //cocoon : fires a few inferno fork things in random directions that explode shortly
            if (smolAttack == 3)
            {
                if (animation == 2)
                {
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    for (int i = 0; i < 7; i++)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, totarget.RotatedBy(Main.rand.NextFloat(0, MathHelper.TwoPi)) * 5, ModContent.ProjectileType<BrimstoneHellfireball>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                        proj.timeLeft = Main.rand.Next(80, 100);
                        proj.netUpdate = true;
                    }
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai1: 10);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), eyePos, toTargetfromEye.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-50, 50))) * 10, ModContent.ProjectileType<BrimstoneHellfireball>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                        proj.timeLeft = 35;
                        proj.netUpdate = true;
                    }
                }
            }
            //shoot the scal small homing thing that explodes into darts
            //cocoon : 3 waves of evenly spread darts at different velocities
            if (smolAttack == 4)
            {
                if (animation == 2)
                {
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    for (int i = 0; i < 10; i++)
                    {
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, totarget.RotatedBy(MathHelper.ToRadians(360 / 10 * i)) * 6, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, totarget.RotatedBy(MathHelper.ToRadians(360 / 12 * i)) * 4, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, totarget.RotatedBy(MathHelper.ToRadians(360 / 10 * i + 18)) * 2, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai1: 10);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), eyePos, toTargetfromEye * 4, ModContent.ProjectileType<SCalBrimstoneFireblast>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
            }
            //laser
            if (smolAttack == 5)
            {
                int side = Main.rand.NextBool() ? 1 : -1;
                float rotation = MathHelper.PiOver2 * (npc.Center.X > target.Center.X ? 1 : -1) + MathHelper.ToRadians(60 * -side);
                Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BrimBeam>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, -1, npc.whoAmI, side, rotation);
                int guh = target.Center.X > npc.Center.X ? 1 : -1;
                if (side == guh)
                {
                    offset.Y = -150;
                }
                else
                {
                    offset.Y = 150;
                }
            }

            //set the actual animation stuff with the animation value
            if (animation == 1) npc.ai[0] = 3;
            else if (animation == 2) npc.ai[0] = 4;
            else npc.ai[0] = 0;

            return false;
        }

    }
}
