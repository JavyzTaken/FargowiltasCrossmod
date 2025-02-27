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
using Terraria.ID;
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
        public const int TotalSeekers = 6;
        public float BaseOffset => Parent.width / 2 + 40;

        #endregion
        public override bool IsLoadingEnabled(Mod mod) => CalamitasCloneEternity.Enabled;
        public override void SetStaticDefaults()
        {
            
        }
        public override void SetDefaults()
        {
            
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
            switch ((States)parentAI.State)
            {
                
                case States.Dash:
                    Dash();
                    break;
                /*
                case States.Flamethrower:
                    Flamethrower();
                    break;
                case States.Fireballs:
                    Fireballs();
                    break;
                */
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
            Vector2 desiredPos = Parent.Center + offset.ToRotationVector2() * BaseOffset;
            Movement(desiredPos, 1f);
        }
        #region Attacks
        public void Dash()
        {
            var parentAI = Parent.GetDLCBehavior<CalamitasBrothersEternity>();
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
            else
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
                        int damage = NPC.GetProjectileDamage(type);
                        float speed = 6f;
                        Vector2 velocity = (NPC.rotation + MathHelper.Pi).ToRotationVector2() * speed;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, damage, 1f, Parent.target, 1f, 0f, speed * 3f);
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

            if (parentAI.Timer < 4)
                AI3 = Parent.rotation;
            if (parentAI.Timer < 50)
            {
                int freq = 10;
                int offset = 2 * Math.Abs(i);
                if (parentAI.Timer % freq == offset - 1)
                {
                    if (DLCUtils.HostCheck)
                    {
                        for (int d = 0; d < 3; d++)
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);

                        int type = ModContent.ProjectileType<BrimstoneBarrage>();
                        int damage = NPC.GetProjectileDamage(type);
                        float speed = 6f;
                        Vector2 velocity = (NPC.rotation + MathHelper.Pi).ToRotationVector2() * speed;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, damage, 1f, Parent.target, 1f, 0f, speed * 3f);
                    }
                }
            }

            Vector2 dir = AI3.ToRotationVector2().RotatedBy(MathHelper.PiOver2 + MathHelper.PiOver2 * 0.2f * (i + sign));
            Vector2 desiredPos = Parent.Center + dir * BaseOffset;
            CustomRotation = 1;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.DirectionTo(Parent.Center).ToRotation(), 0.2f);
            Movement(desiredPos, 1f);
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
