using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
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
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.PirateInvasion;
using FargowiltasSouls.Content.Patreon.DanielTheRobot;
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
using static FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone.CalamitasBrothersEternity;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SoulSeekerEternity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<SoulSeeker>();
        #region Fields
        public Player Target => Main.player[Parent.target];
        public NPC Parent => Main.npc[Owner];
        public static float Acceleration => 3f;
        public static float MaxMovementSpeed => 30f;

        public int Owner
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        public int SeekerNumber
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        public ref float CustomRotation => ref NPC.ai[2];
        public ref float AI3 => ref NPC.ai[3];
        public ref float PreviousState => ref NPC.localAI[0];
        public const int TotalSeekers = 6;
        public float BaseOffset => Parent.width / 2 + 40;

        #endregion
        public override bool IsLoadingEnabled(Mod mod) => CalamitasCloneEternity.Enabled;
        public override void SetStaticDefaults()
        {
            
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.buffImmune[BuffID.Darkness] = true;
        }
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            for (int i = 0; i < NPC.localAI.Length; i++)
            {
                binaryWriter.Write(NPC.localAI[i]);
            }
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            for (int i = 0; i < NPC.localAI.Length; i++)
            {
                NPC.localAI[i] = binaryReader.ReadSingle();
            }
        }
        public override bool PreAI()
        {
            NPC.damage = 0;
            NPC.dontTakeDamage = true;
            if (Owner < 0 || !(Parent.TypeAlive<Cataclysm>() || Parent.TypeAlive<Catastrophe>()))
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return false;
            }
            NPC.position += Parent.velocity;
            if (CustomRotation == 0)
                NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.DirectionTo(Target.Center).ToRotation() + MathHelper.Pi, 0.08f);
            CustomRotation = 0;
            if (!Parent.HasPlayerTarget)
            {
                DefaultBehavior();
                return false;
            }

            var parentAI = Parent.GetDLCBehavior<CalamitasBrothersEternity>();
            if (PreviousState != parentAI.State)
            {
                PreviousState = parentAI.State;
                NPC.netUpdate = true;
            }
            switch ((States)parentAI.State)
            {
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
                default:
                    DefaultBehavior();
                    break;
            }

            return false;
        }
            #region States
        public void DefaultBehavior()
        {
            float offset = SeekerNumber * MathHelper.TwoPi / TotalSeekers;
            Vector2 desiredPos = Parent.Center + (Parent.rotation + offset).ToRotationVector2() * BaseOffset;
            Movement(desiredPos, 1f);
        }
        #region Attacks
        public void Dash()
        {
            var parentAI = Parent.GetDLCBehavior<CalamitasBrothersEternity>();
            int windupTime = 80;
            int windbackTime = 20;
            int chargeTime = 38;

            int i;
            if (SeekerNumber < 3)
                i = SeekerNumber + 1;
            else // 3 4 5 -> -1 -2 -3
                i = SeekerNumber - 6;
            int sign = Math.Sign(i);

            if (parentAI.Timer < 100)
            {
                AI3 = Parent.rotation;
                if (parentAI.Timer == 99)
                    NPC.netUpdate = true;
            }
            else if (parentAI.Timer < windupTime + windbackTime + chargeTime + 2)
            {
                int freq = 15;
                int offset = 5 * Math.Abs(i);
                if (parentAI.Timer % freq == offset - 1)
                {
                    if (DLCUtils.HostCheck)
                    {
                        for (int d = 0; d < 3; d++)
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);

                        int type = ModContent.ProjectileType<BrimstoneBarrage>();
                        int damage = FargoSoulsUtil.ScaledProjectileDamage(Parent.defDamage);
                        float speed = 6f;
                        Vector2 velocity = (NPC.rotation + MathHelper.Pi).ToRotationVector2() * speed;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, damage, 1f, Main.myPlayer, 1f, 0f, speed * 3f);
                    }
                }
            }


            Vector2 dir = AI3.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * (1 + sign * (1 + 0.37f * Math.Abs(i) - 0.7f)));
            Vector2 desiredPos = Parent.Center + dir * BaseOffset;
            CustomRotation = 1;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.DirectionTo(Parent.Center).ToRotation(), 0.2f);
            Movement(desiredPos, 1f);
        }
        public void Flamethrower()
        {
            var parentAI = Parent.GetDLCBehavior<CalamitasBrothersEternity>();


            int flameStart = parentAI.Flamethrower_WindupTime + parentAI.Flamethrower_PullbackTime + parentAI.Flamethrower_SweepTime;

            float sweepDir = -parentAI.AI2;
            float seekerDir = parentAI.ForwardRotation + (sweepDir * MathHelper.PiOver2 * 2.4f);

            Vector2 dir = seekerDir.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * 0.13f * (SeekerNumber - 2));

            if (sweepDir != 0)
            {
                if (parentAI.Timer >= flameStart && parentAI.Timer < flameStart + parentAI.Flamethrower_SweepTime)
                {
                    float progress = (parentAI.Timer - flameStart) / parentAI.Flamethrower_SweepTime;
                    dir = dir.RotatedBy(-MathHelper.Pi * 1.1f * sweepDir * progress);
                    if (SeekerNumber == 3) // flamethrower
                    {
                        float speed = 6f + progress * 8f;
                        float speedMod = LumUtils.Saturate((parentAI.Timer - flameStart) / 25f);
                        speed *= speedMod;
                        speed = MathHelper.Clamp(speed, 2, 100);
                        if (parentAI.Timer % 22 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item34 with { Volume = 5 }, NPC.Center);
                        }
                        if (parentAI.Timer % 3 == 0)
                        {
                            if (DLCUtils.HostCheck)
                            {
                                int type = ModContent.ProjectileType<BrimstoneFire>();
                                int damage = FargoSoulsUtil.ScaledProjectileDamage(Parent.defDamage);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + dir * NPC.width / 3, dir * speed, type, damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
                Vector2 desiredPos = Parent.Center + dir * BaseOffset;
                CustomRotation = 1;
                NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.DirectionTo(Parent.Center).ToRotation(), 0.2f);
                Movement(desiredPos, 3f);
            }
            else
            {
                float offset = SeekerNumber * MathHelper.TwoPi / TotalSeekers;
                Vector2 desiredPos = Parent.Center + offset.ToRotationVector2() * BaseOffset;
                Movement(desiredPos, 1f);
            }
        }
        public void Fireballs()
        {
            var parentAI = Parent.GetDLCBehavior<CalamitasBrothersEternity>();
            int i;
            if (SeekerNumber < 3)
                i = SeekerNumber + 1;
            else // 3 4 5 -> -1 -2 -3
                i = SeekerNumber - 6;
            int sign = Math.Sign(i);


            int telegraph = 50;
            int shotStart = telegraph + 12 * 5;
            if (parentAI.Timer < telegraph + shotStart)
            {
                if (parentAI.Timer < telegraph + shotStart - 15)
                    AI3 = Parent.rotation;

                if (parentAI.Timer > telegraph + shotStart / 2)
                {
                    // particle telegraph
                    float freq = 2f;
                    float rand = MathHelper.PiOver4 * 0.05f;
                    Vector2 particledir = (-NPC.rotation + Main.rand.NextFloat(-rand, rand)).ToRotationVector2();
                    if (Main.rand.NextBool((int)freq))
                    {
                        Vector2 velocity = particledir * 12;
                        velocity += Parent.velocity + NPC.velocity;
                        PointParticle spark2 = new(NPC.Center + velocity, velocity * Main.rand.NextFloat(0.3f, 1f), false, 15, 1.25f, (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * 0.6f);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    if (Main.rand.NextBool((int)freq))
                    {
                        Dust failShotDust = Dust.NewDustPerfect(NPC.Center, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = particledir * 16 * Main.rand.NextFloat(0.5f, 1.3f);
                        failShotDust.velocity += Parent.velocity + NPC.velocity;
                        failShotDust.scale = Main.rand.NextFloat(0.9f, 1.8f);
                    }
                }
            }
            else
            {
                int freq = 10;
                if (parentAI.Timer % freq == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        for (int d = 0; d < 3; d++)
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);

                        int type = ModContent.ProjectileType<BrimstoneBarrage>();
                        int damage = FargoSoulsUtil.ScaledProjectileDamage(Parent.defDamage);
                        float speed = 10f;
                        Vector2 velocity = (NPC.rotation + MathHelper.Pi).ToRotationVector2() * speed;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, damage, 1f, Main.myPlayer, 1f, 0f, speed * 3f);
                        if (SeekerNumber == 0)
                        {
                            velocity = (AI3 + MathHelper.PiOver2).ToRotationVector2() * speed;
                            Projectile.NewProjectile(Parent.GetSource_FromAI(), Parent.Center, velocity, type, damage, 1f, Main.myPlayer, 1f, 0f, speed * 3f);
                        }
                    }
                }
            }

            Vector2 dir = AI3.ToRotationVector2().RotatedBy(MathHelper.PiOver2 + MathHelper.PiOver2 * 0.3f * i);
            Vector2 desiredPos = Parent.Center + dir * BaseOffset;
            CustomRotation = 1;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.DirectionTo(Parent.Center).ToRotation(), 1f);
            Movement(desiredPos, 1f);
        }
        public void SpinDashes()
        {
            var parentAI = Parent.GetDLCBehavior<CalamitasBrothersEternity>();

            float offset = SeekerNumber * MathHelper.TwoPi / TotalSeekers;
            Vector2 desiredPos = Parent.Center + (Parent.rotation + offset).ToRotationVector2() * BaseOffset;
            Movement(desiredPos, 1f);
            CustomRotation = 1;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.DirectionTo(Parent.Center).ToRotation(), 0.2f);

            int windupTime = 40;
            int windbackTime = 15;
            int chargeTime = 40;
            int endTime = 3;
            int cycle = windupTime + windbackTime + chargeTime + endTime;
            float cycleTimer = parentAI.Timer % cycle;
            if (parentAI.Timer > cycle && cycleTimer == windupTime / 2)
            {
                SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                for (int d = 0; d < 3; d++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                if (DLCUtils.HostCheck)
                {
                    int type = ModContent.ProjectileType<BrimstoneBarrage>();
                    int damage = FargoSoulsUtil.ScaledProjectileDamage(Parent.defDamage);
                    float speed = 9f;
                    Vector2 velocity = offset.ToRotationVector2() * speed;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, damage, 1f, Main.myPlayer, 1f, 0f, speed * 3f);
                }
            }
        }
        #endregion
        #endregion

        #region Help Methods
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
            if (NPC.Distance(desiredPos) < 10)
            {
                NPC.velocity = Vector2.Zero;
                NPC.Center = desiredPos;
            }
        }
        #endregion
    }
}
