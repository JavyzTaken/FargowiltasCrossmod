
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.CalClone;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Patreon.DanielTheRobot;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
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
    public class CataclysmEternity : CalamitasBrothersEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.CalClone.Cataclysm>();
    }
    public class CatastropheEternity : CalamitasBrothersEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.CalClone.Catastrophe>();
    }
    public abstract class CalamitasBrothersEternity : CalDLCEmodeBehavior
    {
        public const bool Enabled = false;
        #region Fields
        public Player Target => Main.player[NPC.target];
        public static float Acceleration => 0.5f;
        public static float MaxMovementSpeed => 30f;

        public ref float Timer => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];
        public ref float AI2 => ref NPC.ai[2];
        public ref float AI3 => ref NPC.ai[3];
        public ref float CustomRotation => ref NPC.localAI[0];
        public ref float PreviousState => ref NPC.localAI[1];
        public ref float StunTimer => ref NPC.localAI[2];
        public const int CycleTime = 60 * 3;

        public bool Cataclysm => NPC.type == ModContent.NPCType<Cataclysm>();
        public float forwardRotation => NPC.rotation + MathHelper.PiOver2;
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
            ChargedBomb,
            Stunned
        }
        public List<States> Attacks
        {
            get
            {
                List<States> attacks = [
                    States.Dash, 
                    States.Flamethrower, 
                    //States.ChargedBomb
                    ];
                return attacks;
            }
        }
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
            int hp = 30000;
            NPC.LifeMaxNERB(hp, hp, 80000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
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
            #region Standard
            if (CalamityGlobalNPC.calamitas < 0 || !Main.npc[CalamityGlobalNPC.calamitas].active)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();

                return false;
            }

            if (Cataclysm)
                CalamityGlobalNPC.cataclysm = NPC.whoAmI;
            else
                CalamityGlobalNPC.catastrophe = NPC.whoAmI;

            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1f, 0f, 0f);


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
                case States.ChargedBomb:
                    ChargedBomb();
                    break;
                case States.Stunned:
                    Stunned();
                    break;
            }
            Timer++;
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
            int chargeTime = 38;
            if (Timer < windupTime)
            {
                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * 400;
                NPC otherBrother = OtherBrother;
                if (otherBrother != null)
                {
                    int minDistance = 400;
                    if (desiredPos.Distance(otherBrother.Center) < minDistance)
                        desiredPos = otherBrother.Center + otherBrother.DirectionTo(desiredPos) * minDistance;
                }
                Movement(desiredPos, 1f);
            }
            else if (Timer < windupTime + windbackTime)
            {
                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * 550;
                Movement(desiredPos, 0.75f);
            }
            else if (Timer < windupTime + windbackTime + chargeTime)
            {
                NPC.velocity += NPC.DirectionTo(Target.Center) * 1.2f;
            }
            else
            {
                NPC.velocity *= 0.93f;
            }
            if (Timer >= CycleTime)
            {
                GoToNeutral();
            }
        }
        public void Flamethrower()
        {
            ref float sweepDir = ref AI2;

            int windupTime = 50;
            int pullbackTime = 45;
            int sweepTime = 80;

            int firePreStartup = 10;
            int idealDistance = 280;

            void Flames(float speed = 8f)
            {
                if (Timer % 3 == 0)
                {
                    int type = ModContent.ProjectileType<BrimstoneFire>();
                    int damage = FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);
                    Vector2 dir = forwardRotation.ToRotationVector2();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + dir * NPC.width / 3, dir * speed, type, damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }

            if (Timer < windupTime)
            {
                Vector2 desiredPos = Target.Center + Target.DirectionTo(NPC.Center) * idealDistance;
                NPC otherBrother = OtherBrother;
                if (otherBrother != null)
                {
                    int minDistance = 400;
                    if (desiredPos.Distance(otherBrother.Center) < minDistance)
                        desiredPos = otherBrother.Center + otherBrother.DirectionTo(desiredPos) * minDistance;
                }
                Movement(desiredPos, 1f);
            }
            else if (Timer < windupTime + pullbackTime)
            {
                CustomRotation = 1;
                if (Timer == windupTime)
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
                Movement(desiredPos, 0.75f);

                if (Timer > windupTime + pullbackTime - firePreStartup)
                {
                    Flames();
                }
            }
            else if (Timer < windupTime + pullbackTime + sweepTime)
            {
                CustomRotation = 1;
                NPC.rotation -= sweepDir * MathHelper.Pi / sweepTime;
                NPC.velocity *= 0.94f;

                float progress = (Timer - windupTime - pullbackTime) / sweepTime;
                Flames(6f + progress * 8f);
            }
            else
            {
                NPC.velocity *= 0.93f;
            }
            if (Timer >= CycleTime)
            {
                GoToNeutral();
            }
        }
        public void ChargedBomb()
        {
            if (Timer >= CycleTime)
            {
                GoToNeutral();
            }
        }
        #endregion
        public void Stunned()
        {
            float stunTime = 90;
            NPC.velocity *= 0.95f;

            StunTimer++;
            Timer++;
            if (StunTimer < 60)
                NPC.rotation += Main.rand.NextFloat(-1, 1) * MathHelper.PiOver4 * 0.75f * (1f - (StunTimer / 60f));
            if (StunTimer >= stunTime && Timer >= CycleTime)
                GoToNeutral();
        }
        #endregion

        #region Help Methods
        public void GoToNeutral()
        {
            Reset();
            if (State != (int)States.Stunned)
                PreviousState = State;
            var attacks = Attacks;
            //attacks.Remove((States)PreviousState);
            State = (int)Main.rand.NextFromCollection(attacks);
        }
        public void Reset()
        {
            Timer = 0;
            AI2 = 0;
            AI3 = 0;
            StunTimer = 0;
        }
        public void GetStunned()
        {
            AI2 = 0;
            AI3 = 0;
            StunTimer = 0;
            State = (int)States.Stunned;
            SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Debuffs/DizzyBird") with { Pitch = -0.2f });
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
