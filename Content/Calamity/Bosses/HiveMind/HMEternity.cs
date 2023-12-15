using System;
using System.IO;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HMEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<CalamityMod.NPCs.HiveMind.HiveMind>());
        public override bool InstancePerEntity => true;

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            base.HitEffect(npc, hit);
        }
        public override void SetDefaults(NPC entity)
        {
            if (!WorldSavingSystem.EternityMode) return;
            base.SetDefaults(entity);
            entity.scale = 0.01f;
            NPCID.Sets.TrailCacheLength[entity.type] = 10;
            NPCID.Sets.TrailingMode[entity.type] = 0;
            //entity.damage = (int)(entity.damage * 1.5f);
            entity.lifeMax = (int)Math.Ceiling(entity.lifeMax * 0.67401960784); //funny number results in 11000
            if (BossRushEvent.BossRushActive)
            {
                entity.lifeMax = 5000000;
            }
        }
        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            base.ApplyDifficultyAndPlayerScaling(npc, numPlayers, balance, bossAdjustment);
        }
        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {

        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);
        }
        public Vector2 sprite = new Vector2(0, 0);
        public int frameCounter = 0;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            Asset<Texture2D> ground = ModContent.Request<Texture2D>("CalamityMod/NPCs/HiveMind/HiveMind");
            Asset<Texture2D> fly = ModContent.Request<Texture2D>("CalamityMod/NPCs/HiveMind/HiveMindP2");
            if (sprite.X == 0)
            {
                Main.EntitySpriteDraw(ground.Value, npc.Center - Main.screenPosition - new Vector2(0, 10), new Rectangle(0, 122 * (int)sprite.Y, 178, 122), drawColor * npc.Opacity, npc.rotation, new Vector2(178, 122) / 2, npc.scale, SpriteEffects.None);
            }
            else
            {
                for (int i = 0; i < (int)npc.localAI[1]; i++)
                {

                    Main.EntitySpriteDraw(fly.Value, npc.oldPos[i] + new Vector2(npc.width / 2, npc.height / 2) - Main.screenPosition + new Vector2(0, 10), new Rectangle(178 * ((int)sprite.X - 1), 142 * (int)sprite.Y, 178, 142), drawColor * (1 - i / 10f), npc.rotation, new Vector2(178, 142) / 2, npc.scale, SpriteEffects.None);
                }
                Main.EntitySpriteDraw(fly.Value, npc.Center - Main.screenPosition + new Vector2(0, 10), new Rectangle(178 * ((int)sprite.X - 1), 142 * (int)sprite.Y, 178, 142), drawColor * npc.Opacity, npc.rotation, new Vector2(178, 142) / 2, npc.scale, SpriteEffects.None);
            }

            return false;
        }
        public override void DrawBehind(NPC npc, int index)
        {
            if (!WorldSavingSystem.EternityMode) return;
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (!WorldSavingSystem.EternityMode) return;
            frameCounter++;
            if (frameCounter >= 5)
            {
                sprite.Y++;
                frameCounter = 0;
                if (sprite.X == 0)
                {
                    if (sprite.Y >= 16)
                    {
                        sprite.Y = 0;
                    }
                }
                else
                {
                    if (sprite.Y >= 8)
                    {
                        sprite.Y = 0;
                        sprite.X++;
                        if (sprite.X >= 3)
                        {
                            sprite.X = 1;
                        }
                    }
                }
            }
        }
        public int phase = 0;
        public Vector2 LockVector1 = Vector2.Zero;
        const int attackCycleLength = 7;
        public int[] attackCycle = new int[attackCycleLength] { 0, 0, 0, 0, 0, -1, -1 };

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            for (int i = 0; i < attackCycleLength; i++)
            {
                binaryWriter.Write7BitEncodedInt(attackCycle[i]);
            }
            binaryWriter.Write7BitEncodedInt(phase);
            binaryWriter.WriteVector2(sprite);
            binaryWriter.WriteVector2(LockVector1);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            for (int i = 0; i < attackCycleLength; i++)
            {
                attackCycle[i] = binaryReader.Read7BitEncodedInt();
            }
            phase = binaryReader.Read7BitEncodedInt();
            sprite = binaryReader.ReadVector2();
            LockVector1 = binaryReader.ReadVector2();
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (!WorldSavingSystem.EternityMode) return base.CanHitPlayer(npc, target, ref cooldownSlot);
            return true;
        }

        public override bool SafePreAI(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return true;

            ref float timer = ref npc.ai[0];
            ref float attackIndex = ref npc.ai[2];
            ref float rotDirection = ref npc.ai[3];


            npc.dontTakeDamage = false;
            npc.defense = 200;
            npc.alpha = 0;
            if (!Targeting())
            {
                return false;
            }
            Player target = Main.player[npc.target];
            Vector2 toTarget = (target.Center - npc.Center).SafeNormalize(Vector2.Zero);
            SoundStyle roar = new SoundStyle("CalamityMod/Sounds/Custom/HiveMindRoar");
            //for afterimage length. localAI[0] is target length. localAI[1] is current length.
            //(so i can turn off the afterimage without it instantly cutting off)
            if (npc.velocity.Length() > 6)
            {
                npc.localAI[0] = 10;
            }
            else
            {
                npc.localAI[0] = 0;
            }
            if (npc.localAI[0] > npc.localAI[1])
            {
                npc.localAI[1] += 1f;
            }
            else if (npc.localAI[0] < npc.localAI[1])
            {
                npc.localAI[1] -= 1f;
            }
            //ai :real:
            if (phase == 0)
            {
                for (int i = 0; i < timer / 100f; i++)
                {
                    Dust.NewDustDirect(npc.Center - new Vector2(100, 0), 200, 0, DustID.Corruption);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Player player = Main.player[Main.myPlayer];
                    PunchCameraModifier modifier = new(player.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), (timer / 60f), 1f, 60, 100f, FullName);
                    Main.instance.CameraModifiers.Add(modifier);
                }
                if (timer < 300)
                {
                    npc.Center += npc.DirectionTo(target.Center) * 5;
                    npc.dontTakeDamage = true;
                }
                timer++;
                if (timer >= 300 && timer < 320)
                {
                    npc.scale += 0.05f;

                }
                if (timer == 300)
                {
                    SoundEngine.PlaySound(roar with { Pitch = -0.5f }, npc.Center);
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int n = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DankCreeper>(), ai0: npc.whoAmI);
                            NPC creeper = Main.npc[n];
                            if (creeper != null && creeper.active && creeper.type == ModContent.NPCType<DankCreeper>())
                            {
                                creeper.lifeMax *= 3;
                                creeper.life = creeper.lifeMax;
                            }
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveBlob>(), ai0: npc.whoAmI);
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveBlob2>(), ai0: npc.whoAmI);
                        }
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DarkHeart>(), ai0: npc.whoAmI);
                    }
                    NetSync(npc);
                }
                if (timer >= 320)
                {
                    phase = 1;
                    timer = 0;
                    for (int i = 0; i < 100f; i++)
                    {
                        Dust.NewDustDirect(npc.Center - new Vector2(100, 0), 200, 0, DustID.Corruption, 0, -5);
                    }
                    Vector2 center = npc.Center;
                    npc.width = 150;
                    npc.height = 100;
                    npc.Center = center - new Vector2(0, npc.height / 2);
                    npc.scale = 1;
                    NetSync(npc);
                }

            }
            else
            {
                npc.scale = 1; //to avoid mp issues
            }
            if (phase == 1)
            {
                timer++;
                if (timer % 10 == 0 && DLCUtils.HostCheck)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, toTarget.RotatedBy(MathHelper.ToRadians(30)) * 15, ModContent.ProjectileType<CurvingCursedFlames>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: -1);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, toTarget.RotatedBy(MathHelper.ToRadians(-30)) * 15, ModContent.ProjectileType<CurvingCursedFlames>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: 1);
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<DankCreeper>()))
                {
                    SoundEngine.PlaySound(roar with { Pitch = 0.5f }, npc.Center);
                    phase = 2;
                    sprite.X = 1;
                    npc.velocity.Y = -20;
                    timer = -90;
                    npc.noGravity = true;
                    npc.ai[1] = 1;
                    npc.noTileCollide = true;
                    attackCycle = new int[attackCycleLength] { 0, 1, 2, -1, -1, -1, -1 };
                    foreach (NPC n in Main.npc)
                    {
                        if ((n.type == ModContent.NPCType<HiveBlob>() || n.type == ModContent.NPCType<HiveBlob2>()) && n.ai[0] == npc.whoAmI)
                        {
                            n.StrikeInstantKill();
                        }
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath23, npc.Center);
                    NetSync(npc);
                }
            }
            if (phase >= 2)
            {
                int attack = attackCycle[(int)attackIndex];
                if (attack < 0)
                {
                    attack = 0;
                }

                if (attack == 0)
                {
                    Follow(npc, 3, toTarget, 180);
                }
                if (attack == 1)
                {
                    const int dashDuration = 100;
                    const float maxSpeedIncrease = 10;
                    float speed = 10;// + (maxSpeedIncrease * Math.Min(timer / baseDashDuration, 1));
                    Dash(npc, toTarget * speed, dashDuration, 80, 0f);
                }
                if (attack == 2)
                {
                    if (timer == 0)
                    {
                        LockVector1 = target.Center;
                        if (Math.Abs(rotDirection) != 1)
                        {
                            rotDirection = Main.rand.NextBool() ? 1 : -1;
                        }
                        rotDirection = -rotDirection;
                    }
                    const int desiredDistance = 450; //prev: 350
                    Vector2 desiredPos = LockVector1 + (LockVector1.DirectionTo(npc.Center).RotatedBy(rotDirection * MathHelper.Pi / 8f) * desiredDistance);
                    Vector2 toTargetPos = npc.DirectionTo(desiredPos);
                    Dash(npc, toTargetPos * 21f, 120, 50);
                    if (timer % 15 == 0 && NPC.CountNPCS(NPCID.EaterofSouls) < 5)
                    {
                        if (DLCUtils.HostCheck)
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.EaterofSouls);
                        SoundEngine.PlaySound(SoundID.NPCDeath23, npc.Center);
                    }
                }
                if (attack == 3)
                {
                    Dash(npc, (target.Center + new Vector2(0, -200) - npc.Center).SafeNormalize(Vector2.Zero) * 15, 100, 80, 0);
                    if (timer % 30 == 0)
                    {
                        if (DLCUtils.HostCheck)
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<ShadeNimbusHostile>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                }
                if (attack == 4)
                {
                    Dash(npc, toTarget * 17, 80, 70, 0.02f);
                }
                if (attack == 5)
                {

                    Teleport(npc, 45, 20, target.Center + toTarget * 500);
                }
                if (attack == 6)
                {
                    if (timer == 0)
                    {
                        LockVector1 = -Vector2.UnitY * 250 + Vector2.UnitX * 400 * -Math.Sign(toTarget.X);
                    }
                    Dash(npc, npc.DirectionTo(target.Center + LockVector1) * 17, 100, 70, 0.02f);

                }
                if (attack == 7)
                {
                    if (timer == 0)
                    {
                        LockVector1 = Vector2.UnitX * Math.Sign(toTarget.X) * 17;
                    }
                    Dash(npc, LockVector1, 45, 35, 0.02f);
                }
                RetractHeart(npc, 0.8f, 2, 7, 12, new int[attackCycleLength] { 0, 1, -1, -1, -1, -1, -1 });
                RetractHeart(npc, 0.5f, 3, 7, 12, new int[attackCycleLength] { 0, 1, 6, 7, -1, -1, -1 });
                RetractHeart(npc, 0.2f, 4, 7, 12, new int[attackCycleLength] { 0, 3, 1, 6, 7, -1, -1 });
                ReleaseHeart(npc);

            }
            return false;

            bool Targeting()
            {
                const float despawnRange = 5000f;
                Player p = Main.player[npc.target];
                if (!p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > despawnRange)
                {
                    npc.TargetClosest();
                    p = Main.player[npc.target];
                    if (!p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > despawnRange)
                    {
                        npc.noTileCollide = true;
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;
                        npc.velocity.Y += 1f;
                        if (npc.timeLeft == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
                            }
                        }
                        return false;
                    }
                }
                return true;
            }
        }
        public void Teleport(NPC npc, int startTime, int endTime, Vector2 location)
        {
            ref float timer = ref npc.ai[0];
            NPC heart = null;
            foreach (NPC n in Main.npc)
            {
                if (n != null && n.type == ModContent.NPCType<DarkHeart>() && n.ai[0] == npc.whoAmI && n.active)
                {
                    heart = n;
                    break;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Corruption);
            }
            timer++;
            if (timer <= startTime)
            {
                npc.Opacity = MathHelper.Lerp(1, 0, (timer / startTime));
                if (heart != null) heart.Opacity = npc.Opacity;
            }
            if (timer == startTime)
            {
                npc.Center = location;
                if (heart != null) heart.Center = npc.Center;
                NetSync(npc);
                npc.netUpdate = true; //doing double here for safetybecause net syncing whenever you teleport is VERY important
            }
            if (timer <= startTime + endTime && timer > startTime)
            {

                npc.Opacity = MathHelper.Lerp(0, 1, ((timer - startTime) / endTime));
                if (heart != null) heart.Opacity = npc.Opacity;
            }
            if (timer == startTime + endTime)
            {
                IncrementCycle(npc);
            }
        }
        public void RetractHeart(NPC npc, float percent, int fromPhase, int numCreeper, int numBlob, int[] attacks)
        {
            if (npc.GetLifePercent() <= percent && phase == fromPhase)
            {
                attackCycle = attacks;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                phase++;
                if (DLCUtils.HostCheck)
                {
                    for (int i = 0; i < numCreeper; i++)
                    {

                        NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DankCreeper>(), ai0: npc.whoAmI);

                    }
                    for (int i = 0; i < numBlob; i++)
                    {
                        if (Main.rand.NextBool())
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveBlob>(), ai0: npc.whoAmI);
                        else
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveBlob2>(), ai0: npc.whoAmI);
                    }
                }
                NetSync(npc);
            }
        }
        public void ReleaseHeart(NPC npc)
        {
            if (npc.ai[1] == 0 && !NPC.AnyNPCs(ModContent.NPCType<DankCreeper>()))
            {
                foreach (NPC n in Main.npc)
                {
                    if (n != null && (n.type == ModContent.NPCType<HiveBlob>() || n.type == ModContent.NPCType<HiveBlob2>()) && n.ai[0] == npc.whoAmI)
                    {
                        n.StrikeInstantKill();
                    }
                }
                npc.ai[1] = 1;
                SoundEngine.PlaySound(SoundID.NPCDeath23, npc.Center);
                if (npc.GetLifePercent() <= 0.8f)
                {
                    attackCycle = new int[attackCycleLength] { 0, 1, 2, -1, -1, -1, -1 };
                }
                if (npc.GetLifePercent() <= 0.5f)
                {
                    attackCycle = new int[attackCycleLength] { 0, 1, 5, 3, 2, 4, -1 };
                }
                if (npc.GetLifePercent() <= 0.2f)
                {
                    attackCycle = new int[attackCycleLength] { 0, 1, 3, 5, 2, 5, 4 };
                }
                NetSync(npc);
            }
        }
        public void Follow(NPC npc, float speed, Vector2 toTarget, int time)
        {
            ref float timer = ref npc.ai[0];

            timer++;
            if (timer < 0)
            {
                npc.velocity /= 1.05f;
                return;
            }
            if (npc.GetLifePercent() <= 0.8f)
            {
                speed *= 1.7f;
                if (timer == time / 2 && npc.ai[1] == 1)
                {
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.velocity, ModContent.ProjectileType<ShadeLightningCloud>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
            }
            npc.velocity = toTarget * speed;

            if (timer >= time)
            {
                IncrementCycle(npc);
            }
        }
        public void Dash(NPC npc, Vector2 vector, int time, int slow = 0, float force = 1)
        {
            ref float timer = ref npc.ai[0];
            int attack = attackCycle[(int)npc.ai[2]];

            if (timer == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/HiveMindRoar"), npc.Center);
                npc.velocity = vector;
            }

            timer++;
            npc.velocity = Vector2.Lerp(npc.velocity, vector, force);
            if (timer >= slow)
            {
                npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Zero, (timer - slow) / (time - slow));
            }
            else if (attack == 1) //accelerate during dash towards player
            {
                const float accel = 0.175f;
                const float maxSpeed = 20;
                npc.velocity += npc.DirectionTo(Main.player[npc.target].Center) * accel;
                if (npc.velocity.LengthSquared() > maxSpeed * maxSpeed)
                {
                    npc.velocity = Vector2.Normalize(npc.velocity) * maxSpeed;
                }
            }
            else if (attack == 7) //shade clouds during shade cloud attack
            {
                if (timer % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int type = ModContent.ProjectileType<ShadeNimbusHostile>();
                    int damage = FargoSoulsUtil.ScaledProjectileDamage(npc.damage);
                    Vector2 cloudSpawnPos = new Vector2(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height));
                    Projectile.NewProjectile(npc.GetSource_FromAI(), cloudSpawnPos, Vector2.Zero, type, damage, 0, Main.myPlayer, 11f);
                }
            }
            if (timer >= time)
            {
                IncrementCycle(npc);
            }
        }
        public void IncrementCycle(NPC npc)
        {
            ref float timer = ref npc.ai[0];

            timer = 0;
            npc.ai[2]++;
            if (npc.ai[2] >= attackCycle.Length || attackCycle[(int)npc.ai[2]] < 0)
            {
                npc.ai[2] = 0;
            }
            NetSync(npc);
        }
    }
}
