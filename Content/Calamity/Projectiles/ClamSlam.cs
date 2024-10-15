using CalamityMod;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Projectiles.Melee;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Humanizer;
using Luminance.Core.Graphics;
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

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    public class ClamSlam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/ClamCrusherFlail";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;

            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Vector2 origin = new Vector2(t.Width() / 2, t.Height() / 2);
            if (Projectile.ai[0] == 0 || Projectile.ai[1] < 0) origin.Y = Projectile.spriteDirection == -1 ? 20 : t.Height() - 20;
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, origin , Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.friendly = false;
            Projectile.ai[0] = 1;
            Projectile.ai[1] = 0;
            Projectile.timeLeft = 200;
            Projectile.ai[2] = (Projectile.Center - target.Center).SafeNormalize(Vector2.Zero).ToRotation();
            
            SoundEngine.PlaySound(GiantClam.SlamSound , Projectile.Center);
            for (int i = 0; i < 30; i++)
            {
                Particle bubble = new Bubble(Projectile.Center, new Vector2(Main.rand.NextFloat(2, 10)).RotatedByRandom(MathHelper.TwoPi), Main.rand.NextFloat(1, 2), 20);
                bubble.Spawn();
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner == null || !owner.active || owner.dead || !owner.HasEffect<MolluskEffect>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.CritChance = 0;
            
            if (Projectile.ai[0] == 0)
            {
                if (owner.velocity.Y == 0)
                {
                    owner.CalamityAddon().ClamSlamTime = 0;
                    for (int i = 0; i < 30; i++)
                    {
                        Particle bubble = new Bubble(Projectile.Center, new Vector2(Main.rand.NextFloat(2, 10)).RotatedByRandom(MathHelper.TwoPi), Main.rand.NextFloat(1, 2), 20);
                        bubble.Spawn();
                    }
                }
                if (owner.CalamityAddon().ClamSlamTime == 0)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.timeLeft = 2;
                Projectile.Center = owner.Center;
                Projectile.rotation = owner.velocity.ToRotation() + MathHelper.PiOver2;
                if (owner.direction == -1)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation += MathHelper.Pi;
                }
                else
                {
                    Projectile.spriteDirection = 1;
                }
                Projectile.Opacity = 1;
                if (owner.velocity.Length() > 15)
                {
                    Particle bubble = new Bubble(Projectile.position + new Vector2(Main.rand.NextFloat(0, Projectile.width), Main.rand.NextFloat(0, Projectile.height)), Vector2.Zero, 1, 20);
                    bubble.Spawn();
                }
            }
            else
            {
                Projectile.Opacity = 1;

                Projectile.rotation += Projectile.velocity.Length() / 40f;

                Projectile.velocity.Y += 0.1f;

                if (Projectile.ai[1] == 0)
                {
                    int cooldown = 60 * 20;
                    owner.CalamityAddon().ClamSlamCooldown = cooldown;
                    CooldownBarManager.Activate("MolluskEnchantCooldown", ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Items/Accessories/Enchantments/MolluskEnchant").Value, Color.SlateBlue,
                    () => owner.CalamityAddon().ClamSlamCooldown / (float)(cooldown), activeFunction: () => owner.HasEffect<MolluskEffect>());
                    owner.CalamityAddon().ClamSlamTime = 0;
                    //SoundEngine.PlaySound(SoundID.Item96, Projectile.Center);
                    Projectile.velocity = new Vector2(30, 0).RotatedBy(Projectile.ai[2]);
                    Projectile.ai[1]++;
                    owner.velocity = Projectile.velocity / 1.5f;
                    owner.CalamityAddon().ClamSlamIframes = 20;
                    if (Projectile.timeLeft > cooldown)
                    {
                        Projectile.timeLeft = cooldown;
                    }
                }

            }
            base.AI();
        }
        public override bool? CanHitNPC(NPC target)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner == null || !owner.active || owner.dead || !owner.HasEffect<MolluskEffect>())
            {
                Projectile.Kill();
                return false;
            }
            if (Projectile.ai[0] == 1 || owner.velocity.Length() < 15)
            {
                return false;
            }   
            return base.CanHitNPC(target);
        }
    }
    
}
