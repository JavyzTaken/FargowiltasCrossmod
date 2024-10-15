using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Summon;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
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

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    public class AstralTrailStar : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/AstralStar";
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = -1;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Projectile.alpha = 50;
            base.SetDefaults();
        }
        public override Color? GetAlpha(Color lightColor) => new Color(200, 100, 250, Projectile.alpha);
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Asset<Texture2D> flame = TextureAssets.Extra[ExtrasID.FallingStar];
            //Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor with { A = 255}, Projectile.rotation, new Vector2(t.Width(), t.Height()) / 2, Projectile.scale, SpriteEffects.None);

            Projectile.DrawStarTrail(Color.LightSkyBlue * MathHelper.Lerp(0, 1, Projectile.localAI[0]*3), Color.White * MathHelper.Lerp(0, 1, Projectile.localAI[0]*3), MathHelper.Lerp(0, 10, Projectile.localAI[0]));
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
            base.OnHitPlayer(target, info);
        }
        public override void AI()
        {
            
            Player owner = Main.player[Projectile.owner];
            if (owner == null || owner.dead || !owner.active || !owner.HasEffect<AstralEffect>())
            {
                Projectile.Kill();
                return;
            }

            int maxStars = owner.ForceEffect<AstralEffect>() ? 20 : 10;
            Projectile.originalDamage = owner.ForceEffect<AstralEffect>() ? 200 : 50;
            
            if (Projectile.ai[0] > 10 && maxStars == 10)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.ai[0] == 10 && Projectile.ai[1] == 11 && maxStars == 10)
            {
                Projectile.ai[1] = 9;
            }

            Projectile.timeLeft = 2;
            if (Projectile.ai[1] < 10)
            {
                Projectile.ai[1]++;
            }
            if (Projectile.ai[0] < maxStars && Projectile.ai[1] == 10 && Main.myPlayer == owner.whoAmI)
            {
                Projectile p = Projectile.NewProjectileDirect(owner.GetSource_EffectItem<AstralEffect>(), Projectile.Center, new Vector2(-5, 0).RotatedBy(Projectile.rotation), Projectile.type, Projectile.originalDamage, Projectile.knockBack, owner.whoAmI, Projectile.ai[0] + 1, 0, Projectile.whoAmI);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p.whoAmI);
                }
                Projectile.ai[1]++;
            }
            Vector2 targetPos = owner.Center;
            if (Projectile.ai[0] > 0 && Main.projectile[(int)Projectile.ai[2]].active)
            {
                targetPos = Main.projectile[(int)Projectile.ai[2]].Center;
            }
            if (Projectile.Distance(targetPos) > 1200)
            {
                Projectile.Center = targetPos;
            }
            float dist = Projectile.Distance(targetPos);
            float x = (dist - 40) / 400;
            x = MathHelper.Clamp(x, 0, 1);
            Projectile.velocity = (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * MathHelper.Lerp(0, 50, x);
            Projectile.localAI[0] = x;
            Projectile.rotation += MathHelper.Lerp(0.01f, 0.5f, x);
            
        }
    }
}
