
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimstoneEternity : CalDLCEmodeBehavior
    {
        public const bool Enabled = true;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;
        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 1f);
            //Main.NewText(NPC.defense);
            NPC.damage = WorldSavingSystem.MasochistModeReal ? 80 : 65;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.HasValidTarget || !CalDLCConfig.Instance.EternityPriorityOverRev || !CalDLCWorldSavingSystem.EternityRev)
                return true;

            //drawing the aura
            /*
            Asset<Texture2D> aura = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/SoftEdgeRing");
            Color auraColor = Color.Red;
            auraColor.A = 0;
            spriteBatch.Draw(aura.Value, auraPos - Main.screenPosition, null, auraColor * auraOpacity, 0, aura.Size() / 2, 4, SpriteEffects.None, 1);
            */

            return true;
        }
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.WriteVector2(offset);
            binaryWriter.WriteVector2(auraPos);
            binaryWriter.Write(auraOpacity);
            binaryWriter.Write7BitEncodedInt(phase);
            binaryWriter.Write7BitEncodedInt(smolAttack);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            offset = binaryReader.ReadVector2();
            auraPos = binaryReader.ReadVector2();
            auraOpacity = binaryReader.ReadSingle();
            phase = binaryReader.Read7BitEncodedInt();
            smolAttack = binaryReader.Read7BitEncodedInt();
        }
        //offset is for movement targeting
        //auraPos is where the aura is drawn (center of the circle on the pos)
        public Vector2 offset = new Vector2(0, 0);
        public Vector2 auraPos = new Vector2(0, 0);
        public float auraOpacity = 0;
        public int phase = 0;
        public int smolAttack = 0;
        //ai[0] is animation (done by base cal, cant be changed)
        //ai[1] is attack/phase or whatever
        //ai[2] is a timer
        //ai[3] is whatever else

        bool extraAI = false;
        public override bool PreAI()
        {

            if (!NPC.HasValidTarget || !CalDLCConfig.Instance.EternityPriorityOverRev || !CalDLCWorldSavingSystem.EternityRev) 
                return true;
            //return true;


            NPC.damage = NPC.defDamage;
            NPC.dontTakeDamage = false;
            NPC.Opacity = 1;
            NPC.chaseable = true;
            CalamityGlobalNPC.brimstoneElemental = NPC.whoAmI;

            //useful values
            Player target = Main.player[NPC.target];

            if (!target.ZoneUnderworldHeight && !extraAI)
            {
                extraAI = true;
                PreAI();
            }
            extraAI = false;

            Vector2 totarget = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            Vector2 eyePos = NPC.Center + new Vector2(10 * NPC.spriteDirection, -60);
            Vector2 toTargetfromEye = (target.Center - eyePos).SafeNormalize(Vector2.Zero);
            //main attack is the int of the value, decimals on ai[1] can be used for extra data
            int mainAttack = (int)NPC.ai[1];
            ref float attack = ref NPC.ai[1];
            ref float timer = ref NPC.ai[2];
            ref float data = ref NPC.ai[3];
            int predeterminedSmolAttack = -1;
            //animation of 1 to be converted to angry moving later in code
            //animation of 2 to be converted to cocoon later in code
            //anything else converted to normal moving
            int animation = 0;
            //same logic as base cals' brimstone elemental frame logic, allowing to easily check which animation its on later in code
            if (NPC.ai[0] == 3 || NPC.ai[0] == 5) animation = 1;
            else if (NPC.ai[0] > 2) animation = 2;

            int laserProj = -1;
            //set this to prevent the once per second simple attacks from happening this frame
            bool dontBasicAttack = false;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BrimBeam>() && Main.projectile[i].ai[0] == NPC.whoAmI)
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
                auraPos = NPC.Center;
            //move aura towards brimstone ele
            auraPos = Vector2.Lerp(auraPos, NPC.Center, 0.03f);

            //debuff if too far away
            if (Main.LocalPlayer.Distance(auraPos) > 220 * 4 && auraOpacity >= 1)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 2);
                //target.AddBuff(BuffID.Obstructed, 2);
            }



            //face the player
            if (laserProj < 0)
            {
                if (NPC.Center.X > target.Center.X)
                    NPC.spriteDirection = -1;
                else
                    NPC.spriteDirection = 1;
            }
            if (mainAttack == 0)
            {
                NPC.defense = 15;
                NPC.Opacity = 0;
                NPC.dontTakeDamage = true;
                NPC.velocity *= 0;
                if (timer == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item109, target.Center);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), target.Center + new Vector2(0, -300), Vector2.Zero, ModContent.ProjectileType<BrimstoneTeleport>(), 0, 0, ai0: NPC.whoAmI, ai1: target.whoAmI, ai2: 0);
                    NPC.netUpdate = true;
                }
                if (timer == 201)
                {
                    SoundEngine.PlaySound(SoundID.Item109, target.Center);
                    attack = 1;
                    timer = 0;
                    auraPos = NPC.Center;
                    NPC.netUpdate = true;
                    if (DLCUtils.HostCheck)
                    {
                        offset = new Vector2(Main.rand.Next(900, 900), Main.rand.Next(-200, 200));
                        NetSync(NPC);
                    }
                }
                return false;
            }
            //move towards the target position (close to edge of screen), occasionally change y offset, occasionally teleport
            if (mainAttack == 1)
            {
                const int teleportTime = 540;
                NPC.defense = 15;
                if (auraOpacity < 1) 
                    auraOpacity += 0.02f;
                animation = 0;
                if (attack > 2.4f) animation = 1;
                //change random Y offset every 3 seconds
                if (timer % 180 == 0 && laserProj < 0 && DLCUtils.HostCheck)
                {

                    offset = new Vector2(Main.rand.Next(950, 950), Main.rand.Next(-200, 200));
                    

                    NetSync(NPC);
                    
                }
                //be in a decent position for laser attack
                if (timer == 740 && phase > 0 && DLCUtils.HostCheck)
                {
                    offset.Y = Main.rand.NextBool() ? 200 : -200;
                    NetSync(NPC);
                }
                int lasertime = 800; 
                if (timer == lasertime && NPC.GetLifePercent() < 0.67)
                {
                    predeterminedSmolAttack = 5;
                }
                offset.X = Math.Abs(offset.X) * -NPC.spriteDirection;

                //summon the teleport telegraph every 3 moves
                if (timer % teleportTime == 0 && DLCUtils.HostCheck)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneTeleport>(), 0, 0, ai0: NPC.whoAmI, ai1: target.whoAmI, ai2: NPC.spriteDirection);
                    
                }
                //phase 3 attack where it shoots in a sweeping motion up and down
                if (timer % 10 == 0 && phase == 2 && timer < lasertime-60)
                {
                    int angle = 60;
                    int increment = 10;
                    if (data >= angle) data += 0.1f;
                    if (data <= -angle) data = -angle;
                    if (data == (int)data) data += increment;
                    else data -= increment;
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    if (DLCUtils.HostCheck)
                    {
                        NetSync(NPC);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), eyePos, (toTargetfromEye * 4).RotatedBy(MathHelper.ToRadians((int)data)), ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }
                    dontBasicAttack = true;
                    if ((timer-20) % 60 == 0 && WorldSavingSystem.MasochistModeReal)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);
                        if (DLCUtils.HostCheck)
                        {
                            for (int i = -2; i < 3; i++)
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), eyePos, (toTargetfromEye * 3).RotatedBy(MathHelper.ToRadians(i * 5)), ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                            NetSync(NPC);
                        }
                    }
                }
                //phase 3 attack where it does walls with a gap
                if (phase == 2 && !dontBasicAttack && timer > lasertime + 60)
                {
                    
                    if (timer % 120 == 0)
                    {
                        if (data == 0 && DLCUtils.HostCheck)
                        {
                            data = Main.rand.Next(-40, 40);
                            NetSync(NPC);
                        }
                        else
                        {
                            data = -data;
                            for (int i = 0; i < 14; i++)
                            {
                                int angle = i * 10 - 70;
                                if (DLCUtils.HostCheck)
                                {
                                    if (angle < data * NPC.spriteDirection - 10 || angle > data * NPC.spriteDirection + 10)
                                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), eyePos, (toTargetfromEye * 3).RotatedBy(MathHelper.ToRadians(angle)), ModContent.ProjectileType<BrimstoneHellfireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                                    
                                }
                            }
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);
                        }
                    }
                    if ((timer+20) % 120 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item20, eyePos);
                        if (DLCUtils.HostCheck)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), eyePos, (toTargetfromEye * 8), ModContent.ProjectileType<BrimstoneFireblast>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                            
                        }
                    }
                    
                    dontBasicAttack = true;
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
                        Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.LifeDrain, Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, 7), Scale: 2).noGravity = true;
                    }
                    offset.X *= -1;
                    SoundEngine.PlaySound(SoundID.Item109, target.Center);
                    //increment attack by a bit every teleport until moved into next attack, reset timer when that happens
                    attack += 0.4f;
                    if (attack >= 2)
                    {
                        attack = 2;
                        timer = 0;
                        data = 0;
                    }
                }
                //moving to target position
                if (NPC.ai[0] != 4)
                {
                    Vector2 topos = (target.Center + offset - NPC.Center).SafeNormalize(Vector2.Zero);
                    float baseSpeed = 0.2f;
                    Vector2 targVel = topos * NPC.Distance(target.Center + offset) / 30;
                    float maxVel = 20;
                    float progress = (timer % teleportTime) / teleportTime; // ratio of time progressed to teleport
                    maxVel *= progress * 0.99f + 0.01f;
                    maxVel += baseSpeed;
                    if (laserProj >= 0)
                    {
                        maxVel = 3;
                    }


                    if (targVel.Length() > maxVel)
                        targVel = targVel.SafeNormalize(Vector2.Zero) * maxVel;

                    NPC.velocity = Vector2.Lerp(NPC.velocity, targVel, 0.05f);

                    
                }
            }
            //cocoon phase
            if (mainAttack == 2)
            {
                const int cocoonTime = 1000;
                int rocks = phase >= 2 ? 3 : 2;
                int rockModulo = cocoonTime / rocks;
                float rockTimer = timer % rockModulo;
                //summon brimlings at the start
                if (timer == 1)
                {
                    NPC.defense = 50;
                    if (DLCUtils.HostCheck)
                    {
                        NPC minion1 = NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<Brimling>(), 0, NPC.whoAmI, 0, Main.rand.Next(0, 360), 2);
                        NPC minion2 = NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<Brimling>(), 0, NPC.whoAmI, 0, Main.rand.Next(0, 360) + 0.4f, 3.5f);
                        NetSync(minion1);
                        NetSync(minion2);
                        if (NPC.GetLifePercent() < 0.67f)
                        {
                            NPC minion3 = NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<Brimling>(), 0, NPC.whoAmI, 1, 0);
                            NPC minion4 = NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<Brimling>(), 0, NPC.whoAmI, 1, 50);
                            NetSync(minion3);
                            NetSync(minion4);
                        }
                        else
                        {
                            NPC minion5 = NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<Brimling>(), 0, NPC.whoAmI, 0, Main.rand.Next(0, 360) + 0.2f, 3);
                            NetSync(minion5);
                        }
                        for (int i = 0; i < Main.projectile.Length; i++)
                        {
                            if (Main.projectile[i].type == ModContent.ProjectileType<BrimstoneBarrage>() || Main.projectile[i].type == ModContent.ProjectileType<BrimstoneFireblast>())
                            {
                                Main.projectile[i].Kill();
                            }
                        }
                    }
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbilitySounds/BloodflareRangerActivation"), NPC.Center);
                }
                //ROCK STUFF
                if (timer < 60)
                {
                    animation = 1;
                    Vector2 topos = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    Vector2 targVel = topos * NPC.Distance(target.Center) / 30;
                    float maxVel = 20;
                    if (laserProj >= 0)
                    {
                        maxVel = 3;
                    }
                    if (targVel.Length() > maxVel)
                        targVel = targVel.SafeNormalize(Vector2.Zero) * maxVel;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targVel, 0.05f);
                    
                }
                if (rockTimer < 220)
                {
                    dontBasicAttack = true;
                }
                if (rockTimer == 60)
                {
                    animation = 2;
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            Vector2 vel = new Vector2(Main.rand.Next(12, 15), 0).RotatedBy(MathHelper.ToRadians(360f / 7 * i + Main.rand.NextFloat(-15, 15)));
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneDebris>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, -1, vel.X, vel.Y);
                        }
                    }
                }
                if (rockTimer == 160)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 60; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), eyePos, toTargetfromEye.RotatedBy(MathHelper.ToRadians(360 / 60f * i)) * 6, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai1: 10);
                    }
                }
                if (rockTimer > 60)
                {
                    animation = 2;
                    NPC.velocity *= 0.7f;
                }
                
                
                
                /*
                if (phase == 2 && timer >= 480 && timer <= 640)
                {
                    dontBasicAttack = true;
                }
                if (phase == 2 && timer == 500 && DLCUtils.HostCheck)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Vector2 vel = new Vector2(Main.rand.Next(12, 15), 0).RotatedBy(MathHelper.ToRadians(360f / 7 * i + Main.rand.NextFloat(-30, 30)));
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneDebris>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, -1, vel.X, vel.Y);
                    }
                }
                if (phase == 2 && timer == 620)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 60; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), eyePos, toTargetfromEye.RotatedBy(MathHelper.ToRadians(360 / 60f * i)) * 6, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai1: 10);
                    }
                }
                */
                

                if (timer > cocoonTime - 60)
                {
                    dontBasicAttack = true;
                }
                if (timer >= cocoonTime)
                {
                    animation = 0;
                    timer = 0;
                    data = 0;
                    attack = 1;
                }
                
                
            }

            //phase transition 1
            if (NPC.GetLifePercent() < 0.67f && phase == 0 && mainAttack == 1 && timer > 100)
            {
                attack = 3;
                timer = 0;
                data = 0;
                phase = 1;
            }
            if (mainAttack == 3)
            {
                NPC.defense = 15;
                animation = 0;
                NPC.velocity *= 0.97f;
                if (timer == 100)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath39, NPC.Center);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai1: 5);
                }
                if (timer >= 140)
                {
                    timer = 0;
                    attack = 1;
                }
                return false;
            }

            //phase transition 2
            if (NPC.GetLifePercent() < 0.33f && phase == 1 && mainAttack == 1 && timer > 100)
            {
                attack = 4;
                timer = 0;
                data = 0;
                phase = 2;
            }
            if (mainAttack == 4)
            {
                animation = 0;
                NPC.velocity *= 0.97f;
                if (timer == 100)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath39, NPC.Center);
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbilitySounds/BloodflareRangerActivation"), NPC.Center);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai1: 5);
                    
                }
                if (timer >= 140)
                {
                    timer = 0;
                    attack = 1;
                }
                return false;
            }


            

            //shoot darts in a line
            //cocoon : shoot darts in even circle
            //also in phase 2 and 3 replaced with wall with a gap
            if (smolAttack == 1)
            {
                if (animation == 2)
                {
                    /*
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, totarget.RotatedBy(MathHelper.ToRadians(360 / 16 * i)) * 4, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai1: 10);
                    }
                    */
                }
                else if (phase == 0)
                {
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), eyePos, toTargetfromEye * (i / 2f + 3), ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        }
                    }
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        int safeangle = Main.rand.Next(-30, 30);
                        for (int i = 0; i < 14; i++)
                        {
                            int angle = i * 10 - 70;
                            if (angle < safeangle * NPC.spriteDirection - 10 || angle > safeangle * NPC.spriteDirection + 10)
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), eyePos, (toTargetfromEye * 3).RotatedBy(MathHelper.ToRadians(angle)), ModContent.ProjectileType<BrimstoneHellfireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        }
                    }
                }
            }
            //shoot a spread of darts
            //cocoon : shoot darts in random directions
            if (smolAttack == 2)
            {
                if (animation == 2)
                {
                    /*
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 14; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, totarget.RotatedBy(Main.rand.NextFloat(0, MathHelper.TwoPi)) * 4, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai1: 10);
                    }
                    */
                }
                else
                {
                    int start = WorldSavingSystem.MasochistModeReal ? -3 : 0;
                    int end = WorldSavingSystem.MasochistModeReal ? 7 : 4;
                    if (DLCUtils.HostCheck)
                        for (int i = start; i < end; i++)
                        {
                            float rotation = MathHelper.ToRadians((i - 1) * 10 - 5f);
                            if (WorldSavingSystem.MasochistModeReal)
                            {
                                if (i < 0)
                                    rotation = MathHelper.ToRadians((0 - 1) * 10 - 5f) + MathF.Tau * 0.078f * i;
                                else if (i >= 4)
                                    rotation = MathHelper.ToRadians((3 - 1) * 10 - 5f) + MathF.Tau * 0.078f * (i - 3);
                            }
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), eyePos, toTargetfromEye.RotatedBy(rotation) * 4, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
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
                    /*
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, totarget.RotatedBy(Main.rand.NextFloat(0, MathHelper.TwoPi)) * 5, ModContent.ProjectileType<BrimstoneHellfireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                            proj.timeLeft = Main.rand.Next(80, 100);
                            proj.netUpdate = true;
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai1: 10);
                    }
                    */
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), eyePos, toTargetfromEye.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-50, 50))) * 10 * 0.5f, ModContent.ProjectileType<BrimstoneHellfireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                            proj.timeLeft = 35;
                            proj.netUpdate = true;
                        }
                    }
                }
            }
            //shoot the scal small homing thing that explodes into darts
            //cocoon : 3 waves of evenly spread darts at different velocities
            if (smolAttack == 4)
            {
                if (animation == 2)
                {
                    /*
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, totarget.RotatedBy(MathHelper.ToRadians(360 / 10 * i)) * 6, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        }
                        for (int i = 0; i < 12; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, totarget.RotatedBy(MathHelper.ToRadians(360 / 12 * i)) * 4, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, totarget.RotatedBy(MathHelper.ToRadians(360 / 10 * i + 18)) * 2, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai1: 10);
                    }
                    */
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item20, eyePos);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), eyePos, toTargetfromEye * 4, ModContent.ProjectileType<BrimstoneFireblast>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                }
            }
            //laser
            if (smolAttack == 5)
            {
                if (DLCUtils.HostCheck)
                {
                    int side = ((offset.Y > 0 && target.Center.X > NPC.Center.X) || (offset.Y < 0 && target.Center.X < NPC.Center.X)) ? -1 : 1;

                    float rotation = MathHelper.PiOver2 * (NPC.Center.X > target.Center.X ? 1 : -1) + MathHelper.ToRadians(60 * -side);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimBeam>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, -1, NPC.whoAmI, side, rotation);
                }
            }
            if (smolAttack != 0) smolAttack = 0;
            //choose an attack every 80 ticks, or 60 ticks if angry animation
            if (((animation == 2 && timer % 60 == 0) || (animation != 2 && timer % 80 == 0)) && !dontBasicAttack && DLCUtils.HostCheck && smolAttack == 0)
            {

                smolAttack = Main.rand.Next(1, 4);
                NetSync(NPC);
            }
            if (predeterminedSmolAttack > 0 && !dontBasicAttack) smolAttack = predeterminedSmolAttack;

            //set the actual animation stuff with the animation value
            if (animation == 1) NPC.ai[0] = 3;
            else if (animation == 2) NPC.ai[0] = 4;
            else NPC.ai[0] = 0;

            return false;
        }

    }
}
