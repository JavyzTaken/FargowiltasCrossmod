
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.Audio;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.DesertScourge
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SuckedSandBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/SandBlast";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.scale = 2;
            
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, new Color(250, 250, 250), Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item2 with { Pitch = -0.75f, PitchVariance = 0.2f, MaxInstances = 10 }, Projectile.Center);
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 5 == 0)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, Scale:Projectile.scale).noGravity = true;
            }
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (owner == null || !owner.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.velocity = (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10;
            if (Projectile.Hitbox.Intersects(owner.Hitbox))
            {
                Projectile.Kill();
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
