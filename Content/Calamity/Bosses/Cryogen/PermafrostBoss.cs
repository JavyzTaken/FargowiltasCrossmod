using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasCrossmod.Core.Calamity;
using Terraria.GameContent.Bestiary;
using System.Threading;
using CalamityMod;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls;
using FargowiltasCrossmod.Core.Utils;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PermafrostBoss : ModNPC
    {
        public override string Texture => "CalamityMod/NPCs/TownNPCs/DILF";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.lifeMax = 20000;
            NPC.knockBackResist = 0;
            NPC.HitSound = new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit", 3);
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
            ref float shieldRot = ref NPC.localAI[0];
            shieldRot += 0.05f;
            Asset<Texture2D> t = TextureAssets.Npc[Type];
            Asset<Texture2D> encasement = ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/GlacialEmbraceBody");
            Asset<Texture2D> shield = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/IceClasperSummonProjectile");
            spriteBatch.Draw(t.Value, NPC.Center - screenPos, new Rectangle(6, 62, 32, 48), drawColor, NPC.rotation, new Vector2(16, 24), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            if (Phase == 0)
            {
                spriteBatch.Draw(encasement.Value, NPC.Center - screenPos + new Vector2(Main.rand.NextFloat(-Timer, Timer), Main.rand.NextFloat(-Timer, Timer)), null, drawColor * 0.7f, NPC.rotation, encasement.Size()/2, NPC.scale, SpriteEffects.None, 0);
            }
            for (int i = 0; i < 15; i++)
            {
                float rotation = MathHelper.ToRadians(360f / 15 * i) + shieldRot;
                if (Phase == 0.5f)
                {
                    float x = (Timer / 120);
                    spriteBatch.Draw(shield.Value, NPC.Center - screenPos + new Vector2(MathHelper.Lerp(1500, 60, (float)Math.Sin((x * Math.PI) / 2)), 0).RotatedBy(rotation), null, drawColor * 0.7f, rotation + MathHelper.PiOver2, shield.Size() / 2, NPC.scale *0.75f, SpriteEffects.None, 0);
                }
                if (Phase > 0.5f)
                {
                    spriteBatch.Draw(shield.Value, NPC.Center - screenPos + new Vector2(60, 0).RotatedBy(rotation), null, drawColor * 0.7f, rotation + MathHelper.PiOver2, shield.Size() / 2, NPC.scale * 0.75f, SpriteEffects.None, 0);
                }
            }
            return false;
        }
        protected static void NetSync(NPC npc, bool onlySendFromServer = true)
        {
            if (onlySendFromServer && Main.netMode != NetmodeID.Server)
                return;

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }
        public ref float Phase => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];
        public ref float Attack => ref NPC.ai[2];
        public ref float Data => ref NPC.ai[3];
        public override void AI()
        {
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
                NetSync(NPC);
            }
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.velocity.Y += -1;
                return;
            }
            Player target = Main.player[NPC.target];
            Vector2 toTarget = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            if (Phase == 0)
            {
                Timer += 0.01f;
                if (Timer >= 3)
                {
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenShieldBreak"), NPC.Center);
                    for (int i = 0; i < 3; i++)
                    {
                        Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)), ModCompatibility.Calamity.Mod.Find<ModGore>("CryoDeathGore" + Main.rand.Next(2, 4)).Type);
                        Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModCompatibility.Calamity.Mod.Find<ModGore>("CryoShieldGore" + Main.rand.Next(1, 5)).Type);
                        Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModCompatibility.Calamity.Mod.Find<ModGore>("CryoShieldGore" + Main.rand.Next(1, 5)).Type);
                    }
                    Timer = 0;
                    Phase = 0.5f;
                }
            }
            if (Phase == 0.5f)
            {
                Timer++;
                if (Timer >= 120)
                {
                    Phase = 1;
                    Timer = 0;
                }
            }
            if (Phase == 1)
            {
                NPC.spriteDirection = NPC.Center.X > target.Center.X ? 1 : -1;
                if (Attack == 0)
                {
                    Vector2 targetpos = target.Center + new Vector2(NPC.Center.X > target.Center.X ? 400 : -400, 0);

                    Movement(targetpos);
                    Timer++;
                    if (Timer >= 200)
                    {
                        Timer = 0;
                        Attack = 1;
                    }
                }
                if (Attack == 1)
                {
                    Timer++;
                    if (Timer < 60)
                    {
                        Vector2 targetpos = target.Center + new Vector2(NPC.Center.X > target.Center.X ? 400 : -400, -300);

                        Movement(targetpos, slowdown: 100, decel: 0.05f);
                    }
                    else
                    {
                        NPC.velocity /= 1.05f;
                        if (Timer == 70 && DLCUtils.HostCheck) {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostHeldWeapon>(), 0, 0, ai0: 0, ai1: toTarget.ToRotation(), ai2: NPC.whoAmI);
                        }
                        if (Timer == 100)
                        {

                            SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                            if (DLCUtils.HostCheck)
                            {
                                
                                for (int i = -5; i < 6; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + toTarget * 50, Vector2.Zero, ModContent.ProjectileType<IceTrident>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, ai0: i * 5, ai2: toTarget.ToRotation());
                                }
                            }
                        }
                        if (Timer >= 200)
                        {
                            Attack = 0;
                            Timer = 0;
                        }
                    }
                }
                
            }
            void Movement( Vector2 pos, float accel = 0.03f, float maxSpeed = 20, float lowspeed = 5, float decel = 0.03f, float slowdown = 30)
            {
                if (NPC.Distance(pos) > slowdown)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed, accel);
                }
                else
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * lowspeed, decel);
                }
            }
            base.AI();
        }
    }
}
