using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Magic;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.Linq;
using ThoriumMod.Items.BossStarScouter;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class PlasmaGrenade : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ShockGrenade";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Prismatic Missile");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ai[0] = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        public Vector2 offset = Vector2.Zero;
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (offset == Vector2.Zero)
            {
                
                Projectile.ai[0] = target.whoAmI;
                Projectile.timeLeft = 480;
                Projectile.ai[1] = 60;
               
                Vector2 center = Projectile.Center;
                Projectile.width = 190;
                Projectile.height = 190;
                Projectile.Center = center;
                offset = center - target.Center;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size()/2, Projectile.scale, SpriteEffects.None);
            if (offset != Vector2.Zero && Projectile.ai[1] < 30)
            {
                int x = (int)Projectile.ai[2];
                int y = 0;
                while (x > 2)
                {
                    y++;
                    x -= 3;
                }
                Asset<Texture2D> aura = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/TeslaAura");
                Main.EntitySpriteDraw(aura.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 218, 218), lightColor * (1- (Projectile.ai[1]/30f)), Projectile.ai[2] % 1f * 6, new Vector2(218, 218)/2, Projectile.scale, SpriteEffects.None);
            }
            return false;
        }
        
        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length() * 2 * Projectile.direction);
            if (offset != Vector2.Zero && Main.npc[(int)Projectile.ai[0]].active)
            {
                Projectile.velocity = Vector2.Zero;
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                Projectile.Center = npc.Center + offset;
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 60)
                {
                    Projectile.ai[1] = -10;
                    Projectile.ai[2] = Main.rand.Next(0, 18) + Main.rand.NextFloat(0, 0.99f);
                    SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
                    int[] targets = new int[10];
                    int amount = 0;
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        if (Main.npc[i].CanBeChasedBy(Projectile) && Main.npc[i].active && Main.npc[i].Distance(Projectile.Center) < 500 && amount < 10)
                        {
                            targets[amount] = i;
                            amount++;
                        }
                    }
                    
                    int[] hit = new int[10];
                    for (int i = 0; i < 10; i++)
                    {
                        int index = targets[Main.rand.Next(0, amount)];
                        if (!hit.Contains(index))
                        {
                            hit[i] = index;
                            NPC target = Main.npc[index];
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, -2), ModContent.ProjectileType<CalamityMod.Projectiles.Typeless.ArcZap>(), 500, 0, Main.myPlayer, target.whoAmI, 1f);
                        }
                    }
                }
            }
            else
            {
                if (Projectile.timeLeft < 270)
                {
                    Projectile.velocity.Y += 0.2f;
                }
            }
            
        }

        public override bool? CanDamage()
        {
            if (Projectile.ai[1] == -10 || offset == Vector2.Zero) return base.CanDamage();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}