
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.CalClone;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Patreon.DanielTheRobot;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CataclysmEternity : CalamitasBrothersEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.CalClone.Cataclysm>();
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CatastropheEternity : CalamitasBrothersEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.CalClone.Catastrophe>();
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public abstract class CalamitasBrothersEternity : CalDLCEmodeBehavior
    {
        #region Fields
        public Player Target => Main.player[NPC.target];
        public static float Acceleration => 0.7f;
        public static float MaxMovementSpeed => 30f;

        public bool PhaseTwo = false;
        public ref float Timer => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];
        public ref float AI2 => ref NPC.ai[2];
        public ref float AI3 => ref NPC.ai[3];
        public ref float CustomRotation => ref NPC.localAI[0];
        public ref float PreviousState => ref NPC.localAI[1];
        public ref float StunTimer => ref NPC.localAI[2];
        public ref float ContactDamage => ref NPC.localAI[3];
        public const int CycleTime = 60 * 3;

        public bool Cataclysm => NPC.type == ModContent.NPCType<Cataclysm>();
        public float ForwardRotation => NPC.rotation + MathHelper.PiOver2;
        public NPC OtherBrother
        {
            get
            {
                if (Cataclysm)
                    return FargoSoulsUtil.NPCExists(CalamityGlobalNPC.catastrophe, ModContent.NPCType<Catastrophe>());
                else
                    return FargoSoulsUtil.NPCExists(CalamityGlobalNPC.cataclysm, ModContent.NPCType<Cataclysm>());
            }
        }

        public enum States
        {
            Intro,
            Dash,
            Flamethrower,
            Fireballs,
            SpinDashes,
            Stunned,
            Transition
        }
        public List<States> Attacks
        {
            get
            {
                List<States> attacks = [
                    States.Dash, 
                    States.Flamethrower, 
                    States.Fireballs
                    ];
                if (PhaseTwo)
                    attacks.Add(States.SpinDashes);
                return attacks;
            }
        }
        public List<int> AvailableStates = [];

        public static readonly SoundStyle StaggerSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/BrotherStagger") with { Volume = 0.5f };
        public static readonly SoundStyle DashSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/BrotherDash");
        #endregion
        public override bool IsLoadingEnabled(Mod mod) => CalamitasCloneEternity.Enabled;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            NPC.scale = 1f; // thanks cal

            NPC.defense = 15;
            NPC.DR_NERD(0.225f);
            int hp = 16000;
            NPC.LifeMaxNERB(hp, hp, 80000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
        }
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(PhaseTwo);
            for (int i = 0; i < NPC.localAI.Length; i++)
            {
                binaryWriter.Write(NPC.localAI[i]);
            }
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            PhaseTwo = binaryReader.ReadBoolean();
            for (int i = 0; i < NPC.localAI.Length; i++)
            {
                NPC.localAI[i] = binaryReader.ReadSingle();
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (ContactDamage == 1)
                return base.CanHitPlayer(target, ref cooldownSlot);
            return false;
        }
        public override bool PreAI()
        {
            #region Standard
            if (CalamityGlobalNPC.calamitas < 0 || !Main.npc[CalamityGlobalNPC.calamitas].active)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return false;
            }
            if (Cataclysm)
                CalamityGlobalNPC.cataclysm = NPC.whoAmI;
            else
                CalamityGlobalNPC.catastrophe = NPC.whoAmI;

            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1f, 0f, 0f);
            NPC.dontTakeDamage = false;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[1] != 0f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }

                    return false;
                }
            }
            #endregion

            #region Rotation
            if (CustomRotation == 0)
            {
                float calCloneBroPlayerXDist = NPC.position.X + (NPC.width / 2) - player.position.X - (player.width / 2);
                float calCloneBroPlayerYDist = NPC.position.Y + NPC.height - 59f - player.position.Y - (player.height / 2);
                float calCloneBroRotation = (float)Math.Atan2(calCloneBroPlayerYDist, calCloneBroPlayerXDist) + MathHelper.PiOver2;
                if (calCloneBroRotation < 0f)
                    calCloneBroRotation += MathHelper.TwoPi;
                else if (calCloneBroRotation > MathHelper.TwoPi)
                    calCloneBroRotation -= MathHelper.TwoPi;

                float calCloneBroRotationSpeed = 0.15f;
                if (NPC.rotation < calCloneBroRotation)
                {
                    if ((calCloneBroRotation - NPC.rotation) > MathHelper.Pi)
                        NPC.rotation -= calCloneBroRotationSpeed;
                    else
                        NPC.rotation += calCloneBroRotationSpeed;
                }
                else if (NPC.rotation > calCloneBroRotation)
                {
                    if ((NPC.rotation - calCloneBroRotation) > MathHelper.Pi)
                        NPC.rotation += calCloneBroRotationSpeed;
                    else
                        NPC.rotation -= calCloneBroRotationSpeed;
                }

                if (NPC.rotation > calCloneBroRotation - calCloneBroRotationSpeed && NPC.rotation < calCloneBroRotation + calCloneBroRotationSpeed)
                    NPC.rotation = calCloneBroRotation;
                if (NPC.rotation < 0f)
                    NPC.rotation += MathHelper.TwoPi;
                else if (NPC.rotation > MathHelper.TwoPi)
                    NPC.rotation -= MathHelper.TwoPi;
                if (NPC.rotation > calCloneBroRotation - calCloneBroRotationSpeed && NPC.rotation < calCloneBroRotation + calCloneBroRotationSpeed)
                    NPC.rotation = calCloneBroRotation;
            }
            CustomRotation = 0;
            ContactDamage = 0;
            #endregion

            if (!NPC.HasPlayerTarget) // just in case
                return false;
            switch ((States)State)
            {
                case States.Intro:
                    Intro();
                    break;
                case States.Dash:
                    Dash();
                    break;
                case States.Flamethrower:
                    Flamethrower();
                    break;
                case States.Fireballs:
                    Fireballs();
                    break;
                case States.SpinDashes:
                    SpinDashes();
                    break;
                case States.Stunned:
                    Stunned();
                    break;
                case States.Transition:
                    Transition();
                    break;
            }
            Timer++;
            if (State != (int)States.Intro && State != (int)States.Transition && !PhaseTwo && OtherBrother == null)
            {
                Reset();
                State = (int)States.Transition;
                NPC.netUpdate = true;
            }
            return false;
        }
        #region States
        public void Intro()
        {
            if (Timer == 1)
            {
                float rot = MathHelper.PiOver2 * (Cataclysm ? 1 : -1);
                NPC.velocity += NPC.DirectionTo(Target.Center).RotatedBy(rot) * 12;
                NPC.rotation = NPC.velocity.ToRotation();
            }
                
            if (Timer > 30)
            {
                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * 400;
                Movement(desiredPos, 1f);
            }
            int delay = Cataclysm ? CycleTime / 2 : 0;
            if (Timer >= 60 + delay)
            {
                GoToNeutral();
                return;
            }
        }
        #region Attacks
        public void Dash()
        {
            int windupTime = 80;
            int windbackTime = 20;
            int chargeTime = 40;
            if (PhaseTwo)
            {
                if (Timer < 30)
                    Timer = 30;
            }
            if (Timer == windupTime + (int)(windbackTime / 2))
            {
                if (DLCUtils.HostCheck)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + ForwardRotation.ToRotationVector2() * 40, Vector2.Zero, ModContent.ProjectileType<BrothersEyeFlash>(), 0, 0, Main.myPlayer, NPC.whoAmI);
            }
            if (Timer < windupTime)
            {
                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * 400;
                RotateTowards(NPC.DirectionTo(Target.Center), 40f);
                RepulseOtherBrother(ref desiredPos);
                Movement(desiredPos, 1f);
            }
            else if (Timer < windupTime + windbackTime)
            {
                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * 550;
                RotateTowards(NPC.DirectionTo(Target.Center), 40f);
                Movement(desiredPos, 0.75f);

                if (Timer == windupTime + windbackTime - 1)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GoblinBomberThrow, NPC.Center);
                }
                    
            }
            else if (Timer < windupTime + windbackTime + chargeTime)
            {
                if (Timer == windupTime + windbackTime + 4)
                {
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + BrothersDashTrail.Offset(NPC), Vector2.Zero, ModContent.ProjectileType<BrothersDashTrail>(), 0, 0, Main.myPlayer, NPC.whoAmI, chargeTime + 40);
                }
                if (Timer == windupTime + windbackTime + 12)
                {
                    SoundEngine.PlaySound(DashSound, NPC.Center);
                }

                NPC.velocity += NPC.DirectionTo(Target.Center) * 1.2f;
                Vector2 dir = NPC.velocity;
                if (Timer < windupTime + windbackTime + 8)
                {
                    dir = NPC.DirectionTo(Target.Center);

                }
                RotateTowards(dir, 40f);
                ContactDamage = 1;
            }
            else
            {
                if (Timer <= CycleTime - 10)
                {
                    Vector2 dir = NPC.velocity;
                    RotateTowards(dir, 40f);
                }
                ContactDamage = 1;
                NPC.velocity *= 0.93f;
            }
            if (Timer > windupTime + windbackTime + 20 && Timer < windupTime + windbackTime + chargeTime + 10)
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 dir = -NPC.velocity.SafeNormalize(Vector2.UnitY);
                    dir = dir.RotatedByRandom(MathHelper.PiOver2 * 0.2f);

                    Dust failShotDust = Dust.NewDustPerfect(NPC.Center, Main.rand.NextBool(3) ? 60 : 114);
                    failShotDust.noGravity = true;
                    failShotDust.velocity = dir * 38 * Main.rand.NextFloat(0.5f, 1.3f);
                    failShotDust.scale = Main.rand.NextFloat(0.9f, 1.8f);
                }
            }
            if (Timer >= CycleTime)
            {
                GoToNeutral();
            }
        }
        public int Flamethrower_WindupTime => 50;  
        public int Flamethrower_PullbackTime => WorldSavingSystem.MasochistModeReal ? 50 : 60;
        public int Flamethrower_SweepTime => (PhaseTwo ? 70 : 80) + (WorldSavingSystem.MasochistModeReal ? 0 : 10);
        public void Flamethrower()
        {
            ref float sweepDir = ref AI2;


            int firePreStartup = 30;
            int idealDistance = 280;

            void Flames(float speed = 10f)
            {
                ContactDamage = 1;
                if (Timer % 22 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item34 with { Volume = 5 }, NPC.Center);
                }
                if (Timer % 3 == 0)
                {
                    int type = ModContent.ProjectileType<BrimstoneFire>();
                    int damage = FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);
                    Vector2 dir = ForwardRotation.ToRotationVector2();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + dir * NPC.width / 3, dir * speed, type, damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }

            if (Timer < Flamethrower_WindupTime)
            {
                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * idealDistance;
                RepulseOtherBrother(ref desiredPos);
                // don't go too close to ground
                Vector2 floor = LumUtils.FindGround(desiredPos.ToTileCoordinates(), Vector2.UnitY).ToWorldCoordinates();
                if (Math.Abs(floor.Y - desiredPos.Y) < 500)
                    desiredPos.Y = floor.Y - 500;

                Movement(desiredPos, 1f);
            }
            else if (Timer < Flamethrower_WindupTime + Flamethrower_PullbackTime)
            {
                CustomRotation = 1;
                if (Timer == Flamethrower_WindupTime)
                {
                    var otherBrother = OtherBrother;
                    if (otherBrother != null && otherBrother.TryGetDLCBehavior(out CalamitasBrothersEternity brotherAI) && brotherAI.State == State && brotherAI.Timer > Timer)
                    {
                        sweepDir = brotherAI.AI2;
                    }
                    else
                    {
                        Vector2 predict = Target.Center + Target.velocity * 10;
                        sweepDir = -FargoSoulsUtil.RotationDifference(NPC.DirectionTo(predict), NPC.DirectionTo(Target.Center)).NonZeroSign();
                    }
                }

                // rotation
                float idealAngle = NPC.DirectionTo(Target.Center).ToRotation() + MathHelper.PiOver2 * 1.2f * sweepDir;
                NPC.rotation = NPC.rotation.ToRotationVector2().RotateTowards(idealAngle - MathHelper.PiOver2, 0.1f).ToRotation();

                // movement
                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * idealDistance;
                // don't go too close to ground
                Vector2 floor = LumUtils.FindGround(desiredPos.ToTileCoordinates(), Vector2.UnitY).ToWorldCoordinates();
                if (Math.Abs(floor.Y - desiredPos.Y) < 500)
                    desiredPos.Y = floor.Y - 500;
                Movement(desiredPos, 0.75f);

                float startFire = Flamethrower_WindupTime + Flamethrower_PullbackTime - firePreStartup;
                if (Timer > startFire)
                {
                    float speed = LumUtils.Saturate((Timer - startFire) / 25f);
                    speed *= 10;
                    speed = MathHelper.Clamp(speed, 2, 10);
                    Flames(speed);
                }
            }
            else if (Timer < Flamethrower_WindupTime + Flamethrower_PullbackTime + Flamethrower_SweepTime)
            {
                CustomRotation = 1;
                NPC.rotation -= sweepDir * MathHelper.Pi / Flamethrower_SweepTime;
                NPC.velocity *= 0.94f;

                float progress = (Timer - Flamethrower_WindupTime - Flamethrower_PullbackTime) / Flamethrower_SweepTime;
                Flames(10f + progress * 12f);
            }
            else
            {
                if (PhaseTwo)
                    CustomRotation = 1;
                NPC.velocity *= 0.93f;
            }
            int endTime = CycleTime;
            if (PhaseTwo)
                endTime += Flamethrower_SweepTime;
            if (Timer >= endTime)
            {
                GoToNeutral();
            }
        }
        public void Fireballs()
        {
            int startup = 60;
            // movement
            int idealDistance = 420;
            Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * idealDistance;
            RepulseOtherBrother(ref desiredPos);
            float movespeed = Timer < startup ? 1f : 0.45f;
            Movement(desiredPos, movespeed);
            if (NPC.Distance(Target.Center) < idealDistance * 0.8f)
                NPC.velocity += Target.DirectionTo(NPC.Center);

            if (Timer < startup)
            {
                // particle telegraph
                float progress = Timer / startup;
                float freq = 6f - 5f * progress;
                float rand = MathHelper.PiOver4 * 0.6f;
                Vector2 dir = (ForwardRotation + Main.rand.NextFloat(-rand, rand)).ToRotationVector2();
                if (Main.rand.NextBool((int)freq))
                {
                    Vector2 velocity = dir * 24;
                    PointParticle spark2 = new(NPC.Center + velocity, velocity * Main.rand.NextFloat(0.3f, 1f), false, 15, 1.25f, (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * 0.6f);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                if (Main.rand.NextBool((int)freq))
                {
                    Dust failShotDust = Dust.NewDustPerfect(NPC.Center, Main.rand.NextBool(3) ? 60 : 114);
                    failShotDust.noGravity = true;
                    failShotDust.velocity = dir * 28 * Main.rand.NextFloat(0.5f, 1.3f);
                    failShotDust.scale = Main.rand.NextFloat(0.9f, 1.8f);
                }
            }
            else
            {
                CustomRotation = 1;
                float diff = FargoSoulsUtil.RotationDifference(ForwardRotation.ToRotationVector2(), NPC.DirectionTo(Target.Center));
                NPC.rotation += diff.NonZeroSign() * Math.Min(Math.Abs(diff), MathHelper.Pi / 120f);

                int shotTimer = (int)Timer - startup;

                int shotCount = 3;
                int freq = 22;
                if (WorldSavingSystem.MasochistModeReal)
                {
                    shotCount = 4;
                    freq = 16;
                }
                if (shotTimer % freq == freq - 1 && shotTimer < freq * shotCount)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        float mult = (shotTimer + 1f) / freq;
                        mult -= 1;
                        mult /= shotCount - 1;
                        for (int i = -1; i <= 1; i += 2)
                        {
                            float rot = mult;
                            rot *= i;
                            rot *= MathHelper.PiOver2 * 0.4f;
                            Vector2 dir = (ForwardRotation + rot).ToRotationVector2();
                            float speed = 3.5f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + dir * NPC.width / 3, dir * speed, ModContent.ProjectileType<BrimstoneBall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f), 1f, Main.myPlayer);
                        }
                    }
                }
            }
            if (Timer >= CycleTime)
            {
                GoToNeutral();
            }
        }
        public void SpinDashes()
        {
            int windupTime = 40;
            int windbackTime = 15;
            int chargeTime = 40;
            int endTime = 3;
            int cycle = windupTime + windbackTime + chargeTime + endTime;
            float cycleTimer = Timer % cycle;

            int totalTime = cycle * 3;

            if (cycleTimer == windupTime + (int)(windbackTime / 2))
            {
                if (DLCUtils.HostCheck)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + ForwardRotation.ToRotationVector2() * 40, Vector2.Zero, ModContent.ProjectileType<BrothersEyeFlash>(), 0, 0, Main.myPlayer, NPC.whoAmI);
            }

            if (cycleTimer < windupTime)
            {
                if (cycleTimer == 1)
                {
                    AI2 = Target.DirectionTo(NPC.Center).ToRotation() + MathHelper.PiOver2 * Main.rand.NextFloat(0.4f, 0.9f) * (Main.rand.NextBool() ? 1 : -1);
                    NPC.netUpdate = true;
                }

                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * 400;
                RotateTowards(NPC.DirectionTo(Target.Center), 40f);
                if (cycleTimer > 4)
                {
                    desiredPos = Target.Center + AI2.ToRotationVector2() * 400;
                }
                RepulseOtherBrother(ref desiredPos);
                Movement(desiredPos, 2f);
            }
            else if (cycleTimer < windupTime + windbackTime)
            {
                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * 550;
                Movement(desiredPos, 1f);
                RotateTowards(NPC.DirectionTo(Target.Center), 40f);

                if (cycleTimer == windupTime + windbackTime - 1)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GoblinBomberThrow, NPC.Center);
                }
            }
            else if (cycleTimer < windupTime + windbackTime + chargeTime)
            {
                if (cycleTimer == windupTime + windbackTime + 4)
                {
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + BrothersDashTrail.Offset(NPC), Vector2.Zero, ModContent.ProjectileType<BrothersDashTrail>(), 0, 0, Main.myPlayer, NPC.whoAmI, chargeTime + 32);
                }
                if (cycleTimer == windupTime + windbackTime + 12)
                {
                    SoundEngine.PlaySound(DashSound, NPC.Center);
                }
                    

                NPC.velocity += NPC.DirectionTo(Target.Center) * 1.5f;
                Vector2 dir = NPC.velocity;
                if (cycleTimer < windupTime + windbackTime + 8)
                    dir = NPC.DirectionTo(Target.Center);
                RotateTowards(dir, 40f);
                CustomRotation = 1;
                float diff = FargoSoulsUtil.RotationDifference(ForwardRotation.ToRotationVector2(), NPC.velocity);
                NPC.rotation += diff.NonZeroSign() * Math.Min(Math.Abs(diff), MathHelper.Pi / 40f);
                ContactDamage = 1;
            }
            else
            {
                if (Timer <= totalTime - 10)
                {
                    Vector2 dir = NPC.velocity;
                    RotateTowards(dir, 40f);
                }
                NPC.velocity *= 0.93f;
                ContactDamage = 1;
            }
            if (cycleTimer > windupTime + windbackTime + 20 && cycleTimer < windupTime + windbackTime + chargeTime + 10)
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 dir = -NPC.velocity.SafeNormalize(Vector2.UnitY);
                    dir = dir.RotatedByRandom(MathHelper.PiOver2 * 0.2f);

                    Dust failShotDust = Dust.NewDustPerfect(NPC.Center, Main.rand.NextBool(3) ? 60 : 114);
                    failShotDust.noGravity = true;
                    failShotDust.velocity = dir * 38 * Main.rand.NextFloat(0.5f, 1.3f);
                    failShotDust.scale = Main.rand.NextFloat(0.9f, 1.8f);
                }
            }
            if (Timer >= totalTime)
            {
                GoToNeutral();
            }
        }
        #endregion
        public void Stunned()
        {
            float stunTime = 90;
            NPC.velocity *= 0.95f;

            if (StunTimer == 5)
            {
                NPC.netUpdate = true;
                SoundEngine.PlaySound(StaggerSound, NPC.Center);
                ScreenShakeSystem.StartShake(8f);
            }
            StunTimer++;
            if (StunTimer < 60)
                NPC.rotation += Main.rand.NextFloat(-1, 1) * MathHelper.PiOver4 * 0.75f * (1f - (StunTimer / 60f));
            else
            {
                float idealDistance = Math.Min(NPC.Distance(Target.Center), 600);
                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * idealDistance;
                RepulseOtherBrother(ref desiredPos);
                Movement(desiredPos, 0.4f);
            }
            if (StunTimer >= stunTime && (PhaseTwo || Timer % CycleTime == 0))
                GoToNeutral();
        }
        public void Transition()
        {
            int transTime = 80;
            int endTime = 30;
            if (Timer < transTime - 15)
            {
                int idealDistance = 400;
                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * idealDistance;
                Movement(desiredPos, 1f);
            }
            else
                NPC.velocity *= 0.92f;
            if (Timer < transTime)
                NPC.dontTakeDamage = true;
            if (Timer == transTime)
            {
                PhaseTwo = true;
                int lifeBonus = (int)(NPC.lifeMax * (2 / 3f));
                NPC.lifeMax += lifeBonus;
                NPC.life += lifeBonus;
                NPC.HealEffect(lifeBonus);
                NPC.netUpdate = true;

                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                ScreenShakeSystem.StartShake(20f);
                if (DLCUtils.HostCheck)
                {
                    float speed = 36;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CalcloneWave>(), 0, 0, Main.myPlayer, ai2: speed);

                    for (int i = 0; i < 6; i++)
                    {
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SoulSeeker>(), 0, NPC.whoAmI, i);
                    }
                }
            }
            if (Timer >= transTime + endTime)
                GoToNeutral();
        }
        #endregion

        #region Help Methods
        public void GoToNeutral()
        {
            Reset();
            if (PhaseTwo)
            {
                int index;
                if (AvailableStates.Count < 1)
                {
                    AvailableStates.Clear();
                    foreach (var attack in Attacks)
                        AvailableStates.Add((int)attack);
                    AvailableStates.Remove((int)PreviousState); //avoid possible repeat after refilling list
                }
                if (FargoSoulsUtil.HostCheck) //only run for host in mp, will sync to others
                {
                    index = Main.rand.Next(AvailableStates.Count);
                    State = AvailableStates[index];
                    AvailableStates.RemoveAt(index);
                }
                PreviousState = (int)State;
                NPC.netUpdate = true;
            }
            else
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    if (State != (int)States.Stunned)
                        PreviousState = State;
                    var attacks = Attacks;
                    attacks.Remove((States)PreviousState);
                    State = (int)Main.rand.NextFromCollection(attacks);
                    NPC.netUpdate = true;
                }
            }
        }
        public void Reset()
        {
            Timer = 0;
            AI2 = 0;
            AI3 = 0;
            StunTimer = 0;
            NPC.netUpdate = true;
        }
        public void RotateTowards(Vector2 angle, float divisor)
        {
            CustomRotation = 1;
            float diff = FargoSoulsUtil.RotationDifference(ForwardRotation.ToRotationVector2(), angle);
            NPC.rotation += diff.NonZeroSign() * Math.Min(Math.Abs(diff), MathHelper.Pi / divisor);
        }
        public void GetStunned()
        {
            if (WorldSavingSystem.MasochistModeReal)
                return;
            AI2 = 0;
            AI3 = 0;
            StunTimer = 0;
            State = (int)States.Stunned;
            NPC.netUpdate = true;
        }
        public void RepulseOtherBrother(ref Vector2 desiredPos)
        {
            NPC otherBrother = OtherBrother;
            if (otherBrother != null)
            {
                int minDistance = 400;
                if (desiredPos.Distance(otherBrother.Center) < minDistance)
                    desiredPos = otherBrother.Center + otherBrother.DirectionTo(desiredPos) * minDistance;
            }
        }
        public void Movement(Vector2 desiredPos, float speedMod)
        {
            speedMod *= 1.6f;
            float accel = Acceleration * speedMod;
            float decel = Acceleration * 2 * speedMod;
            float max = MaxMovementSpeed * speedMod;
            if (max > Target.velocity.Length() + MaxMovementSpeed)
                max = Target.velocity.Length() + MaxMovementSpeed;
            float resistance = NPC.velocity.Length() * accel / max;
            NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, desiredPos, NPC.velocity, accel - resistance, decel + resistance);
        }
        #endregion
    }
}
