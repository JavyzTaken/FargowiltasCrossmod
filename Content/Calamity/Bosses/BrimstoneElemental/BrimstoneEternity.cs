using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Systems;
using FargowiltasSouls;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimstoneEternity : EModeCalBehaviour
    {
        public const bool Enabled = false;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>());
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!npc.HasValidTarget || !DLCCalamityConfig.Instance.EternityPriorityOverRev || !DLCWorldSavingSystem.EternityRev)
                return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
            Player target = Main.player[npc.target];

            Asset<Texture2D> aura = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/SoftEdgeRing");
            Color auraColor = Color.Red;
            auraColor.A = 0;
            spriteBatch.Draw(aura.Value, auraPos - Main.screenPosition, null, auraColor, 0, aura.Size() / 2, 4, SpriteEffects.None, 1);

            Asset<Texture2D> TPtele = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion];
            Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
            Vector2 TPpos = target.Center + new Vector2(-300, 0);
            if (target.Center.X > npc.Center.X)
                TPpos.X += 600;
            if (npc.ai[1] > 600)
            {
                float x = (npc.ai[1] - 600) / 100f;
                float lerper = 1 - (float)Math.Pow(1 - x, 3);
                spriteBatch.Draw(TPtele.Value, TPpos - Main.screenPosition, null, new Color(250, 50, 50) * 0.7f, 0, TPtele.Size() / 2, new Vector2(MathHelper.Lerp(0, 2, lerper), MathHelper.Lerp(0, 3, lerper)), SpriteEffects.None, 1);
                spriteBatch.Draw(TPtele.Value, TPpos - Main.screenPosition, null, new Color(250, 50, 50) * 0.7f, MathHelper.Lerp(0, MathHelper.TwoPi*2, lerper), TPtele.Size() / 2, new Vector2(MathHelper.Lerp(0, 2, lerper), MathHelper.Lerp(0, 3, lerper)), SpriteEffects.None, 1);
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        //ai[0] = 0 - 2 = idle animation
        //ai[0] = 3 = angry animation
        //ai[0] = 4 = cocoon animation
        public Vector2 offset = new Vector2(0, 0);
        public Vector2 auraPos = new Vector2(0, 0);
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget || !DLCCalamityConfig.Instance.EternityPriorityOverRev || !DLCWorldSavingSystem.EternityRev) 
                return true;
            //return true;
            if (auraPos == Vector2.Zero)
                auraPos = npc.Center;
            //move aura towards brimstone ele
            auraPos = Vector2.Lerp(auraPos, npc.Center, 0.03f);
            Player target = Main.player[npc.target];
            //debuff if too far away
            if (target.Distance(auraPos) > 220 * 4)
            {
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 2);
                //target.AddBuff(BuffID.Obstructed, 2);
            }
            Vector2 totarget = (target.Center - npc.Center).SafeNormalize(Vector2.Zero);

            //face the player
            if (npc.Center.X > target.Center.X) 
                npc.spriteDirection = -1;
            else
                npc.spriteDirection = 1;
            npc.ai[1]++;

            //change random Y offset every 3 seconds
            if (npc.ai[1] % 180 == 0)
            {
                
                offset = new Vector2(Main.rand.Next(900, 900), Main.rand.Next(-200, 200));
                
                
                NetSync(npc);
            }
            offset.X = Math.Abs(offset.X) * -npc.spriteDirection;
            npc.ai[2] += 1f;

            //animations
            if (npc.ai[2] > 1000)
            {
                npc.ai[0] = 3;
            }
            else
            {
                npc.ai[0] = 0;
            }
            if (npc.ai[2] > 1600)
            {
                npc.ai[0] = 4;
            }


            //stop moving when cocoon phsae and reset timers
            if (npc.ai[2] == 1600)
            {
                npc.defense = 150;
                for (int i = 0; i < 3; i++)
                {
                    NPC.NewNPCDirect(npc.GetSource_FromAI(), npc.Center, ModContent.NPCType<Brimling>(), 0, npc.whoAmI, 0, Main.rand.NextFloat(0, MathHelper.TwoPi));
                }
                for (int i = 0; i < 2; i++)
                {
                    NPC.NewNPCDirect(npc.GetSource_FromAI(), npc.Center, ModContent.NPCType<Brimling>(), 0, npc.whoAmI, 1, Main.rand.Next(0, 100));
                }
            }
            if (npc.ai[2] > 1600)
            {
                
                npc.ai[1] = 0;
                npc.velocity *= 0.1f;
                if (!NPC.AnyNPCs(ModContent.NPCType<Brimling>()))
                {
                    npc.ai[2] = 0;
                    npc.defense = 15;
                }
                return false;
            }

            //teleport after 10 seconds
            if (npc.ai[1] >= 600)
            {
                //telegraph dust and sound
                Vector2 telepos = Vector2.Zero;
                if (npc.Center.X > target.Center.X)
                    telepos = target.Center + new Vector2(-300, 0);
                else
                    telepos = target.Center + new Vector2(300, 0);
                for (int i = 0; i < 10; i++)
                    Dust.NewDustDirect(telepos, 0, 0, DustID.LifeDrain, Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)).noGravity = true;
                if (npc.ai[1] == 600)
                {
                    SoundEngine.PlaySound(SoundID.Item109, target.Center);
                }
                //teleport and dust
                if (npc.ai[1] >= 700)
                {
                    for (int i = 0; i < 200; i++)
                        Dust.NewDustDirect(npc.Center, 0, 0, DustID.LifeDrain, Main.rand.Next(-10, 10), Main.rand.Next(-10, 10), Scale:2).noGravity = true;
                    npc.Center = telepos;
                    npc.ai[1] = 0;
                    offset.X *= -1;
                    SoundEngine.PlaySound(SoundID.Item109, target.Center);
                    
                }
                NetSync(npc);
            }

            
            //moving to target position
            Vector2 topos = (target.Center + offset - npc.Center).SafeNormalize(Vector2.Zero);
            Vector2 targVel = topos * npc.Distance(target.Center + offset) / 30;
            if (targVel.Length() > 20) 
                targVel = targVel.SafeNormalize(Vector2.Zero)*20;
            npc.velocity = Vector2.Lerp(npc.velocity, targVel, 0.05f);

            //choose an attack every second
            int attack = 0;
            if (npc.ai[1] % 60 == 0)
            {
                attack = Main.rand.Next(1, 5);
            }
            Vector2 eyePos = npc.Center + new Vector2(10 * npc.spriteDirection, -60);
            Vector2 toTargetfromEye = (target.Center  - eyePos).SafeNormalize(Vector2.Zero);

            //shoot darts in a line
            if (attack == 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), eyePos, toTargetfromEye * (i / 2f + 3), ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
                SoundEngine.PlaySound(SoundID.Item20, eyePos);
            }
            //shoot a spread of darts
            if (attack == 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), eyePos, toTargetfromEye.RotatedBy(MathHelper.ToRadians((i - 1)*5 - 2.5f)) * 4, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
                SoundEngine.PlaySound(SoundID.Item20, eyePos);
            }
            //shoot the scal small homing thing that explodes into darts
            if (attack == 3)
            {
                SoundEngine.PlaySound(SoundID.Item20, eyePos);
                Projectile.NewProjectileDirect(npc.GetSource_FromAI(), eyePos, toTargetfromEye * 4, ModContent.ProjectileType<SCalBrimstoneFireblast>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
            }
            //shoot 2 inferno fork things that explode shortly
            if (attack == 4)
            {
                SoundEngine.PlaySound(SoundID.Item20, eyePos);
                for (int i = 0; i < 2; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), eyePos, toTargetfromEye.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-50, 50))) * 10, ModContent.ProjectileType<BrimstoneHellfireball>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    proj.timeLeft = 20;
                    proj.netUpdate = true;
                }
            }
            return false;
        }

    }
}
