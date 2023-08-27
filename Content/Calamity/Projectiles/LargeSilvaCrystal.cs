using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.Collections.Generic;
using FargowiltasSouls.Core.Toggler;
using System.IO;
using System;
using Terraria.Audio;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class LargeSilvaCrystal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/SilvaCrystal";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Silva Crystal");
            //Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 300;
            Projectile.width = 22;
            Projectile.height = 52;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.scale = 2.3f;
            Projectile.netImportant = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> main = TextureAssets.Projectile[Type];
            Asset<Texture2D> left = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Projectiles/SilvaCrystalShard");
            Asset<Texture2D> right = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Projectiles/SilvaCrystalShard2");
            Asset<Texture2D> shine = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion];
            Color color = new Color(250, 250, 250) * 0.5f;
            Color color2 = new Color(250, 0, 0) * 0.5f;
            if (Projectile.owner != -1)
            {
                Player owner = Main.player[Projectile.owner];
                for (int i = 0; i < owner.armor.Length; i++)
                {
                    if (owner.armor[i].type == ModContent.ItemType<SilvaEnchantment>() && owner.hideVisibleAccessory[i] == true)
                    {
                        color *= 0;
                    }
                }

            }
            if (Projectile.ai[0] == 0)
                Main.EntitySpriteDraw(main.Value, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, main.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            else
            {
                Main.EntitySpriteDraw(left.Value, Projectile.Center - Main.screenPosition - new Vector2(4 + Projectile.ai[1] / 2.5f, 4) * Projectile.scale, null, Color.Lerp(color, color2, Projectile.ai[1] / 30), Projectile.rotation, left.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(right.Value, Projectile.Center - Main.screenPosition + new Vector2(4 + Projectile.ai[1] / 2.5f, 0) * Projectile.scale, null, Color.Lerp(color, color2, Projectile.ai[1] / 30), Projectile.rotation, right.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shine.Value, Projectile.Center - Main.screenPosition, null, new Color(250, 0, 0, 100) * 0.75f, MathHelper.ToRadians(timer), shine.Size() / 2, Projectile.ai[1] / 15, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shine.Value, Projectile.Center - Main.screenPosition, null, new Color(250, 0, 0, 100) * 0.75f, MathHelper.ToRadians(timer * 2), shine.Size() / 2, Projectile.ai[1] / 15, SpriteEffects.None, 0);
            }
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public int timer;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(timer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.Read7BitEncodedInt();
        }

        public override void AI()
        {
            if (Projectile.owner == -1)
            {
                Projectile.Kill();
                return;
            }
            Player owner = Main.player[Projectile.owner];
            if (!CheckActive(owner))
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center;
            if (owner.GetModPlayer<CrossplayerCalamity>().SilvaTimer == 1)
            {
                SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
                for (int i = 0; i < 20; i++)
                {
                    Vector2 speed = new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(360f / 20 * i));
                    Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.GreenFairy, (int)speed.X, (int)speed.Y, Scale: 1.5f);
                    dust.noGravity = true;
                }
            }
            if (Projectile.ai[0] == 1)
            {
                int maxtime = 600;
                if (timer == 0)
                {
                    for (int i = 0; i < 5; i++)
                        Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, Main.rand.Next(-7, -7)).RotatedByRandom(MathHelper.ToRadians(45)), ModLoader.GetMod("CalamityMod").Find<ModGore>("CrawlerEmerald").Type);
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenDeath"), Projectile.Center);
                }
                if (timer == 0 && owner.GetModPlayer<CrossplayerCalamity>().Auric && owner.GetToggleValue("AuricExplosions"))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AuricExplosion>(), 1000, 0, Main.myPlayer, 22);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AuricExplosion>(), 500, 0, Main.myPlayer, 11, 1);
                }
                timer++;

                if (timer <= 10)
                {
                    double x = timer / 10d;
                    Projectile.ai[1] = MathHelper.Lerp(0, 30, (float)Math.Sin(x * Math.PI / 2));
                }
                if (timer >= maxtime - 30 && timer <= maxtime)
                {
                    double x = (timer - (maxtime - 30)) / 30d;
                    Projectile.ai[1] = MathHelper.Lerp(30, 0, (float)Math.Sin(x * Math.PI / 2));
                }
                if (timer == maxtime)
                {
                    Projectile.ai[0] = 0;
                    timer = 0;
                }
                if (timer % 7 == 0 && Main.myPlayer == owner.whoAmI && owner.GetToggleValue("SilvaProjectiles"))
                {
                    if (Main.rand.NextBool() && owner.GetModPlayer<CrossplayerCalamity>().Auric && owner.GetToggleValue("AuricLightning"))
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AuricLightning>(), 1000, 0, Main.myPlayer, MathHelper.ToRadians(Main.rand.Next(0, 360)), 0.6f);
                    }
                    if (Main.rand.NextBool())
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 7).RotatedByRandom(MathHelper.TwoPi), ProjectileID.FairyQueenMagicItemShot, 300, 0, Main.myPlayer, Main.rand.NextFloat(), Main.rand.NextFloat());
                    }

                    else
                    {
                        int npc = -1;
                        for (int i = 0; i < Main.npc.Length; i++)
                        {
                            NPC n = Main.npc[i];
                            if (n.active && n.CanBeChasedBy(ProjectileID.RainbowCrystalExplosion) && (npc == -1 || n.Distance(Projectile.Center) < Main.npc[npc].Distance(Projectile.Center)) && n.Distance(Projectile.Center) < 800)
                            {
                                npc = i;
                            }
                        }
                        if (npc != -1)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.npc[npc].Center + new Vector2(Main.rand.Next(-50, 50), Main.rand.Next(-50, 50)) + Main.npc[npc].velocity * 30, Vector2.Zero, ProjectileID.RainbowCrystalExplosion, 450, 0, Main.myPlayer, Main.rand.NextFloat(), Projectile.whoAmI);
                        }
                        else
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, Main.rand.Next(100, 700)).RotatedByRandom(MathHelper.TwoPi), Vector2.Zero, ModContent.ProjectileType<CalamityMod.Projectiles.Summon.SilvaCrystalExplosion>(), 450, 0, Main.myPlayer, Main.rand.NextFloat(), Projectile.whoAmI);
                        }
                    }
                }
            }
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active || !owner.GetModPlayer<CrossplayerCalamity>().Silva || !owner.GetToggleValue("SilvaCrystal"))

            {
                owner.ClearBuff(ModContent.BuffType<LifeShell>());
                return false;
            }
            if (owner.HasBuff(ModContent.BuffType<LifeShell>()))
            {
                Projectile.timeLeft = 2;
            }
            return true;
        }
    }
}
