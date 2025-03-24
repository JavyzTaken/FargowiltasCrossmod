using FargowiltasCrossmod.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

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
            if (Projectile.localAI[2] == 2)
                t = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/DesertScourge/SandChunk");
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
            if (Projectile.localAI[2] == 0)
                Projectile.localAI[2] += Main.rand.Next(1, 8);
            if (Projectile.timeLeft % 5 == 0)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, Scale: Projectile.scale).noGravity = true;
            }
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (owner == null || !owner.active)
            {
                Projectile.Kill();
                return;
            }
            float speed = 10;
            if (WorldSavingSystem.MasochistModeReal)
                speed = 14;
            Projectile.velocity = (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
            if (Projectile.Hitbox.Intersects(owner.Hitbox))
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[2] == 2)
                Projectile.rotation += MathHelper.PiOver2 * 0.05f;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
