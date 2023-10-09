
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Systems;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    public class CryogenEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<CalamityMod.NPCs.Cryogen.Cryogen>());
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> shield = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/CryogenShield");
            float x = shieldDrawTimer / 200f;
            x = (float)-(Math.Cos(Math.PI * x) - 1) / 2;
            float scaleAdd = MathHelper.Lerp(-0.2f, 0.2f, x);

            if (npc.ai[0] != 3)
            {
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    Color glowColor = Color.Blue with { A = 0 } * 0.7f;


                    spriteBatch.Draw(shield.Value, npc.Center + afterimageOffset - screenPos, null, glowColor, 0, shield.Size() / 2, npc.scale + scaleAdd, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(shield.Value, npc.Center - screenPos, null, drawColor, 0, shield.Size() / 2, npc.scale + scaleAdd, SpriteEffects.None, 0);
            }
            if (npc.ai[0] == 3)
            {
                Asset<Texture2D> cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase1");
                if (npc.ai[2] == 1) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase2");
                if (npc.ai[2] == 2) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase3");
                if (npc.ai[2] == 3) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase4");
                if (npc.ai[2] == 4) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase5");
                if (npc.ai[2] == 5) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase6");
                Vector2 offset = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)) * npc.ai[1] / 120;
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    Color glowColor = Color.Blue with { A = 0 } * 0.7f;


                    spriteBatch.Draw(cryo1.Value, npc.Center + afterimageOffset - screenPos + offset, null, glowColor, 0, cryo1.Size() / 2, npc.scale, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(cryo1.Value, npc.Center - screenPos + offset, null, drawColor, 0, cryo1.Size() / 2, npc.scale, SpriteEffects.None, 0);
                return false;
            }
            return true;
        }
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            base.HitEffect(npc, hit);
        }
        public int[] Chains = new int[] { -1, -1, -1, -1 , -1, -1};
        public float shieldDrawTimer;
        public float shieldDrawCounter;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget || !DLCCalamityConfig.Instance.EternityPriorityOverRev || !DLCWorldSavingSystem.EternityRev) return true;
            Player target = Main.player[npc.target];
            ref float attack = ref npc.ai[0];
            ref float timer = ref npc.ai[1];
            ref float data = ref npc.ai[2];
            ref float data2 = ref npc.ai[3];
            npc.scale = 1.5f;
            
            if (shieldDrawCounter == 0)
            {
                shieldDrawTimer++;
                if (shieldDrawTimer >= 200) shieldDrawCounter = 1;
            }
            else
            {
                shieldDrawTimer--;
                if (shieldDrawTimer <= 0) shieldDrawCounter = 0;
            }
            //move towards player
            if (npc.ai[0] == 0)
            {
                npc.rotation = npc.velocity.X / 15f;
                timer++;
                if (timer == 1)
                {
                    Main.NewText("j");
                    Vector2 pos = target.Center + new Vector2(0, -400);
                    data = pos.X; data2 = pos.Y;
                    NetSync(npc);
                }
                else if (npc.Distance(new Vector2(data, data2)) > 300){
                    Vector2 pos = new Vector2(data, data2);
                    npc.velocity = Vector2.Lerp(npc.velocity, (pos - npc.Center).SafeNormalize(Vector2.Zero) * 30, 0.05f);
                    
                }
                if (npc.Distance(new Vector2(data, data2)) < 300)
                {
                    npc.velocity /= 1.1f;
                }
                    if (npc.velocity.Length() < 1 && timer > 20)
                {
                    npc.rotation = 0;
                    data = 0;
                    data2 = 0;
                    timer = 0;
                    attack = 2;
                    npc.velocity *= 0;
                    NetSync(npc);
                }
                
            }
            //teleport around player and spit out ice bombs
            if (npc.ai[0] == 2)
            {
                
                npc.velocity = Vector2.Zero;
                timer++;
                if (timer == 2)
                {
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenShieldBreak"), npc.Center);
                    if (DLCUtils.HostCheck)
                    {
                        Chains[0] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(31));
                        Chains[1] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(-31));
                        Chains[2] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(149));
                        Chains[3] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(-149));
                        Chains[4] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(-90));
                        Chains[5] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(90));
                    }
                    DustExplode(npc);
                }
                if (timer % 100 == 0)
                {
                    
                    int p = Chains[Main.rand.Next(0, 6)];

                    if (p >= 0) {
                        Main.projectile[p].ai[2] = 200;
                    }
                }
                if (timer % 200 == 0)
                {
                    if (DLCUtils.HostCheck) {
                        if (Main.rand.NextBool())
                            for (int i = 3; i < 8; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * i * 4, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.IceBomb>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                            }
                        else
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(0, 1).RotatedBy(MathHelper.ToRadians(360f / 12 * i)), ModContent.ProjectileType<IceRain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: 1);
                            }
                            for (int i = 0; i < 12; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(0, 3).RotatedBy(MathHelper.ToRadians(360f / 12 * i)), ModContent.ProjectileType<IceRain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: 1);
                            }
                            for (int i = 0; i < 12; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(0, 2).RotatedBy(MathHelper.ToRadians(360f / 12 * i + 12)), ModContent.ProjectileType<IceRain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0:1);
                            }
                        }
                    }
                }
                if (npc.GetLifePercent() <= 0.8f)
                {
                    attack = 3;
                    timer = 0;
                    data = -1;
                    data2 = 0;
                }
                
            }
            if (attack == 3) {
                npc.dontTakeDamage = true;

                //Main.musicVolume -= 0.003f;
                if (timer % 60 == 0)
                {
                    data++;
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenShieldBreak") with { Volume = 2}, target.Center);
                    for (int i = 0; i < 3; i++)
                    {
                        Gore.NewGore(npc.GetSource_FromAI(), npc.Center, new Vector2(0, -5).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ModCompatibility.Calamity.Mod.Find<ModGore>("CryoDeathGore" + Main.rand.Next(2, 4)).Type, 1.5f);
                    }
                }
                if (data == 6)
                {
                    DustExplode(npc);
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenDeath") with { Volume = 2}, target.Center);
                    for (int i = 0; i < 5; i++)
                    {
                        Gore.NewGore(npc.GetSource_FromAI(), npc.Center, new Vector2(0, -5).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ModCompatibility.Calamity.Mod.Find<ModGore>("CryoDeathGore" + Main.rand.Next(2, 4)).Type, 1.5f);
                    }
                    npc.active = false;
                    //Main.musicVolume = 1;
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y + 50, ModContent.NPCType<PermafrostBoss>());
                }
                timer++;
            }

            return false;
        }
        public void DustExplode(NPC npc)
        {
            for (int i = 0; i < 200; i++)
            {
                Vector2 speed = new Vector2(0, Main.rand.Next(0, 15)).RotatedByRandom(MathHelper.TwoPi);
                Dust d = Dust.NewDustDirect(npc.Center, 0, 0, DustID.SnowflakeIce, speed.X, speed.Y, Scale:1.5f);
                d.noGravity = true;
                
            }
        }
    }
}
