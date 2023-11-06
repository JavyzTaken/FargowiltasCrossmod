using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class AngryGhost : ModProjectile
    {
        public override string Texture => "ThoriumMod/Items/Misc/SpiritDroplet";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.minion = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rect = texture.Bounds;
            Vector2 origin = rect.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rect, Color.Red, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            copyProjType = -1;
        }

        public int shootTimer;
        public int copyProjType;
        public int copyProjDamage;
        public float copyProjKB;
        public override void PostAI()
        {
            if (shootTimer++ >= 45)
            {
                shootTimer = 0;

                int tar = Projectile.FindTargetWithLineOfSight(320);
                if (tar < 0) return;
                NPC target = Main.npc[tar];
                if (target == null) return;

                int closest = -1;
                float closestDist = 25600f;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (!Main.projectile[i].active || Main.projectile[i].minion || Main.projectile[i].sentry || Main.projectile[i].damage <= 0) continue;

                    float newDist = Main.projectile[i].Center.DistanceSQ(Projectile.Center);
                    if (newDist < closestDist)
                    {
                        closestDist = newDist;
                        closest = i;
                    }
                }

                int typeToSpawn;
                if (closest == -1)
                {
                    typeToSpawn = copyProjType;
                }
                else
                {
                    typeToSpawn = Main.projectile[closest].type;
                    copyProjType = typeToSpawn;
                    copyProjDamage = Main.projectile[closest].damage;
                    copyProjKB = Main.projectile[closest].knockBack;
                }
                if (typeToSpawn == -1) return;

                Projectile copied = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Normalize(target.Center - Projectile.Center) * 8f, typeToSpawn, copyProjDamage, copyProjKB, Projectile.owner);
                copied.friendly = true;
                copied.hostile = false;
            }
        }

        public override void AI()
        {
            float num;
            num = 0f;
            float num2;
            num2 = 0f;
            float num3;
            num3 = 20f;
            float num4;
            num4 = 40f;
            float num5;
            num5 = 0.69f;
            if (!Main.player[Projectile.owner].dead && Main.player[Projectile.owner].ThoriumDLC().SpiritTrapperEnch)
            {
                Projectile.timeLeft = 2;
            }
            float num10;
            num10 = 0.05f;
            float num11;
            num11 = Projectile.width;
            for (int m = 0; m < 1000; m++)
            {
                if (m != Projectile.whoAmI && Main.projectile[m].active && Main.projectile[m].owner == Projectile.owner && Main.projectile[m].type == Projectile.type && Math.Abs(Projectile.position.X - Main.projectile[m].position.X) + Math.Abs(Projectile.position.Y - Main.projectile[m].position.Y) < num11)
                {
                    if (Projectile.position.X < Main.projectile[m].position.X)
                    {
                        Projectile.velocity.X -= num10;
                    }
                    else
                    {
                        Projectile.velocity.X += num10;
                    }
                    if (Projectile.position.Y < Main.projectile[m].position.Y)
                    {
                        Projectile.velocity.Y -= num10;
                    }
                    else
                    {
                        Projectile.velocity.Y += num10;
                    }
                }
            }
            Vector2 vector;
            vector = Projectile.position;
            float num12;
            num12 = 400f;
            bool flag;
            flag = false;
            Projectile.tileCollide = true;

            NPC ownerMinionAttackTargetNPC2;
            ownerMinionAttackTargetNPC2 = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(Projectile))
            {
                float num17;
                num17 = Vector2.Distance(ownerMinionAttackTargetNPC2.Center, Projectile.Center);
                float num18;
                num18 = num12 * 3f;
                if (num17 < num18 && !flag)
                {
                    if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, ownerMinionAttackTargetNPC2.position, ownerMinionAttackTargetNPC2.width, ownerMinionAttackTargetNPC2.height))
                    {
                        num12 = num17;
                        vector = ownerMinionAttackTargetNPC2.Center;
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                for (int num19 = 0; num19 < 200; num19++)
                {
                    NPC nPC2;
                    nPC2 = Main.npc[num19];
                    if (!nPC2.CanBeChasedBy(Projectile))
                    {
                        continue;
                    }
                    float num20;
                    num20 = Vector2.Distance(nPC2.Center, Projectile.Center);
                    if (!(num20 >= num12))
                    {
                        if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, nPC2.position, nPC2.width, nPC2.height))
                        {
                            num12 = num20;
                            vector = nPC2.Center;
                            flag = true;
                        }
                    }
                }
            }

            int num21;
            num21 = 500;
            Player player;
            player = Main.player[Projectile.owner];
            if (Vector2.Distance(player.Center, Projectile.Center) > (float)num21)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 1f)
            {
                Projectile.tileCollide = false;
            }
            bool flag5;
            flag5 = false; if (Projectile.ai[0] >= 2f)
            {
                if (!flag)
                {
                    Projectile.ai[0] += 1f;
                }
                if (Projectile.ai[0] > num4)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                Projectile.velocity *= num5;
            }
            else if (flag && (flag5 || Projectile.ai[0] == 0f))
            {
                Vector2 v;
                v = vector - Projectile.Center;
                float num22;
                num22 = v.Length();
                v = v.SafeNormalize(Vector2.Zero);


                if (num22 > 200f)
                {
                    float num26;
                    num26 = 6f + num2 * num;
                    v *= num26;
                    float num27;
                    num27 = num3 * 2f;
                    Projectile.velocity.X = (Projectile.velocity.X * num27 + v.X) / (num27 + 1f);
                    Projectile.velocity.Y = (Projectile.velocity.Y * num27 + v.Y) / (num27 + 1f);
                }
                else
                {
                    if (num22 < 150f)
                    {
                        float num30;
                        num30 = 4f;
                        v *= 0f - num30;
                        Projectile.velocity.X = (Projectile.velocity.X * 40f + v.X) / 41f;
                        Projectile.velocity.Y = (Projectile.velocity.Y * 40f + v.Y) / 41f;
                    }
                    else
                    {
                        Projectile.velocity *= 0.97f;
                    }
                }
            }
            else
            {
                float num31;
                num31 = 6f;
                if (Projectile.ai[0] == 1f)
                {
                    num31 = 15f;
                }
                Vector2 center2;
                center2 = Projectile.Center;
                Vector2 v2;
                Projectile.ai[1] = 80f;
                Projectile.netUpdate = true;
                v2 = player.Center - center2;
                v2.X -= 10 * Main.player[Projectile.owner].direction + 40 * Main.player[Projectile.owner].direction;
                v2.Y -= 10f;
                float num34;
                num34 = v2.Length();
                if (num34 > 200f && num31 < 9f)
                {
                    num31 = 9f;
                }
                num31 = (int)((double)num31 * 0.75);
                if (num34 > 2000f)
                {
                    Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.width / 2);
                }
                if (num34 > 10f)
                {
                    v2 = v2.SafeNormalize(Vector2.Zero);
                    if (num34 < 50f)
                    {
                        num31 /= 2f;
                    }
                    v2 *= num31;
                    Projectile.velocity = (Projectile.velocity * 20f + v2) / 21f;
                }
                else
                {
                    Projectile.direction = Main.player[Projectile.owner].direction;
                    Projectile.velocity *= 0.9f;
                }
            }
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            if (Main.rand.NextBool(10))
            {
                int num40;
                num40 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.RedTorch, 0f, 0f, 100, default, 2f);
                Main.dust[num40].velocity *= 0.3f;
                Main.dust[num40].noGravity = true;
                Main.dust[num40].noLight = true;
            }
            if (Projectile.velocity.X > 0f)
            {
                Projectile.spriteDirection = (Projectile.direction = -1);
            }
            else if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = (Projectile.direction = 1);
            }
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += 1f;
                if (Main.rand.NextBool(3))
                {
                    Projectile.ai[1] += 1f;
                }
            }
            if (Projectile.ai[1] > 90f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
        }
    }
}
