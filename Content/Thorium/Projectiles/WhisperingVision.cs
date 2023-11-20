using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria.GameContent;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class WhisperingVision : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.width = 192;
            Projectile.height = 268;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        // ai[0]: timer
        //      0 - 1200: waiting to open
        //      1200 - 1260: opening
        //      1260 - 1500: firing
        //      1500 - 1560: closing
        //      1560: returns to 0

        public override void PostAI()
        {
            const int chargeTime = 180;

            //int leechType = ModContent.ProjectileType<WhisperingLeech>();
            //if (Main.player[Projectile.owner].ownedProjectileCounts[leechType] <= 0)
            //{
            //    if (Projectile.ai[2] <= 0 && Main.npc.Any(npc => npc.active && !npc.friendly && npc.Distance(Main.player[Projectile.owner].Center) < 640f))
            //    {
            //        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + 30 * Vector2.UnitY, Vector2.Zero, leechType, 20, 0f, Projectile.owner);
            //    }
            //    else
            //    {
            //        Projectile.ai[2]--;
            //    }
            //}

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= chargeTime + 360)
            {
                Projectile.frame = 0;
                Projectile.ai[0] = 0;
                return;
            }

            if (Projectile.ai[0] >= chargeTime + 300)
            {
                Projectile.frame = (int)MathHelper.Max(3 - (Projectile.ai[0] - chargeTime - 300) / 20, 0);
                return;
            }

            if (Projectile.ai[0] >= chargeTime + 60)
            {
                Projectile.frame = 3;

                if (Projectile.ai[1]-- <= 0)
                {
                    Projectile.ai[1] = 10;

                    Vector2 beamPoint = Projectile.Center - Vector2.UnitY * 70;
                    var targets = Main.npc.Where(npc => npc.active && Projectile.localNPCImmunity[npc.whoAmI] <= 0 && npc.Distance(beamPoint) < 480);
                    foreach (NPC target in targets)
                    {
                        Projectile beam = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), beamPoint, target.Center - beamPoint, ModContent.ProjectileType<WhisperingBeam>(), 0, 0f, Projectile.owner, 0f, Projectile.whoAmI);
                        beam.Center = target.Center;
                        beam.damage = Projectile.damage;
                        beam.Damage();
                        beam.damage = 0;
                        beam.Center = beamPoint;
                        Projectile.localNPCImmunity[target.whoAmI] = Projectile.localNPCHitCooldown;
                    }
                }
                return;
			}

            if (Projectile.ai[0] >= chargeTime)
            {
                Projectile.frame = (int)MathHelper.Min((Projectile.ai[0] - chargeTime) / 20, 3);
                return;
            }

            Projectile.frame = 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || !player.ThoriumDLC().WhisperingEnch)
            {
                return;
            }

            Projectile.timeLeft = 2;
            Projectile.Center = player.Center - Vector2.UnitY * (player.height + Projectile.height * 0.5f);
            Projectile.velocity = Vector2.Zero;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor with {A = 50};
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return Main.myPlayer == Projectile.owner; // only draw for the enchant user
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class WhisperingBeam : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.MedusaHeadRay}";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.MedusaHeadRay);
            Projectile.timeLeft = 20;
            Projectile.hide = false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.alpha = (int)MathHelper.Lerp(0f, 255f, Projectile.ai[0] / 20f);
            int vision = (int)Projectile.ai[1];
            if (vision < 0 || vision > 1000 || !Main.projectile[vision].active || Main.projectile[vision].type != ModContent.ProjectileType<WhisperingVision>())
            {
                Projectile.Kill();
                Main.NewText("test");
                return;
            }
            Projectile.position = Main.projectile[(int)Projectile.ai[1]].Center - Vector2.UnitY * 70f - Projectile.velocity;
            //Projectile.Center = Main.projectile[num830].Center - Projectile.velocity;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI / 2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ThoriumMod.Buffs.Insanity>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 63 - Projectile.alpha / 4);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[ProjectileID.MedusaHeadRay].Value;
            Vector2 position = Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition + Projectile.velocity;
            Main.EntitySpriteDraw(texture, position, texture.Bounds, GetAlpha(lightColor).Value, Projectile.rotation, new Vector2(texture.Width / 2f, Projectile.width / 2f), new Vector2(1, Projectile.velocity.Length() / texture.Height), SpriteEffects.None);
            return false;
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class WhisperingLeech : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.MoonLeech}";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.MoonLeech);
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D headTexture = TextureAssets.Projectile[ProjectileID.MoonLeech].Value;
            Texture2D chainTexture = TextureAssets.Extra[23].Value;
            Texture2D endTexture = TextureAssets.Extra[24].Value;

            Vector2 toOwner = Main.player[Projectile.owner].Center - Projectile.Center + Vector2.UnitY * 64f;
            float distToOwner = toOwner.Length();
            Vector2 dirToOwner = Vector2.Normalize(toOwner);

            Rectangle rectangle17;
            rectangle17 = headTexture.Frame();
            rectangle17.Height /= 4;
            rectangle17.Y += Projectile.frame * rectangle17.Height;
            //Main.EntitySpriteDraw(color: Projectile.GetAlpha(lightColor), texture: headTexture, position: Projectile.Center - Main.screenPosition, sourceRectangle: headTexture.Bounds, rotation: Projectile.rotation, origin: headTexture.Bounds.Size() / 2f, scale: 1f, effects: SpriteEffects.None);
            Main.EntitySpriteDraw(headTexture, Projectile.Center - Main.screenPosition, headTexture.Bounds, Projectile.GetAlpha(lightColor), Projectile.rotation, headTexture.Bounds.Size() / 2f, 1f, SpriteEffects.None);

            distToOwner -= (rectangle17.Height / 2 + endTexture.Height) * Projectile.scale;
            Vector2 center4 = Projectile.Center + (dirToOwner * Projectile.scale * rectangle17.Height / 2f);
            if (distToOwner > 0f)
            {
                float num284;
                num284 = 0f;
                Rectangle rectangle18;
                rectangle18 = new Rectangle(0, 0, chainTexture.Width, chainTexture.Height);
                while (num284 + 1f < distToOwner)
                {
                    if (distToOwner - num284 < rectangle18.Height)
                    {
                        rectangle18.Height = (int)(distToOwner - num284);
                    }
                    Point point3;
                    point3 = center4.ToTileCoordinates();
                    Color color75;
                    color75 = Lighting.GetColor(point3.X, point3.Y);
                    color75 = Color.Lerp(color75, Color.White, 0.3f);
                    Main.EntitySpriteDraw(chainTexture, center4 - Main.screenPosition, rectangle18, Projectile.GetAlpha(color75), Projectile.rotation, rectangle18.Bottom(), Projectile.scale, SpriteEffects.None);
                    num284 += rectangle18.Height * Projectile.scale;
                    center4 += dirToOwner * rectangle18.Height * Projectile.scale;
                }
            }

            Point point4 = center4.ToTileCoordinates();
            Color color76 = Lighting.GetColor(point4.X, point4.Y);
            color76 = Color.Lerp(color76, Color.White, 0.3f);
            Rectangle value75;
            value75 = endTexture.Frame();
            if (distToOwner < 0f)
            {
                value75.Height += (int)distToOwner;
            }
            Main.EntitySpriteDraw(endTexture, center4 - Main.screenPosition, value75, color76, Projectile.rotation, new Vector2((float)value75.Width / 2f, value75.Height), Projectile.scale, SpriteEffects.None);
            return false;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[1] = -1;
            NPC selected = Main.npc.Where(npc => IsValidTarget(npc.whoAmI)).MinBy(npc => npc.DistanceSQ(Main.player[Projectile.owner].Center));
            if (selected == null)
            {
                Projectile.timeLeft = 0;
                return;
            }
            Projectile.ai[1] = selected.whoAmI;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].Heal(damageDone / 3);
        }

        bool IsValidTarget(int i) => i >= 0 && i < Main.maxNPCs && Main.npc[i].active && !Main.npc[i].boss && !Main.npc[i].friendly && Main.npc[i].DistanceSQ(Main.player[Projectile.owner].Center) < 640f * 640f;
        public override void AI()
        {
            if (Projectile.ai[1] == -1) return;

            if (Projectile.owner < 0 || Projectile.owner >= Main.maxPlayers || !Main.player[Projectile.owner].active || Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return;
            }
            if (!IsValidTarget((int)Projectile.ai[1]))
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            Projectile.rotation = (Main.player[Projectile.owner].Center - Projectile.Center).ToRotation();

            if (Projectile.Hitbox.Intersects(Main.npc[(int)Projectile.ai[1]].Hitbox)) Projectile.ai[2] = 1;

            if (Projectile.ai[2] == 1) // attached
            {
                Projectile.friendly = true;
                Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center;
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                Projectile.friendly = false;

                Projectile.velocity = Projectile.Center.DirectionTo(Main.npc[(int)Projectile.ai[1]].Center) * 2;
            }
        }
    }
}