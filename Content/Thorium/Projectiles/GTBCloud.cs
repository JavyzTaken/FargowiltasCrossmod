using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Collections.Generic;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    public class GTBCloud : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.scale = 8f;
            Projectile.width = 54;
            Projectile.height = 28;
            //Projectile.friendly = false;
            //Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 500;
            DrawOffsetX = 188;
            DrawOriginOffsetY = 98;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void PostAI()
        {
            if (Projectile.timeLeft < 100)
            {
                Projectile.alpha = (int)((100 - Projectile.timeLeft) * 2.55f);
            }
            else if (Projectile.timeLeft > 450)
            {
                Projectile.alpha = (int)((Projectile.timeLeft - 450) * 5.1f);
            }
            else
            {
                Projectile.alpha = 0;
            }

            Projectile.frame = (Projectile.frameCounter++ / 10) % 6;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.rand.NextBool(100))
                {
                    Vector2 spawnPos = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Projectile.height / 2f);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPos, Vector2.Zero, ModContent.ProjectileType<GFBZapTelegraph>(), 12, 0f, Main.myPlayer);
                }
                if (Main.rand.NextBool(Main.player[(int)Projectile.ai[0]].Center.Y < Projectile.Center.Y + Projectile.width / 2 ? 120 : 240))
                {
                    Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2f, Projectile.height / 3f);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPos, Vector2.Zero, ModContent.ProjectileType<KluexOrb>(), 15, 2f, Main.myPlayer);
                }
            }
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GFBZapTelegraph : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.hide = true;
            Projectile.timeLeft = 30;
            Projectile.width = 16;
            Projectile.height = 16;
        }

        public override string Texture => "FargowiltasCrossmod/icon";
        public override bool PreDraw(ref Color lightColor) => false;

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(2) ? DustID.Sand : DustID.Electric);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.UnitY * 8f, ModContent.ProjectileType<GrandThunderBirdZap>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
