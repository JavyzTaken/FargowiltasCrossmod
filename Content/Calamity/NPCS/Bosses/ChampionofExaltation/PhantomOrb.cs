using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using System.IO;
using Terraria.DataStructures;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExaltation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class PhantomOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/PhantomMine";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {

            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.scale = 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, new Color(240, 240, 240, 100), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(position);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            position = reader.ReadVector2();
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Main.rand.NextBool())
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Scale: 1.2f);
                }
                else
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Scale: 0.5f);
                }
            }
            SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 center = Projectile.Center;
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.Center = center;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 30; i++)
            {
                if (Main.rand.NextBool())
                {
                    if (Main.rand.NextBool())
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Scale: 1.2f);
                        dust.velocity *= 3;
                    }
                    else
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Scale: 0.5f);
                        dust.velocity *= 3;
                    }
                }
                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Scale: 1.7f);
                dust2.noGravity = true;
                dust2.velocity *= 5;
                Dust dust3 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch);
                dust3.velocity *= 2;
            }
        }
        public Vector2 position = Vector2.Zero;
        public override void AI()
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            Vector2 offset = new Vector2(0, 500).RotatedBy(Projectile.ai[1]);
            if (Projectile.timeLeft <= 60)
            {
                float painis = 1 - Projectile.timeLeft / 60f;
                offset = Vector2.Lerp(new Vector2(0, 500).RotatedBy(Projectile.ai[1]), Vector2.Zero.RotatedBy(Projectile.ai[1]), painis * painis);

            }
            if (Projectile.timeLeft > 60)
            {
                position = target.Center;
            }
            Projectile.ai[1] += MathHelper.ToRadians(2);
            Projectile.Center = position + offset;
        }
    }
}
