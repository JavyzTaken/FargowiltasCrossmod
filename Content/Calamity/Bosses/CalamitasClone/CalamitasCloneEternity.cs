
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.CalClone;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.NPCMatching;
using Luminance.Common.Utilities;
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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamitasCloneEternity : CalDLCEmodeBehavior
    {
        public const bool Enabled = false;
        #region Fields
        public Player Target => Main.player[NPC.target];
        public static float Acceleration => 0.8f;
        public static float MaxMovementSpeed => 30f;

        public ref float Timer => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];

        public enum States
        {
            ChargedGigablast
        }
        public List<States> Attacks
        {
            get
            {
                List<States> attacks = [States.ChargedGigablast];
                return attacks;
            }
        }
        #endregion
        public override bool IsLoadingEnabled(Mod mod) => Enabled;
        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.CalClone.CalamitasClone>();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            
        }
        
        public override bool PreAI()
        {
            #region Standard
            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1f, 0f, 0f);

            // Don't take damage if any brothers are alive
            NPC.dontTakeDamage = Main.npc.Any(n => n.TypeAlive<Cataclysm>() || n.TypeAlive<Catastrophe>());

            CalamityGlobalNPC.calamitas = NPC.whoAmI;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Target.dead || !Target.active)
                NPC.TargetClosest();


            // Despawn
            if (!Target.active || Target.dead)
            {
                NPC.TargetClosest(false);
                if (!Target.active || Target.dead)
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
                        NPC.Calamity().newAI[2] = 0f;
                        NPC.Calamity().newAI[3] = 0f;
                        NPC.alpha = 0;
                        NPC.netUpdate = true;
                    }
                    return false;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;
            #endregion

            #region Rotation
            // Rotation
            Vector2 npcCenter = new Vector2(NPC.Center.X, NPC.position.Y + NPC.height - 59f);
            Vector2 lookAt = new Vector2(Target.position.X - (Target.width / 2), Target.position.Y - (Target.height / 2));
            Vector2 rotationVector = npcCenter - lookAt;


            float rotation = (float)Math.Atan2(rotationVector.Y, rotationVector.X) + MathHelper.PiOver2;
            if (rotation < 0f)
                rotation += MathHelper.TwoPi;
            else if (rotation > MathHelper.TwoPi)
                rotation -= MathHelper.TwoPi;

            float rotationAmt = 0.1f;
            if (NPC.rotation < rotation)
            {
                if ((rotation - NPC.rotation) > MathHelper.Pi)
                    NPC.rotation -= rotationAmt;
                else
                    NPC.rotation += rotationAmt;
            }
            else if (NPC.rotation > rotation)
            {
                if ((NPC.rotation - rotation) > MathHelper.Pi)
                    NPC.rotation += rotationAmt;
                else
                    NPC.rotation -= rotationAmt;
            }

            if (NPC.rotation > rotation - rotationAmt && NPC.rotation < rotation + rotationAmt)
                NPC.rotation = rotation;
            if (NPC.rotation < 0f)
                NPC.rotation += MathHelper.TwoPi;
            else if (NPC.rotation > MathHelper.TwoPi)
                NPC.rotation -= MathHelper.TwoPi;
            if (NPC.rotation > rotation - rotationAmt && NPC.rotation < rotation + rotationAmt)
                NPC.rotation = rotation;
            #endregion

            if (!NPC.HasPlayerTarget) // just in case
                return false;
            switch ((States)State)
            {
                case States.ChargedGigablast:
                    ChargedGigablast();
                    break;
            }

            return false;
        }
        #region States
        public void ChargedGigablast()
        {
            float distance = 660f;
            Vector2 pos = Target.Center;
            Vector2 offset = Target.DirectionTo(NPC.Center) * distance;
            if (Math.Abs(FargoSoulsUtil.RotationDifference(offset, -Vector2.UnitY)) > MathHelper.PiOver2 * 0.7f)
                offset = offset.RotateTowards(-Vector2.UnitY.ToRotation(), 0.07f);
            pos += offset;
            float speed = 0.4f;
            if (Target.Distance(NPC.Center) < distance)
                speed /= 2;
            Movement(pos, speed);
        }
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
        }
        #endregion
    }
}
