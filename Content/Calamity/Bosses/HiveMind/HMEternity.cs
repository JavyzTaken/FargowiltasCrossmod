
using System;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs.HiveMind;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Audio;
using ReLogic.Content;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;

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
            entity.lifeMax = 5000;
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
                    
                    Main.EntitySpriteDraw(fly.Value, npc.oldPos[i]+ new Vector2(npc.width / 2, npc.height / 2) - Main.screenPosition + new Vector2(0, 10), new Rectangle(178 * ((int)sprite.X - 1), 142 * (int)sprite.Y, 178, 142), drawColor * (1 - i/10f), npc.rotation, new Vector2(178, 142) / 2, npc.scale, SpriteEffects.None);
                }
                Main.EntitySpriteDraw(fly.Value, npc.Center - Main.screenPosition + new Vector2(0, 10), new Rectangle(178*((int)sprite.X-1), 142 * (int)sprite.Y, 178, 142), drawColor * npc.Opacity, npc.rotation, new Vector2(178, 142) / 2, npc.scale, SpriteEffects.None);
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
        public int[] attackCycle = { 0, 0, 0, 0, 0 };
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (!WorldSavingSystem.EternityMode) return base.CanHitPlayer(npc, target, ref cooldownSlot);
            return true;
        }
        
        public override bool SafePreAI(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            npc.dontTakeDamage = false;
            npc.defense = 200;
            npc.alpha = 0;
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
            }
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.velocity.Y += 1;
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
            }else if (npc.localAI[0] < npc.localAI[1])
            {
                npc.localAI[1] -= 1f;
            }
            //ai :real:
            if ( phase == 0)
            {
                for (int i = 0; i < npc.ai[0]/100f; i++)
                {
                    Dust.NewDustDirect(npc.Center - new Vector2(100, 0), 200, 0, DustID.Corruption);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Player player = Main.player[Main.myPlayer];
                    PunchCameraModifier modifier = new(player.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), (npc.ai[0]/60f), 1f, 60, 100f, FullName);
                    Main.instance.CameraModifiers.Add(modifier);
                }
                npc.ai[0]++;
                if (npc.ai[0] >= 300)
                {
                    npc.scale += 0.05f;
                    
                }
                if (npc.ai[0] == 300)
                {
                    SoundEngine.PlaySound(roar with { Pitch = -0.5f }, npc.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DankCreeper>(), ai0: npc.whoAmI);
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveBlob>(), ai0: npc.whoAmI);
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveBlob2>(), ai0: npc.whoAmI);
                        }
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DarkHeart>(), ai0: npc.whoAmI);
                    }
                }
                if (npc.ai[0] == 320)
                {
                    phase = 1;
                    npc.ai[0] = 0;
                    for (int i = 0; i < 100f; i++)
                    {
                        Dust.NewDustDirect(npc.Center - new Vector2(100, 0), 200, 0, DustID.Corruption, 0, -5);
                    }
                    Vector2 center = npc.Center;
                    npc.width = 150;
                    npc.height = 100;
                    npc.Center = center - new Vector2(0, npc.height/2);
                    npc.scale = 1;
                }
                
            }
            if (phase == 1)
            {
                npc.ai[0]++;
                if (npc.ai[0] % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, toTarget.RotatedBy(MathHelper.ToRadians(30))*15, ModContent.ProjectileType<CurvingCursedFlames>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: -1);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, toTarget.RotatedBy(MathHelper.ToRadians(-30)) * 15, ModContent.ProjectileType<CurvingCursedFlames>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: 1);
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<DankCreeper>()))
                {
                    SoundEngine.PlaySound(roar with { Pitch = 0.5f }, npc.Center);
                    phase = 2;
                    sprite.X = 1;
                    npc.velocity.Y = -20;
                    npc.ai[0] = -90;
                    npc.noGravity = true;
                    npc.ai[1] = 1;
                    npc.damage = 80;
                    npc.noTileCollide = true;
                    attackCycle = new int[] { 0, 1};
                    foreach (NPC n in Main.npc)
                    {
                        if ((n.type == ModContent.NPCType<HiveBlob>() || n.type == ModContent.NPCType<HiveBlob2>()) && n.ai[0] == npc.whoAmI)
                        {
                            n.StrikeInstantKill();
                        }
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath23, npc.Center);
                }
            }
            if (phase >= 2)
            {
                npc.damage = 50;
                int attack = attackCycle[(int)npc.ai[2]];
                if (attack == 0)
                {
                    Follow(npc, 3, toTarget, 180);
                }
                if (attack == 1)
                {
                    Dash(npc, toTarget * 10, 100, 80, 0f);
                }
                if (attack == 2)
                {
                    Dash(npc, toTarget.RotatedBy(MathHelper.ToRadians(90)) * 20, 60, 50);
                    if (npc.ai[0] % 15 == 0 && NPC.CountNPCS(NPCID.EaterofSouls) < 5)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.EaterofSouls);
                        SoundEngine.PlaySound(SoundID.NPCDeath23, npc.Center);
                    }
                }
                if (attack == 3)
                {
                    Dash(npc, (target.Center + new Vector2(0, -200) - npc.Center).SafeNormalize(Vector2.Zero) * 15, 100, 80, 0);
                    if (npc.ai[0] % 30 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<ShadeNimbusHostile>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                }
                if (attack == 4)
                {
                    Dash(npc, toTarget * 17, 80, 70, 0.02f);
                }
                if (attack == 5)
                {
                    
                    Teleport(npc, 30, 20, target.Center + toTarget*500);
                }
                RetractHeart(npc, 0.8f, 2, 7, 12, new int[] {0, 1});
                RetractHeart(npc, 0.5f, 3, 7, 12, new int[] { 0, 1 });
                RetractHeart(npc, 0.2f, 4, 7, 12, new int[] { 0, 3, 1 });
                ReleaseHeart(npc);
                
            }
            return false;
        }
        public void Teleport(NPC npc, int startTime, int endTime, Vector2 location)
        {
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
            npc.ai[0]++;
            if (npc.ai[0] <= startTime)
            {
                npc.Opacity = MathHelper.Lerp(1, 0, (npc.ai[0] / startTime));
                if (heart != null) heart.Opacity = npc.Opacity;
            }
            if (npc.ai[0] == startTime)
            {
                npc.Center = location;
                if (heart != null) heart.Center= npc.Center;
            }
            if (npc.ai[0] <= startTime +endTime && npc.ai[0] > startTime)
            {
                
                npc.Opacity = MathHelper.Lerp(0, 1, ((npc.ai[0] - startTime) / endTime));
                if (heart != null) heart.Opacity = npc.Opacity;
            }
            if (npc.ai[0] == startTime + endTime)
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
                if (Main.netMode != NetmodeID.MultiplayerClient)
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
                    attackCycle = new int[] { 0, 1, 2 };
                }
                if (npc.GetLifePercent() <= 0.5f)
                {
                    attackCycle = new int[] { 0, 1, 5, 3, 2, 4 };
                }
                if (npc.GetLifePercent() <= 0.2f)
                {
                    attackCycle = new int[] { 0, 1, 3,5, 2, 5, 4 };
                }
            }
        }
        public void Follow(NPC npc, float speed, Vector2 toTarget, int time)
        {
            npc.ai[0]++;
            if (npc.ai[0] < 0)
            {
                npc.velocity /= 1.05f;
                return;
            }
            if (phase > 3)
            {
                speed *= 1.7f;
                if (npc.ai[0] == time / 2 && npc.ai[1] == 1)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.velocity, ModContent.ProjectileType<ShadeLightningCloud>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
            }
            npc.velocity = toTarget * speed;
           
            if (npc.ai[0] >= time)
            {
                IncrementCycle(npc);
            }
        }
        public void Dash(NPC npc, Vector2 vector, int time, int slow = 0, float force = 1)
        {
            if (npc.ai[0] == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/HiveMindRoar"), npc.Center);
                npc.velocity = vector;
            }

            npc.ai[0]++;
            npc.velocity = Vector2.Lerp(npc.velocity, vector, force);
            if (npc.ai[0] >= slow)
            {
                npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Zero, (npc.ai[0] - slow) / (time - slow));
            }
            if (npc.ai[0] >= time)
            {
                IncrementCycle(npc);
            }
        }
        public void IncrementCycle(NPC npc)
        {
            npc.ai[0] = 0;
            npc.ai[2]++;
            if (npc.ai[2] >= attackCycle.Length)
            {
                npc.ai[2] = 0;
            }
        }
    }
}
