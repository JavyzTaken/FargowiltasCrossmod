using CalamityMod;
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
    public class FathomAngler : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_" + NPCID.AnglerFish;
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 25;
            Projectile.friendly = false;
            Projectile.hostile = false;

            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Asset<Texture2D> light = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/Bloom");
            Player owner = Main.player[Projectile.owner];
            if (owner == null || owner.dead || !owner.active)
            {
                Projectile.Kill();
                return false;
            }
            float scale = owner.ForceEffect<FathomSwarmerEffect>() ? 2.2f : 1.6f;

            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Main.EntitySpriteDraw(light.Value, Projectile.Center - Main.screenPosition, null, Color.Yellow  * 0.7f, Projectile.rotation, new Vector2(light.Width(), light.Height()) / 2, Projectile.scale * scale, SpriteEffects.None);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 28, 48, 28), lightColor, Projectile.rotation, new Vector2(t.Width(), t.Height() / 6) / 2, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
        public override void AI()
        {
            
            Player owner = Main.player[Projectile.owner];
            if (owner == null || owner.dead || !owner.active || !owner.HasEffect<FathomSwarmerEffect>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            if (Main.myPlayer == owner.whoAmI)
            {
                float speed = owner.ForceEffect<FathomSwarmerEffect>() ? 0.06f : 0.035f;
                float acceleration = owner.ForceEffect<FathomSwarmerEffect>() ? 0.06f : 0.04f;

                Vector2 pos = Main.MouseWorld;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.Distance(pos) * speed, acceleration);
                Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
                }
            }
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0));
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }
            float size = owner.ForceEffect<FathomSwarmerEffect>() ? 220 : 160f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                
                if (Main.projectile[i].active && Main.projectile[i].hostile && Projectile.Distance(Main.projectile[i].Center) < size && Main.projectile[i].velocity.Length() > 1.5f)
                {
                    //Main.NewText("grah");
                    Main.projectile[i].velocity *= 0.95f;
                }
            }
           
        }
    }
}
