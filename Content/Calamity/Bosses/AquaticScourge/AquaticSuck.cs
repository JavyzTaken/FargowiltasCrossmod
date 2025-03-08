using FargowiltasSouls;
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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge
{
    public class AquaticSuck : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ToxicBubble;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 450;
            Projectile.ai[1] = 200;
            Projectile.ai[2] = 200;
            Projectile.light = 1f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(ProjectileID.SandnadoHostile);
            Texture2D wind = TextureAssets.Projectile[ProjectileID.SandnadoHostile].Value;
            for (int i = (int)Projectile.ai[2]; i > (int)Projectile.ai[1]; i--)
            {
                float opacity = 1;
                if (i < 100)
                {
                    opacity = i / 100f;
                }
                if (i < Projectile.ai[2] + 50)
                {
                    opacity = i / (Projectile.ai[2] + 70);
                }
                Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(20, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2) + new Vector2(2, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2) * i * 5 * (i / 100f);
                Vector2 pos2 = Projectile.Center + new Vector2(20, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2) + new Vector2(2, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2) * i * 5 * (i / 100f);
                Color color = new Color(100, 178, 128, 120) * 0.3f * opacity;
                if (Lighting.GetColor(pos2.ToTileCoordinates()).Equals(Color.Black))
                {
                    color = Color.Black * 0;
                }
                //Main.NewText(wind + "" + pos + "" + color + "" + opacity);
                Main.EntitySpriteDraw(wind, pos, null, color, Projectile.localAI[0] + MathHelper.ToRadians(i), wind.Size() / 2, 1 + i / (7f), SpriteEffects.None);
            }
            Projectile.localAI[0] += MathHelper.ToRadians(3);
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 449)
            {
                Projectile.ai[2] = 200;
                Projectile.ai[1] = 200;
            }
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (owner == null || !owner.active || !owner.HasValidTarget)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center;
            Projectile.rotation = owner.rotation;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active)
                {
                    float angleDif = (MathHelper.ToDegrees(FargoSoulsUtil.RotationDifference(Projectile.rotation.ToRotationVector2(), Projectile.AngleTo(Main.player[i].Center).ToRotationVector2())));
                    //Main.NewText(angleDif);
                    if (angleDif - 90 < 30 && angleDif - 90 > -30)
                    {
                        Main.player[i].velocity -= Main.player[i].AngleFrom(Projectile.Center).ToRotationVector2() * 0.15f;
                    }
                }
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active)
                {
                    float angleDif = (MathHelper.ToDegrees(FargoSoulsUtil.RotationDifference(Projectile.rotation.ToRotationVector2(), Projectile.AngleTo(Main.projectile[i].Center).ToRotationVector2())));
                   
                    if (angleDif + 90 < 30 && angleDif + 90 > -30 && Main.projectile[i].type == ModContent.ProjectileType<ToxicGas>() && Main.projectile[i].ai[1] == -1)
                    {
                        //Main.NewText(angleDif - 90);
                        Main.projectile[i].ai[1] = Projectile.whoAmI;
                    }else if (!(angleDif + 90 < 30 && angleDif + 90 > -30) && Main.projectile[i].type == ModContent.ProjectileType<ToxicGas>())
                    {
                        Main.projectile[i].ai[1] = -1;
                    }
                }
            }

            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1]--;
            }
            if (Projectile.timeLeft < 200)
            {
                if (Projectile.ai[2] > 0)
                    Projectile.ai[2]--;
                if (Projectile.ai[2] <= 0)
                {
                    Projectile.Kill();
                }
            }

            base.AI();
        }
    }
}
