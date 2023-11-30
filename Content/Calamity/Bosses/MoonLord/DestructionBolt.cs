using CalamityMod.Buffs.DamageOverTime;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.MoonLord
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DestructionBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/DestructionBolt";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 30;
            Projectile.timeLeft = 200;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) / 2 - Main.screenPosition, null, lightColor * (1 - ((float)i / Projectile.oldPos.Length)), Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);

            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CalamityMod.Dusts.ShadowspecBarDust>());
            }
        }
        public override void AI()
        {
            base.AI();
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            int p = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
            if (p >= 0 && Projectile.Distance(Main.player[p].Center) <= 500)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Main.player[p].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10, 0.03f);
            }
        }
    }
}
