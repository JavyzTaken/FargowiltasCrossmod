using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.DataStructures;
using System.Collections.Generic;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExaltation
{
    
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class TarragonRoot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Healing/SilvaOrb";
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 200;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.scale = 3;
            Projectile.hide = true;
        }
        public override void SetStaticDefaults()
        {

        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade, Scale: 2);
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade, Scale: 2);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft > 150 || Projectile.timeLeft < 60)
            {
                return false;
            }
            return Collision.CheckAABBvLineCollision(targetHitbox.Location.ToVector2(), targetHitbox.Size(), Projectile.Center, Projectile.Center - new Vector2(0, Projectile.localAI[0] * 24).RotatedBy(Projectile.ai[1] + MathHelper.Pi));

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> spike = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Enemy/CragmawSpike");
            Main.EntitySpriteDraw(spike.Value, Projectile.Center - Main.screenPosition, null, Lighting.GetColor(Projectile.Center.ToTileCoordinates()), Projectile.ai[1] + MathHelper.Pi, new Vector2(spike.Width() / 2, spike.Height() / 1.14f), new Vector2(Projectile.scale, Projectile.localAI[0]), SpriteEffects.None, 0);
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, Lighting.GetColor(Projectile.Center.ToTileCoordinates()), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 197 && Projectile.ai[0] > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float angle = Projectile.ai[1] + MathHelper.ToRadians(Main.rand.Next(-30, 30));
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, Projectile.height).RotatedBy(angle), Vector2.Zero, Projectile.type, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(310), 0, Main.myPlayer, Projectile.ai[0] - 1, angle);

                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Main.rand.NextBool())
                    {
                        Projectile.ai[1] += MathHelper.PiOver2;
                    }
                    else
                    {
                        Projectile.ai[1] -= MathHelper.PiOver2;
                    }
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.timeLeft < 150 && Projectile.localAI[0] < 10 && Projectile.timeLeft > 100)
            {
                if (Projectile.timeLeft == 149)
                {
                    SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
                }
                Projectile.localAI[0] += 2;
            }
            if (Projectile.timeLeft < 50 && Projectile.localAI[0] > 0)
            {
                Projectile.localAI[0] -= 2;
            }
        }
    }
}
