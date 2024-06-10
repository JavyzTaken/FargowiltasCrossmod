
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PermafrostHeldWeapon : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Magic/IcicleTrident";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.timeLeft = 60 * 10;
            Projectile.hostile = false;
            Projectile.friendly = false;

            Projectile.light = 0.5f;
            Projectile.tileCollide = false;
            //Projectile.coldDamage = true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        ref float Weapon => ref Projectile.ai[0];
        ref float TimeLeft => ref Projectile.localAI[2];
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Vector2 origin = new Vector2(0, t.Height());
            float maxscale = 1;
            SpriteEffects se = SpriteEffects.None;
            if (Weapon == 1)
            {
                Main.instance.LoadItem(ModContent.ItemType<FrostbiteBlaster>());

                t = TextureAssets.Item[ModContent.ItemType<FrostbiteBlaster>()];
                origin = new Vector2(10, t.Height() / 2);
                float degrees = MathHelper.ToDegrees(Projectile.rotation + MathHelper.Pi);
                if (degrees > 270 || degrees < 90) se = SpriteEffects.FlipVertically;
            }
            if (Weapon == 2)
            {
                int asset = ModContent.ProjectileType<ArcticPaw>();
                Main.instance.LoadProjectile(asset);

                t = TextureAssets.Projectile[asset];
                origin = t.Size() / 2;
                maxscale = 2;
            }
            if (Weapon == 3)
            {
                int asset = ModContent.ItemType<WintersFury>();
                Main.instance.LoadItem(asset);

                t = TextureAssets.Item[asset];
                origin = new Vector2(2, 28);
                float degrees = MathHelper.ToDegrees(Projectile.rotation + MathHelper.Pi);
                if (degrees > 270 || degrees < 90) se = SpriteEffects.FlipVertically;
            }
            if (Weapon == 4)
            {
                int asset = ModContent.ItemType<AbsoluteZero>();
                Main.instance.LoadItem(asset);

                t = TextureAssets.Item[asset];
                origin = new Vector2(0, t.Height());
                maxscale = 1.5f;
            }
            if (Weapon == 5)
            {
                int asset = ModContent.ItemType<EternalBlizzard>();
                Main.instance.LoadItem(asset);

                t = TextureAssets.Item[asset];
                origin = new Vector2(10, t.Height() / 2);
                float degrees = MathHelper.ToDegrees(Projectile.rotation + MathHelper.Pi);
                if (degrees > 270 || degrees < 90) se = SpriteEffects.FlipVertically;
            }
            if (Projectile.localAI[0] < maxscale)
            {
                Projectile.localAI[0] += 0.03f;
            }
            float scale = MathHelper.Lerp(0, Projectile.scale, Projectile.localAI[0]);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, origin, scale, se);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[2]];

            if (owner == null || !owner.active || owner.type != ModContent.NPCType<PermafrostBoss>())
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 1;
                TimeLeft = Weapon switch
                {
                    0 => 60,
                    1 => 54,
                    2 => 79,
                    3 => 180,
                    4 => 120,
                    _ => 165
                };
                Projectile.netUpdate = true;
            }

            float angle = Projectile.ai[1];
            float scale = 1;
            //Ice Trident
            if (Weapon == 0)
            {
                angle += MathHelper.PiOver4;
                if (owner.ai[1] < 60)
                    Projectile.rotation = owner.Center.AngleTo(Main.player[owner.target].Center) + MathHelper.PiOver4;
                if (Projectile.localAI[0] < scale)
                {
                    Vector2 thing = new Vector2(0, -50 * Projectile.localAI[0]).RotatedBy(Projectile.rotation);
                    if (Weapon == 0) thing = thing.RotatedBy(MathHelper.PiOver4);
                    for (int i = 0; i < 5; i++)
                        Dust.NewDustDirect(Projectile.Center + thing * Main.rand.NextFloat(0f, 1f), 0, 0, DustID.SnowflakeIce).noGravity = true;
                }
                if (TimeLeft == 1)
                {
                    Vector2 thing = new Vector2(0, -50 * Projectile.localAI[0]).RotatedBy(Projectile.rotation);
                    if (Weapon == 0) thing = thing.RotatedBy(MathHelper.PiOver4);
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDustDirect(Projectile.Center + thing * Main.rand.NextFloat(0f, 1f), 0, 0, DustID.SnowflakeIce).noGravity = true;
                    }
                }
            }
            //Frostbite Blaster
            if (Weapon == 1)
            {
                if (Projectile.localAI[0] < scale)
                {
                    Vector2 off = new Vector2(Main.rand.Next(0, 56) * Projectile.localAI[0], 0).RotatedBy(Projectile.rotation);
                    for (int i = 0; i < 5; i++)
                        Dust.NewDustDirect(Projectile.Center + off, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
                }
                if (owner.HasValidTarget)
                {
                    Projectile.rotation = owner.AngleTo(Main.player[owner.target].Center);
                }
                
                if (TimeLeft == 1)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Vector2 off = new Vector2(Main.rand.Next(0, 56) * Projectile.localAI[0], 0).RotatedBy(Projectile.rotation);
                        Dust.NewDustDirect(Projectile.Center + off, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
                    }
                    Projectile.Kill();
                }
            }
            //Arctic Bear Paw
            if (Weapon == 2)
            {
                Projectile.Opacity = 0.7f;
                Projectile.rotation = owner.velocity.ToRotation() + MathHelper.PiOver2;
                scale = 2;
                if (Projectile.localAI[0] < scale)
                {
                    Projectile.Resize((int)(Projectile.localAI[0] * 30), (int)(Projectile.localAI[0] * 30));
                    for (int i = 0; i < 5; i++)
                        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
                }
                if (TimeLeft == 1)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
                    }
                }
            }
            //Winter's fury
            if (Weapon == 3)
            {
                if (owner.HasValidTarget)
                {
                    Projectile.rotation = owner.AngleTo(Main.player[owner.target].Center);
                }
                if (Projectile.localAI[0] < scale)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustDirect(Projectile.position + new Vector2(Main.rand.Next(-26, 0) * Projectile.localAI[0], Main.rand.Next(0, 30) * Projectile.localAI[0]).RotatedBy(Projectile.rotation), 0, 0, DustID.SnowflakeIce).noGravity = true;
                    }
                }
                if (TimeLeft == 1)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDustDirect(Projectile.position + new Vector2(Main.rand.Next(-26, 0) * Projectile.localAI[0], Main.rand.Next(0, 30) * Projectile.localAI[0]).RotatedBy(Projectile.rotation), 0, 0, DustID.SnowflakeIce).noGravity = true;
                    }
                }
            }
            //Absolute zero
            if (Weapon == 4)
            {
                scale = 1.5f;
                if (Projectile.localAI[0] < scale)
                {
                    Vector2 off = new Vector2(Main.rand.Next(0, 40) * Projectile.localAI[0], Main.rand.Next(-40, 0) * Projectile.localAI[0]).RotatedBy(Projectile.rotation);
                    for (int i = 0; i < 5; i++)
                        Dust.NewDustDirect(Projectile.Center + off, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
                }
                if (owner.HasValidTarget)
                {
                    float x = 1- ((TimeLeft - 20) / 10f);
                    if (TimeLeft < 20) x = 1;
                    if (TimeLeft > 30) x = 0;
                    float angleAdd = MathHelper.Lerp(0, MathHelper.PiOver2, x);
                    Projectile.rotation = owner.AngleTo(Main.player[owner.target].Center) + angleAdd;
                }
                if (TimeLeft == 1)
                {

                    for (int i = 0; i < 50; i++)
                    {
                        Vector2 off = new Vector2(Main.rand.Next(0, 40) * Projectile.localAI[0], Main.rand.Next(-40, 0) * Projectile.localAI[0]).RotatedBy(Projectile.rotation);
                        Dust.NewDustDirect(Projectile.Center + off, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
                    }
                }
            }
            //Eternal Blizzard
            if (Weapon == 5)
            {
                if (Projectile.localAI[0] < scale)
                {
                    Vector2 off = new Vector2(Main.rand.Next(0, 56) * Projectile.localAI[0], 0).RotatedBy(Projectile.rotation);
                    for (int i = 0; i < 5; i++)
                        Dust.NewDustDirect(Projectile.Center + off, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
                }
                if (owner.HasValidTarget)
                {
                    Projectile.rotation = owner.AngleTo(Main.player[owner.target].Center);
                }
                if (TimeLeft == 1)
                {

                    for (int i = 0; i < 50; i++)
                    {
                        Vector2 off = new Vector2(Main.rand.Next(0, 56) * Projectile.localAI[0], 0).RotatedBy(Projectile.rotation);
                        Dust.NewDustDirect(Projectile.Center + off, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
                    }
                }
            }
            TimeLeft--;
            if (TimeLeft == 0)
                Projectile.Kill();
            Projectile.Center = owner.Center;
            
            
            base.AI();
        }
    }
}
